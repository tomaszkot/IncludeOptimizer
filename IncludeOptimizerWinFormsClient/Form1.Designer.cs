
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
      this.SuspendLayout();
      // 
      // AnalyseBtn
      // 
      this.AnalyseBtn.Location = new System.Drawing.Point(21, 23);
      this.AnalyseBtn.Name = "AnalyseBtn";
      this.AnalyseBtn.Size = new System.Drawing.Size(75, 23);
      this.AnalyseBtn.TabIndex = 0;
      this.AnalyseBtn.Text = "Analyse";
      this.AnalyseBtn.UseVisualStyleBackColor = true;
      this.AnalyseBtn.Click += new System.EventHandler(this.AnalyseBtn_Click);
      // 
      // IncludeOptimizerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.AnalyseBtn);
      this.Name = "IncludeOptimizerForm";
      this.Text = "Include Optimizer";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button AnalyseBtn;
  }
}

