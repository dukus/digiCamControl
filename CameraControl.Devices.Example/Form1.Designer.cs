namespace CameraControl.Devices.Example
{
  partial class Form1
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
      this.cmb_cameras = new System.Windows.Forms.ComboBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.btn_liveview = new System.Windows.Forms.Button();
      this.btn_capture = new System.Windows.Forms.Button();
      this.img_photo = new System.Windows.Forms.PictureBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.img_photo)).BeginInit();
      this.SuspendLayout();
      // 
      // cmb_cameras
      // 
      this.cmb_cameras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmb_cameras.FormattingEnabled = true;
      this.cmb_cameras.Location = new System.Drawing.Point(12, 12);
      this.cmb_cameras.Name = "cmb_cameras";
      this.cmb_cameras.Size = new System.Drawing.Size(184, 21);
      this.cmb_cameras.TabIndex = 0;
      this.cmb_cameras.SelectedIndexChanged += new System.EventHandler(this.cmb_cameras_SelectedIndexChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.btn_liveview);
      this.groupBox1.Controls.Add(this.btn_capture);
      this.groupBox1.Location = new System.Drawing.Point(12, 50);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(184, 150);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Control";
      // 
      // btn_liveview
      // 
      this.btn_liveview.Location = new System.Drawing.Point(16, 58);
      this.btn_liveview.Name = "btn_liveview";
      this.btn_liveview.Size = new System.Drawing.Size(75, 23);
      this.btn_liveview.TabIndex = 3;
      this.btn_liveview.Text = "Live view";
      this.btn_liveview.UseVisualStyleBackColor = true;
      this.btn_liveview.Click += new System.EventHandler(this.btn_liveview_Click);
      // 
      // btn_capture
      // 
      this.btn_capture.Location = new System.Drawing.Point(16, 29);
      this.btn_capture.Name = "btn_capture";
      this.btn_capture.Size = new System.Drawing.Size(75, 23);
      this.btn_capture.TabIndex = 2;
      this.btn_capture.Text = "Capture";
      this.btn_capture.UseVisualStyleBackColor = true;
      this.btn_capture.Click += new System.EventHandler(this.btn_capture_Click);
      // 
      // img_photo
      // 
      this.img_photo.Location = new System.Drawing.Point(245, 12);
      this.img_photo.Name = "img_photo";
      this.img_photo.Size = new System.Drawing.Size(362, 266);
      this.img_photo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.img_photo.TabIndex = 2;
      this.img_photo.TabStop = false;
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(12, 298);
      this.textBox1.Multiline = true;
      this.textBox1.Name = "textBox1";
      this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.textBox1.Size = new System.Drawing.Size(595, 118);
      this.textBox1.TabIndex = 3;
      this.textBox1.WordWrap = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(619, 428);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.img_photo);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.cmb_cameras);
      this.Name = "Form1";
      this.Text = "CameraControl.Devices.Example";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.img_photo)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox cmb_cameras;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button btn_capture;
    private System.Windows.Forms.PictureBox img_photo;
    private System.Windows.Forms.Button btn_liveview;
    private System.Windows.Forms.TextBox textBox1;
  }
}

