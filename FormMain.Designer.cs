namespace SkyViewer
{
  partial class FormMain
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      components = new System.ComponentModel.Container();
      panelStatusBar = new Panel();
      labelStatusMessage = new Label();
      labelStatusSiderealTime = new Label();
      labelStatusFieldOfView = new Label();
      labelStatusStarsDrawn = new Label();
      panelSideBar = new Panel();
      panelLookToward = new Panel();
      buttonUp = new Button();
      buttonW = new Button();
      buttonS = new Button();
      buttonE = new Button();
      buttonN = new Button();
      label13 = new Label();
      panelWhatYouSee = new Panel();
      label18 = new Label();
      checkBoxEcliptic = new CheckBox();
      checkBoxHorizonGrid = new CheckBox();
      checkBoxEquatorialGrid = new CheckBox();
      checkBoxDaylight = new CheckBox();
      checkBoxCompassPoints = new CheckBox();
      checkBoxGround = new CheckBox();
      checkBoxSolarSystemBodies = new CheckBox();
      checkBoxDeepSky = new CheckBox();
      checkBoxMilkyWay = new CheckBox();
      checkBoxStarNames = new CheckBox();
      checkBoxTheirNames = new CheckBox();
      checkBoxConstellations = new CheckBox();
      trackBarMagnitude = new TrackBar();
      labelMagnitude = new Label();
      label12 = new Label();
      trackBarFieldOfView = new TrackBar();
      labelFieldOfView = new Label();
      label9 = new Label();
      label8 = new Label();
      panelWhen = new Panel();
      comboBoxTimeRunsAt = new ComboBox();
      label7 = new Label();
      buttonNow = new Button();
      dateTimePickerTime = new DateTimePicker();
      dateTimePickerDate = new DateTimePicker();
      label6 = new Label();
      panelLatitudeLongitude = new Panel();
      buttonSetCustomSite = new Button();
      comboBoxYourLocation = new ComboBox();
      label1 = new Label();
      label5 = new Label();
      label4 = new Label();
      numericUpDownLatitude = new NumericUpDown();
      numericUpDownLongitude = new NumericUpDown();
      label3 = new Label();
      label2 = new Label();
      panelSky = new Panel();
      skyView = new SkyView();
      timer = new System.Windows.Forms.Timer(components);
      panelSelectedObjectDetails = new Panel();
      panelCatalog = new Panel();
      labelCatalog = new Label();
      label22 = new Label();
      panelAdditionalDetails = new Panel();
      label14 = new Label();
      panelSolarSystemBody = new Panel();
      labelPercentIlluminated = new Label();
      labelIlluminatedLabel = new Label();
      labelDiameter = new Label();
      label19 = new Label();
      panelConstellation = new Panel();
      labelConstellation = new Label();
      label17 = new Label();
      labelIsCurrentlyBelowHorizon = new Label();
      labelSelectedObjectName = new Label();
      label16 = new Label();
      labelSelectedObjectColor = new Label();
      label27 = new Label();
      labelSelectedObjectAzimuth = new Label();
      labelSelectedObjectElevation = new Label();
      label24 = new Label();
      label25 = new Label();
      labelSelectedObjectDeclination = new Label();
      labelSelectedObjectRa = new Label();
      label20 = new Label();
      label21 = new Label();
      labelSelectedObjectMagnitude = new Label();
      labelSelectedObjectType = new Label();
      label15 = new Label();
      label11 = new Label();
      label10 = new Label();
      toolTip = new ToolTip(components);
      panelStatusBar.SuspendLayout();
      panelSideBar.SuspendLayout();
      panelLookToward.SuspendLayout();
      panelWhatYouSee.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)trackBarMagnitude).BeginInit();
      ((System.ComponentModel.ISupportInitialize)trackBarFieldOfView).BeginInit();
      panelWhen.SuspendLayout();
      panelLatitudeLongitude.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)numericUpDownLatitude).BeginInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownLongitude).BeginInit();
      panelSky.SuspendLayout();
      panelSelectedObjectDetails.SuspendLayout();
      panelCatalog.SuspendLayout();
      panelAdditionalDetails.SuspendLayout();
      panelSolarSystemBody.SuspendLayout();
      panelConstellation.SuspendLayout();
      SuspendLayout();
      // 
      // panelStatusBar
      // 
      panelStatusBar.Controls.Add(labelStatusMessage);
      panelStatusBar.Controls.Add(labelStatusSiderealTime);
      panelStatusBar.Controls.Add(labelStatusFieldOfView);
      panelStatusBar.Controls.Add(labelStatusStarsDrawn);
      panelStatusBar.Dock = DockStyle.Bottom;
      panelStatusBar.Location = new Point(0, 778);
      panelStatusBar.Name = "panelStatusBar";
      panelStatusBar.Size = new Size(974, 21);
      panelStatusBar.TabIndex = 0;
      // 
      // labelStatusMessage
      // 
      labelStatusMessage.BorderStyle = BorderStyle.Fixed3D;
      labelStatusMessage.Dock = DockStyle.Fill;
      labelStatusMessage.Location = new Point(0, 0);
      labelStatusMessage.Name = "labelStatusMessage";
      labelStatusMessage.Size = new Size(598, 21);
      labelStatusMessage.TabIndex = 3;
      labelStatusMessage.Text = "messages";
      labelStatusMessage.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelStatusSiderealTime
      // 
      labelStatusSiderealTime.BorderStyle = BorderStyle.Fixed3D;
      labelStatusSiderealTime.Dock = DockStyle.Right;
      labelStatusSiderealTime.Location = new Point(598, 0);
      labelStatusSiderealTime.Name = "labelStatusSiderealTime";
      labelStatusSiderealTime.Size = new Size(143, 21);
      labelStatusSiderealTime.TabIndex = 2;
      labelStatusSiderealTime.Text = "sidereal time";
      labelStatusSiderealTime.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // labelStatusFieldOfView
      // 
      labelStatusFieldOfView.BorderStyle = BorderStyle.Fixed3D;
      labelStatusFieldOfView.Dock = DockStyle.Right;
      labelStatusFieldOfView.Location = new Point(741, 0);
      labelStatusFieldOfView.Name = "labelStatusFieldOfView";
      labelStatusFieldOfView.Size = new Size(117, 21);
      labelStatusFieldOfView.TabIndex = 1;
      labelStatusFieldOfView.Text = "field of view";
      labelStatusFieldOfView.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // labelStatusStarsDrawn
      // 
      labelStatusStarsDrawn.BorderStyle = BorderStyle.Fixed3D;
      labelStatusStarsDrawn.Dock = DockStyle.Right;
      labelStatusStarsDrawn.Location = new Point(858, 0);
      labelStatusStarsDrawn.Name = "labelStatusStarsDrawn";
      labelStatusStarsDrawn.Size = new Size(116, 21);
      labelStatusStarsDrawn.TabIndex = 0;
      labelStatusStarsDrawn.Text = "stars drawn";
      labelStatusStarsDrawn.TextAlign = ContentAlignment.MiddleRight;
      // 
      // panelSideBar
      // 
      panelSideBar.BackColor = SystemColors.Control;
      panelSideBar.Controls.Add(panelLookToward);
      panelSideBar.Controls.Add(panelWhatYouSee);
      panelSideBar.Controls.Add(panelWhen);
      panelSideBar.Controls.Add(panelLatitudeLongitude);
      panelSideBar.Dock = DockStyle.Right;
      panelSideBar.Location = new Point(704, 0);
      panelSideBar.Name = "panelSideBar";
      panelSideBar.Size = new Size(270, 675);
      panelSideBar.TabIndex = 1;
      // 
      // panelLookToward
      // 
      panelLookToward.Controls.Add(buttonUp);
      panelLookToward.Controls.Add(buttonW);
      panelLookToward.Controls.Add(buttonS);
      panelLookToward.Controls.Add(buttonE);
      panelLookToward.Controls.Add(buttonN);
      panelLookToward.Controls.Add(label13);
      panelLookToward.Dock = DockStyle.Top;
      panelLookToward.Location = new Point(0, 607);
      panelLookToward.Name = "panelLookToward";
      panelLookToward.Size = new Size(270, 67);
      panelLookToward.TabIndex = 0;
      // 
      // buttonUp
      // 
      buttonUp.Location = new Point(161, 34);
      buttonUp.Name = "buttonUp";
      buttonUp.Size = new Size(75, 23);
      buttonUp.TabIndex = 5;
      buttonUp.Text = "Zenith";
      toolTip.SetToolTip(buttonUp, "Point straight up");
      buttonUp.UseVisualStyleBackColor = true;
      buttonUp.Click += ButtonZenith_Click;
      // 
      // buttonW
      // 
      buttonW.Location = new Point(125, 34);
      buttonW.Name = "buttonW";
      buttonW.Size = new Size(34, 23);
      buttonW.TabIndex = 4;
      buttonW.Tag = "270";
      buttonW.Text = "W";
      toolTip.SetToolTip(buttonW, "Point West");
      buttonW.UseVisualStyleBackColor = true;
      buttonW.Click += ButtonCompassHeading_Click;
      // 
      // buttonS
      // 
      buttonS.Location = new Point(89, 34);
      buttonS.Name = "buttonS";
      buttonS.Size = new Size(34, 23);
      buttonS.TabIndex = 3;
      buttonS.Tag = "180";
      buttonS.Text = "S";
      toolTip.SetToolTip(buttonS, "Point South");
      buttonS.UseVisualStyleBackColor = true;
      buttonS.Click += ButtonCompassHeading_Click;
      // 
      // buttonE
      // 
      buttonE.Location = new Point(53, 34);
      buttonE.Name = "buttonE";
      buttonE.Size = new Size(34, 23);
      buttonE.TabIndex = 2;
      buttonE.Tag = "90";
      buttonE.Text = "E";
      toolTip.SetToolTip(buttonE, "Point East");
      buttonE.UseVisualStyleBackColor = true;
      buttonE.Click += ButtonCompassHeading_Click;
      // 
      // buttonN
      // 
      buttonN.Location = new Point(17, 34);
      buttonN.Name = "buttonN";
      buttonN.Size = new Size(34, 23);
      buttonN.TabIndex = 1;
      buttonN.Tag = "0";
      buttonN.Text = "N";
      toolTip.SetToolTip(buttonN, "Point North");
      buttonN.UseVisualStyleBackColor = true;
      buttonN.Click += ButtonCompassHeading_Click;
      // 
      // label13
      // 
      label13.AutoSize = true;
      label13.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      label13.Location = new Point(15, 10);
      label13.Name = "label13";
      label13.Size = new Size(104, 15);
      label13.TabIndex = 0;
      label13.Text = "Compass Heading";
      // 
      // panelWhatYouSee
      // 
      panelWhatYouSee.Controls.Add(label18);
      panelWhatYouSee.Controls.Add(checkBoxEcliptic);
      panelWhatYouSee.Controls.Add(checkBoxHorizonGrid);
      panelWhatYouSee.Controls.Add(checkBoxEquatorialGrid);
      panelWhatYouSee.Controls.Add(checkBoxDaylight);
      panelWhatYouSee.Controls.Add(checkBoxCompassPoints);
      panelWhatYouSee.Controls.Add(checkBoxGround);
      panelWhatYouSee.Controls.Add(checkBoxSolarSystemBodies);
      panelWhatYouSee.Controls.Add(checkBoxDeepSky);
      panelWhatYouSee.Controls.Add(checkBoxMilkyWay);
      panelWhatYouSee.Controls.Add(checkBoxStarNames);
      panelWhatYouSee.Controls.Add(checkBoxTheirNames);
      panelWhatYouSee.Controls.Add(checkBoxConstellations);
      panelWhatYouSee.Controls.Add(trackBarMagnitude);
      panelWhatYouSee.Controls.Add(labelMagnitude);
      panelWhatYouSee.Controls.Add(label12);
      panelWhatYouSee.Controls.Add(trackBarFieldOfView);
      panelWhatYouSee.Controls.Add(labelFieldOfView);
      panelWhatYouSee.Controls.Add(label9);
      panelWhatYouSee.Controls.Add(label8);
      panelWhatYouSee.Dock = DockStyle.Top;
      panelWhatYouSee.Location = new Point(0, 297);
      panelWhatYouSee.Name = "panelWhatYouSee";
      panelWhatYouSee.Size = new Size(270, 310);
      panelWhatYouSee.TabIndex = 0;
      // 
      // label18
      // 
      label18.AutoSize = true;
      label18.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      label18.Location = new Point(14, 144);
      label18.Name = "label18";
      label18.Size = new Size(42, 15);
      label18.TabIndex = 47;
      label18.Text = "Layers";
      label18.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // checkBoxEcliptic
      // 
      checkBoxEcliptic.AutoSize = true;
      checkBoxEcliptic.Location = new Point(152, 286);
      checkBoxEcliptic.Name = "checkBoxEcliptic";
      checkBoxEcliptic.Size = new Size(64, 19);
      checkBoxEcliptic.TabIndex = 46;
      checkBoxEcliptic.Text = "Ecliptic";
      toolTip.SetToolTip(checkBoxEcliptic, "Show ecliptic projected on sky");
      checkBoxEcliptic.UseVisualStyleBackColor = true;
      checkBoxEcliptic.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxHorizonGrid
      // 
      checkBoxHorizonGrid.AutoSize = true;
      checkBoxHorizonGrid.Location = new Point(152, 263);
      checkBoxHorizonGrid.Name = "checkBoxHorizonGrid";
      checkBoxHorizonGrid.Size = new Size(92, 19);
      checkBoxHorizonGrid.TabIndex = 45;
      checkBoxHorizonGrid.Text = "Horizon grid";
      toolTip.SetToolTip(checkBoxHorizonGrid, "Show latitude/longitude lines projected on sky");
      checkBoxHorizonGrid.UseVisualStyleBackColor = true;
      checkBoxHorizonGrid.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxEquatorialGrid
      // 
      checkBoxEquatorialGrid.AutoSize = true;
      checkBoxEquatorialGrid.Location = new Point(152, 240);
      checkBoxEquatorialGrid.Name = "checkBoxEquatorialGrid";
      checkBoxEquatorialGrid.Size = new Size(103, 19);
      checkBoxEquatorialGrid.TabIndex = 44;
      checkBoxEquatorialGrid.Text = "Equatorial grid";
      toolTip.SetToolTip(checkBoxEquatorialGrid, "Show earth equator projected onto sky");
      checkBoxEquatorialGrid.UseVisualStyleBackColor = true;
      checkBoxEquatorialGrid.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxDaylight
      // 
      checkBoxDaylight.AutoSize = true;
      checkBoxDaylight.Location = new Point(152, 217);
      checkBoxDaylight.Name = "checkBoxDaylight";
      checkBoxDaylight.Size = new Size(70, 19);
      checkBoxDaylight.TabIndex = 43;
      checkBoxDaylight.Text = "Daylight";
      toolTip.SetToolTip(checkBoxDaylight, "Show illuminated sky if daytime");
      checkBoxDaylight.UseVisualStyleBackColor = true;
      checkBoxDaylight.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxCompassPoints
      // 
      checkBoxCompassPoints.AutoSize = true;
      checkBoxCompassPoints.Location = new Point(152, 194);
      checkBoxCompassPoints.Name = "checkBoxCompassPoints";
      checkBoxCompassPoints.Size = new Size(111, 19);
      checkBoxCompassPoints.TabIndex = 42;
      checkBoxCompassPoints.Text = "Compass points";
      toolTip.SetToolTip(checkBoxCompassPoints, "Show main compass directions");
      checkBoxCompassPoints.UseVisualStyleBackColor = true;
      checkBoxCompassPoints.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxGround
      // 
      checkBoxGround.AutoSize = true;
      checkBoxGround.Location = new Point(152, 171);
      checkBoxGround.Name = "checkBoxGround";
      checkBoxGround.Size = new Size(66, 19);
      checkBoxGround.TabIndex = 41;
      checkBoxGround.Text = "Ground";
      toolTip.SetToolTip(checkBoxGround, "Show ground and horizon");
      checkBoxGround.UseVisualStyleBackColor = true;
      checkBoxGround.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxSolarSystemBodies
      // 
      checkBoxSolarSystemBodies.AutoSize = true;
      checkBoxSolarSystemBodies.Location = new Point(12, 286);
      checkBoxSolarSystemBodies.Name = "checkBoxSolarSystemBodies";
      checkBoxSolarSystemBodies.Size = new Size(130, 19);
      checkBoxSolarSystemBodies.TabIndex = 40;
      checkBoxSolarSystemBodies.Text = "Solar system bodies";
      toolTip.SetToolTip(checkBoxSolarSystemBodies, "Show Sun, Moon and  planets");
      checkBoxSolarSystemBodies.UseVisualStyleBackColor = true;
      checkBoxSolarSystemBodies.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxDeepSky
      // 
      checkBoxDeepSky.AutoSize = true;
      checkBoxDeepSky.Location = new Point(12, 263);
      checkBoxDeepSky.Name = "checkBoxDeepSky";
      checkBoxDeepSky.Size = new Size(73, 19);
      checkBoxDeepSky.TabIndex = 39;
      checkBoxDeepSky.Text = "Deep sky";
      toolTip.SetToolTip(checkBoxDeepSky, "Show very distant objects");
      checkBoxDeepSky.UseVisualStyleBackColor = true;
      checkBoxDeepSky.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxMilkyWay
      // 
      checkBoxMilkyWay.AutoSize = true;
      checkBoxMilkyWay.Location = new Point(12, 240);
      checkBoxMilkyWay.Name = "checkBoxMilkyWay";
      checkBoxMilkyWay.Size = new Size(81, 19);
      checkBoxMilkyWay.TabIndex = 38;
      checkBoxMilkyWay.Text = "Milky Way";
      toolTip.SetToolTip(checkBoxMilkyWay, "Show Milky Way galaxy");
      checkBoxMilkyWay.UseVisualStyleBackColor = true;
      checkBoxMilkyWay.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxStarNames
      // 
      checkBoxStarNames.AutoSize = true;
      checkBoxStarNames.Location = new Point(12, 217);
      checkBoxStarNames.Name = "checkBoxStarNames";
      checkBoxStarNames.Size = new Size(84, 19);
      checkBoxStarNames.TabIndex = 37;
      checkBoxStarNames.Text = "Star names";
      toolTip.SetToolTip(checkBoxStarNames, "Show star names");
      checkBoxStarNames.UseVisualStyleBackColor = true;
      checkBoxStarNames.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxTheirNames
      // 
      checkBoxTheirNames.AutoSize = true;
      checkBoxTheirNames.Location = new Point(12, 194);
      checkBoxTheirNames.Name = "checkBoxTheirNames";
      checkBoxTheirNames.Size = new Size(90, 19);
      checkBoxTheirNames.TabIndex = 36;
      checkBoxTheirNames.Text = "Their names";
      toolTip.SetToolTip(checkBoxTheirNames, "Show constellation names");
      checkBoxTheirNames.UseVisualStyleBackColor = true;
      checkBoxTheirNames.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // checkBoxConstellations
      // 
      checkBoxConstellations.AutoSize = true;
      checkBoxConstellations.Location = new Point(12, 171);
      checkBoxConstellations.Name = "checkBoxConstellations";
      checkBoxConstellations.Size = new Size(101, 19);
      checkBoxConstellations.TabIndex = 35;
      checkBoxConstellations.Text = "Constellations";
      toolTip.SetToolTip(checkBoxConstellations, "Show constellation lines");
      checkBoxConstellations.UseVisualStyleBackColor = true;
      checkBoxConstellations.CheckedChanged += CheckBoxLayer_CheckedChanged;
      // 
      // trackBarMagnitude
      // 
      trackBarMagnitude.Location = new Point(19, 102);
      trackBarMagnitude.Maximum = 65;
      trackBarMagnitude.Minimum = 10;
      trackBarMagnitude.Name = "trackBarMagnitude";
      trackBarMagnitude.Size = new Size(236, 45);
      trackBarMagnitude.TabIndex = 34;
      trackBarMagnitude.TickStyle = TickStyle.None;
      trackBarMagnitude.Value = 58;
      trackBarMagnitude.Scroll += TrackBarMagnitude_Scroll;
      // 
      // labelMagnitude
      // 
      labelMagnitude.AutoSize = true;
      labelMagnitude.Location = new Point(206, 86);
      labelMagnitude.Name = "labelMagnitude";
      labelMagnitude.Size = new Size(49, 15);
      labelMagnitude.TabIndex = 33;
      labelMagnitude.Text = "mag .58";
      // 
      // label12
      // 
      label12.AutoSize = true;
      label12.Location = new Point(16, 86);
      label12.Name = "label12";
      label12.Size = new Size(70, 15);
      label12.TabIndex = 32;
      label12.Text = "Faintest star";
      // 
      // trackBarFieldOfView
      // 
      trackBarFieldOfView.Location = new Point(19, 52);
      trackBarFieldOfView.Maximum = 1000;
      trackBarFieldOfView.Minimum = 10;
      trackBarFieldOfView.Name = "trackBarFieldOfView";
      trackBarFieldOfView.Size = new Size(236, 45);
      trackBarFieldOfView.TabIndex = 31;
      trackBarFieldOfView.TickStyle = TickStyle.None;
      trackBarFieldOfView.Value = 105;
      trackBarFieldOfView.Scroll += TrackBarFieldOfView_Scroll;
      // 
      // labelFieldOfView
      // 
      labelFieldOfView.AutoSize = true;
      labelFieldOfView.Location = new Point(213, 35);
      labelFieldOfView.Name = "labelFieldOfView";
      labelFieldOfView.Size = new Size(42, 15);
      labelFieldOfView.TabIndex = 30;
      labelFieldOfView.Text = "105.0° ";
      // 
      // label9
      // 
      label9.AutoSize = true;
      label9.Location = new Point(16, 35);
      label9.Name = "label9";
      label9.Size = new Size(73, 15);
      label9.TabIndex = 29;
      label9.Text = "Field of view";
      // 
      // label8
      // 
      label8.AutoSize = true;
      label8.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      label8.Location = new Point(14, 12);
      label8.Name = "label8";
      label8.Size = new Size(82, 15);
      label8.TabIndex = 28;
      label8.Text = "What you see";
      label8.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // panelWhen
      // 
      panelWhen.Controls.Add(comboBoxTimeRunsAt);
      panelWhen.Controls.Add(label7);
      panelWhen.Controls.Add(buttonNow);
      panelWhen.Controls.Add(dateTimePickerTime);
      panelWhen.Controls.Add(dateTimePickerDate);
      panelWhen.Controls.Add(label6);
      panelWhen.Dock = DockStyle.Top;
      panelWhen.Location = new Point(0, 144);
      panelWhen.Name = "panelWhen";
      panelWhen.Size = new Size(270, 153);
      panelWhen.TabIndex = 3;
      // 
      // comboBoxTimeRunsAt
      // 
      comboBoxTimeRunsAt.FormattingEnabled = true;
      comboBoxTimeRunsAt.Location = new Point(16, 117);
      comboBoxTimeRunsAt.Name = "comboBoxTimeRunsAt";
      comboBoxTimeRunsAt.Size = new Size(239, 23);
      comboBoxTimeRunsAt.TabIndex = 13;
      comboBoxTimeRunsAt.Text = "Realtime";
      comboBoxTimeRunsAt.SelectedIndexChanged += ComboBoxTimeRunsAt_SelectedIndexChanged;
      // 
      // label7
      // 
      label7.AutoSize = true;
      label7.Location = new Point(16, 99);
      label7.Name = "label7";
      label7.Size = new Size(72, 15);
      label7.TabIndex = 12;
      label7.Text = "Time runs at";
      label7.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // buttonNow
      // 
      buttonNow.Location = new Point(131, 61);
      buttonNow.Name = "buttonNow";
      buttonNow.Size = new Size(124, 23);
      buttonNow.TabIndex = 11;
      buttonNow.Text = "Now";
      buttonNow.UseVisualStyleBackColor = true;
      buttonNow.Click += ButtonNow_Click;
      // 
      // dateTimePickerTime
      // 
      dateTimePickerTime.Format = DateTimePickerFormat.Time;
      dateTimePickerTime.Location = new Point(15, 61);
      dateTimePickerTime.Name = "dateTimePickerTime";
      dateTimePickerTime.ShowUpDown = true;
      dateTimePickerTime.Size = new Size(87, 23);
      dateTimePickerTime.TabIndex = 10;
      dateTimePickerTime.ValueChanged += DateTimePicker_ValueChanged;
      // 
      // dateTimePickerDate
      // 
      dateTimePickerDate.Location = new Point(15, 32);
      dateTimePickerDate.Name = "dateTimePickerDate";
      dateTimePickerDate.Size = new Size(240, 23);
      dateTimePickerDate.TabIndex = 9;
      dateTimePickerDate.ValueChanged += DateTimePicker_ValueChanged;
      // 
      // label6
      // 
      label6.AutoSize = true;
      label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      label6.Location = new Point(14, 12);
      label6.Name = "label6";
      label6.Size = new Size(35, 15);
      label6.TabIndex = 8;
      label6.Text = "Time";
      label6.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // panelLatitudeLongitude
      // 
      panelLatitudeLongitude.Controls.Add(buttonSetCustomSite);
      panelLatitudeLongitude.Controls.Add(comboBoxYourLocation);
      panelLatitudeLongitude.Controls.Add(label1);
      panelLatitudeLongitude.Controls.Add(label5);
      panelLatitudeLongitude.Controls.Add(label4);
      panelLatitudeLongitude.Controls.Add(numericUpDownLatitude);
      panelLatitudeLongitude.Controls.Add(numericUpDownLongitude);
      panelLatitudeLongitude.Controls.Add(label3);
      panelLatitudeLongitude.Controls.Add(label2);
      panelLatitudeLongitude.Dock = DockStyle.Top;
      panelLatitudeLongitude.Location = new Point(0, 0);
      panelLatitudeLongitude.Name = "panelLatitudeLongitude";
      panelLatitudeLongitude.Size = new Size(270, 144);
      panelLatitudeLongitude.TabIndex = 2;
      // 
      // buttonSetCustomSite
      // 
      buttonSetCustomSite.Location = new Point(16, 113);
      buttonSetCustomSite.Name = "buttonSetCustomSite";
      buttonSetCustomSite.Size = new Size(239, 23);
      buttonSetCustomSite.TabIndex = 3;
      buttonSetCustomSite.Text = "Set a Custom Site";
      buttonSetCustomSite.UseVisualStyleBackColor = true;
      buttonSetCustomSite.Click += ButtonSetCustomSite_Click;
      // 
      // comboBoxYourLocation
      // 
      comboBoxYourLocation.FormattingEnabled = true;
      comboBoxYourLocation.Location = new Point(16, 29);
      comboBoxYourLocation.Name = "comboBoxYourLocation";
      comboBoxYourLocation.Size = new Size(239, 23);
      comboBoxYourLocation.TabIndex = 8;
      comboBoxYourLocation.SelectedIndexChanged += ComboBoxYourLocation_SelectedIndexChanged;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      label1.Location = new Point(14, 9);
      label1.Name = "label1";
      label1.Size = new Size(79, 15);
      label1.TabIndex = 7;
      label1.Text = "Your location";
      label1.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.Location = new Point(154, 90);
      label5.Name = "label5";
      label5.Size = new Size(36, 15);
      label5.TabIndex = 6;
      label5.Text = "° east";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.Location = new Point(154, 62);
      label4.Name = "label4";
      label4.Size = new Size(44, 15);
      label4.TabIndex = 5;
      label4.Text = "° north";
      // 
      // numericUpDownLatitude
      // 
      numericUpDownLatitude.DecimalPlaces = 3;
      numericUpDownLatitude.Location = new Point(84, 58);
      numericUpDownLatitude.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
      numericUpDownLatitude.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
      numericUpDownLatitude.Name = "numericUpDownLatitude";
      numericUpDownLatitude.Size = new Size(67, 23);
      numericUpDownLatitude.TabIndex = 4;
      numericUpDownLatitude.Value = new decimal(new int[] { 33640, 0, 0, 196608 });
      numericUpDownLatitude.ValueChanged += LatitudeLongitude_ValueChanged;
      // 
      // numericUpDownLongitude
      // 
      numericUpDownLongitude.DecimalPlaces = 3;
      numericUpDownLongitude.Location = new Point(84, 84);
      numericUpDownLongitude.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
      numericUpDownLongitude.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
      numericUpDownLongitude.Name = "numericUpDownLongitude";
      numericUpDownLongitude.Size = new Size(67, 23);
      numericUpDownLongitude.TabIndex = 3;
      numericUpDownLongitude.Value = new decimal(new int[] { 117603, 0, 0, -2147287040 });
      numericUpDownLongitude.ValueChanged += LatitudeLongitude_ValueChanged;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.Location = new Point(16, 86);
      label3.Name = "label3";
      label3.Size = new Size(61, 15);
      label3.TabIndex = 1;
      label3.Text = "Longitude";
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.Location = new Point(16, 62);
      label2.Name = "label2";
      label2.Size = new Size(50, 15);
      label2.TabIndex = 0;
      label2.Text = "Latitude";
      // 
      // panelSky
      // 
      panelSky.Controls.Add(skyView);
      panelSky.Dock = DockStyle.Fill;
      panelSky.Location = new Point(0, 0);
      panelSky.Name = "panelSky";
      panelSky.Size = new Size(974, 675);
      panelSky.TabIndex = 2;
      // 
      // skyView
      // 
      skyView.BackColor = Color.FromArgb(5, 7, 12);
      skyView.CenterAltitude = 45D;
      skyView.CenterAzimuth = 0D;
      skyView.Dock = DockStyle.Fill;
      skyView.FieldOfView = 100D;
      skyView.Location = new Point(0, 0);
      skyView.Margin = new Padding(0);
      skyView.Name = "skyView";
      skyView.ShowGround = true;
      skyView.Size = new Size(974, 675);
      skyView.TabIndex = 0;
      skyView.SelectionChanged += SkyView_SelectionChanged;
      skyView.ViewChanged += SkyView_ViewChanged;
      // 
      // timer
      // 
      timer.Tick += Timer_Tick;
      // 
      // panelSelectedObjectDetails
      // 
      panelSelectedObjectDetails.Controls.Add(panelCatalog);
      panelSelectedObjectDetails.Controls.Add(panelAdditionalDetails);
      panelSelectedObjectDetails.Controls.Add(panelSolarSystemBody);
      panelSelectedObjectDetails.Controls.Add(panelConstellation);
      panelSelectedObjectDetails.Controls.Add(labelIsCurrentlyBelowHorizon);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectName);
      panelSelectedObjectDetails.Controls.Add(label16);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectColor);
      panelSelectedObjectDetails.Controls.Add(label27);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectAzimuth);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectElevation);
      panelSelectedObjectDetails.Controls.Add(label24);
      panelSelectedObjectDetails.Controls.Add(label25);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectDeclination);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectRa);
      panelSelectedObjectDetails.Controls.Add(label20);
      panelSelectedObjectDetails.Controls.Add(label21);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectMagnitude);
      panelSelectedObjectDetails.Controls.Add(labelSelectedObjectType);
      panelSelectedObjectDetails.Controls.Add(label15);
      panelSelectedObjectDetails.Controls.Add(label11);
      panelSelectedObjectDetails.Controls.Add(label10);
      panelSelectedObjectDetails.Dock = DockStyle.Bottom;
      panelSelectedObjectDetails.Location = new Point(0, 675);
      panelSelectedObjectDetails.Name = "panelSelectedObjectDetails";
      panelSelectedObjectDetails.Size = new Size(974, 103);
      panelSelectedObjectDetails.TabIndex = 3;
      // 
      // panelCatalog
      // 
      panelCatalog.Controls.Add(labelCatalog);
      panelCatalog.Controls.Add(label22);
      panelCatalog.Location = new Point(738, 77);
      panelCatalog.Name = "panelCatalog";
      panelCatalog.Size = new Size(221, 19);
      panelCatalog.TabIndex = 21;
      // 
      // labelCatalog
      // 
      labelCatalog.AutoEllipsis = true;
      labelCatalog.Location = new Point(86, 3);
      labelCatalog.Name = "labelCatalog";
      labelCatalog.Size = new Size(116, 15);
      labelCatalog.TabIndex = 13;
      labelCatalog.Text = "Cr 464";
      toolTip.SetToolTip(labelCatalog, "The catalog classificaion of the object. Cr denotes the Collinder Catalog.");
      // 
      // label22
      // 
      label22.AutoSize = true;
      label22.Location = new Point(34, 3);
      label22.Name = "label22";
      label22.Size = new Size(51, 15);
      label22.TabIndex = 12;
      label22.Text = "Catalog:";
      // 
      // panelAdditionalDetails
      // 
      panelAdditionalDetails.Controls.Add(label14);
      panelAdditionalDetails.Location = new Point(495, 28);
      panelAdditionalDetails.Name = "panelAdditionalDetails";
      panelAdditionalDetails.Size = new Size(200, 67);
      panelAdditionalDetails.TabIndex = 20;
      // 
      // label14
      // 
      label14.Location = new Point(13, 2);
      label14.Name = "label14";
      label14.Size = new Size(164, 33);
      label14.TabIndex = 0;
      label14.Text = "Additional details on the right will be moved here";
      // 
      // panelSolarSystemBody
      // 
      panelSolarSystemBody.Controls.Add(labelPercentIlluminated);
      panelSolarSystemBody.Controls.Add(labelIlluminatedLabel);
      panelSolarSystemBody.Controls.Add(labelDiameter);
      panelSolarSystemBody.Controls.Add(label19);
      panelSolarSystemBody.Location = new Point(738, 34);
      panelSolarSystemBody.Name = "panelSolarSystemBody";
      panelSolarSystemBody.Size = new Size(221, 38);
      panelSolarSystemBody.TabIndex = 19;
      // 
      // labelPercentIlluminated
      // 
      labelPercentIlluminated.AutoEllipsis = true;
      labelPercentIlluminated.Location = new Point(86, 20);
      labelPercentIlluminated.Name = "labelPercentIlluminated";
      labelPercentIlluminated.Size = new Size(116, 15);
      labelPercentIlluminated.TabIndex = 15;
      labelPercentIlluminated.Text = "27%";
      toolTip.SetToolTip(labelPercentIlluminated, "Percentage of the object's disk illuminated by the sun, as seen by observer");
      // 
      // labelIlluminatedLabel
      // 
      labelIlluminatedLabel.AutoSize = true;
      labelIlluminatedLabel.Location = new Point(15, 20);
      labelIlluminatedLabel.Name = "labelIlluminatedLabel";
      labelIlluminatedLabel.Size = new Size(70, 15);
      labelIlluminatedLabel.TabIndex = 14;
      labelIlluminatedLabel.Text = "Illuminated:";
      // 
      // labelDiameter
      // 
      labelDiameter.AutoEllipsis = true;
      labelDiameter.Location = new Point(86, 3);
      labelDiameter.Name = "labelDiameter";
      labelDiameter.Size = new Size(116, 15);
      labelDiameter.TabIndex = 13;
      labelDiameter.Text = "0.6 arcmin";
      // 
      // label19
      // 
      label19.AutoSize = true;
      label19.Location = new Point(27, 3);
      label19.Name = "label19";
      label19.Size = new Size(58, 15);
      label19.TabIndex = 12;
      label19.Text = "Diameter:";
      // 
      // panelConstellation
      // 
      panelConstellation.Controls.Add(labelConstellation);
      panelConstellation.Controls.Add(label17);
      panelConstellation.Location = new Point(738, 9);
      panelConstellation.Name = "panelConstellation";
      panelConstellation.Size = new Size(221, 19);
      panelConstellation.TabIndex = 18;
      // 
      // labelConstellation
      // 
      labelConstellation.AutoEllipsis = true;
      labelConstellation.Location = new Point(86, 3);
      labelConstellation.Name = "labelConstellation";
      labelConstellation.Size = new Size(116, 15);
      labelConstellation.TabIndex = 13;
      labelConstellation.Text = "Andromeda";
      // 
      // label17
      // 
      label17.AutoSize = true;
      label17.Location = new Point(5, 3);
      label17.Name = "label17";
      label17.Size = new Size(80, 15);
      label17.TabIndex = 12;
      label17.Text = "Constellation:";
      // 
      // labelIsCurrentlyBelowHorizon
      // 
      labelIsCurrentlyBelowHorizon.AutoSize = true;
      labelIsCurrentlyBelowHorizon.ForeColor = Color.Red;
      labelIsCurrentlyBelowHorizon.Location = new Point(249, 4);
      labelIsCurrentlyBelowHorizon.Name = "labelIsCurrentlyBelowHorizon";
      labelIsCurrentlyBelowHorizon.Size = new Size(181, 15);
      labelIsCurrentlyBelowHorizon.TabIndex = 17;
      labelIsCurrentlyBelowHorizon.Text = "Object is currently below horizon";
      // 
      // labelSelectedObjectName
      // 
      labelSelectedObjectName.AutoEllipsis = true;
      labelSelectedObjectName.Location = new Point(75, 30);
      labelSelectedObjectName.Name = "labelSelectedObjectName";
      labelSelectedObjectName.Size = new Size(196, 15);
      labelSelectedObjectName.TabIndex = 16;
      labelSelectedObjectName.Text = "<unknown>";
      toolTip.SetToolTip(labelSelectedObjectName, "Name of object");
      // 
      // label16
      // 
      label16.AutoSize = true;
      label16.Location = new Point(31, 30);
      label16.Name = "label16";
      label16.Size = new Size(42, 15);
      label16.TabIndex = 15;
      label16.Text = "Name:";
      // 
      // labelSelectedObjectColor
      // 
      labelSelectedObjectColor.AutoSize = true;
      labelSelectedObjectColor.Location = new Point(75, 65);
      labelSelectedObjectColor.Name = "labelSelectedObjectColor";
      labelSelectedObjectColor.Size = new Size(58, 15);
      labelSelectedObjectColor.TabIndex = 14;
      labelSelectedObjectColor.Text = "B-V +1.23";
      toolTip.SetToolTip(labelSelectedObjectColor, "Blue component-Yellow component");
      // 
      // label27
      // 
      label27.AutoSize = true;
      label27.Location = new Point(35, 65);
      label27.Name = "label27";
      label27.Size = new Size(39, 15);
      label27.TabIndex = 13;
      label27.Text = "Color:";
      // 
      // labelSelectedObjectAzimuth
      // 
      labelSelectedObjectAzimuth.AutoSize = true;
      labelSelectedObjectAzimuth.Location = new Point(342, 80);
      labelSelectedObjectAzimuth.Name = "labelSelectedObjectAzimuth";
      labelSelectedObjectAzimuth.Size = new Size(77, 15);
      labelSelectedObjectAzimuth.TabIndex = 12;
      labelSelectedObjectAzimuth.Text = "232° 26' 59.8\"";
      toolTip.SetToolTip(labelSelectedObjectAzimuth, "the horizontal direction, measured in degrees clockwise");
      // 
      // labelSelectedObjectElevation
      // 
      labelSelectedObjectElevation.AutoSize = true;
      labelSelectedObjectElevation.Location = new Point(340, 63);
      labelSelectedObjectElevation.Name = "labelSelectedObjectElevation";
      labelSelectedObjectElevation.Size = new Size(79, 15);
      labelSelectedObjectElevation.TabIndex = 11;
      labelSelectedObjectElevation.Text = "+07° 00' 15.7\"";
      toolTip.SetToolTip(labelSelectedObjectElevation, "Also called Altitude, measures the vertical angle above the observer's horizon");
      // 
      // label24
      // 
      label24.AutoSize = true;
      label24.Location = new Point(286, 80);
      label24.Name = "label24";
      label24.Size = new Size(55, 15);
      label24.TabIndex = 10;
      label24.Text = "Azimuth:";
      // 
      // label25
      // 
      label25.AutoSize = true;
      label25.Location = new Point(282, 63);
      label25.Name = "label25";
      label25.Size = new Size(58, 15);
      label25.TabIndex = 9;
      label25.Text = "Elevation:";
      // 
      // labelSelectedObjectDeclination
      // 
      labelSelectedObjectDeclination.AutoSize = true;
      labelSelectedObjectDeclination.Location = new Point(341, 47);
      labelSelectedObjectDeclination.Name = "labelSelectedObjectDeclination";
      labelSelectedObjectDeclination.Size = new Size(76, 15);
      labelSelectedObjectDeclination.TabIndex = 8;
      labelSelectedObjectDeclination.Text = "-25° 56' 13.9\"";
      toolTip.SetToolTip(labelSelectedObjectDeclination, "The celestial equivalent of latitude on Earth.");
      // 
      // labelSelectedObjectRa
      // 
      labelSelectedObjectRa.AutoSize = true;
      labelSelectedObjectRa.Location = new Point(343, 30);
      labelSelectedObjectRa.Name = "labelSelectedObjectRa";
      labelSelectedObjectRa.Size = new Size(81, 15);
      labelSelectedObjectRa.TabIndex = 7;
      labelSelectedObjectRa.Text = "16h 04m 57.9s";
      toolTip.SetToolTip(labelSelectedObjectRa, "Right Ascension. The celestial equivalent of longitude on Earth., but measured in hours.");
      // 
      // label20
      // 
      label20.AutoSize = true;
      label20.Location = new Point(271, 47);
      label20.Name = "label20";
      label20.Size = new Size(70, 15);
      label20.TabIndex = 6;
      label20.Text = "Declination:";
      // 
      // label21
      // 
      label21.AutoSize = true;
      label21.Location = new Point(318, 30);
      label21.Name = "label21";
      label21.Size = new Size(25, 15);
      label21.TabIndex = 5;
      label21.Text = "RA:";
      toolTip.SetToolTip(label21, "Right Ascension");
      // 
      // labelSelectedObjectMagnitude
      // 
      labelSelectedObjectMagnitude.AutoSize = true;
      labelSelectedObjectMagnitude.Location = new Point(73, 80);
      labelSelectedObjectMagnitude.Name = "labelSelectedObjectMagnitude";
      labelSelectedObjectMagnitude.Size = new Size(36, 15);
      labelSelectedObjectMagnitude.TabIndex = 4;
      labelSelectedObjectMagnitude.Text = "+4.96";
      toolTip.SetToolTip(labelSelectedObjectMagnitude, "Logarithm value of apparent brightness. Smaller values are brighter.");
      // 
      // labelSelectedObjectType
      // 
      labelSelectedObjectType.AutoSize = true;
      labelSelectedObjectType.Location = new Point(75, 47);
      labelSelectedObjectType.Name = "labelSelectedObjectType";
      labelSelectedObjectType.Size = new Size(27, 15);
      labelSelectedObjectType.TabIndex = 3;
      labelSelectedObjectType.Text = "Star";
      toolTip.SetToolTip(labelSelectedObjectType, "Star, planet, galaxy, etc.");
      // 
      // label15
      // 
      label15.AutoSize = true;
      label15.Location = new Point(5, 80);
      label15.Name = "label15";
      label15.Size = new Size(68, 15);
      label15.TabIndex = 2;
      label15.Text = "Magnitude:";
      // 
      // label11
      // 
      label11.AutoSize = true;
      label11.Location = new Point(39, 47);
      label11.Name = "label11";
      label11.Size = new Size(34, 15);
      label11.TabIndex = 1;
      label11.Text = "Type:";
      // 
      // label10
      // 
      label10.AutoSize = true;
      label10.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
      label10.Location = new Point(10, 4);
      label10.Name = "label10";
      label10.Size = new Size(137, 15);
      label10.TabIndex = 0;
      label10.Text = "Selected Object Details";
      // 
      // FormMain
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(974, 799);
      Controls.Add(panelSideBar);
      Controls.Add(panelSky);
      Controls.Add(panelSelectedObjectDetails);
      Controls.Add(panelStatusBar);
      Name = "FormMain";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "SkyView";
      panelStatusBar.ResumeLayout(false);
      panelSideBar.ResumeLayout(false);
      panelLookToward.ResumeLayout(false);
      panelLookToward.PerformLayout();
      panelWhatYouSee.ResumeLayout(false);
      panelWhatYouSee.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)trackBarMagnitude).EndInit();
      ((System.ComponentModel.ISupportInitialize)trackBarFieldOfView).EndInit();
      panelWhen.ResumeLayout(false);
      panelWhen.PerformLayout();
      panelLatitudeLongitude.ResumeLayout(false);
      panelLatitudeLongitude.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)numericUpDownLatitude).EndInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownLongitude).EndInit();
      panelSky.ResumeLayout(false);
      panelSelectedObjectDetails.ResumeLayout(false);
      panelSelectedObjectDetails.PerformLayout();
      panelCatalog.ResumeLayout(false);
      panelCatalog.PerformLayout();
      panelAdditionalDetails.ResumeLayout(false);
      panelSolarSystemBody.ResumeLayout(false);
      panelSolarSystemBody.PerformLayout();
      panelConstellation.ResumeLayout(false);
      panelConstellation.PerformLayout();
      ResumeLayout(false);
    }

    #endregion

    private Panel panelStatusBar;
    private Panel panelSideBar;
    private Panel panelSky;
    private Label labelStatusStarsDrawn;
    private Label labelStatusMessage;
    private Label labelStatusSiderealTime;
    private Label labelStatusFieldOfView;
    private Panel panelLatitudeLongitude;
    private NumericUpDown numericUpDownLatitude;
    private NumericUpDown numericUpDownLongitude;
    private Label label3;
    private Label label2;
    private Label label4;
    private Label label5;
    private ComboBox comboBoxYourLocation;
    private Label label1;
    private Panel panelWhen;
    private DateTimePicker dateTimePickerTime;
    private DateTimePicker dateTimePickerDate;
    private Label label6;
    private Button buttonSetCustomSite;
    private ComboBox comboBoxTimeRunsAt;
    private Label label7;
    private Button buttonNow;
    private Panel panelWhatYouSee;
    private CheckBox checkBoxEcliptic;
    private CheckBox checkBoxHorizonGrid;
    private CheckBox checkBoxEquatorialGrid;
    private CheckBox checkBoxDaylight;
    private CheckBox checkBoxCompassPoints;
    private CheckBox checkBoxGround;
    private CheckBox checkBoxSolarSystemBodies;
    private CheckBox checkBoxDeepSky;
    private CheckBox checkBoxMilkyWay;
    private CheckBox checkBoxStarNames;
    private CheckBox checkBoxTheirNames;
    private CheckBox checkBoxConstellations;
    private TrackBar trackBarMagnitude;
    private Label labelMagnitude;
    private Label label12;
    private TrackBar trackBarFieldOfView;
    private Label labelFieldOfView;
    private Label label9;
    private Label label8;
    private Panel panelLookToward;
    private Button buttonUp;
    private Button buttonW;
    private Button buttonS;
    private Button buttonE;
    private Button buttonN;
    private Label label13;
    private SkyView skyView;
    private System.Windows.Forms.Timer timer;
    private Panel panelSelectedObjectDetails;
    private Label label10;
    private Label labelSelectedObjectColor;
    private Label label27;
    private Label labelSelectedObjectAzimuth;
    private Label labelSelectedObjectElevation;
    private Label label24;
    private Label label25;
    private Label labelSelectedObjectDeclination;
    private Label labelSelectedObjectRa;
    private Label label20;
    private Label label21;
    private Label labelSelectedObjectMagnitude;
    private Label labelSelectedObjectType;
    private Label label15;
    private Label label11;
    private Label labelSelectedObjectName;
    private Label label16;
    private Label labelIsCurrentlyBelowHorizon;
    private Panel panelConstellation;
    private Label labelConstellation;
    private Label label17;
    private ToolTip toolTip;
    private Panel panelSolarSystemBody;
    private Label labelDiameter;
    private Label label19;
    private Panel panelAdditionalDetails;
    private Label label14;
    private Label labelPercentIlluminated;
    private Label labelIlluminatedLabel;
    private Panel panelCatalog;
    private Label labelCatalog;
    private Label label22;
    private Label label18;
  }
}