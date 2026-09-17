namespace SkyViewer;

/// <summary>
/// A unit vector in the local horizon frame.
/// N points at the north cardinal point, E at the east cardinal point, U at the zenith.
/// </summary>
public readonly record struct HorizonVector(double N, double E, double U)
{
    public double Dot(in HorizonVector o) => N * o.N + E * o.E + U * o.U;

    public static HorizonVector FromAltAz(double altRad, double azRad)
    {
        double c = Math.Cos(altRad);
        return new HorizonVector(c * Math.Cos(azRad), c * Math.Sin(azRad), Math.Sin(altRad));
    }
}

/// <summary>Equatorial position in degrees, plus the distance in AU where meaningful.</summary>
public readonly record struct Equatorial(double RightAscension, double Declination, double DistanceAu = 0);

public static class AstroMath
{
    public const double Deg2Rad = Math.PI / 180.0;
    public const double Rad2Deg = 180.0 / Math.PI;

    /// <summary>Obliquity of the ecliptic at J2000, in degrees.</summary>
    public const double ObliquityJ2000 = 23.4392911;

    public static double Normalize360(double degrees)
    {
        degrees %= 360.0;
        return degrees < 0 ? degrees + 360.0 : degrees;
    }

    /// <summary>Wraps an angle to the range -180..+180.</summary>
    public static double NormalizeSigned(double degrees)
    {
        degrees = Normalize360(degrees);
        return degrees > 180.0 ? degrees - 360.0 : degrees;
    }

    // ------------------------------------------------------------------ time

    /// <summary>Julian Day for an instant expressed in UTC (Gregorian calendar).</summary>
    public static double JulianDay(DateTime utc)
    {
        int year = utc.Year;
        int month = utc.Month;
        double day = utc.Day
                     + (utc.Hour
                        + (utc.Minute
                           + (utc.Second + utc.Millisecond / 1000.0) / 60.0) / 60.0) / 24.0;

        if (month <= 2)
        {
            year -= 1;
            month += 12;
        }

        int a = (int)Math.Floor(year / 100.0);
        int b = 2 - a + a / 4;

        return Math.Floor(365.25 * (year + 4716))
               + Math.Floor(30.6001 * (month + 1))
               + day + b - 1524.5;
    }

    /// <summary>Julian centuries elapsed since the J2000.0 epoch.</summary>
    public static double JulianCenturies(double julianDay) => (julianDay - 2451545.0) / 36525.0;

    /// <summary>Greenwich mean sidereal time in degrees.</summary>
    public static double GreenwichMeanSiderealTime(double julianDay)
    {
        double d = julianDay - 2451545.0;
        double t = d / 36525.0;
        double gmst = 280.46061837
                      + 360.98564736629 * d
                      + 0.000387933 * t * t
                      - t * t * t / 38710000.0;
        return Normalize360(gmst);
    }

    /// <summary>Local mean sidereal time in degrees. Longitude is positive east of Greenwich.</summary>
    public static double LocalSiderealTime(double julianDay, double longitudeEast)
        => Normalize360(GreenwichMeanSiderealTime(julianDay) + longitudeEast);

    // ----------------------------------------------------------- coordinates

    /// <summary>
    /// Converts an equatorial position to a horizon unit vector.
    /// The hour angle runs westward, so +E in the result really is east on the sky.
    /// </summary>
    public static HorizonVector EquatorialToHorizon(
        double raDeg, double decDeg, double lstDeg, double latDeg)
    {
        double h = (lstDeg - raDeg) * Deg2Rad;
        double dec = decDeg * Deg2Rad;
        double lat = latDeg * Deg2Rad;

        double cosDec = Math.Cos(dec);
        double x = cosDec * Math.Cos(h);   // toward the meridian at dec = 0
        double y = cosDec * Math.Sin(h);   // toward the west point of the equator
        double z = Math.Sin(dec);          // toward the north celestial pole

        double sinLat = Math.Sin(lat);
        double cosLat = Math.Cos(lat);

        return new HorizonVector(
            N: -x * sinLat + z * cosLat,
            E: -y,
            U: x * cosLat + z * sinLat);
    }

    public static (double AltitudeDeg, double AzimuthDeg) ToAltAz(in HorizonVector v)
    {
        double alt = Math.Asin(Math.Clamp(v.U, -1.0, 1.0)) * Rad2Deg;
        double az = Normalize360(Math.Atan2(v.E, v.N) * Rad2Deg);
        return (alt, az);
    }

    public static (double AltitudeDeg, double AzimuthDeg) EquatorialToAltAz(
        double raDeg, double decDeg, double lstDeg, double latDeg)
        => ToAltAz(EquatorialToHorizon(raDeg, decDeg, lstDeg, latDeg));

