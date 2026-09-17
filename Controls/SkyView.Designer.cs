namespace SkyViewer;

partial class SkyView
{
    /// <summary>Required designer variable.</summary>
    private System.ComponentModel.IContainer components = null!;

    /// <summary>Clean up any resources being used.</summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ReleaseDrawingResources();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        // 
        // SkyView
        // 
        // The canvas is drawn in device pixels, so it inherits scaling from its parent
        // rather than rescaling itself a second time.
        AutoScaleMode = AutoScaleMode.Inherit;
        BackColor = SkyPalette.Void;
        Cursor = Cursors.Cross;
        DoubleBuffered = true;
        Margin = new Padding(0);
        Name = "SkyView";
        Size = new Size(800, 600);
        TabStop = true;
        ResumeLayout(false);
    }

    #endregion
}
