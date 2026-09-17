namespace SkyViewer;

/// <summary>
/// Positions for the Sun, the Moon and the naked-eye planets.
///
/// Accuracy is what a visual planetarium needs rather than what an ephemeris needs:
/// the planets use the JPL "approximate positions" element set, good to roughly an
/// arcminute between 1800 and 2050, and the Moon uses a truncated lunar theory good to
/// a couple of arcminutes. Nobody will see the difference at 60 degrees per screen.
/// </summary>
public sealed class SolarSystem
{
    private readonly SolarSystemBody _sun;
    private readonly SolarSystemBody _moon;
    private readonly SolarSystemBody[] _planets;

    public SolarSystem()
    {
        _sun = new SolarSystemBody
        {
            Name = "Sun",
            Kind = SolarSystemBodyKind.Sun,
            Tint = Color.FromArgb(255, 245, 200),
        };

        _moon = new SolarSystemBody
        {
            Name = "Moon",
            Kind = SolarSystemBodyKind.Moon,
            Tint = Color.FromArgb(232, 230, 220),
        };

        _planets =
        [
            MakePlanet("Mercury", Color.FromArgb(198, 190, 178)),
            MakePlanet("Venus",   Color.FromArgb(252, 245, 214)),
            MakePlanet("Mars",    Color.FromArgb(226, 123, 82)),
            MakePlanet("Jupiter", Color.FromArgb(236, 214, 178)),
            MakePlanet("Saturn",  Color.FromArgb(226, 208, 150)),
            MakePlanet("Uranus",  Color.FromArgb(170, 218, 224)),
            MakePlanet("Neptune", Color.FromArgb(138, 168, 232)),
        ];

        All = [_sun, _moon, .. _planets];
    }

    private static SolarSystemBody MakePlanet(string name, Color tint) =>
        new() { Name = name, Kind = SolarSystemBodyKind.Planet, Tint = tint };

    public SolarSystemBody Sun => _sun;
    public SolarSystemBody Moon => _moon;
    public IReadOnlyList<SolarSystemBody> Planets => _planets;

    /// <summary>Every solar system body, Sun first.</summary>
    public IReadOnlyList<SolarSystemBody> All { get; }

    public void Update(double julianDay, double lstDeg, double latitudeDeg, bool refract = true)
    {
        double t = AstroMath.JulianCenturies(julianDay);
        double obliquity = AstroMath.MeanObliquity(t);

        UpdateSun(t, obliquity);
        UpdatePlanets(t);
        UpdateMoon(julianDay, obliquity);

        foreach (SolarSystemBody body in All)
        {
            body.Horizon = AstroMath.EquatorialToHorizon(
                body.RaOfDate, body.DecOfDate, lstDeg, latitudeDeg);
            (body.Altitude, body.Azimuth) = AstroMath.ToAltAz(body.Horizon);
        }

        // The Moon is close enough that an observer on the surface sees it up to a
        // degree lower than a hypothetical observer at the centre of the Earth.
        ApplyLunarParallax(lstDeg, latitudeDeg);

        if (refract)
        {
            foreach (SolarSystemBody body in All)
            {
                if (body.Altitude > 12) continue;
                double apparent = AstroMath.ApplyRefraction(body.Altitude);
                body.Altitude = apparent;
                body.Horizon = HorizonVector.FromAltAz(
                    apparent * AstroMath.Deg2Rad, body.Azimuth * AstroMath.Deg2Rad);
            }
        }

        UpdateMoonPhase();
    }

    // ------------------------------------------------------------------- Sun

