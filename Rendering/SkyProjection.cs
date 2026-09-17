namespace SkyViewer;

/// <summary>
/// Stereographic projection of the celestial sphere onto the canvas, centred on
/// wherever the observer is looking.
///
/// Stereographic is the projection real planetariums and star atlases use: it keeps
/// shapes locally correct, so constellations stay recognisable no matter how far from
/// the centre of the view they sit. The cost is that area grows toward the edges,
/// which is why the field of view is capped well short of 180 degrees.
/// </summary>
public sealed class SkyProjection
{
  HorizonVector forward, right, up;

  float centerX, centerY;
  double scale;

  /// <summary>Centre of the view in degrees above the horizon.</summary>
  public double CenterAltitude { get; private set; }

  /// <summary>Centre of the view in degrees clockwise from north.</summary>
  public double CenterAzimuth { get; private set; }

  /// <summary>Vertical field of view in degrees.</summary>
  public double FieldOfView { get; private set; } = 90;

  public SizeF Viewport { get; private set; }

  /// <summary>Pixels per radian at the centre of the view; used to size discs.</summary>
  public double PixelsPerRadian { get; private set; }

  public void Configure(double centerAltitudeDeg, double centerAzimuthDeg, double fovDeg, SizeF viewport)
  {
    CenterAltitude = Math.Clamp(centerAltitudeDeg, -89.9, 89.9);
    CenterAzimuth = AstroMath.Normalize360(centerAzimuthDeg);
    FieldOfView = Math.Clamp(fovDeg, 0.5, 170.0);
    Viewport = viewport;

    double alt = CenterAltitude * AstroMath.Deg2Rad;
    double az = CenterAzimuth * AstroMath.Deg2Rad;

    double sinAlt = Math.Sin(alt), cosAlt = Math.Cos(alt);
    double sinAz = Math.Sin(az), cosAz = Math.Cos(az);

    forward = new HorizonVector(cosAlt * cosAz, cosAlt * sinAz, sinAlt);
    right = new HorizonVector(-sinAz, cosAz, 0);                        // east is screen right
    up = new HorizonVector(-sinAlt * cosAz, -sinAlt * sinAz, cosAlt);   // toward the zenith

    centerX = viewport.Width / 2f;
    centerY = viewport.Height / 2f;

    // A point theta from the centre lands at radius 2*tan(theta/2) on the
    // projection plane, so half the vertical FOV must reach half the canvas height.
    double halfFov = FieldOfView * 0.5 * AstroMath.Deg2Rad;
    scale = viewport.Height * 0.5 / (2.0 * Math.Tan(halfFov * 0.5));
    PixelsPerRadian = scale;
  }

  /// <summary>
  /// Projects a horizon vector. Returns false for points behind the observer's
  /// head, where the projection blows up.
  /// </summary>
  public bool TryProject(in HorizonVector v, out PointF point)
  {
    double c = v.Dot(forward);
    if (c <= -0.9995)
    {
      point = default;
      return false;
    }

    double k = 2.0 / (1.0 + c);
    double x = k * v.Dot(right);
    double y = k * v.Dot(up);

    point = new PointF(
        (float)(centerX + x * scale),
        (float)(centerY - y * scale));
    return true;
  }

  /// <summary>Projects and also reports the angular separation from the view centre.</summary>
  public bool TryProject(in HorizonVector v, out PointF point, out double angleFromCenterDeg)
  {
    double c = Math.Clamp(v.Dot(forward), -1, 1);
    angleFromCenterDeg = Math.Acos(c) * AstroMath.Rad2Deg;
    return TryProject(v, out point);
  }

