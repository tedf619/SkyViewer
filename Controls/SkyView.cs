using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace SkyViewer;

/// <summary>
/// The planetarium canvas. Owns celestial projections, drawing, the mouse and
/// keyboard navigation. It reads from the catalogue but never changes it, so the
/// clock can advance on its own schedule.
/// </summary>
public sealed partial class SkyView : UserControl
{
  readonly SkyProjection projection = new();
  readonly BrushCache brushes = new();
  Font labelFont = null!, constellationFont = null!, cardinalFont = null!, readoutFont = null!;

  bool isDraggingMouse;
  Point dragOrigin;
  double dragStartAltitude, dragStartAzimuth;

  PointF cursorLocation;
  bool isCursorInside;

  // Simple occupancy grid so labels do not pile on top of each other.
  readonly HashSet<long> labelCells = [];

  public SkyView()
  {
    InitializeComponent();

    // Every pixel of this control is painted by OnPaint, so let GDI+ skip the
    // background erase and draw the whole frame off-screen before it is shown.
    SetStyle(ControlStyles.AllPaintingInWmPaint
             | ControlStyles.UserPaint
             | ControlStyles.OptimizedDoubleBuffer
             | ControlStyles.ResizeRedraw
             | ControlStyles.Selectable, true);

    CreateFonts();
  }

  // ---------------------------------------------------------------- inputs

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public SkyCatalog? Catalog { get; set; }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public Observer? Observer { get; set; }

  double centerAltitude = 45, centerAzimuth, fieldOfView = 100;

  [DefaultValue(0)]
  public double CenterAltitude
  {
    get => centerAltitude;
    set { centerAltitude = Math.Clamp(value, -89, 89); Invalidate(); }
  }

  [DefaultValue(0)]
  public double CenterAzimuth
  {
    get => centerAzimuth;
    set { centerAzimuth = AstroMath.Normalize360(value); Invalidate(); }
  }

  [DefaultValue(0)]
  public double FieldOfView
  {
    get => fieldOfView;
    set { fieldOfView = Math.Clamp(value, 1, 160); Invalidate(); }
  }

  [DefaultValue(6.0)]
  public double LimitingMagnitude { get; set; } = 6.0;

  [DefaultValue(true)]
  public bool ShowConstellationLines { get; set; } = true;

  [DefaultValue(true)]
  public bool ShowConstellationNames { get; set; } = true;

  [DefaultValue(true)]
  public bool ShowStarNames { get; set; } = true;

  [DefaultValue(true)]
  public bool ShowMilkyWay { get; set; } = true;

  [DefaultValue(true)]
  public bool ShowDeepSky { get; set; } = true;

  [DefaultValue(false)]
  public bool ShowEquatorialGrid { get; set; }

  [DefaultValue(false)]
  public bool ShowHorizonGrid { get; set; }

  [DefaultValue(false)]
  public bool ShowEcliptic { get; set; }

  [DefaultValue(false)]
  public bool ShowGround { get; set; } = true;

  [DefaultValue(true)]
  public bool ShowCardinalPoints { get; set; } = true;

  [DefaultValue(true)]
  public bool ShowAtmosphere { get; set; } = true;

  [DefaultValue(true)]
  public bool ShowSolarSystem { get; set; } = true;

  [Browsable(false)]
  public SkyBody? Selected { get; private set; }

  /// <summary>Altitude and azimuth under the mouse pointer, or null when it is outside.</summary>
  [Browsable(false)]
  public (double Altitude, double Azimuth)? PointerDirection { get; private set; }

  /// <summary>Number of stars drawn in the most recent frame.</summary>
  [Browsable(false)]
  public int LastStarCount { get; private set; }

  public void LookAt(double altitudeDeg, double azimuthDeg)
  {
    centerAltitude = Math.Clamp(altitudeDeg, -89, 89);
    centerAzimuth = AstroMath.Normalize360(azimuthDeg);
    FireViewChanged();
    Invalidate();
  }

  public void CenterOn(SkyBody body) => LookAt(body.Altitude, body.Azimuth);

  public void Zoom(double factor)
  {
    FieldOfView = fieldOfView * factor;
    FireViewChanged();
    Invalidate();
  }

  public void SelectBody(SkyBody? body)
  {
    if (ReferenceEquals(Selected, body)) return;
    Selected = body;
    FireSelectionChanged();
    Invalidate();
  }

  protected override void OnMouseDown(MouseEventArgs e)
  {
    base.OnMouseDown(e);
    Focus();

    if (e.Button == MouseButtons.Left)
    {
      isDraggingMouse = true;
      dragOrigin = e.Location;
      dragStartAltitude = centerAltitude;
      dragStartAzimuth = centerAzimuth;
    }
  }

