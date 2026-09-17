using System.Globalization;
using System.Reflection;

namespace SkyViewer;

/// <summary>
/// Holds every catalogue the viewer draws and converts all of it into horizon
/// coordinates. Conversion is the expensive part of a frame, so it happens only when
/// the clock or the observer's location changes - never on a simple pan or zoom.
/// </summary>
public sealed class SkyCatalog
{
  private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

  public IReadOnlyList<Star> Stars { get; private set; } = [];
  public IReadOnlyList<DeepSkyObject> DeepSkyObjects { get; private set; } = [];
  public IReadOnlyList<Constellation> Constellations { get; private set; } = [];
  public IReadOnlyList<MilkyWayContour> MilkyWay { get; private set; } = [];
  public SolarSystem SolarSystem { get; } = new();

  /// <summary>Stars sorted brightest first, so the renderer can stop at the magnitude limit.</summary>
  public IReadOnlyList<Star> StarsByBrightness { get; private set; } = [];

  public double LocalSiderealTime { get; private set; }
  public double ObserverLatitude { get; private set; }

  /// <summary>
  /// When set, horizon coordinates are the apparent ones an observer actually sees,
  /// lifted by atmospheric refraction. RA/Dec readouts stay geometric.
  /// </summary>
  public bool ApplyRefraction { get; set; } = true;

  public void Load()
  {
    Stars = LoadStars();
    StarsByBrightness = Stars.OrderBy(s => s.Magnitude).ToArray();
    DeepSkyObjects = LoadDeepSky();
    Constellations = LoadConstellations();
    MilkyWay = LoadMilkyWay();
  }

  /// <summary>Recomputes every horizon coordinate for a new instant and place.</summary>
  // Precession moves the stars by well under an arcsecond over a couple of weeks,
  // which is far below one screen pixel, so the precessed coordinates are cached
  // and only rebuilt when the displayed date has moved appreciably. Everything
  // else (the hour angle) still updates on every single frame.
  const double PrecessionTolerance = 0.0003; // in Julian centuries, about 11 days.
  double _precessionEpoch = double.NaN;

  public void Update(DateTime utc, double latitude, double longitude)
  {
    double jd = AstroMath.JulianDay(utc);
    double t = AstroMath.JulianCenturies(jd);
    double lst = AstroMath.LocalSiderealTime(jd, longitude);

    LocalSiderealTime = lst;
    ObserverLatitude = latitude;

    bool precess = double.IsNaN(_precessionEpoch)
                   || Math.Abs(t - _precessionEpoch) > PrecessionTolerance;
    if (precess) _precessionEpoch = t;

    foreach (Star star in Stars)
      PlaceFixedObject(star, t, lst, latitude, ApplyRefraction, precess);

    foreach (DeepSkyObject dso in DeepSkyObjects)
      PlaceFixedObject(dso, t, lst, latitude, ApplyRefraction, precess);

    foreach (Constellation c in Constellations)
    {
      // The horizon buffers are allocated once and then written in place, so
      // panning the sky at 30 frames a second does not churn the heap.
      if (c.HorizonSegments.Length != c.Segments.Count)
      {
        c.HorizonSegments = CreateBuffer(c.Segments);
        c.PrecessedSegments = CloneShape(c.Segments);
        PrecessSegments(c.Segments, c.PrecessedSegments, t);
      }
      else if (precess)
      {
        PrecessSegments(c.Segments, c.PrecessedSegments, t);
      }

      for (int i = 0; i < c.Segments.Count; i++)
      {
        (double Ra, double Dec)[] source = c.PrecessedSegments[i];
        HorizonVector[] target = c.HorizonSegments[i];
        for (int j = 0; j < source.Length; j++)
          target[j] = AstroMath.EquatorialToHorizon(
              source[j].Ra, source[j].Dec, lst, latitude);
      }

      Equatorial label = AstroMath.PrecessFromJ2000(c.LabelRaJ2000, c.LabelDecJ2000, t);
      c.LabelHorizon = AstroMath.EquatorialToHorizon(
          label.RightAscension, label.Declination, lst, latitude);
    }

    foreach (MilkyWayContour contour in MilkyWay)
    {
      if (contour.HorizonRings.Length != contour.Rings.Count)
        contour.HorizonRings = CreateBuffer(contour.Rings);

      for (int i = 0; i < contour.Rings.Count; i++)
      {
        (double Ra, double Dec)[] source = contour.Rings[i];
        HorizonVector[] target = contour.HorizonRings[i];
        for (int j = 0; j < source.Length; j++)
        {
          // The Milky Way outline is a diffuse band; precession is far below
          // the resolution of the contour, so skip it and save the work.
          target[j] = AstroMath.EquatorialToHorizon(
              source[j].Ra, source[j].Dec, lst, latitude);
        }
      }
    }

    SolarSystem.Update(jd, lst, latitude, ApplyRefraction);
  }

  static HorizonVector[][] CreateBuffer(IReadOnlyList<(double Ra, double Dec)[]> shape)
  {
    var buffer = new HorizonVector[shape.Count][];
    for (int i = 0; i < shape.Count; i++)
      buffer[i] = new HorizonVector[shape[i].Length];
    return buffer;
  }

