# SkyViewer

Sky Viewer is a desktop planetarium for Windows — a .NET 10 C# Windows Forms application that renders the sky as it actually appears from a given place at a given moment. The following figure shows SkyViewer in action.

<img width="976" height="831" alt="image" src="https://github.com/user-attachments/assets/ce3a03ae-f134-464c-b82b-f1a9cb087147" />

*Figure 1* - SkyViewer in action.

The UI is split into parts:
* *Sky View*. This panel displays the sky and stars. It fills all space left over by the other panels. All the astronomical heavy lifting and rendering is carried out in the black SkyView UserControl hosted by this panel.
* *Side Panel*.  Appears on the right of the Sky View. It has several sections:
  * *Your Location*. The dropdown selects an observing site from several presets. The *Set a Custom Site* button opens a dialog for entering custom coordinates. 
  * *Time*. These controls set the date and time and how fast the clock runs — paused, real time or faster. 
  * *What you see*. Controls the zoom level (field of view) and the brightness of the faintest stars to show.
  * *Layers*. A series of checkboxes for turning on and off various layers in the sky view.
  * *Compass Heading*. Buttons to point the sky view in specific directions. Zenith points the view straight up.
* *Selected Object Details*. This panel appears only when you click on a known star or object in the Sky View.
* *Status Bar*. Shows the cursor's elevation and azimuth, the sidereal time, the field of view angle, and how many stars are currently drawn. Sidereal time is similar to earth 24-hour time but is more astronomically accurate: A 360-degree rotation of the Earth axis takes 23:56:04 -- not 24:00:00.

## The UI Layout

To simplify the proper layout of the various panels regardless of the main window size, docked panels are used as shown in the following figure.

<img width="763" height="531" alt="image" src="https://github.com/user-attachments/assets/442a6533-ef12-4417-8a7e-68ec3134bd37" />

*Figure 2* - The UI laid out with docked panels.

In order for the panels to lay out correctly, they must have the proper back-to-front order. The following order is required:

	1. Panel StatusBar. Backmost panel (first one added to form).
	2. Panel SelectedObjectDetails.
	3. Panel SideBar.
	4. Panel SkyView. Topmost panel (last one added to form).

The sky objects are those contained in various catalogs, contained in the project as embedded-resource files. Check out the files in the Data folder. The app is completely self-contained, with no dependencies on NuGet packages or plugins.

The app shows the following:

* *Stars*. Over 5,000 stars contained in a Hipparcos-derived star catalog. Each star is colored by converting its B−V (blue-yellow) color index to a blackbody curve, so hot stars come out blue-white and cool ones amber. 
* *Constellations*. There are also 88 constellation figures with labels, such as Pegasus and Sagittarius.
* *The Milky Wa*y. Shown as five nested contour levels that build up a diffuse glow. 
* *Deep Sky Objects*. Typically galaxies or other objects outside the Milky Way. Up to 193 deep-sky objects. 
* *Solar System Object*s. The Sun, the Moon drawn at its correct phase, and the planets. 

Using the various Layers checkboxes, you can display the equatorial grid, the horizon grid, the ecliptic, the ground below the horizon, cardinal points and other options.

The background isn't a flat black — it runs through a seven-stop gradient keyed to the Sun's altitude, so you get the full progression from daylight through astronomical twilight into night, with a glow low on the horizon where the Sun has set. Stars dim as they approach the horizon through atmospheric extinction, and refraction lifts objects below 12° altitude the way the real atmosphere does.

## Under the hood
Stereographic projection, GDI+ drawing on a double-buffered surface, a 100-ms timer driving the clock. Precessed star coordinates are cached and only rebuilt when the date moves more than about eleven days, and the horizon-coordinate buffers for constellation lines and Milky Way contours are allocated once and written in place, so panning at 30 fps doesn't create a massive memory allocation hit. 

