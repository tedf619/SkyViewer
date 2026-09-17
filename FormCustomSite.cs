using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SkyViewer
{
  public partial class FormCustomSite : Form
  {
    public ObservingSite Result { get; private set; }
    
    public FormCustomSite(string name, double latitude, double longitude, double utcOffsetHours)
    {
      InitializeComponent();

      textBoxName.Text = name;
      numericUpDownLatitude.Value = (decimal)Math.Clamp(latitude, -90, 90);
      numericUpDownLongitude.Value = (decimal)Math.Clamp(longitude, -180, 180);
      numericUpDownClockOffset.Value = (decimal)Math.Clamp(utcOffsetHours, -12, 14);

      Result = new ObservingSite(name, latitude, longitude, utcOffsetHours);
    }

    void ButtonOk_Click(object? sender, EventArgs e)
    {
      string name = textBoxName.Text.Trim();
      if (name.Length == 0)
      {
        double lat = (double)numericUpDownLatitude.Value;
        double lon = (double)numericUpDownLongitude.Value;
        name = $"{Math.Abs(lat):0.##}°{(lat < 0 ? 'S' : 'N')} {Math.Abs(lon):0.##}°{(lon < 0 ? 'W' : 'E')}";
      }

      Result = new ObservingSite(
          name,
          (double)numericUpDownLatitude.Value,
          (double)numericUpDownLongitude.Value,
          (double)numericUpDownClockOffset.Value);
    }
  }
}
