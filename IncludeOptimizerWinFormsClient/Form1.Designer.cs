
namespace IncludeOptimizerWinFormsClient
{
  partial class IncludeOptimizerForm
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
      this.AnalyseBtn = new System.Windows.Forms.Button();
      this.FileNameCb = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // AnalyseBtn
      // 
      this.AnalyseBtn.Location = new System.Drawing.Point(139, 12);
      this.AnalyseBtn.Name = "AnalyseBtn";
      this.AnalyseBtn.Size = new System.Drawing.Size(75, 23);
      this.AnalyseBtn.TabIndex = 0;
      this.AnalyseBtn.Text = "Run";
      this.AnalyseBtn.UseVisualStyleBackColor = true;
      this.AnalyseBtn.Click += new System.EventHandler(this.AnalyseBtn_Click);
      // 
      // FileNameCb
      // 
      this.FileNameCb.FormattingEnabled = true;
      this.FileNameCb.Items.AddRange(new object[] {
            "Company"});
      this.FileNameCb.Location = new System.Drawing.Point(12, 12);
      this.FileNameCb.Name = "FileNameCb";
      this.FileNameCb.Size = new System.Drawing.Size(121, 21);
      this.FileNameCb.TabIndex = 1;
      // 
      // IncludeOptimizerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.FileNameCb);
      this.Controls.Add(this.AnalyseBtn);
      this.Name = "IncludeOptimizerForm";
      this.Text = "Include Optimizer";
      this.Load += new System.EventHandler(this.IncludeOptimizerForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button AnalyseBtn;
    private System.Windows.Forms.ComboBox FileNameCb;
  }
}

