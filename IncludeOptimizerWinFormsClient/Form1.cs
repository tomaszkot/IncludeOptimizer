﻿using IncludeOptimizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

      var filePath = @"C:\Users\Koto\source\repos\IncludeOptimizerTestApp\IncludeOptimizerTestApp\InstType.h";
      analyser.Analyse(filePath);

      var applicator = new Applicator();
      applicator.Apply(filePath, analyser.Declarations);
    }
  }
}
