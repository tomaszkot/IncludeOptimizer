using IncludeOptimizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IncludeOptimizerWinFormsClient
{
  public partial class IncludeOptimizerForm : Form
  {
    public IncludeOptimizerForm()
    {
      InitializeComponent();
    }

    private void AnalyseBtn_Click(object sender, EventArgs e)
    {
      var analyser = new Analyser();

      //var filePath = @"F:\repos\IncludeOptimizer\IncludeOptimizerTestApp\InstType.h";
      var fileName = "Car.h";
      var filePathBase = @"F:\repos\IncludeOptimizer\IncludeOptimizerTestApp\";
      var inputFilePath = filePathBase+"/Input/";
      inputFilePath += fileName;
      var outputFilePath = filePathBase + "/Output/"+ fileName;
      //var filePath = @"F:\repos\IncludeOptimizer\IncludeOptimizerTestApp\ScriptParameterBL.h";
      var optimizationSettings = new OptimizationSettings();
      analyser.Analyse(inputFilePath, optimizationSettings);
      string res = analyser.ResultsToString();
      //var save = MessageBox.Show(res, "Save Converted File ?", MessageBoxButtons.YesNo);
      //if (save == DialogResult.OK)
      {
        var applicator = new Applicator();
        applicator.Apply(inputFilePath, outputFilePath, analyser, optimizationSettings);
        Debug.Assert(analyser.Declarations.Count > 0);
      }
    }
  }
}
