using System.Diagnostics;

namespace SkyViewer
{
  public partial class FormMain : Form
  {
    readonly SkyCatalog catalog = new();
    readonly Observer observer = new();
    readonly Stopwatch wallClock = Stopwatch.StartNew();
    TimeSpan lastTick;
    bool isUpdatingUi, isCatalogDirty = true;

    public FormMain()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      panelConstellation.Location = panelAdditionalDetails.Location;
      panelSolarSystemBody.Location = panelAdditionalDetails.Location;
      panelCatalog.Location = panelAdditionalDetails.Location;

      panelAdditionalDetails.Visible = false;  // panel was only a placeholder

      UseWaitCursor = true;
      catalog.Load();
      UseWaitCursor = false;

      skyView.Catalog = catalog;
      skyView.Observer = observer;

      PopulateLists();
      ApplyDefaults();
      RefreshCatalog();
      UpdateViewCheckBoxes();
      UpdateStatusBar();

      lastTick = wallClock.Elapsed;
      timer.Start();
      skyView.Focus();
    }

    void PopulateLists()
    {
      isUpdatingUi = true;

      comboBoxYourLocation.Items.AddRange(ObservingSite.Presets.Cast<object>().ToArray());
      comboBoxYourLocation.SelectedIndex = 0;

      foreach (TimeFlow flow in Enum.GetValues<TimeFlow>())
        comboBoxTimeRunsAt.Items.Add(Observer.Describe(flow));

      comboBoxTimeRunsAt.SelectedIndex = (int)TimeFlow.RealTime;

      isUpdatingUi = false;
    }

    void ApplyDefaults()
    {
      isUpdatingUi = true;

      observer.ApplySite(ObservingSite.Presets[0]);
      observer.SetToNow();

      numericUpDownLatitude.Value = (decimal)observer.Latitude;
      numericUpDownLongitude.Value = (decimal)observer.Longitude;
      dateTimePickerDate.Value = observer.LocalTime.Date;
      dateTimePickerTime.Value = observer.LocalTime;

      skyView.FieldOfView = 105;
      skyView.LimitingMagnitude = 5.8;
      skyView.LookAt(32, 180);

      trackBarFieldOfView.Value = FieldOfViewToSliderValue(skyView.FieldOfView);
      trackBarMagnitude.Value = (int)Math.Round(skyView.LimitingMagnitude * 10);

      checkBoxConstellations.Checked = true;
      checkBoxTheirNames.Checked = true;
      checkBoxStarNames.Checked = true;
      checkBoxMilkyWay.Checked = true;
      checkBoxDeepSky.Checked = true;
      checkBoxSolarSystemBodies.Checked = true;
      checkBoxGround.Checked = true;
      checkBoxCompassPoints.Checked = true;
      checkBoxDaylight.Checked = true;
      checkBoxEquatorialGrid.Checked = false;
      checkBoxHorizonGrid.Checked = false;
      checkBoxEcliptic.Checked = false;

      UpdateSliderReadouts();
      isUpdatingUi = false;
    }

    void Timer_Tick(object sender, EventArgs e)
    {
      TimeSpan now = wallClock.Elapsed;
      TimeSpan elapsed = now - lastTick;
      lastTick = now;

      if (observer.Flow != TimeFlow.Paused)
      {
        observer.Advance(elapsed);
        isCatalogDirty = true;
        UpdateDateTimePickers();
      }

      if (isCatalogDirty)
      {
        RefreshCatalog();
        skyView.Invalidate();
        UpdateSelectionPanel();
      }

      UpdateStatusBar();
    }

    void RefreshCatalog()
    {
      catalog.ApplyRefraction = checkBoxDaylight.Checked;
      catalog.Update(observer.Utc, observer.Latitude, observer.Longitude);
      isCatalogDirty = false;
    }

    void UpdateDateTimePickers()
    {
      isUpdatingUi = true;
      DateTime localTime = observer.LocalTime;

      if (dateTimePickerDate.Value.Date != localTime.Date)
        dateTimePickerDate.Value = localTime.Date;

      dateTimePickerTime.Value = localTime;
      isUpdatingUi = false;
    }

    void UpdateViewCheckBoxes()
    {
      skyView.ShowConstellationLines = checkBoxConstellations.Checked;
      skyView.ShowConstellationNames = checkBoxTheirNames.Checked;
      skyView.ShowStarNames = checkBoxStarNames.Checked;
      skyView.ShowMilkyWay = checkBoxMilkyWay.Checked;
      skyView.ShowDeepSky = checkBoxDeepSky.Checked;
      skyView.ShowSolarSystem = checkBoxSolarSystemBodies.Checked;
      skyView.ShowGround = checkBoxGround.Checked;
      skyView.ShowCardinalPoints = checkBoxCompassPoints.Checked;
      skyView.ShowAtmosphere = checkBoxDaylight.Checked;
      skyView.ShowEquatorialGrid = checkBoxEquatorialGrid.Checked;
      skyView.ShowHorizonGrid = checkBoxHorizonGrid.Checked;
      skyView.ShowEcliptic = checkBoxEcliptic.Checked;

      skyView.Invalidate();
    }