## About the Hipparcos Catalog
The astronomical data used in SkyViewer is based on the standard Hipparcos Catalog, which is a highly precise database of 118,218 stars. Published in 1997 by the European Space Agency (ESA), it represents one of the most significant breakthroughs in the history of astrometry at the time. The catalog was created using data collected by the Hipparcos satellite, which operated in space from 1989 to 1993. It was named in honor of the ancient Greek astronomer Hipparchus, who compiled the Western world's very first quantitative star catalog in the 2nd century BCE. By taking a telescope into the vacuum of space, the Hipparcos mission achieved several major milestones:

  * Unprecedented Precision: It measured stellar coordinates down to milliarcseconds (equivalent to measuring the thickness of a coin from thousands of miles away).
  * True Distance Mapping: It measured stellar parallax (the apparent shift of a star against the background sky as Earth orbits the Sun). This provided the first highly accurate, direct 3D distances for over 100,000 stars. 
  * Proper Motion: The satellite didn't just capture a snapshot; it tracked how fast stars are physically drifting through space over time (proper motion), revealing the structural dynamics of our Milky Way galaxy.

## The 3D Representation of Sky Objects
Every object in the sky — a star, planet, constellation vertex — is reduced to a unit vector on the celestial sphere, expressed in a local horizon frame: HorizonVector(N, E, U) — components toward north, east, and straight up (the zenith). No depth, no distance; everything is treated as infinitely far away and sitting on a sphere of radius 1 centered on the observer. That logic doesn't appreciably affect star positions and is a reasonable simplification for the planets, whose angular size but not parallax the renderer cares about.

Getting a vector onto that sphere starts from equatorial coordinates (RA/Dec, precessed to the current date) and the observer's latitude and local sidereal time LST. The hour angle h = LST − RA gives the object's position relative to the meridian. A standard rotation by latitude then swings that into the north/east/up frame. Here are the basic calculations:

```
x = cos(dec)·cos(h)      // toward the meridian
y = cos(dec)·sin(h)      // toward the west point
z = sin(dec)             // toward the pole
N = −x·sin(lat) + z·cos(lat)
E = −y
U =  x·cos(lat) + z·sin(lat)
```

That's the only 3D transform in the astronomy layer — it happens once per object per frame (or is skipped via the precession cache described earlier).

### Projecting that sphere onto the screen
The camera is defined by where you're looking: a forward vector f (toward the center of the view), a right vector r (east-ish, screen-right), and an up vector u (toward the zenith), all built from the center altitude/azimuth. This is just a local orthonormal basis — a tiny camera frame sitting at the origin of the unit sphere. For any object vector v, its position in front of the camera is (v·r, v·u, v·f) — a dot-product change of basis, no matrix class needed. Then stereographic projection turns the angle from the camera's forward direction into a screen radius. Here are the calculations:

```
k = 2 / (1 + v·f)
x = k · (v·r)
y = k · (v·u)
```

