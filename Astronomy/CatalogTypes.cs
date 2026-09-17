namespace SkyViewer;

/// <summary>
/// Anything that can be drawn on the sky and clicked on. Position of date and the
/// horizon vector are recomputed whenever the clock or the observer moves; the
/// renderer only ever reads them.
/// </summary>
public abstract class SkyBody
{
  /// <summary>Catalogue right ascension at J2000, in degrees.</summary>
  public double RaJ2000 { get; init; }

  /// <summary>Catalogue declination at J2000, in degrees.</summary>
  public double DecJ2000 { get; init; }

  public double Magnitude { get; set; }

  public double RaOfDate { get; set; }
  public double DecOfDate { get; set; }

  public HorizonVector Horizon { get; set; }
  public double Altitude { get; set; }
  public double Azimuth { get; set; }

  /// <summary>Set by the renderer so hit testing can use the last drawn position.</summary>
  public PointF ScreenPosition { get; set; }
  public bool WasDrawn { get; set; }

  public abstract string DisplayName { get; }
  public abstract string TypeLabel { get; }

  /// <summary>Short label drawn next to the object, or null to leave it unlabelled.</summary>
  public virtual string? ChartLabel => null;
}

public sealed class Star : SkyBody
{
  /// <summary>B-V colour index, which drives the rendered star colour.</summary>
  public double ColorIndex { get; init; }

  /// <summary>Bayer letter such as "α", empty when the star has none.</summary>
  public string Bayer { get; init; } = "";

  public string ProperName { get; init; } = "";

  /// <summary>Three-letter constellation abbreviation, e.g. "Ori".</summary>
  public string Constellation { get; init; } = "";

  public override string DisplayName =>
      !string.IsNullOrEmpty(ProperName) ? ProperName
      : !string.IsNullOrEmpty(Bayer) ? $"{Bayer} {Constellation}"
      : $"Star at {AstroMath.FormatRightAscension(RaJ2000)}";

  public override string TypeLabel => "Star";

  public override string? ChartLabel =>
      !string.IsNullOrEmpty(ProperName) ? ProperName
      : !string.IsNullOrEmpty(Bayer) ? Bayer
      : null;
}

public sealed class DeepSkyObject : SkyBody
{
  public string Designation { get; init; } = "";
  public string ProperName { get; init; } = "";

  /// <summary>Two-letter type code from the source catalogue: oc, gc, pn, bn, g, snr...</summary>
  public string TypeCode { get; init; } = "";

  public override string DisplayName =>
      string.IsNullOrEmpty(ProperName) ? Designation : $"{ProperName} ({Designation})";

  public override string TypeLabel => TypeCode switch
  {
    "oc" => "Open cluster",
    "gc" => "Globular cluster",
    "pn" => "Planetary nebula",
    "bn" or "en" => "Bright nebula",
    "rn" => "Reflection nebula",
    "snr" => "Supernova remnant",
    "g" or "ga" or "gg" => "Galaxy",
    "cg" => "Galaxy cluster",
    "sfr" => "Star forming region",
    _ => "Deep sky object",
  };

  public override string? ChartLabel =>
      string.IsNullOrEmpty(ProperName) ? Designation : ProperName;
}

/// <summary>The Sun, the Moon, or a planet. Position is recomputed from the clock.</summary>
public sealed class SolarSystemBody : SkyBody
{
  public required string Name { get; init; }

  public SolarSystemBodyKind Kind { get; init; }

  /// <summary>Apparent angular diameter in degrees; drives the drawn disc size.</summary>
  public double AngularDiameter { get; set; }

  /// <summary>Illuminated fraction, 0 to 1. Only meaningful for the Moon.</summary>
  public double IlluminatedFraction { get; set; }

  /// <summary>Position angle of the bright limb, in degrees. Only used for the Moon.</summary>
  public double BrightLimbAngle { get; set; }

  public Color Tint { get; init; } = Color.White;

  public override string DisplayName => Name;

  public override string TypeLabel => Kind switch
  {
    SolarSystemBodyKind.Sun => "Star (Sol)",
    SolarSystemBodyKind.Moon => "Natural satellite",
    _ => "Planet",
  };

  public override string ChartLabel => Name;
}

public enum SolarSystemBodyKind
{
  Sun,
  Moon,
  Planet,
}

/// <summary>One IAU constellation: its stick figure and where to put its name.</summary>
public sealed class Constellation
{
  public required string Abbreviation { get; init; }
  public required string Name { get; init; }

  public double LabelRaJ2000 { get; init; }
  public double LabelDecJ2000 { get; init; }

  /// <summary>Each segment is an open polyline of J2000 (ra, dec) pairs in degrees.</summary>
  public required IReadOnlyList<(double Ra, double Dec)[]> Segments { get; init; }

  /// <summary>Horizon vectors for every vertex, in the same shape as <see cref="Segments"/>.</summary>
  public HorizonVector[][] HorizonSegments { get; set; } = [];

  /// <summary>Segment vertices precessed to the epoch of date, cached between frames.</summary>
  public (double Ra, double Dec)[][] PrecessedSegments { get; set; } = [];

  public HorizonVector LabelHorizon { get; set; }
}

/// <summary>One brightness contour of the Milky Way, as a set of closed rings.</summary>
public sealed class MilkyWayContour
{
  public int Level { get; init; }
  public required IReadOnlyList<(double Ra, double Dec)[]> Rings { get; init; }
  public HorizonVector[][] HorizonRings { get; set; } = [];
}