  protected override void OnMouseMove(MouseEventArgs e)
  {
    base.OnMouseMove(e);
    cursorLocation = e.Location;
    isCursorInside = true;

    if (isDraggingMouse)
    {
      // Drag the sky, not the camera: the point under the pointer should follow it.
      double degreesPerPixel = fieldOfView / Math.Max(Height, 1);
      double dx = (e.X - dragOrigin.X) * degreesPerPixel;
      double dy = (e.Y - dragOrigin.Y) * degreesPerPixel;

      double newAltitude = Math.Clamp(dragStartAltitude + dy, -89, 89);

      // Near the zenith a horizontal drag sweeps through far more azimuth than it
      // does near the horizon, so scale it by the cosine of the altitude.
      double altitudeScale = Math.Max(Math.Cos(newAltitude * AstroMath.Deg2Rad), 0.12);
      centerAltitude = newAltitude;
      centerAzimuth = AstroMath.Normalize360(dragStartAzimuth - dx / altitudeScale);

      FireViewChanged();
      Invalidate();
      return;
    }

    UpdatePointerDirection();
    Invalidate();
  }

  protected override void OnMouseUp(MouseEventArgs e)
  {
    base.OnMouseUp(e);
    if (e.Button != MouseButtons.Left) return;

    bool wasClick = Math.Abs(e.X - dragOrigin.X) < 4 && Math.Abs(e.Y - dragOrigin.Y) < 4;
    isDraggingMouse = false;

    if (wasClick) SelectBody(HitTest(e.Location));
  }

  protected override void OnMouseLeave(EventArgs e)
  {
    base.OnMouseLeave(e);
    isCursorInside = false;
    PointerDirection = null;
    FireViewChanged();
    Invalidate();
  }

  protected override void OnMouseDoubleClick(MouseEventArgs e)
  {
    base.OnMouseDoubleClick(e);
    SkyBody? body = HitTest(e.Location);
    if (body is null) return;

    SelectBody(body);
    CenterOn(body);
  }

  protected override void OnMouseWheel(MouseEventArgs e)
  {
    base.OnMouseWheel(e);
    Zoom(e.Delta > 0 ? 1 / 1.18 : 1.18);
    UpdatePointerDirection();
  }

  protected override bool IsInputKey(Keys keyData) => keyData switch
  {
    Keys.Left or Keys.Right or Keys.Up or Keys.Down => true,
    _ => base.IsInputKey(keyData),
  };

  protected override void OnKeyDown(KeyEventArgs e)
  {
    base.OnKeyDown(e);
    double step = fieldOfView / 12.0;

    switch (e.KeyCode)
    {
      case Keys.Left: CenterAzimuth -= step; break;
      case Keys.Right: CenterAzimuth += step; break;
      case Keys.Up: CenterAltitude += step; break;
      case Keys.Down: CenterAltitude -= step; break;
      case Keys.Oemplus or Keys.Add: Zoom(1 / 1.25); return;
      case Keys.OemMinus or Keys.Subtract: Zoom(1.25); return;
      case Keys.N: LookAt(20, 0); return;
      case Keys.E: LookAt(20, 90); return;
      case Keys.S: LookAt(20, 180); return;
      case Keys.W: LookAt(20, 270); return;
      case Keys.Z: LookAt(89, centerAzimuth); return;
      case Keys.Escape: SelectBody(null); return;
      default: return;
    }

    FireViewChanged();
    Invalidate();
    e.Handled = true;
  }

  void UpdatePointerDirection()
  {
    if (!isCursorInside || Width < 2 || Height < 2)
    {
      PointerDirection = null;
      return;
    }

    projection.Configure(centerAltitude, centerAzimuth, fieldOfView, Size);
    PointerDirection = AstroMath.ToAltAz(projection.Unproject(cursorLocation));
    FireViewChanged();
  }

  SkyBody? HitTest(Point location)
  {
    if (Catalog is null) return null;

    SkyBody? best = null;
    double bestDistance = 18 * 18;

    void Consider(SkyBody body)
    {
      if (!body.WasDrawn) return;
      double dx = body.ScreenPosition.X - location.X;
      double dy = body.ScreenPosition.Y - location.Y;
      double d = dx * dx + dy * dy;

      // Prefer the brighter object when two candidates are equally close.
      if (d < bestDistance || (d < bestDistance * 1.4 && best is not null && body.Magnitude < best.Magnitude - 1))
      {
        bestDistance = Math.Min(d, bestDistance);
        best = body;
      }
    }

    if (ShowSolarSystem)
      foreach (SolarSystemBody body in Catalog.SolarSystem.All) Consider(body);

    foreach (Star star in Catalog.StarsByBrightness)
    {
      if (star.Magnitude > LimitingMagnitude) break;
      Consider(star);
    }

    if (ShowDeepSky)
      foreach (DeepSkyObject dso in Catalog.DeepSkyObjects) Consider(dso);

    return best;
  }

  void CreateFonts()
  {
    labelFont?.Dispose();
    constellationFont?.Dispose();
    cardinalFont?.Dispose();
    readoutFont?.Dispose();

    labelFont = new Font("Segoe UI", 8f, FontStyle.Regular, GraphicsUnit.Point);
    constellationFont = new Font("Segoe UI Semilight", 11f, FontStyle.Regular, GraphicsUnit.Point);
    cardinalFont = new Font("Segoe UI", 11f, FontStyle.Bold, GraphicsUnit.Point);
    readoutFont = new Font("Consolas", 8.5f, FontStyle.Regular, GraphicsUnit.Point);
  }

