namespace SkyViewer;

/// <summary>
/// Colours for the chart. The interface chrome borrows from observatory practice:
/// near-black surfaces so nothing in the room ruins your dark adaptation, and a warm
/// amber for anything interactive, because amber is the one bright colour that does
/// not wreck night vision.
/// </summary>
public static class SkyPalette
{
  public static readonly Color Void = Color.FromArgb(5, 7, 12);
  public static readonly Color Panel = Color.FromArgb(13, 17, 25);
  public static readonly Color PanelRaised = Color.FromArgb(19, 24, 35);
  public static readonly Color Divider = Color.FromArgb(28, 35, 51);

  public static readonly Color Text = Color.FromArgb(185, 196, 214);
  public static readonly Color TextDim = Color.FromArgb(107, 122, 147);
  public static readonly Color Amber = Color.FromArgb(232, 184, 75);

  public static readonly Color ConstellationLine = Color.FromArgb(120, 46, 92, 138);
  public static readonly Color ConstellationName = Color.FromArgb(150, 108, 152, 196);
  public static readonly Color EquatorialGrid = Color.FromArgb(70, 44, 82, 110);
  public static readonly Color HorizonGrid = Color.FromArgb(70, 70, 96, 84);
  public static readonly Color Ecliptic = Color.FromArgb(110, 176, 138, 70);
  public static readonly Color HorizonLine = Color.FromArgb(200, 108, 92, 72);
  public static readonly Color Ground = Color.FromArgb(9, 11, 14);
  public static readonly Color Cardinal = Color.FromArgb(215, 198, 122, 78);
  public static readonly Color StarLabel = Color.FromArgb(190, 196, 206, 222);
  public static readonly Color DeepSkyMarker = Color.FromArgb(180, 118, 190, 170);
  public static readonly Color Selection = Color.FromArgb(232, 184, 75);

  /// <summary>
  /// Zenith and horizon colours for the sky itself, blended from the Sun's altitude.
  /// Daylight, the three twilights and full night all get their own pair, and the
  /// result is what makes a sunrise animation look like a sunrise.
  /// </summary>
  public static (Color Zenith, Color Horizon) SkyGradient(double sunAltitudeDeg)
  {
    (double Alt, Color Zenith, Color Horizon)[] stops =
    [
        (-90, Color.FromArgb(3, 5, 10),    Color.FromArgb(7, 10, 18)),      // astronomical night
            (-18, Color.FromArgb(5, 9, 22),    Color.FromArgb(14, 20, 40)),     // astronomical twilight
            (-12, Color.FromArgb(10, 18, 44),  Color.FromArgb(38, 42, 74)),     // nautical twilight
            (-6,  Color.FromArgb(24, 40, 82),  Color.FromArgb(104, 74, 88)),    // civil twilight
            (-1,  Color.FromArgb(48, 82, 134), Color.FromArgb(198, 128, 88)),   // sunrise / sunset
            (3,   Color.FromArgb(70, 118, 178), Color.FromArgb(180, 190, 206)), // low sun
            (90,  Color.FromArgb(58, 118, 196), Color.FromArgb(158, 190, 224)), // full daylight
        ];

    double alt = Math.Clamp(sunAltitudeDeg, -90, 90);

    for (int i = 1; i < stops.Length; i++)
    {
      if (alt > stops[i].Alt) continue;
      double span = stops[i].Alt - stops[i - 1].Alt;
      double f = span <= 0 ? 0 : (alt - stops[i - 1].Alt) / span;
      return (Blend(stops[i - 1].Zenith, stops[i].Zenith, f),
              Blend(stops[i - 1].Horizon, stops[i].Horizon, f));
    }

    return (stops[^1].Zenith, stops[^1].Horizon);
  }

  /// <summary>
  /// How much of the star field survives the sky brightness: 1 at full night,
  /// 0 once the Sun is well up.
  /// </summary>
  public static double StarVisibility(double sunAltitudeDeg)
  {
    if (sunAltitudeDeg <= -18) return 1.0;
    if (sunAltitudeDeg >= 0) return 0.0;
    double f = (-sunAltitudeDeg) / 18.0;
    return f * f;   // stars come out quickly once the Sun is properly down
  }

  public static Color Blend(Color a, Color b, double f)
  {
    f = Math.Clamp(f, 0, 1);
    return Color.FromArgb(
        (int)(a.A + (b.A - a.A) * f),
        (int)(a.R + (b.R - a.R) * f),
        (int)(a.G + (b.G - a.G) * f),
        (int)(a.B + (b.B - a.B) * f));
  }

  public static Color WithAlpha(Color color, double alpha) =>
      Color.FromArgb((int)Math.Clamp(alpha * 255, 0, 255), color.R, color.G, color.B);
}

/// <summary>Maps a B-V colour index to a screen colour, via black-body temperature.</summary>
public static class StarColor
{
  private const int Buckets = 96;
  private const double MinBv = -0.4;
  private const double MaxBv = 2.2;

  private static readonly Color[] Table = BuildTable();

  public static Color FromColorIndex(double bv)
  {
    double f = (Math.Clamp(bv, MinBv, MaxBv) - MinBv) / (MaxBv - MinBv);
    return Table[(int)Math.Clamp(f * (Buckets - 1), 0, Buckets - 1)];
  }

  private static Color[] BuildTable()
  {
    var table = new Color[Buckets];
    for (int i = 0; i < Buckets; i++)
    {
      double bv = MinBv + (MaxBv - MinBv) * i / (Buckets - 1.0);
      table[i] = FromTemperature(TemperatureFromColorIndex(bv));
    }
    return table;
  }

  /// <summary>Ballesteros' relation between B-V and effective temperature.</summary>
  private static double TemperatureFromColorIndex(double bv) =>
      4600.0 * (1.0 / (0.92 * bv + 1.7) + 1.0 / (0.92 * bv + 0.62));

  /// <summary>
  /// Approximate black-body colour. Real stars are desaturated by the eye at low
  /// light levels, so the result is pulled part of the way toward white - a fully
  /// saturated orange Betelgeuse looks wrong next to what you see outdoors.
  /// </summary>
  private static Color FromTemperature(double kelvin)
  {
    double t = Math.Clamp(kelvin, 1000, 40000) / 100.0;

    double r = t <= 66
        ? 255
        : 329.698727446 * Math.Pow(t - 60, -0.1332047592);

    double g = t <= 66
        ? 99.4708025861 * Math.Log(t) - 161.1195681661
        : 288.1221695283 * Math.Pow(t - 60, -0.0755148492);

    double b = t >= 66
        ? 255
        : t <= 19
            ? 0
            : 138.5177312231 * Math.Log(t - 10) - 305.0447927307;

    const double desaturate = 0.35;
    r = r + (255 - r) * desaturate;
    g = g + (255 - g) * desaturate;
    b = b + (255 - b) * desaturate;

    return Color.FromArgb(
        (int)Math.Clamp(r, 0, 255),
        (int)Math.Clamp(g, 0, 255),
        (int)Math.Clamp(b, 0, 255));
  }
}