    private void UpdateSun(double t, double obliquity)
    {
        double l0 = 280.46646 + 36000.76983 * t + 0.0003032 * t * t;
        double m = AstroMath.Normalize360(357.52911 + 35999.05029 * t - 0.0001537 * t * t);
        double mRad = m * AstroMath.Deg2Rad;

        double center = (1.914602 - 0.004817 * t - 0.000014 * t * t) * Math.Sin(mRad)
                        + (0.019993 - 0.000101 * t) * Math.Sin(2 * mRad)
                        + 0.000289 * Math.Sin(3 * mRad);

        double trueLongitude = l0 + center;
        double omega = 125.04 - 1934.136 * t;
        double apparentLongitude = trueLongitude - 0.00569 - 0.00478 * Math.Sin(omega * AstroMath.Deg2Rad);

        double eccentricity = 0.016708634 - 0.000042037 * t - 0.0000001267 * t * t;
        double trueAnomaly = m + center;
        double radiusAu = 1.000001018 * (1 - eccentricity * eccentricity)
                          / (1 + eccentricity * Math.Cos(trueAnomaly * AstroMath.Deg2Rad));

        Equatorial eq = AstroMath.EclipticToEquatorial(apparentLongitude, 0, obliquity, radiusAu);
        _sun.RaOfDate = eq.RightAscension;
        _sun.DecOfDate = eq.Declination;
        _sun.AngularDiameter = 0.533128 / radiusAu;
        _sun.Magnitude = -26.74;
        _sun.IlluminatedFraction = 1.0;

        SunEclipticLongitude = AstroMath.Normalize360(apparentLongitude);
        SunDistanceAu = radiusAu;
    }

    /// <summary>Apparent ecliptic longitude of the Sun in degrees; used for the lunar phase.</summary>
    public double SunEclipticLongitude { get; private set; }

    public double SunDistanceAu { get; private set; } = 1.0;

    // ------------------------------------------------------------------ Moon

    private double _moonEclipticLongitude;
    private double _moonEclipticLatitude;
    private double _moonDistanceEarthRadii = 60.0;

    private void UpdateMoon(double julianDay, double obliquity)
    {
        // Days since the 1999-12-31 00:00 UT epoch used by the element set below.
        double d = julianDay - 2451543.5;

        double nodeLon = 125.1228 - 0.0529538083 * d;
        const double inclination = 5.1454;
        double argPeriapsis = 318.0634 + 0.1643573223 * d;
        const double semiMajor = 60.2666;          // Earth radii
        const double eccentricity = 0.054900;
        double meanAnomaly = 115.3654 + 13.0649929509 * d;

        double sunMeanAnomaly = 356.0470 + 0.9856002585 * d;
        double sunArgPeriapsis = 282.9404 + 0.0000470935 * d;
        double sunMeanLongitude = sunMeanAnomaly + sunArgPeriapsis;

        double eccentricAnomaly = SolveKepler(meanAnomaly, eccentricity);
        double eRad = eccentricAnomaly * AstroMath.Deg2Rad;

        double xv = semiMajor * (Math.Cos(eRad) - eccentricity);
        double yv = semiMajor * (Math.Sqrt(1 - eccentricity * eccentricity) * Math.Sin(eRad));
        double distance = Math.Sqrt(xv * xv + yv * yv);
        double trueAnomaly = Math.Atan2(yv, xv) * AstroMath.Rad2Deg;

        double u = (trueAnomaly + argPeriapsis) * AstroMath.Deg2Rad;
        double nodeRad = nodeLon * AstroMath.Deg2Rad;
        double incRad = inclination * AstroMath.Deg2Rad;

        double xe = distance * (Math.Cos(nodeRad) * Math.Cos(u) - Math.Sin(nodeRad) * Math.Sin(u) * Math.Cos(incRad));
        double ye = distance * (Math.Sin(nodeRad) * Math.Cos(u) + Math.Cos(nodeRad) * Math.Sin(u) * Math.Cos(incRad));
        double ze = distance * Math.Sin(u) * Math.Sin(incRad);

        double longitude = Math.Atan2(ye, xe) * AstroMath.Rad2Deg;
        double latitude = Math.Atan2(ze, Math.Sqrt(xe * xe + ye * ye)) * AstroMath.Rad2Deg;

        // Main periodic perturbations. The first two terms, evection and variation,
        // are worth more than a degree between them and are very visible.
        double moonMeanLongitude = nodeLon + argPeriapsis + meanAnomaly;
        double elongation = moonMeanLongitude - sunMeanLongitude;
        double argLatitude = moonMeanLongitude - nodeLon;

        double mm = meanAnomaly * AstroMath.Deg2Rad;
        double ms = sunMeanAnomaly * AstroMath.Deg2Rad;
        double dd = elongation * AstroMath.Deg2Rad;
        double ff = argLatitude * AstroMath.Deg2Rad;

        longitude +=
            -1.274 * Math.Sin(mm - 2 * dd)
            + 0.658 * Math.Sin(2 * dd)
            - 0.186 * Math.Sin(ms)
            - 0.059 * Math.Sin(2 * mm - 2 * dd)
            - 0.057 * Math.Sin(mm - 2 * dd + ms)
            + 0.053 * Math.Sin(mm + 2 * dd)
            + 0.046 * Math.Sin(2 * dd - ms)
            + 0.041 * Math.Sin(mm - ms)
            - 0.035 * Math.Sin(dd)
            - 0.031 * Math.Sin(mm + ms)
            - 0.015 * Math.Sin(2 * ff - 2 * dd)
            + 0.011 * Math.Sin(mm - 4 * dd);

        latitude +=
            -0.173 * Math.Sin(ff - 2 * dd)
            - 0.055 * Math.Sin(mm - ff - 2 * dd)
            - 0.046 * Math.Sin(mm + ff - 2 * dd)
            + 0.033 * Math.Sin(ff + 2 * dd)
            + 0.017 * Math.Sin(2 * mm + ff);

        distance += -0.58 * Math.Cos(mm - 2 * dd) - 0.46 * Math.Cos(2 * dd);

        _moonEclipticLongitude = AstroMath.Normalize360(longitude);
        _moonEclipticLatitude = latitude;
        _moonDistanceEarthRadii = distance;

        Equatorial eq = AstroMath.EclipticToEquatorial(
            _moonEclipticLongitude, latitude, obliquity, distance * 4.26352e-5);

        _moon.RaOfDate = eq.RightAscension;
        _moon.DecOfDate = eq.Declination;
        _moon.AngularDiameter = 2 * 0.2725 * Math.Asin(1.0 / distance) * AstroMath.Rad2Deg;
    }