  protected override void OnPaint(PaintEventArgs e)
  {
    Graphics g = e.Graphics;
    g.SmoothingMode = SmoothingMode.AntiAlias;
    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    g.InterpolationMode = InterpolationMode.Bilinear;
    g.CompositingQuality = CompositingQuality.HighSpeed;

    if (Catalog is null || Width < 8 || Height < 8)
    {
      g.Clear(SkyPalette.Void);
      return;
    }

    projection.Configure(centerAltitude, centerAzimuth, fieldOfView, Size);
    labelCells.Clear();

    foreach (Star star in Catalog.Stars) star.WasDrawn = false;
    foreach (DeepSkyObject dso in Catalog.DeepSkyObjects) dso.WasDrawn = false;
    foreach (SolarSystemBody body in Catalog.SolarSystem.All) body.WasDrawn = false;

    double sunAltitude = ShowAtmosphere ? Catalog.SolarSystem.Sun.Altitude : -90;
    double starVisibility = SkyPalette.StarVisibility(sunAltitude);

    DrawSky(g, sunAltitude);

    if (ShowMilkyWay && starVisibility > 0.05) DrawMilkyWay(g, starVisibility);
    if (ShowEquatorialGrid) DrawEquatorialGrid(g);
    if (ShowHorizonGrid) DrawHorizonGrid(g);
    if (ShowEcliptic) DrawEcliptic(g);
    if (ShowConstellationLines) DrawConstellationLines(g, starVisibility);
    if (ShowDeepSky) DrawDeepSkyObjects(g, starVisibility);

    DrawStars(g, starVisibility);

    if (ShowSolarSystem) DrawSolarSystem(g);
    if (ShowGround) DrawGround(g, sunAltitude);
    if (ShowConstellationNames) DrawConstellationNames(g, starVisibility);
    if (ShowCardinalPoints) DrawCardinalPoints(g);

    DrawSelection(g);
    DrawScaleBar(g);
  }

  void DrawSky(Graphics g, double sunAltitude)
  {
    (Color zenith, Color horizon) = SkyPalette.SkyGradient(sunAltitude);

    // Sample the true altitude at a few heights down the canvas and build a
    // gradient from that, so a tilted view still darkens in the right direction.
    const int Samples = 9;
    var colors = new Color[Samples];
    var positions = new float[Samples];

    for (int i = 0; i < Samples; i++)
    {
      float y = Height * i / (Samples - 1f);
      var direction = projection.Unproject(new PointF(Width / 2f, y));
      double altitude = Math.Asin(Math.Clamp(direction.U, -1, 1)) * AstroMath.Rad2Deg;

      double f = Math.Clamp(altitude / 42.0, 0, 1);
      f = f * f * (3 - 2 * f);   // ease so the brightening hugs the horizon
      colors[i] = SkyPalette.Blend(horizon, zenith, f);
      positions[i] = i / (Samples - 1f);
    }

    using var brush = new LinearGradientBrush(
        new RectangleF(0, -1, Width, Height + 2), colors[0], colors[^1], 90f);
    brush.InterpolationColors = new ColorBlend
    {
      Colors = colors,
      Positions = positions,
    };
    g.FillRectangle(brush, 0, 0, Width, Height);

    if (ShowAtmosphere) DrawTwilightGlow(g, sunAltitude);
  }

  /// <summary>The warm bloom around the Sun that makes sunset animations read correctly.</summary>
  void DrawTwilightGlow(Graphics g, double sunAltitude)
  {
    if (Catalog is null || sunAltitude < -14 || sunAltitude > 25) return;

    SolarSystemBody sun = Catalog.SolarSystem.Sun;
    if (!projection.TryProject(sun.Horizon, out PointF center, out double angle)) return;
    if (angle > 110) return;

    double strength = sunAltitude < 0
        ? Math.Clamp((sunAltitude + 14) / 14.0, 0, 1) * 0.75
        : Math.Clamp(1 - sunAltitude / 25.0, 0, 1) * 0.5;

    if (strength <= 0.01) return;

    float radius = Math.Max(Width, Height) * 0.75f;
    var bounds = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);

    using var path = new GraphicsPath();
    path.AddEllipse(bounds);

    using var glow = new PathGradientBrush(path)
    {
      CenterPoint = center,
      CenterColor = Color.FromArgb((int)(150 * strength), 255, 178, 108),
      SurroundColors = [Color.FromArgb(0, 255, 150, 90)],
    };

    var blend = new Blend(4)
    {
      Positions = [0f, 0.25f, 0.6f, 1f],
      Factors = [1f, 0.55f, 0.16f, 0f],
    };
    glow.Blend = blend;

