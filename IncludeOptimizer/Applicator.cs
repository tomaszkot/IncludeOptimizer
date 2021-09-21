using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IncludeOptimizer
{
  public class Applicator
  {
    public string ApplyToString(string input, Analyser analyser, OptimizationSettings optimizationSettings)
    {
      var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
      var result = new List<string>();
      int lastIncludeIndex = 0;
      int index = 0;
      var replaced = false;

      foreach (var line in lines)
      {
        var res = line;
        var isInc = Analyser.IsIncludeLine(line);
        if (isInc)
          lastIncludeIndex = index;
        if (line.Trim().Any() && !isInc && !Analyser.IsClassDefLine(line))
        {
          foreach (var decl in analyser.Declarations)
          {
            if (line.Contains(decl.Header))
            {
              var toReplace = " " + decl.Header + " ";
              var replacerKind = optimizationSettings.UseSharedPtrs ? "std::shared_ptr" : "std::unique_ptr";
              res = line.Replace(decl.Type, replacerKind + "<" + decl.Type + ">");
              replaced = true;
              break;
            }
          }
        }
        index++;
        result.Add(res);
      }

      if (replaced)
      {
        result.InsertRange(lastIncludeIndex, analyser.IncludesToAdd);
      }
      return string.Join("\r\n", result);
    }

    public void Apply(string inputFilePath, string outputFilePath, Analyser analyser, OptimizationSettings optimizationSettings)
    {
      var fileContent = File.ReadAllText(inputFilePath);
      var output = ApplyToString(fileContent, analyser, optimizationSettings);
      File.WriteAllText(outputFilePath, output);
    }
  }
}
