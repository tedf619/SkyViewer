namespace SkyViewer;

public sealed record ObservingSite(string Name, double Latitude, double Longitude, double UtcOffsetHours)
{
  public override string ToString() => Name;

  public static readonly ObservingSite[] Presets =
  [
      new("Rancho Santa Margarita, US",  33.640, -117.603,  -8),
        new("Mauna Kea Observatory, US",   19.826, -155.472, -10),
        new("New York, US",                40.713,  -74.006,  -5),
        new("Chicago, US",                 41.878,  -87.630,  -6),
        new("Mexico City, MX",             19.433,  -99.133,  -6),
        new("Santiago, CL",               -33.449,  -70.669,  -4),
        new("Paranal Observatory, CL",    -24.628,  -70.404,  -4),
        new("Reykjavík, IS",               64.147,  -21.942,   0),
        new("London, GB",                  51.507,   -0.128,   0),
        new("Greenwich Meridian",           0.000,    0.000,   0),
        new("Paris, FR",                   48.857,    2.352,   1),
        new("Cape Town, ZA",              -33.925,   18.424,   2),
        new("Moscow, RU",                  55.756,   37.617,   3),
        new("Delhi, IN",                   28.614,   77.209,   5.5),
        new("Beijing, CN",                 39.904,  116.407,   8),
        new("Tokyo, JP",                   35.690,  139.692,   9),
        new("Sydney, AU",                 -33.869,  151.209,  10),
        new("Amundsen-Scott, AQ",         -89.997,    0.000,  12),
    ];
}

public enum TimeFlow
{
  Paused,
  RealTime,
  MinutePerSecond,
  HourPerSecond,
}

/// <summary>
/// Where and when the observer is standing. Keeps a UTC instant internally and
/// exposes local civil time through the site's fixed UTC offset.
/// </summary>
public sealed class Observer
{
  private DateTime _utc = DateTime.UtcNow;

  public ObservingSite Site { get; set; } = ObservingSite.Presets[0];

  public double Latitude { get; set; } = ObservingSite.Presets[0].Latitude;
  public double Longitude { get; set; } = ObservingSite.Presets[0].Longitude;
  public double UtcOffsetHours { get; set; } = ObservingSite.Presets[0].UtcOffsetHours;

  public TimeFlow Flow { get; set; } = TimeFlow.RealTime;

  public DateTime Utc
  {
    get => _utc;
    set => _utc = value;
  }

  public DateTime LocalTime
  {
    get => _utc.AddHours(UtcOffsetHours);
    set => _utc = value.AddHours(-UtcOffsetHours);
  }

  public void ApplySite(ObservingSite site)
  {
    Site = site;
    Latitude = site.Latitude;
    Longitude = site.Longitude;
    UtcOffsetHours = site.UtcOffsetHours;
  }

  public void SetToNow() => _utc = DateTime.UtcNow;

  /// <summary>Advances the clock for one animation tick of the given wall-clock length.</summary>
  public void Advance(TimeSpan elapsed)
  {
    double seconds = elapsed.TotalSeconds * Flow switch
    {
      TimeFlow.Paused => 0,
      TimeFlow.RealTime => 1,
      TimeFlow.MinutePerSecond => 60,
      TimeFlow.HourPerSecond => 3600,
      _ => 0,
    };

    if (seconds != 0) _utc = _utc.AddSeconds(seconds);
  }

  public static string Describe(TimeFlow flow) => flow switch
  {
    TimeFlow.Paused => "Paused",
    TimeFlow.RealTime => "Real time",
    TimeFlow.MinutePerSecond => "1 minute per second",
    TimeFlow.HourPerSecond => "1 hour per second",
    _ => flow.ToString(),
  };
}