    g.FillPath(glow, path);
  }

  void DrawMilkyWay(Graphics g, double visibility)
  {
    if (Catalog is null) return;

    var clip = new RectangleF(-Width, -Height, Width * 3, Height * 3);

    foreach (MilkyWayContour contour in Catalog.MilkyWay)
    {
      int alpha = (int)Math.Clamp((5 + contour.Level * 5) * visibility, 0, 255);
      if (alpha < 2) continue;

      using var brush = new SolidBrush(Color.FromArgb(alpha, 150, 168, 210));
      using var path = new GraphicsPath { FillMode = FillMode.Winding };

      foreach (HorizonVector[] ring in contour.HorizonRings)
      {
        PointF[] projected = ProjectRing(ring, clip);
        if (projected.Length > 2) path.AddPolygon(projected);
      }

      g.FillPath(brush, path);
    }
  }

  /// <summary>Projects a closed ring, dropping it entirely if it wraps behind the viewer.</summary>
  PointF[] ProjectRing(HorizonVector[] ring, RectangleF clip)
  {
    var points = new List<PointF>(ring.Length);
    bool anyNear = false;

    foreach (HorizonVector v in ring)
    {
      if (!projection.TryProject(v, out PointF p)) return [];
      if (Math.Abs(p.X) > clip.Right * 4 || Math.Abs(p.Y) > clip.Bottom * 4) return [];
      if (clip.Contains(p)) anyNear = true;
      points.Add(p);
    }

    return anyNear ? points.ToArray() : [];
  }

  void DrawProjectedPolyline(Graphics g, Pen pen, ReadOnlySpan<HorizonVector> points, bool close = false)
  {
    var run = new List<PointF>(points.Length);
    float maxJump = Math.Max(Width, Height) * 1.5f;

    void Flush()
    {
      if (run.Count > 1) g.DrawLines(pen, [.. run]);
      run.Clear();
    }

    for (int i = 0; i < points.Length; i++)
    {
      if (!projection.TryProject(points[i], out PointF p)) { Flush(); continue; }

      if (run.Count > 0)
      {
        PointF previous = run[^1];
        if (Math.Abs(p.X - previous.X) > maxJump || Math.Abs(p.Y - previous.Y) > maxJump)
          Flush();
      }

      run.Add(p);
    }

    if (close && run.Count > 2 && projection.TryProject(points[0], out PointF first))
      run.Add(first);

    Flush();
  }

  void DrawEquatorialGrid(Graphics g)
  {
    if (Catalog is null) return;

    double lst = Catalog.LocalSiderealTime;
    double lat = Catalog.ObserverLatitude;

    using var pen = new Pen(SkyPalette.EquatorialGrid, 1f);
    using var equatorPen = new Pen(Color.FromArgb(120, 70, 116, 150), 1.4f);

    // Hour circles every two hours of right ascension.
    for (int hour = 0; hour < 24; hour += 2)
    {
      double ra = hour * 15.0;
      var line = new HorizonVector[73];
      for (int i = 0; i < line.Length; i++)
        line[i] = AstroMath.EquatorialToHorizon(ra, -88 + i * 176 / 72.0, lst, lat);
      DrawProjectedPolyline(g, pen, line);
    }

    // Declination parallels every 15 degrees, with the equator picked out.
    for (int dec = -75; dec <= 75; dec += 15)
    {
      var circle = new HorizonVector[145];
      for (int i = 0; i < circle.Length; i++)
        circle[i] = AstroMath.EquatorialToHorizon(i * 2.5, dec, lst, lat);
      DrawProjectedPolyline(g, dec == 0 ? equatorPen : pen, circle);
    }
  }

  void DrawHorizonGrid(Graphics g)
  {
    using var pen = new Pen(SkyPalette.HorizonGrid, 1f);

    for (int az = 0; az < 360; az += 15)
    {
      var line = new HorizonVector[73];
      for (int i = 0; i < line.Length; i++)
      {
        double alt = -88 + i * 176 / 72.0;
        line[i] = HorizonVector.FromAltAz(alt * AstroMath.Deg2Rad, az * AstroMath.Deg2Rad);
      }
      DrawProjectedPolyline(g, pen, line);
    }

    for (int alt = -75; alt <= 75; alt += 15)
    {
      if (alt == 0) continue;   // the horizon itself is drawn separately
      var circle = new HorizonVector[145];
      for (int i = 0; i < circle.Length; i++)
        circle[i] = HorizonVector.FromAltAz(alt * AstroMath.Deg2Rad, i * 2.5 * AstroMath.Deg2Rad);
      DrawProjectedPolyline(g, pen, circle);
    }
  }

  void DrawEcliptic(Graphics g)
  {
    if (Catalog is null) return;

    double lst = Catalog.LocalSiderealTime;
    double lat = Catalog.ObserverLatitude;
    double obliquity = AstroMath.ObliquityJ2000;

    var line = new HorizonVector[181];
    for (int i = 0; i < line.Length; i++)
    {
      Equatorial eq = AstroMath.EclipticToEquatorial(i * 2.0, 0, obliquity);
      line[i] = AstroMath.EquatorialToHorizon(eq.RightAscension, eq.Declination, lst, lat);
    }

    using var pen = new Pen(SkyPalette.Ecliptic, 1.2f) { DashStyle = DashStyle.Dash };
    DrawProjectedPolyline(g, pen, line);
  }

  void DrawConstellationLines(Graphics g, double visibility)
  {
    if (Catalog is null) return;

    int alpha = (int)Math.Clamp(SkyPalette.ConstellationLine.A * (0.35 + 0.65 * visibility), 0, 255);
    using var pen = new Pen(SkyPalette.WithAlpha(SkyPalette.ConstellationLine, alpha / 255.0), 1f);

    foreach (Constellation constellation in Catalog.Constellations)
      foreach (HorizonVector[] segment in constellation.HorizonSegments)
        DrawProjectedPolyline(g, pen, segment);
  }

  void DrawStars(Graphics g, double visibility)
  {
    if (Catalog is null || visibility <= 0.01)
    {
      LastStarCount = 0;
      return;
    }

    var bounds = new RectangleF(-24, -24, Width + 48, Height + 48);

    // Stars shrink as the field widens so a wide view does not turn into porridge.
    double zoomScale = Math.Clamp(Math.Pow(60.0 / fieldOfView, 0.32), 0.62, 2.6);
    double limit = LimitingMagnitude;
    int drawn = 0;

    foreach (Star star in Catalog.StarsByBrightness)
    {
      if (star.Magnitude > limit) break;
      if (!projection.TryProject(star.Horizon, out PointF p)) continue;
      if (!bounds.Contains(p)) continue;

      star.ScreenPosition = p;
      star.WasDrawn = true;
      drawn++;

      double extinction = ShowAtmosphere ? Extinction(star.Altitude) : 0;
      double effectiveMagnitude = star.Magnitude + extinction;

      double alpha = Math.Clamp((limit - effectiveMagnitude) / 1.4, 0, 1) * visibility;
      if (alpha <= 0.02) continue;

      double radius = (0.62 + Math.Max(0, limit - effectiveMagnitude) * 0.42) * zoomScale;
      radius = Math.Min(radius, 11);

      Color color = StarColor.FromColorIndex(star.ColorIndex);
      DrawStarDisc(g, p, (float)radius, color, alpha, star.Magnitude);
    }

    LastStarCount = drawn;

    if (ShowStarNames) DrawStarLabels(g, visibility);
  }

  void DrawStarDisc(Graphics g, PointF p, float radius, Color color, double alpha, double magnitude)
  {
    if (radius < 1.0f)
    {
      // Below a pixel, antialiased ellipses turn into grey mush. A plain dot with
      // the alpha carrying the brightness reads far better.
      Brush dot = brushes.Get(color, alpha * radius);
      g.FillRectangle(dot, p.X, p.Y, 1f, 1f);
      return;
    }

    if (radius > 2.4f)
    {
      float halo = radius * 2.6f;
      Brush haloBrush = brushes.Get(color, alpha * 0.16);
      g.FillEllipse(haloBrush, p.X - halo, p.Y - halo, halo * 2, halo * 2);
    }

    Brush brush = brushes.Get(color, alpha);
    g.FillEllipse(brush, p.X - radius, p.Y - radius, radius * 2, radius * 2);

    // Only the handful of first-magnitude showpieces get spikes.
    if (magnitude < 0.6 && radius > 3.2f)
    {
      float spike = radius * 4.2f;
      using var pen = new Pen(brushes.GetColor(color, alpha * 0.32), 1f);
      g.DrawLine(pen, p.X - spike, p.Y, p.X + spike, p.Y);
      g.DrawLine(pen, p.X, p.Y - spike, p.X, p.Y + spike);
    }
  }

  /// <summary>Extra magnitudes are lost to the atmosphere at a given altitude.</summary>
  static double Extinction(double altitudeDeg)
  {
    if (altitudeDeg <= 0) return 8;
    double zenithAngle = (90 - altitudeDeg) * AstroMath.Deg2Rad;
    double airmass = 1.0 / (Math.Cos(zenithAngle) + 0.50572 * Math.Pow(96.07995 - zenithAngle * AstroMath.Rad2Deg, -1.6364));
    return 0.23 * Math.Min(airmass, 12);
  }

  void DrawStarLabels(Graphics g, double visibility)
  {
    if (Catalog is null) return;

    // Show more names as you zoom in, but never enough to clutter the chart.
    double nameLimit = Math.Clamp(1.6 + Math.Log2(90.0 / Math.Max(fieldOfView, 1)) * 1.5, 1.6, 5.2);
    nameLimit = Math.Min(nameLimit, LimitingMagnitude - 0.5);

    Brush brush = brushes.Get(SkyPalette.StarLabel, visibility);
    bool showBayer = fieldOfView < 55;

    foreach (Star star in Catalog.StarsByBrightness)
    {
      if (star.Magnitude > nameLimit) break;
      if (!star.WasDrawn) continue;

      string? label = star.ProperName;
      if (string.IsNullOrEmpty(label) && showBayer && !string.IsNullOrEmpty(star.Bayer))
        label = $"{star.Bayer} {star.Constellation}".Trim();
      if (string.IsNullOrEmpty(label)) continue;

      TryDrawLabel(g, label, star.ScreenPosition, 7, brush, labelFont);
    }
  }

  /// <summary>Draws a label unless something already occupies that patch of canvas.</summary>
  bool TryDrawLabel(Graphics g, string text, PointF anchor, float offset, Brush brush, Font font)
  {
    float x = anchor.X + offset;
    float y = anchor.Y - font.Height * 0.5f;

    if (x < -40 || y < -20 || x > Width || y > Height) return false;

    long cell = ((long)(x / 58) << 32) ^ (long)(y / 15);
    if (!labelCells.Add(cell)) return false;

    g.DrawString(text, font, brush, x, y);
    return true;
  }

  // deep sky objects are typically galaxies, nebulae, and clusters. They are drawn as symbols rather than discs.
  void DrawDeepSkyObjects(Graphics g, double visibility)
  {
    if (Catalog is null || visibility <= 0.05) return;

    var bounds = new RectangleF(-20, -20, Width + 40, Height + 40);
    double limit = Math.Min(LimitingMagnitude + 2.5, 9.0);

    using var pen = new Pen(SkyPalette.WithAlpha(SkyPalette.DeepSkyMarker, visibility), 1.1f);
    Brush labelBrush = brushes.Get(SkyPalette.DeepSkyMarker, visibility * 0.85);
    bool label = fieldOfView < 75;

    foreach (DeepSkyObject dso in Catalog.DeepSkyObjects)
    {
      if (dso.Magnitude > limit) continue;
      if (!projection.TryProject(dso.Horizon, out PointF p)) continue;
      if (!bounds.Contains(p)) continue;

      dso.ScreenPosition = p;
      dso.WasDrawn = true;

      float r = 4.5f;
      switch (dso.TypeCode)
      {
        case "g" or "ga" or "gg" or "cg":
          g.DrawEllipse(pen, p.X - r * 1.4f, p.Y - r * 0.75f, r * 2.8f, r * 1.5f);
          break;
        case "gc":
          g.DrawEllipse(pen, p.X - r, p.Y - r, r * 2, r * 2);
          g.DrawLine(pen, p.X - r, p.Y, p.X + r, p.Y);
          g.DrawLine(pen, p.X, p.Y - r, p.X, p.Y + r);
          break;
        case "pn":
          g.DrawEllipse(pen, p.X - r * 0.65f, p.Y - r * 0.65f, r * 1.3f, r * 1.3f);
          g.DrawLine(pen, p.X - r * 1.5f, p.Y, p.X - r * 0.7f, p.Y);
          g.DrawLine(pen, p.X + r * 0.7f, p.Y, p.X + r * 1.5f, p.Y);
          break;
        case "bn" or "en" or "rn" or "snr" or "sfr":
          g.DrawRectangle(pen, p.X - r, p.Y - r, r * 2, r * 2);
          break;
        default:
          {
            // Open clusters: a dashed circle, the standard chart symbol.
            using var dashed = new Pen(pen.Color, 1.1f) { DashStyle = DashStyle.Dot };
            g.DrawEllipse(dashed, p.X - r, p.Y - r, r * 2, r * 2);
            break;
          }
      }

      if (label && dso.ChartLabel is { Length: > 0 } text)
        TryDrawLabel(g, text, p, r + 3, labelBrush, labelFont);
    }
  }

  void DrawSolarSystem(Graphics g)
  {
    if (Catalog is null) return;

    var bounds = new RectangleF(-40, -40, Width + 80, Height + 80);

    foreach (SolarSystemBody body in Catalog.SolarSystem.All)
    {
      if (!projection.TryProject(body.Horizon, out PointF p)) continue;
      if (!bounds.Contains(p)) continue;

      body.ScreenPosition = p;
      body.WasDrawn = true;

      // True angular size, with a floor so a planet never vanishes into one pixel.
      float trueRadius = (float)(body.AngularDiameter * 0.5 * AstroMath.Deg2Rad * projection.PixelsPerRadian);
      float radius = Math.Max(trueRadius, body.Kind == SolarSystemBodyKind.Planet ? 2.2f : 4f);

      switch (body.Kind)
      {
        case SolarSystemBodyKind.Sun: DrawSun(g, p, radius, body); break;
        case SolarSystemBodyKind.Moon: DrawMoon(g, p, radius, body); break;
        default: DrawPlanet(g, p, radius, body); break;
      }

      if (fieldOfView < 130)
        TryDrawLabel(g, body.Name, p, radius + 4, brushes.Get(body.Tint, 0.9), labelFont);
    }
  }

  void DrawSun(Graphics g, PointF p, float radius, SolarSystemBody sun)
  {
    float glow = radius * 7f;
    using (var path = new GraphicsPath())
    {
      path.AddEllipse(p.X - glow, p.Y - glow, glow * 2, glow * 2);
      using var brush = new PathGradientBrush(path)
      {
        CenterPoint = p,
        CenterColor = Color.FromArgb(180, 255, 236, 180),
        SurroundColors = [Color.FromArgb(0, 255, 210, 130)],
      };
      g.FillPath(brush, path);
    }

    g.FillEllipse(brushes.Get(sun.Tint, 1.0), p.X - radius, p.Y - radius, radius * 2, radius * 2);
  }

  void DrawMoon(Graphics g, PointF p, float radius, SolarSystemBody moon)
  {
    // Earthshine: the unlit part is faintly visible, especially at thin crescents.
    g.FillEllipse(brushes.Get(moon.Tint, 0.13), p.X - radius, p.Y - radius, radius * 2, radius * 2);

    double illuminated = Math.Clamp(moon.IlluminatedFraction, 0, 1);
    using var lit = new GraphicsPath();

    // The terminator is a half-ellipse whose width tracks the illuminated fraction.
    float terminator = (float)((1 - 2 * illuminated) * radius);
    bool waxing = moon.BrightLimbAngle < 180;

    var box = new RectangleF(p.X - radius, p.Y - radius, radius * 2, radius * 2);
    lit.AddArc(box, waxing ? -90 : 90, 180);

    var terminatorBox = new RectangleF(
        p.X - Math.Abs(terminator), p.Y - radius, Math.Abs(terminator) * 2, radius * 2);

    if (Math.Abs(terminator) < 0.35f)
    {
      lit.AddLine(p.X, p.Y + radius, p.X, p.Y - radius);
    }
    else
    {
      bool bulgeSameSide = waxing ? terminator < 0 : terminator > 0;
      lit.AddArc(terminatorBox, waxing ? 90 : -90, bulgeSameSide ? -180 : 180);
    }

    lit.CloseFigure();
    g.FillPath(brushes.Get(moon.Tint, 0.97), lit);

    using var rim = new Pen(brushes.GetColor(moon.Tint, 0.3), 1f);
    g.DrawEllipse(rim, box);
  }

  void DrawPlanet(Graphics g, PointF p, float radius, SolarSystemBody planet)
  {
    float halo = radius * 3.2f;
    g.FillEllipse(brushes.Get(planet.Tint, 0.14), p.X - halo, p.Y - halo, halo * 2, halo * 2);
    g.FillEllipse(brushes.Get(planet.Tint, 1.0), p.X - radius, p.Y - radius, radius * 2, radius * 2);

    // Saturn earns its rings once the disc is big enough to hang them on.
    if (planet.Name == "Saturn" && radius > 3.5f)
    {
      using var pen = new Pen(brushes.GetColor(planet.Tint, 0.75), 1.2f);
      g.DrawEllipse(pen, p.X - radius * 2.3f, p.Y - radius * 0.55f, radius * 4.6f, radius * 1.1f);
    }
  }

  void DrawGround(Graphics g, double sunAltitude)
  {
    using var groundBrush = new SolidBrush(SkyPalette.Ground);
    using var horizonPen = new Pen(SkyPalette.HorizonLine, 1.4f);

    if (projection.TryGetHorizonCircle(out PointF center, out float radius, out bool groundInside))
    {
      var box = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);

      using var path = new GraphicsPath();
      path.AddEllipse(box);

      if (groundInside)
      {
        g.FillPath(groundBrush, path);
      }
      else
      {
        using var outside = new Region(new RectangleF(-1, -1, Width + 2, Height + 2));
        outside.Exclude(path);
        g.FillRegion(groundBrush, outside);
      }

      DrawHorizonHaze(g, path, box, groundInside, sunAltitude);
      g.DrawEllipse(horizonPen, box);
    }
    else
    {
      PointF[] halfPlane = projection.GetHorizonHalfPlane();
      g.FillPolygon(groundBrush, halfPlane);
      g.DrawLine(horizonPen, halfPlane[0], halfPlane[1]);
    }
  }

  /// <summary>A thin band of light hugging the sky side of the horizon.</summary>
  void DrawHorizonHaze(Graphics g, GraphicsPath horizon, RectangleF box, bool groundInside, double sunAltitude)
  {
    if (!ShowAtmosphere) return;

    (_, Color horizonColor) = SkyPalette.SkyGradient(sunAltitude);
    int steps = 7;

    for (int i = 0; i < steps; i++)
    {
      float grow = (i + 1) * 3.5f * (groundInside ? 1 : -1);
      var band = RectangleF.Inflate(box, grow, grow);
      if (band.Width <= 2 || band.Height <= 2) continue;

      int alpha = (int)(26 * (1 - i / (float)steps));
      using var pen = new Pen(Color.FromArgb(alpha, horizonColor.R, horizonColor.G, horizonColor.B), 4f);
      g.DrawEllipse(pen, band);
    }
  }

  void DrawCardinalPoints(Graphics g)
  {
    (string Label, double Azimuth)[] points =
    [
        ("N", 0), ("NE", 45), ("E", 90), ("SE", 135),
            ("S", 180), ("SW", 225), ("W", 270), ("NW", 315),
        ];

    using var format = new StringFormat
    {
      Alignment = StringAlignment.Center,
      LineAlignment = StringAlignment.Far,
    };

    using var tickPen = new Pen(SkyPalette.Cardinal, 1.4f);

    foreach ((string label, double azimuth) in points)
    {
      bool major = label.Length == 1;
      var direction = HorizonVector.FromAltAz(0, azimuth * AstroMath.Deg2Rad);
      if (!projection.TryProject(direction, out PointF p, out double angle)) continue;
      if (angle > 92 || p.X < -60 || p.X > Width + 60 || p.Y < -40 || p.Y > Height + 40) continue;

      var above = HorizonVector.FromAltAz(2.2 * AstroMath.Deg2Rad, azimuth * AstroMath.Deg2Rad);
      if (!projection.TryProject(above, out PointF top)) continue;

      g.DrawLine(tickPen, p, top);
      g.DrawString(label, major ? cardinalFont : labelFont,
          brushes.Get(SkyPalette.Cardinal, major ? 1.0 : 0.6), top.X, top.Y, format);
    }
  }

  void DrawConstellationNames(Graphics g, double visibility)
  {
    if (Catalog is null || visibility < 0.15) return;

    Brush brush = brushes.Get(SkyPalette.ConstellationName, visibility);
    using var format = new StringFormat { Alignment = StringAlignment.Center };

    foreach (Constellation constellation in Catalog.Constellations)
    {
      if (!projection.TryProject(constellation.LabelHorizon, out PointF p, out double angle)) continue;
      if (angle > fieldOfView * 0.72) continue;
      if (p.X < 0 || p.X > Width || p.Y < 0 || p.Y > Height) continue;

      long cell = ((long)(p.X / 80) << 32) ^ (long)(p.Y / 24);
      if (!labelCells.Add(cell)) continue;

      g.DrawString(constellation.Name, constellationFont, brush, p.X, p.Y, format);
    }
  }

  void DrawSelection(Graphics g)
  {
    if (Selected is null || !Selected.WasDrawn) return;

    PointF p = Selected.ScreenPosition;
    using var pen = new Pen(SkyPalette.Selection, 1.3f);

    const float Gap = 7f;
    const float Arm = 7f;

    g.DrawLine(pen, p.X - Gap - Arm, p.Y, p.X - Gap, p.Y);
    g.DrawLine(pen, p.X + Gap, p.Y, p.X + Gap + Arm, p.Y);
    g.DrawLine(pen, p.X, p.Y - Gap - Arm, p.X, p.Y - Gap);
    g.DrawLine(pen, p.X, p.Y + Gap, p.X, p.Y + Gap + Arm);

    g.DrawString(Selected.DisplayName, labelFont,
        brushes.Get(SkyPalette.Selection, 1.0), p.X + Gap + Arm + 3, p.Y - labelFont.Height - 2);
  }

  /// <summary>A short ruler in the corner so the field of view has a physical meaning.</summary>
  void DrawScaleBar(Graphics g)
  {
    double[] candidates = [0.5, 1, 2, 5, 10, 15, 30, 45, 60];
    double span = candidates.FirstOrDefault(c => c >= fieldOfView / 5, 90);

    double pixels = 2 * Math.Tan(span * AstroMath.Deg2Rad / 2) * projection.PixelsPerRadian;
    if (pixels < 20 || pixels > Width - 40) return;

    float y = Height - 22;
    float x0 = 20;
    float x1 = x0 + (float)pixels;

    using var pen = new Pen(SkyPalette.WithAlpha(SkyPalette.TextDim, 0.75), 1f);
    g.DrawLine(pen, x0, y, x1, y);
    g.DrawLine(pen, x0, y - 3, x0, y + 3);
    g.DrawLine(pen, x1, y - 3, x1, y + 3);

    string text = span < 1 ? $"{span * 60:0}'" : $"{span:0.#}°";
    g.DrawString(text, readoutFont, brushes.Get(SkyPalette.TextDim, 0.9), (x0 + x1) / 2 - 10, y - 17);
  }

  protected override void OnFontChanged(EventArgs e)
  {
    base.OnFontChanged(e);
    CreateFonts();
  }

  /// <summary>
  /// Releases the fonts and cached brushes. Called from the designer's
  /// <see cref="Dispose(bool)"/>, which is where the Windows Forms template keeps it.
  /// </summary>
  void ReleaseDrawingResources()
  {
    labelFont?.Dispose();
    constellationFont?.Dispose();
    cardinalFont?.Dispose();
    readoutFont?.Dispose();
    brushes.Dispose();
  }

  #region Events
  public delegate void VoidHandler();
  public event VoidHandler? SelectionChanged;
  void FireSelectionChanged()
  {
    SelectionChanged?.Invoke();
  }

  public event VoidHandler? ViewChanged;
  void FireViewChanged()
  {
    ViewChanged?.Invoke();
  }

  #endregion
}

/// <summary>
/// Allocating a brush per star would create thousands of GDI objects per frame, so
/// colours are quantised and the brushes reused.
/// </summary>
internal sealed class BrushCache : IDisposable
{
  readonly Dictionary<int, SolidBrush> _brushes = [];

  public Color GetColor(Color color, double alpha)
  {
    int a = (int)Math.Clamp(Math.Round(alpha * 255 / 4.0) * 4, 0, 255);
    return Color.FromArgb(a, color.R, color.G, color.B);
  }

  public SolidBrush Get(Color color, double alpha)
  {
    Color quantised = GetColor(color, alpha);
    int key = quantised.ToArgb();

    if (!_brushes.TryGetValue(key, out SolidBrush? brush))
    {
      brush = new SolidBrush(quantised);
      _brushes[key] = brush;
    }

    return brush;
  }

  public void Dispose()
  {
    foreach (SolidBrush brush in _brushes.Values) brush.Dispose();
    _brushes.Clear();
  }
}
