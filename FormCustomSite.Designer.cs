namespace SkyViewer
{
  partial class FormCustomSite
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
      label1 = new Label();
      label2 = new Label();
      textBoxName = new TextBox();
      label3 = new Label();
      numericUpDownLatitude = new NumericUpDown();
      label6 = new Label();
      label4 = new Label();
      numericUpDownLongitude = new NumericUpDown();
      label7 = new Label();
      label5 = new Label();
      numericUpDownClockOffset = new NumericUpDown();
      label8 = new Label();
      buttonOk = new Button();
      buttonCancel = new Button();
      ((System.ComponentModel.ISupportInitialize)numericUpDownLatitude).BeginInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownLongitude).BeginInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownClockOffset).BeginInit();
      SuspendLayout();
      // 
      // label1
      // 
      label1.Dock = DockStyle.Top;
      label1.ForeColor = Color.Teal;
      label1.Location = new Point(0, 0);
      label1.Name = "label1";
      label1.Size = new Size(438, 39);
      label1.TabIndex = 0;
      label1.Text = "Positive latitude is north of the equator, positive longitude is east of Greenwich.";
      label1.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // label2
      // 
      label2.AutoSize = true;
      label2.ForeColor = SystemColors.ControlText;
      label2.Location = new Point(55, 56);
      label2.Name = "label2";
      label2.Size = new Size(42, 15);
      label2.TabIndex = 0;
      label2.Text = "Name:";
      // 
      // textBoxName
      // 
      textBoxName.BackColor = SystemColors.Window;
      textBoxName.BorderStyle = BorderStyle.FixedSingle;
      textBoxName.ForeColor = SystemColors.ControlText;
      textBoxName.Location = new Point(102, 53);
      textBoxName.Name = "textBoxName";
      textBoxName.Size = new Size(270, 23);
      textBoxName.TabIndex = 1;
      // 
      // label3
      // 
      label3.AutoSize = true;
      label3.ForeColor = SystemColors.ControlText;
      label3.Location = new Point(44, 94);
      label3.Name = "label3";
      label3.Size = new Size(53, 15);
      label3.TabIndex = 2;
      label3.Text = "Latitude:";
      // 
      // numericUpDownLatitude
      // 
      numericUpDownLatitude.BorderStyle = BorderStyle.FixedSingle;
      numericUpDownLatitude.DecimalPlaces = 4;
      numericUpDownLatitude.ForeColor = SystemColors.ControlText;
      numericUpDownLatitude.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
      numericUpDownLatitude.Location = new Point(102, 91);
      numericUpDownLatitude.Maximum = new decimal(new int[] { 90, 0, 0, 0 });
      numericUpDownLatitude.Minimum = new decimal(new int[] { 90, 0, 0, int.MinValue });
      numericUpDownLatitude.Name = "numericUpDownLatitude";
      numericUpDownLatitude.Size = new Size(82, 23);
      numericUpDownLatitude.TabIndex = 3;
      // 
      // label6
      // 
      label6.ForeColor = Color.Teal;
      label6.Location = new Point(191, 94);
      label6.Name = "label6";
      label6.Size = new Size(142, 20);
      label6.TabIndex = 0;
      label6.Text = "degrees, -90 to 90";
      // 
      // label4
      // 
      label4.AutoSize = true;
      label4.ForeColor = SystemColors.ControlText;
      label4.Location = new Point(33, 132);
      label4.Name = "label4";
      label4.Size = new Size(64, 15);
      label4.TabIndex = 4;
      label4.Text = "Longitude:";
      // 
      // numericUpDownLongitude
      // 
      numericUpDownLongitude.BorderStyle = BorderStyle.FixedSingle;
      numericUpDownLongitude.DecimalPlaces = 4;
      numericUpDownLongitude.ForeColor = SystemColors.ControlText;
      numericUpDownLongitude.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
      numericUpDownLongitude.Location = new Point(102, 129);
      numericUpDownLongitude.Maximum = new decimal(new int[] { 180, 0, 0, 0 });
      numericUpDownLongitude.Minimum = new decimal(new int[] { 180, 0, 0, int.MinValue });
      numericUpDownLongitude.Name = "numericUpDownLongitude";
      numericUpDownLongitude.Size = new Size(82, 23);
      numericUpDownLongitude.TabIndex = 5;
      // 
      // label7
      // 
      label7.ForeColor = Color.Teal;
      label7.Location = new Point(191, 132);
      label7.Name = "label7";
      label7.Size = new Size(142, 20);
      label7.TabIndex = 0;
      label7.Text = "degrees, -180 to 180";
      // 
      // label5
      // 
      label5.AutoSize = true;
      label5.ForeColor = SystemColors.ControlText;
      label5.Location = new Point(24, 170);
      label5.Name = "label5";
      label5.Size = new Size(75, 15);
      label5.TabIndex = 6;
      label5.Text = "Clock Offset:";
      // 
      // numericUpDownClockOffset
      // 
      numericUpDownClockOffset.BorderStyle = BorderStyle.FixedSingle;
      numericUpDownClockOffset.DecimalPlaces = 1;
      numericUpDownClockOffset.ForeColor = SystemColors.ControlText;
      numericUpDownClockOffset.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
      numericUpDownClockOffset.Location = new Point(102, 167);
      numericUpDownClockOffset.Maximum = new decimal(new int[] { 14, 0, 0, 0 });
      numericUpDownClockOffset.Minimum = new decimal(new int[] { 12, 0, 0, int.MinValue });
      numericUpDownClockOffset.Name = "numericUpDownClockOffset";
      numericUpDownClockOffset.Size = new Size(82, 23);
      numericUpDownClockOffset.TabIndex = 7;
      // 
      // label8
      // 
      label8.ForeColor = Color.Teal;
      label8.Location = new Point(191, 170);
      label8.Name = "label8";
      label8.Size = new Size(142, 20);
      label8.TabIndex = 0;
      label8.Text = "hours from UTC";
      // 
      // buttonOk
      // 
      buttonOk.BackColor = SystemColors.Control;
      buttonOk.DialogResult = DialogResult.OK;
      buttonOk.FlatAppearance.BorderColor = Color.FromArgb(96, 76, 34);
      buttonOk.FlatStyle = FlatStyle.System;
      buttonOk.ForeColor = SystemColors.ControlText;
      buttonOk.Location = new Point(223, 220);
      buttonOk.Name = "buttonOk";
      buttonOk.Size = new Size(80, 30);
      buttonOk.TabIndex = 9;
      buttonOk.Text = "OK";
      buttonOk.UseVisualStyleBackColor = false;
      buttonOk.Click += ButtonOk_Click;
      // 
      // buttonCancel
      // 
      buttonCancel.BackColor = SystemColors.Control;
      buttonCancel.DialogResult = DialogResult.Cancel;
      buttonCancel.FlatAppearance.BorderColor = Color.FromArgb(28, 35, 51);
      buttonCancel.FlatStyle = FlatStyle.System;
      buttonCancel.ForeColor = SystemColors.ControlText;
      buttonCancel.Location = new Point(139, 220);
      buttonCancel.Name = "buttonCancel";
      buttonCancel.Size = new Size(78, 30);
      buttonCancel.TabIndex = 8;
      buttonCancel.Text = "Cancel";
      buttonCancel.UseVisualStyleBackColor = false;
      // 
      // FormCustomSite
      // 
      AcceptButton = buttonOk;
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      BackColor = SystemColors.Control;
      CancelButton = buttonCancel;
      ClientSize = new Size(438, 269);
      Controls.Add(label1);
      Controls.Add(label2);
      Controls.Add(textBoxName);
      Controls.Add(label3);
      Controls.Add(numericUpDownLatitude);
      Controls.Add(label6);
      Controls.Add(label4);
      Controls.Add(numericUpDownLongitude);
      Controls.Add(label7);
      Controls.Add(label5);
      Controls.Add(numericUpDownClockOffset);
      Controls.Add(label8);
      Controls.Add(buttonOk);
      Controls.Add(buttonCancel);
      ForeColor = SystemColors.ControlText;
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      MinimizeBox = false;
      Name = "FormCustomSite";
      ShowInTaskbar = false;
      StartPosition = FormStartPosition.CenterParent;
      Text = "Custom Site";
      ((System.ComponentModel.ISupportInitialize)numericUpDownLatitude).EndInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownLongitude).EndInit();
      ((System.ComponentModel.ISupportInitialize)numericUpDownClockOffset).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private Label label1;

    private Label label2;
    private TextBox textBoxName;

    private Label label3;
    private NumericUpDown numericUpDownLatitude;
    private Label label6;

    private Label label4;
    private NumericUpDown numericUpDownLongitude;
    private Label label7;

    private Label label5;
    private NumericUpDown numericUpDownClockOffset;
    private Label label8;

    private Button buttonOk;
    private Button buttonCancel;
  }
}