    private void ApplyLunarParallax(double lstDeg, double latitudeDeg)
    {
        double parallax = Math.Asin(1.0 / _moonDistanceEarthRadii) * AstroMath.Rad2Deg;
        double geocentricAlt = _moon.Altitude;
        double correction = parallax * Math.Cos(geocentricAlt * AstroMath.Deg2Rad);
        double topocentricAlt = geocentricAlt - correction;

        _moon.Altitude = topocentricAlt;
        _moon.Horizon = HorizonVector.FromAltAz(
            topocentricAlt * AstroMath.Deg2Rad, _moon.Azimuth * AstroMath.Deg2Rad);

        Equatorial eq = AstroMath.HorizonToEquatorial(_moon.Horizon, lstDeg, latitudeDeg);
        _moon.RaOfDate = eq.RightAscension;
        _moon.DecOfDate = eq.Declination;
    }

    private void UpdateMoonPhase()
    {
        double elongation = AstroMath.Normalize360(_moonEclipticLongitude - SunEclipticLongitude);
        double phaseAngle = 180.0 - elongation;
        _moon.IlluminatedFraction = (1 + Math.Cos(phaseAngle * AstroMath.Deg2Rad)) / 2.0;

        // Waxing moons are lit on the side toward increasing ecliptic longitude.
        _moon.BrightLimbAngle = elongation < 180 ? 90 : 270;

        // Full moon is about -12.7; fade toward a thin crescent as the lit fraction drops.
        double illuminated = Math.Max(_moon.IlluminatedFraction, 0.005);
        _moon.Magnitude = -12.73 + 2.5 * Math.Log10(1.0 / illuminated);
    }

    /// <summary>Elongation of the Moon from the Sun, 0 at new moon and 180 at full.</summary>
    public double MoonElongation =>
        AstroMath.Normalize360(_moonEclipticLongitude - SunEclipticLongitude);

    // --------------------------------------------------------------- planets

    private readonly record struct KeplerElements(
        double SemiMajorAxis, double Eccentricity, double Inclination,
        double MeanLongitude, double LongitudeOfPeriapsis, double LongitudeOfNode,
        double DSemiMajorAxis, double DEccentricity, double DInclination,
        double DMeanLongitude, double DLongitudeOfPeriapsis, double DLongitudeOfNode);

