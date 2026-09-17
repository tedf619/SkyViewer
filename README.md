# Sky Viewer

A planetarium for Windows: .NET 10, C# Windows Forms, GDI+. Point it at a place and a
moment and it draws the sky you would actually see — 5,044 stars in their true colours,
88 constellation figures, the Milky Way as a layered glow, 193 deep-sky objects, the Sun,
the Moon with its correct phase, and the seven other planets.

## Build and run

```
dotnet run --project SkyViewer.csproj
```

Requires the .NET 10 SDK on Windows. There are no NuGet dependencies; the star
catalogues are embedded resources, so the built output is xcopy-deployable.

## Using it

| Action | Control |
| --- | --- |
| Pan the sky | Drag with the left mouse button, or use the arrow keys |
| Zoom | Mouse wheel, the field-of-view slider, or `+` / `-` |
| Identify an object | Click it — details appear in the sidebar |
| Centre on an object | Double-click it |
| Snap to a compass point | `N`, `E`, `S`, `W`, or `Z` for the zenith |
| Clear the selection | `Esc` |

The sidebar sets the observing site (18 presets plus a custom-location dialog), the date
and time, how fast time flows (paused through a week per second), which layers are drawn,
and the limiting magnitude.

## Layout

```
SkyViewer/
  Program.cs                     Entry point, dark mode
  MainForm.cs / .Designer.cs     Main window: sky canvas + sidebar + status bar
  LocationDialog.cs / .Designer.cs   Custom observing-site editor
  Controls/SkyView.cs / .Designer.cs
                                 The sky canvas (a UserControl): painting, mouse, keyboard
  Astronomy/
    AstroMath.cs                 Julian dates, sidereal time, coordinate transforms,
                                 precession, refraction
    CatalogTypes.cs              Star, DeepSkyObject, SolarSystemBody, Constellation
    SkyCatalog.cs                Catalogue loading and per-frame positioning
    SolarSystem.cs               Sun, Moon, and planet ephemerides
    Observer.cs                  Observing sites and the clock
  Rendering/
    SkyProjection.cs             Stereographic projection
    SkyPalette.cs                Twilight gradient, blackbody star colours
  Data/                          Embedded catalogues (pipe-delimited text)
```

Every form and the `SkyView` user control has its own `.Designer.cs` file holding the
field declarations, `InitializeComponent()`, and `Dispose(bool)`, exactly as the Visual
Studio designer expects; the matching `.cs` file holds only logic and event handlers.
`SkyView` derives from `UserControl`, so it appears in the toolbox and can be dropped
onto a form in the designer.

## Accuracy

The astronomy was checked against PyEphem at several epochs and latitudes:

| Quantity | Agreement |
| --- | --- |
| Julian Day | exact |
| Local sidereal time | 0.1–0.2 arcmin (we use mean, not apparent, sidereal time) |
| Stars (alt/az) | 0.06–0.98 arcmin |
| Sun | 0.16–0.32 arcmin |
| Moon | 0.30–1.62 arcmin |
| Planets | under 1 arcmin, rising to ~3′ for Jupiter and ~5′ for Saturn |

All of these are comfortably below one screen pixel at normal zoom levels.

Star positions come from the Hipparcos-derived d3-celestial catalogue, precessed from
J2000 to the epoch of date. Planets use the JPL approximate Keplerian elements, valid
1800–2050. Bennett's refraction formula is applied below 12° altitude, and stars are
dimmed by atmospheric extinction near the horizon.