    /// <summary>Inverse of <see cref="EquatorialToHorizon"/>: horizon vector back to RA/Dec.</summary>
    public static Equatorial HorizonToEquatorial(in HorizonVector v, double lstDeg, double latDeg)
    {
        double lat = latDeg * Deg2Rad;
        double sinLat = Math.Sin(lat);
        double cosLat = Math.Cos(lat);

        double x = -v.N * sinLat + v.U * cosLat;
        double y = -v.E;
        double z = v.N * cosLat + v.U * sinLat;

        double dec = Math.Asin(Math.Clamp(z, -1.0, 1.0)) * Rad2Deg;
        double hourAngle = Math.Atan2(y, x) * Rad2Deg;
        return new Equatorial(Normalize360(lstDeg - hourAngle), dec);
    }

    /// <summary>Ecliptic (longitude, latitude) in degrees to equatorial, for a given obliquity.</summary>
    public static Equatorial EclipticToEquatorial(
        double lonDeg, double latDeg, double obliquityDeg, double distanceAu = 0)
    {
        double lon = lonDeg * Deg2Rad;
        double lat = latDeg * Deg2Rad;
        double eps = obliquityDeg * Deg2Rad;

        double sinLon = Math.Sin(lon);
        double cosLat = Math.Cos(lat);
        double sinLat = Math.Sin(lat);

        double ra = Math.Atan2(sinLon * Math.Cos(eps) - Math.Tan(lat) * Math.Sin(eps), Math.Cos(lon));
        double dec = Math.Asin(Math.Clamp(sinLat * Math.Cos(eps) + cosLat * Math.Sin(eps) * sinLon, -1, 1));

        return new Equatorial(Normalize360(ra * Rad2Deg), dec * Rad2Deg, distanceAu);
    }

    /// <summary>Mean obliquity of the ecliptic in degrees (IAU 1980 polynomial).</summary>
    public static double MeanObliquity(double centuriesSinceJ2000)
    {
        double t = centuriesSinceJ2000;
        return 23.439291111
               - 0.0130041667 * t
               - 1.638889e-7 * t * t
               + 5.036111e-7 * t * t * t;
    }

    // ------------------------------------------------------------ precession

    /// <summary>
    /// Precesses a J2000 catalogue position to the equinox of date (IAU 1976 angles).
    /// Over a human lifetime this is under a degree, but it keeps bright stars sitting
    /// exactly on their constellation lines.
    /// </summary>
    public static Equatorial PrecessFromJ2000(double raDeg, double decDeg, double centuriesSinceJ2000)
    {
        double t = centuriesSinceJ2000;
        if (Math.Abs(t) < 1e-9) return new Equatorial(raDeg, decDeg);

        const double ArcsecToRad = Math.PI / (180.0 * 3600.0);
        double zeta = (2306.2181 * t + 0.30188 * t * t + 0.017998 * t * t * t) * ArcsecToRad;
        double z = (2306.2181 * t + 1.09468 * t * t + 0.018203 * t * t * t) * ArcsecToRad;
        double theta = (2004.3109 * t - 0.42665 * t * t - 0.041833 * t * t * t) * ArcsecToRad;

        double ra0 = raDeg * Deg2Rad;
        double dec0 = decDeg * Deg2Rad;

        double cosDec0 = Math.Cos(dec0);
        double sinDec0 = Math.Sin(dec0);
        double cosRaZeta = Math.Cos(ra0 + zeta);
        double sinRaZeta = Math.Sin(ra0 + zeta);

        double a = cosDec0 * sinRaZeta;
        double b = Math.Cos(theta) * cosDec0 * cosRaZeta - Math.Sin(theta) * sinDec0;
        double c = Math.Sin(theta) * cosDec0 * cosRaZeta + Math.Cos(theta) * sinDec0;

        return new Equatorial(
            Normalize360((Math.Atan2(a, b) + z) * Rad2Deg),
            Math.Asin(Math.Clamp(c, -1, 1)) * Rad2Deg);
    }

    // ----------------------------------------------------------- atmosphere

    /// <summary>
    /// Bennett's refraction formula. Returns the altitude an object appears to have,
    /// which lifts objects near the horizon by roughly half a degree.
    /// </summary>
    public static double ApplyRefraction(double trueAltitudeDeg)
    {
        if (trueAltitudeDeg < -2.0) return trueAltitudeDeg;
        double r = 1.02 / Math.Tan((trueAltitudeDeg + 10.3 / (trueAltitudeDeg + 5.11)) * Deg2Rad);
        return trueAltitudeDeg + r / 60.0;
    }

    // --------------------------------------------------------------- display

    public static string FormatRightAscension(double raDeg)
    {
        double hours = Normalize360(raDeg) / 15.0;
        int h = (int)hours;
        double remainder = (hours - h) * 60.0;
        int m = (int)remainder;
        double s = (remainder - m) * 60.0;
        return $"{h:00}h {m:00}m {s:00.0}s";
    }

    public static string FormatDegrees(double degrees, bool signed = true)
    {
        char sign = degrees < 0 ? '-' : '+';
        double a = Math.Abs(degrees);
        int d = (int)a;
        double remainder = (a - d) * 60.0;
        int m = (int)remainder;
        double s = (remainder - m) * 60.0;
        return signed ? $"{sign}{d:00}° {m:00}' {s:00.0}\"" : $"{d:000}° {m:00}' {s:00.0}\"";
    }
}