    // JPL approximate elements, valid 1800-2050. Rates are per Julian century.
    private static readonly KeplerElements Earth = new(
        1.00000261, 0.01671123, -0.00001531, 100.46457166, 102.93768193, 0.0,
        0.00000562, -0.00004392, -0.01294668, 35999.37244981, 0.32327364, 0.0);

    private static readonly KeplerElements[] PlanetElements =
    [
        new(0.38709927, 0.20563593, 7.00497902, 252.25032350, 77.45779628, 48.33076593,
            0.00000037, 0.00001906, -0.00594749, 149472.67411175, 0.16047689, -0.12534081),
        new(0.72333566, 0.00677672, 3.39467605, 181.97909950, 131.60246718, 76.67984255,
            0.00000390, -0.00004107, -0.00078890, 58517.81538729, 0.00268329, -0.27769418),
        new(1.52371034, 0.09339410, 1.84969142, -4.55343205, -23.94362959, 49.55953891,
            0.00001847, 0.00007882, -0.00813131, 19140.30268499, 0.44441088, -0.29257343),
        new(5.20288700, 0.04838624, 1.30439695, 34.39644051, 14.72847983, 100.47390909,
            -0.00011607, -0.00013253, -0.00183714, 3034.74612775, 0.21252668, 0.20469106),
        new(9.53667594, 0.05386179, 2.48599187, 49.95424423, 92.59887831, 113.66242448,
            -0.00125060, -0.00050991, 0.00193609, 1222.49362201, -0.41897216, -0.28867794),
        new(19.18916464, 0.04725744, 0.77263783, 313.23810451, 170.95427630, 74.01692503,
            -0.00196176, -0.00004397, -0.00242939, 428.48202785, 0.40805281, 0.04240589),
        new(30.06992276, 0.00859048, 1.77004347, -55.12002969, 44.96476227, 131.78422574,
            0.00026291, 0.00005105, 0.00035372, 218.45945325, -0.32241464, -0.00508664),
    ];

    // Equatorial radii in AU, for apparent diameter.
    private static readonly double[] PlanetRadiiAu =
        [1.63e-5, 4.04e-5, 2.27e-5, 4.78e-4, 4.03e-4, 1.71e-4, 1.65e-4];

    private void UpdatePlanets(double t)
    {
        (double ex, double ey, double ez) = Heliocentric(Earth, t);

        for (int i = 0; i < _planets.Length; i++)
        {
            (double px, double py, double pz) = Heliocentric(PlanetElements[i], t);

            double gx = px - ex;
            double gy = py - ey;
            double gz = pz - ez;

            double distanceFromEarth = Math.Sqrt(gx * gx + gy * gy + gz * gz);
            double distanceFromSun = Math.Sqrt(px * px + py * py + pz * pz);

            double longitude = Math.Atan2(gy, gx) * AstroMath.Rad2Deg;
            double latitude = Math.Atan2(gz, Math.Sqrt(gx * gx + gy * gy)) * AstroMath.Rad2Deg;

            // The JPL element set is referred to the mean ecliptic and equinox of
            // J2000, so the conversion has to use the J2000 obliquity and the result
            // then has to be precessed. Skipping that leaves every planet about a
            // third of a degree out by the late 2020s - small, but enough to put a
            // planet visibly off its true spot against the stars.
            Equatorial j2000 = AstroMath.EclipticToEquatorial(
                longitude, latitude, AstroMath.ObliquityJ2000, distanceFromEarth);
            Equatorial eq = AstroMath.PrecessFromJ2000(
                j2000.RightAscension, j2000.Declination, t);

            SolarSystemBody planet = _planets[i];
            planet.RaOfDate = eq.RightAscension;
            planet.DecOfDate = eq.Declination;
            planet.AngularDiameter =
                2 * Math.Atan(PlanetRadiiAu[i] / distanceFromEarth) * AstroMath.Rad2Deg;

            double phaseAngle = PhaseAngle(distanceFromSun, distanceFromEarth, SunDistanceAu);
            planet.IlluminatedFraction = (1 + Math.Cos(phaseAngle * AstroMath.Deg2Rad)) / 2.0;
            planet.Magnitude = ApparentMagnitude(i, distanceFromSun, distanceFromEarth, phaseAngle);
        }
    }