  static (double Ra, double Dec)[][] CloneShape(
      IReadOnlyList<(double Ra, double Dec)[]> shape)
  {
    var buffer = new (double Ra, double Dec)[shape.Count][];
    for (int i = 0; i < shape.Count; i++)
      buffer[i] = new (double Ra, double Dec)[shape[i].Length];
    return buffer;
  }

  static void PrecessSegments(
      IReadOnlyList<(double Ra, double Dec)[]> source,
      (double Ra, double Dec)[][] target,
      double centuries)
  {
    for (int i = 0; i < source.Count; i++)
    {
      (double Ra, double Dec)[] from = source[i];
      (double Ra, double Dec)[] to = target[i];
      for (int j = 0; j < from.Length; j++)
      {
        Equatorial p = AstroMath.PrecessFromJ2000(from[j].Ra, from[j].Dec, centuries);
        to[j] = (p.RightAscension, p.Declination);
      }
    }
  }

  static void PlaceFixedObject(SkyBody body, double centuries, double lst, double latitude, bool refract, bool precess)
  {
    if (precess)
    {
      Equatorial p = AstroMath.PrecessFromJ2000(body.RaJ2000, body.DecJ2000, centuries);
      body.RaOfDate = p.RightAscension;
      body.DecOfDate = p.Declination;
    }

    body.Horizon = AstroMath.EquatorialToHorizon(body.RaOfDate, body.DecOfDate, lst, latitude);
    (body.Altitude, body.Azimuth) = AstroMath.ToAltAz(body.Horizon);

    if (!refract || body.Altitude > 12) return;

    double apparent = AstroMath.ApplyRefraction(body.Altitude);
    body.Altitude = apparent;
    body.Horizon = HorizonVector.FromAltAz(
        apparent * AstroMath.Deg2Rad, body.Azimuth * AstroMath.Deg2Rad);
  }

  static IEnumerable<string> ReadLines(string fileName)
  {
    Assembly assembly = typeof(SkyCatalog).Assembly;
    string resource = $"SkyViewer.Data.{fileName}";
    using Stream stream = assembly.GetManifestResourceStream(resource)
        ?? throw new InvalidOperationException(
            $"Embedded resource '{resource}' is missing. Check the EmbeddedResource items in SkyViewer.csproj.");

    using var reader = new StreamReader(stream);
    while (reader.ReadLine() is { } line)
    {
      if (line.Length == 0 || line[0] == '#') continue;
      yield return line;
    }
  }

  static double Num(ReadOnlySpan<char> s) => double.Parse(s, Inv);

  static Star[] LoadStars()
  {
    var list = new List<Star>(5200);
    foreach (string line in ReadLines("Stars.dat"))
    {
      string[] f = line.Split('|');
      if (f.Length < 7) continue;
      list.Add(new Star
      {
        RaJ2000 = Num(f[0]),
        DecJ2000 = Num(f[1]),
        Magnitude = Num(f[2]),
        ColorIndex = Num(f[3]),
        Bayer = f[4],
        ProperName = f[5],
        Constellation = f[6],
      });
    }
    return [.. list];
  }

  static DeepSkyObject[] LoadDeepSky()
  {
    var list = new List<DeepSkyObject>(256);
    foreach (string line in ReadLines("DeepSkyObjects.dat"))
    {
      string[] f = line.Split('|');
      if (f.Length < 6) continue;
      list.Add(new DeepSkyObject
      {
        RaJ2000 = Num(f[0]),
        DecJ2000 = Num(f[1]),
        Magnitude = Num(f[2]),
        TypeCode = f[3],
        Designation = f[4],
        ProperName = f[5],
      });
    }
    return [.. list];
  }

  static (double, double)[] ParsePoints(string segment)
  {
    string[] pairs = segment.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var points = new (double, double)[pairs.Length];
    for (int i = 0; i < pairs.Length; i++)
    {
      int comma = pairs[i].IndexOf(',');
      points[i] = (Num(pairs[i].AsSpan(0, comma)), Num(pairs[i].AsSpan(comma + 1)));
    }
    return points;
  }

  static Constellation[] LoadConstellations()
  {
    var list = new List<Constellation>(90);
    foreach (string line in ReadLines("Constellations.dat"))
    {
      string[] f = line.Split('|');
      if (f.Length < 4) continue;

      int comma = f[2].IndexOf(',');
      var segments = f[3]
          .Split(';', StringSplitOptions.RemoveEmptyEntries)
          .Select(ParsePoints)
          .Where(p => p.Length > 1)
          .ToArray();

      list.Add(new Constellation
      {
        Abbreviation = f[0],
        Name = f[1],
        LabelRaJ2000 = Num(f[2].AsSpan(0, comma)),
        LabelDecJ2000 = Num(f[2].AsSpan(comma + 1)),
        Segments = segments,
      });
    }
    return [.. list];
  }

  static MilkyWayContour[] LoadMilkyWay()
  {
    var list = new List<MilkyWayContour>(8);
    foreach (string line in ReadLines("MilkyWay.dat"))
    {
      string[] f = line.Split('|');
      if (f.Length < 2) continue;
      list.Add(new MilkyWayContour
      {
        Level = int.Parse(f[0], Inv),
        Rings = f[1]
              .Split(';', StringSplitOptions.RemoveEmptyEntries)
              .Select(ParsePoints)
              .Where(p => p.Length > 2)
              .ToArray(),
      });
    }
    return [.. list];
  }
}
