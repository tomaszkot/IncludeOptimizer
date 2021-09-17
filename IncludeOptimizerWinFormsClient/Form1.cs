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
      var filePath = @"F:\repos\IncludeOptimizer\IncludeOptimizerTestApp\Company.h";
      var optimizationSettings = new OptimizationSettings();
      analyser.Analyse(filePath, optimizationSettings);

      var applicator = new Applicator();
      applicator.Apply(filePath, analyser.Declarations, optimizationSettings);
      Debug.Assert(analyser.Declarations.Count == 1);
    }
  }
}