    private static (double X, double Y, double Z) Heliocentric(in KeplerElements e, double t)
    {
        double a = e.SemiMajorAxis + e.DSemiMajorAxis * t;
        double ecc = e.Eccentricity + e.DEccentricity * t;
        double inc = (e.Inclination + e.DInclination * t) * AstroMath.Deg2Rad;
        double meanLongitude = e.MeanLongitude + e.DMeanLongitude * t;
        double periapsis = e.LongitudeOfPeriapsis + e.DLongitudeOfPeriapsis * t;
        double node = (e.LongitudeOfNode + e.DLongitudeOfNode * t) * AstroMath.Deg2Rad;

        double argPeriapsis = (periapsis - e.LongitudeOfNode - e.DLongitudeOfNode * t) * AstroMath.Deg2Rad;
        double meanAnomaly = AstroMath.NormalizeSigned(meanLongitude - periapsis);

        double eccentricAnomaly = SolveKepler(meanAnomaly, ecc) * AstroMath.Deg2Rad;

        // Position in the orbital plane, periapsis along +x.
        double xOrbit = a * (Math.Cos(eccentricAnomaly) - ecc);
        double yOrbit = a * Math.Sqrt(1 - ecc * ecc) * Math.Sin(eccentricAnomaly);

        double cosW = Math.Cos(argPeriapsis), sinW = Math.Sin(argPeriapsis);
        double cosN = Math.Cos(node), sinN = Math.Sin(node);
        double cosI = Math.Cos(inc), sinI = Math.Sin(inc);

        double xEcl = (cosW * cosN - sinW * sinN * cosI) * xOrbit
                      + (-sinW * cosN - cosW * sinN * cosI) * yOrbit;
        double yEcl = (cosW * sinN + sinW * cosN * cosI) * xOrbit
                      + (-sinW * sinN + cosW * cosN * cosI) * yOrbit;
        double zEcl = sinW * sinI * xOrbit + cosW * sinI * yOrbit;

        return (xEcl, yEcl, zEcl);
    }

    /// <summary>Solves Kepler's equation by Newton iteration. Angles in degrees.</summary>
    private static double SolveKepler(double meanAnomalyDeg, double eccentricity)
    {
        double m = AstroMath.NormalizeSigned(meanAnomalyDeg) * AstroMath.Deg2Rad;
        double e = m + eccentricity * Math.Sin(m) * (1 + eccentricity * Math.Cos(m));

        for (int i = 0; i < 12; i++)
        {
            double delta = (e - eccentricity * Math.Sin(e) - m)
                           / (1 - eccentricity * Math.Cos(e));
            e -= delta;
            if (Math.Abs(delta) < 1e-11) break;
        }

        return e * AstroMath.Rad2Deg;
    }

    /// <summary>Sun-planet-Earth angle, from the three side lengths of that triangle.</summary>
    private static double PhaseAngle(double sunToPlanet, double planetToEarth, double sunToEarth)
    {
        double cos = (sunToPlanet * sunToPlanet + planetToEarth * planetToEarth
                      - sunToEarth * sunToEarth)
                     / (2 * sunToPlanet * planetToEarth);
        return Math.Acos(Math.Clamp(cos, -1, 1)) * AstroMath.Rad2Deg;
    }

    private static double ApparentMagnitude(int index, double r, double d, double phase)
    {
        double baseTerm = 5 * Math.Log10(Math.Max(r * d, 1e-6));
        return index switch
        {
            0 => -0.36 + baseTerm + 0.027 * phase + 2.2e-13 * Math.Pow(phase, 6),  // Mercury
            1 => -4.34 + baseTerm + 0.013 * phase + 4.2e-7 * Math.Pow(phase, 3),   // Venus
            2 => -1.51 + baseTerm + 0.016 * phase,                                  // Mars
            3 => -9.25 + baseTerm + 0.014 * phase,                                  // Jupiter
            4 => -9.00 + baseTerm,                                                  // Saturn, rings ignored
            5 => -7.15 + baseTerm,                                                  // Uranus
            _ => -6.90 + baseTerm,                                                  // Neptune
        };
    }
}