  /// <summary>Screen point back to a direction in the sky.</summary>
  public HorizonVector Unproject(PointF point)
  {
    double x = (point.X - centerX) / scale;
    double y = (centerY - point.Y) / scale;

    double radius = Math.Sqrt(x * x + y * y);
    double theta = 2.0 * Math.Atan(radius / 2.0);

    double sinTheta = Math.Sin(theta);
    double cosTheta = Math.Cos(theta);

    double a = radius < 1e-12 ? 0 : x * sinTheta / radius;
    double b = radius < 1e-12 ? 0 : y * sinTheta / radius;

    return new HorizonVector(
        a * right.N + b * up.N + cosTheta * forward.N,
        a * right.E + b * up.E + cosTheta * forward.E,
        a * right.U + b * up.U + cosTheta * forward.U);
  }

  /// <summary>
  /// The horizon is a great circle, and stereographic projection maps great circles
  /// to circles or straight lines. Getting that shape exactly lets the ground be
  /// filled with a clip region instead of a sampled polygon.
  /// </summary>
  public bool TryGetHorizonCircle(out PointF center, out float radius, out bool groundIsInside)
  {
    center = default;
    radius = 0;
    groundIsInside = false;

    // Three well-separated points on the horizon, chosen relative to where we look.
    Span<PointF> samples = stackalloc PointF[3];
    for (int i = 0; i < 3; i++)
    {
      double az = (CenterAzimuth + 120.0 * i) * AstroMath.Deg2Rad;
      var v = new HorizonVector(Math.Cos(az), Math.Sin(az), 0);
      if (!TryProject(v, out samples[i])) return false;
    }

    double ax = samples[0].X, ay = samples[0].Y;
    double bx = samples[1].X, by = samples[1].Y;
    double cx = samples[2].X, cy = samples[2].Y;

    double d = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
    if (Math.Abs(d) < 1e-6) return false;   // horizon is a straight line through the centre

    double a2 = ax * ax + ay * ay;
    double b2 = bx * bx + by * by;
    double c2 = cx * cx + cy * cy;

    double ux = (a2 * (by - cy) + b2 * (cy - ay) + c2 * (ay - by)) / d;
    double uy = (a2 * (cx - bx) + b2 * (ax - cx) + c2 * (bx - ax)) / d;

    double r = Math.Sqrt((ax - ux) * (ax - ux) + (ay - uy) * (ay - uy));
    if (double.IsNaN(r) || r > 2e6) return false;

    center = new PointF((float)ux, (float)uy);
    radius = (float)r;

    // The nadir is always below the horizon, so whichever side it lands on is ground.
    var nadir = new HorizonVector(0, 0, -1);
    if (!TryProject(nadir, out PointF nadirPoint)) return false;

    double dx = nadirPoint.X - ux;
    double dy = nadirPoint.Y - uy;
    groundIsInside = dx * dx + dy * dy < r * r;
    return true;
  }

  /// <summary>
  /// Fallback for a view centred exactly on the horizon, where the projected horizon
  /// degenerates into a straight line. Returns the line as a very wide quad covering
  /// the ground side.
  /// </summary>
  public PointF[] GetHorizonHalfPlane()
  {
    double az = CenterAzimuth * AstroMath.Deg2Rad;
    var left = new HorizonVector(Math.Cos(az - Math.PI / 2), Math.Sin(az - Math.PI / 2), 0);
    var rightPoint = new HorizonVector(Math.Cos(az + Math.PI / 2), Math.Sin(az + Math.PI / 2), 0);

    TryProject(left, out PointF p0);
    TryProject(rightPoint, out PointF p1);

    float far = Math.Max(Viewport.Width, Viewport.Height) * 4f;

    // Extend well past the canvas so the fill covers the whole lower half.
    float dx = p1.X - p0.X, dy = p1.Y - p0.Y;
    float len = MathF.Max(MathF.Sqrt(dx * dx + dy * dy), 1e-3f);
    dx /= len; dy /= len;

    PointF a = new(p0.X - dx * far, p0.Y - dy * far);
    PointF b = new(p1.X + dx * far, p1.Y + dy * far);

    return [
      a, 
      b, 
      new PointF(b.X, b.Y + far), 
      new PointF(a.X, a.Y + far),];
  }
}