    // the slider is logarithmic
    const double MinFov = 1.0, MaxFov = 160.0;

    int FieldOfViewToSliderValue(double fov)
    {
      double t = Math.Log(Math.Clamp(fov, MinFov, MaxFov) / MinFov) / Math.Log(MaxFov / MinFov);
      return (int)Math.Round(10 + t * 990);
    }

    double SliderValueToFieldOfView(int value)
    {
      double t = (value - 10) / 990.0;
      return MinFov * Math.Pow(MaxFov / MinFov, t);
    }

    void UpdateSliderReadouts()
    {
      double fov = skyView.FieldOfView;
      labelFieldOfView.Text = fov < 1 ? $"{fov * 60:0}'" : $"{fov:0.0}°";
      labelMagnitude.Text = $"mag {skyView.LimitingMagnitude:0.0}";
    }

    void ComboBoxYourLocation_SelectedIndexChanged(object? sender, EventArgs e)
    {
      if (isUpdatingUi || comboBoxYourLocation.SelectedItem is not ObservingSite site) return;

      isUpdatingUi = true;
      DateTime localBefore = observer.LocalTime;
      observer.ApplySite(site);
      observer.LocalTime = localBefore;   // keep the clock reading the same wall time

      numericUpDownLatitude.Value = (decimal)site.Latitude;
      numericUpDownLongitude.Value = (decimal)site.Longitude;

      isUpdatingUi = false;
      isCatalogDirty = true;

      UpdateStatusBar();
    }

    void LatitudeLongitude_ValueChanged(object? sender, EventArgs e)
    {
      if (isUpdatingUi) return;

      observer.Latitude = (double)numericUpDownLatitude.Value;
      observer.Longitude = (double)numericUpDownLongitude.Value;

      isCatalogDirty = true;
    }

    void ButtonSetCustomSite_Click(object? sender, EventArgs e)
    {
      using var form = new FormCustomSite(
          observer.Site.Name, observer.Latitude, observer.Longitude, observer.UtcOffsetHours);

      if (form.ShowDialog() != DialogResult.OK) return;

      isUpdatingUi = true;
      DateTime localBefore = observer.LocalTime;
      observer.ApplySite(form.Result);
      observer.LocalTime = localBefore;

      comboBoxYourLocation.SelectedIndex = -1;
      numericUpDownLatitude.Value = (decimal)Math.Clamp(form.Result.Latitude, -90, 90);
      numericUpDownLongitude.Value = (decimal)Math.Clamp(form.Result.Longitude, -180, 180);
      isUpdatingUi = false;

      isCatalogDirty = true;
      UpdateStatusBar();
    }

    void DateTimePicker_ValueChanged(object? sender, EventArgs e)
    {
      if (isUpdatingUi) return;

      observer.LocalTime = dateTimePickerDate.Value.Date + dateTimePickerTime.Value.TimeOfDay;
      isCatalogDirty = true;
    }

    void ButtonNow_Click(object? sender, EventArgs e)
    {
      observer.SetToNow();
      UpdateDateTimePickers();
      isCatalogDirty = true;
    }

    void ComboBoxTimeRunsAt_SelectedIndexChanged(object? sender, EventArgs e)
    {
      if (isUpdatingUi) return;

      observer.Flow = (TimeFlow)comboBoxTimeRunsAt.SelectedIndex;
      UpdateStatusBar();
    }

    void TrackBarFieldOfView_Scroll(object? sender, EventArgs e)
    {
      skyView.FieldOfView = SliderValueToFieldOfView(trackBarFieldOfView.Value);
      UpdateSliderReadouts();
      UpdateStatusBar();
    }

    void TrackBarMagnitude_Scroll(object? sender, EventArgs e)
    {
      skyView.LimitingMagnitude = trackBarMagnitude.Value / 10.0;
      UpdateSliderReadouts();
      skyView.Invalidate();
    }

    void CheckBoxLayer_CheckedChanged(object? sender, EventArgs e)
    {
      if (isUpdatingUi) return;

      if (ReferenceEquals(sender, checkBoxDaylight)) 
        isCatalogDirty = true;

      UpdateViewCheckBoxes();
    }

    void ButtonCompassHeading_Click(object? sender, EventArgs e)
    {
      Button? button = sender as Button;
      if (button != null && button.Tag is not null)
      {
        if (double.TryParse(button.Tag.ToString(), out double azimuth))
          skyView.LookAt(25, azimuth);
      }
      skyView.Focus();
    }