That k factor is the actual "3D-ness": stereographic projection maps the point at angle θ from center to a plane distance 2·tan(θ/2) from the origin, so this single division does the perspective-like foreshortening. Objects near the edge of a wide field of view get stretched (that's why the field of view is capped at 170°), but shapes stay locally correct everywhere — which is why the constellations still look right no matter where they sit in frame. It's the same projection real star atlases use for that reason.
Finally x, y are scaled by pixels-per-radian (set from the field-of-view and viewport height) and flipped/offset into screen pixel coordinates, with y negated since screen Y grows downward while "up" should mean up.
Points behind the observer's head (v·f ≤ −0.9995) return "don't draw" rather than blowing up — k would approach infinity there.

### The Horizon
The horizon itself is handled specially because stereographic projection will map any great circle on the sphere to either a circle or a straight line in the plane. The TryGetHorizonCircle method projects three sample points around the horizon and solves for the circle through them, then fills the ground as a clip region rather than a sampled polygon.

## Stars

The Star catalog contains 5,044 stars, drawn from the d3-celestial dataset (derived from Hipparcos), covering the whole sky down to visual magnitude 6.0 — the naked-eye limit under a dark sky. Magnitudes span from magnitude −1.44 (Sirius, the brightest star) up to that 6.0 cutoff. 493 of the 5,044 carry a proper name (Betelgeuse, Vega, Polaris, etc.); the rest are identified only by position or, where available, a Bayer letter and constellation.
Stars.dat is a text file, with one line per star. On each line, fields are separated by pipe characters. Here is a sample line with a header to show the field names:

```
ra_deg | dec_deg | mag  |   bv  | bayer | proper_name | constellation
0.2691 |-48.8099 | 5.71 | 0.911 |   τ   |             |Phe
```
The fields have the following meaning:

* *ra_deg, dec_deg*. The J2000 position, in decimal degrees. 
* *mag*. Visual magnitude (brightness).
* *bv*. B−V (blue-yellow) color index, the measured colour.
* *bayer*. Standardized Greek-letter designation (e.g. τ, θ). Blank if none.
* *proper_name*. Traditional name (e.g. Polaris). Blank if none.
* *constellation*. Three-letter abbreviation (Phe, Oct, Ori, ...). Blank if none.

The following simplified code highlights the handling of stars:

```csharp
public class Star
{
  public double RaJ2000 { get; init; }
  public double DecJ2000 { get; init; }
  public double Magnitude { get; set; }
  public double ColorIndex { get; init; }
  public string Bayer { get; init; } = "";
  public string ProperName { get; init; } = "";
  public string Constellation { get; init; } = "";
}

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
```

*Listing 1* - Simplified code showing how stars are loaded and parsed from the catalog.

## Constellations
A constellation is a formally recognized area of the celestial sphere. While we often think of them as the familiar connect-the-dots star patterns depicting mythological figures, animals, or objects, modern astronomers define them as precise boundaries that divide the entire night sky into a grid, much like borders divide a map into countries. In 1922, the International Astronomical Union (IAU) officially divided the sky into exactly 88 constellations, ensuring that every single point and celestial object in space falls into exactly one specific territory. There are two ways to define a constellation:
  1. *The Astronomical Definition*. To an astronomer, a constellation is a 3D wedge of space projected from Earth outward into the universe. If a telescope captures a new supernova or a distant galaxy, its location is logged by the constellation boundary it sits within (e.g., "located in the constellation Pegasus").
  2. *The Visual Definition*. The recognizable shapes we see in the stars are called asterisms when they are only part of a constellation or combine stars from multiple constellations.
<br><br>
Major Categories of Constellations:
* *The 13 Zodiac Constellations*. A narrow band of constellations that sit perfectly along the ecliptic. While astrology uses 12 signs, astronomically, the path intersects 13 constellations, including Ophiuchus.
* *Circumpolar Constellations*. Constellations located close to the Earth's celestial poles (like Ursa Major in the north). Because of their position, they never set below the horizon from certain latitudes and are visible every single night of the year.

### Examples of Constellations
For northern observers, some are visible all year round, but most are only visible in certain seasons.
* *Ursa Major*. The Great Bear, the third-largest constellation. It contains the Big Dipper asterism, which is used by stargazers worldwide to find the North Star.
* *Ursa Minor*. The Little Bear, home to Polaris (the North Star), the pivotal anchor point around which the entire night sky appears to rotate.
* *Orion*. The Hunter, perhaps the most famous constellation of all. It is instantly recognizable by the three perfectly aligned stars of Orion's Belt and contains the blazing stars Betelgeuse (a red supergiant) and Rigel (a blue supergiant).
* *Canis Major*. The Greater Dog, follows Orion across the sky and holds Sirius, which is the absolute brightest star in the entire night sky.

The constellation catalog catalog contains 89 constellation figures, obtained from the same d3-celestial dataset as the stars. Each entry is not a filled boundary but a stick-figure line drawing
The Constellations.dat file contains a text file, with one line per constellation. On each line, fields are separated by pipe characters. Here is a sample line with a header to show the field names:

```
designation |    name    |    label_ra,  label_dec  |   segment;                                                       segment; ...
And         |Andromeda   |    0.7500,    43.0000    |   30.9748,42.3297 17.4330,35.6206 9.8320,30.8610 2.0969,29.0904; ...
```

The fields have the following meaning:

* *designation*. Three-letter abbreviation (And, Ant, Ori, ...).
* *name*. Full name (Andromeda, Antlia, ...).
* *label_ra, label_dec*. J2000 position of the constellation's name.
* *segments*. One or more semicolon-separated segments, each an open polyline of space-separated (ra, dec) vertex pairs.

A constellation figure is a set of disconnected open polylines rather than one closed shape — Andromeda above has five separate strokes — because the actual figure has branches and gaps, not one continuous path.
The following simplified code highlights the handling of constellations:

```csharp
public class Constellation
{
  public string Abbreviation { get; init; }
  public string Name { get; init; }

  public double LabelRaJ2000 { get; init; }
  public double LabelDecJ2000 { get; init; }
  public List<(double Ra, double Dec)[]> Segments { get; init; }
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
```

*Listing 2* - Simplified code showing how constellations are loaded and parsed from the catalog.

## The Milky Way

The Milky Was catalog doesn't contain point objects like stars, but rather a brightness map of the Milky Way band itself, stored as *isophote* contours. These contours show closed outlines connecting points of equal brightness.
There are 5 levels, 0 (faintest/outermost) through 4 (brightest/innermost core), nested inside one another like rings on a target. Together they total 202 closed rings and 16,890 vertices.
MilkyWay.dat is a text file, with one line per ring. On each line, fields are separated by pipe characters. Here is a sample line with a header to show the field names:

```
level   |    ring;                                     ring;...
0       |    97.75,34.66 94.83,35.90 92.66,37.34 ... ; <next ring>;...
``` 
Each ring is a closed polygon in J2000 equatorial degrees. Polygons are space-separated (ra,dec) pairs. Multiple rings per level separated by semicolons.

The following simplified code highlights the handling of the Milky Way:

```csharp
public class MilkyWayContour
{
  public int Level { get; init; }
  public List<(double Ra, double Dec)[]> Rings { get; init; }
  public HorizonVector[][] HorizonRings { get; set; } = [];
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
```

*Listing 3* - Simplified code showing how the Milky Way is loaded and parsed from the catalog.

## Deep Sky Objects

For the most part, Deep Sky Objects are things outside the Milky Way. The DeepSkyObjects catalog contains almost 200 objects from the d3-celestial dataset. Some of them carry a proper or popular name (e.g. Hyades or the Orion Belt Cluster), but most are identified only by their catalogue designation. Deep Sky Objects include the following:

* *Nebulae*. Interstellar clouds of dust, hydrogen, helium, and other ionized gases.
* *Open Clusters*. Loosely bound groups of a few dozen to a few thousand young stars that formed from the same giant molecular cloud. Examples:  The Pleiades (M45) and the Beehive Cluster (M44).
* *Globular Clusters*. Tightly bound, spherical collections of tens of thousands to millions of extremely old stars that orbit the cores of galaxies like satellites. They are packed so densely that their centers look like a solid ball of light. Example: The Great Globular Cluster in Hercules (M13).
* *Galaxies*. Gravitationally bound systems consisting of billions of stars or other matter. They exist far outside our own Milky Way galaxy. Example: The Andromeda Galaxy (M31).

DeepSkyObjects.dat is s a text file with one line per item. On each line, fields are separated by pipe characters. Here is a sample line with a header to show the field names:

```
ra_deg   | dec_deg  |  mag  | type | designation | proper_name
66.7500  | 16.0000  |  0.50 |  oc  |    C 41     |  Hyades
```

The fields have the following meaning:

* *ra_deg/dec_deg*. J2000 equatorial degrees.
* *mag*. Visual magnitude.
* *type*. the Type, as described below.
* *designation*. Catalogue number.
* *proper_name*. Blank unless the object has a common name.

Deep Sky Object types use a two/three-letter code:

```
  Code	Meaning
  oc    Open cluster
  bn    Bright nebula
  rn    Reflection nebula
  gc    Globular cluster
  en    Emission nebula
  pn    Planetary nebula
```

The following simplified code highlights the handling of Deep Sky Objects:

```csharp
public class DeepSkyObject
{
  public double LabelRaJ2000 { get; init; }
  public double LabelDecJ2000 { get; init; }
  public double Magnitude { get; set; }
  public string TypeCode { get; init; } = "";
  public string Designation { get; init; } = "";
  public string ProperName { get; init; } = "";
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

```

*Listing 4* - Simplified code showing how Deep Sky Objects are loaded and parsed from the catalog.

DrawDeepSkyObjects use a looser magnitude cutoff than stars — the limiting magnitude slider plus 2.5, capped at 9.0 — since these objects are worth showing a bit fainter than the star limit. 
Each type gets its own standard star-chart symbol: 

* galaxies are drawn as a flattened ellipse.
* globular clusters are drawn as a circle with a cross through it.
* planetary nebulae are drawn as a small circle with two stub lines.
* bright/emission/reflection nebulae and supernova remnants are drawn as a square.
* everything else is drawn as a dotted circle, which is the traditional loose-cluster symbol.

Labels are only drawn when the field of view is under 75°, to avoid clutter in wide all-sky views.

## AI Acknowledgements

The workhorse of this app is the SkyView UserControl, which was largely generated by Anthropic Claude. I made several changes to the Claude code but none that affected astronomic functionality. The UI was completed rewritten, to simplify it and to support the Visual Studio Forms Designer.



