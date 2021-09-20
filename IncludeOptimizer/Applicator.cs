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
    public string ApplyToString(string input, List<Declaration> declarations, OptimizationSettings optimizationSettings)
    {
      var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
      var result = new List<string>();
      foreach (var line in lines)
      {
        var res = line;
        if (line.Trim().Any() && !Analyser.IsIncludeLine(line) && !Analyser.IsClassDefLine(line))
        {
          foreach (var decl in declarations)
          {
            if (line.Contains(decl.Header))
            {
              var toReplace = " " + decl.Header + " ";
              var replacerKind = optimizationSettings.UseSharedPtrs ? "std::shared_ptr" : "std::unique_ptr";
              res = line.Replace(decl.Type, replacerKind + "<" + decl.Type + ">");
              break;
            }
          }
        }

        result.Add(res);
      }

      return string.Join("\r\n", result);
    }

    public void Apply(string filePath, List<Declaration> declarations, OptimizationSettings optimizationSettings)
    {
      var fileContent = File.ReadAllText(filePath);
      var output = ApplyToString(fileContent, declarations, optimizationSettings);
      File.WriteAllText(filePath, fileContent);
    }
  }
}