    void ButtonZenith_Click(object? sender, EventArgs e)
    {
      skyView.LookAt(89, skyView.CenterAzimuth);
      skyView.Focus();
    }

    void SkyView_SelectionChanged()
    {
      UpdateSelectionPanel();
    }

    void SkyView_ViewChanged()
    {
      isUpdatingUi = true;
      int slider = FieldOfViewToSliderValue(skyView.FieldOfView);

      if (trackBarFieldOfView.Value != slider)
        trackBarFieldOfView.Value = Math.Clamp(slider, trackBarFieldOfView.Minimum, trackBarFieldOfView.Maximum);

      isUpdatingUi = false;

      UpdateSliderReadouts();
      UpdateStatusBar();
    }

    void UpdateSelectionPanel()
    {
      SkyBody? body = skyView.Selected;

      panelSelectedObjectDetails.Visible = body is not null;

      if (body is null) return;

      labelSelectedObjectName.Text = body.DisplayName;

      labelSelectedObjectType.Text = body.TypeLabel;
      labelSelectedObjectMagnitude.Text = body.Magnitude.ToString("+0.00;-0.00;0.00");
      labelSelectedObjectRa.Text = AstroMath.FormatRightAscension(body.RaOfDate);
      labelSelectedObjectDeclination.Text = AstroMath.FormatDegrees(body.DecOfDate);
      labelSelectedObjectElevation.Text = AstroMath.FormatDegrees(body.Altitude);
      labelSelectedObjectAzimuth.Text = AstroMath.FormatDegrees(body.Azimuth, signed: false);

      switch (body)
      {
        case Star star:
          panelSolarSystemBody.Visible = false;
          panelCatalog.Visible = false;
          panelConstellation.Visible = !string.IsNullOrEmpty(star.Bayer) || !string.IsNullOrEmpty(star.Constellation);

          if (panelConstellation.Visible)
            labelConstellation.Text = $"{star.Bayer} {star.Constellation}".Trim();

          labelSelectedObjectColor.Text = "B-V " + star.ColorIndex.ToString("+0.00;-0.00;0.00");
          break;

        case SolarSystemBody solar:
          panelConstellation.Visible = false;
          panelSolarSystemBody.Visible = true;
          panelCatalog.Visible = false;

          labelDiameter.Text = $"{solar.AngularDiameter * 60:0.0} arcmin";

          labelIlluminatedLabel.Visible = solar.Kind != SolarSystemBodyKind.Sun;
          labelPercentIlluminated.Visible = solar.Kind != SolarSystemBodyKind.Sun;

          if (solar.Kind != SolarSystemBodyKind.Sun)
            labelPercentIlluminated.Text = $"{solar.IlluminatedFraction * 100:0}%";
          break;

        case DeepSkyObject dso:
          panelConstellation.Visible = false;
          panelSolarSystemBody.Visible = false;
          panelCatalog.Visible = true;
          labelCatalog.Text = dso.Designation;
          break;
      }

      labelIsCurrentlyBelowHorizon.Visible = body.Altitude < 0;
    }

    void UpdateStatusBar()
    {
      if (skyView.PointerDirection is { } pointer)
      {
        Equatorial eq = AstroMath.HorizonToEquatorial(
            HorizonVector.FromAltAz(
                pointer.Altitude * AstroMath.Deg2Rad,
                pointer.Azimuth * AstroMath.Deg2Rad),
            catalog.LocalSiderealTime,
            catalog.ObserverLatitude);

        labelStatusMessage.Text =
            $"alt {AstroMath.FormatDegrees(pointer.Altitude)}   "
            + $"az {AstroMath.FormatDegrees(pointer.Azimuth, signed: false)}   "
            + $"{AstroMath.FormatRightAscension(eq.RightAscension)}  "
            + $"{AstroMath.FormatDegrees(eq.Declination)}";
      }
      else
      {
        labelStatusMessage.Text = "Move the pointer over the sky for coordinates";
      }

      double lstHours = catalog.LocalSiderealTime / 15.0;
      int h = (int)lstHours;
      int m = (int)((lstHours - h) * 60);
      int s = (int)((((lstHours - h) * 60) - m) * 60);
      labelStatusSiderealTime.Text = $"Sidereal {h:00}h {m:00}m {s:00}s";

      labelStatusFieldOfView.Text = $"Field {skyView.FieldOfView:0.0}°";
      labelStatusStarsDrawn.Text = $"{skyView.LastStarCount:N0} Stars drawn";

      string offset = observer.UtcOffsetHours >= 0
          ? $"UTC+{observer.UtcOffsetHours:0.#}"
          : $"UTC{observer.UtcOffsetHours:0.#}";
    }
  }
}
