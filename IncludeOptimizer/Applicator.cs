using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IncludeOptimizer
{
  public class Applicator
  {
    public void Apply(string filePath, List<Declaration> declarations, OptimizationSettings optimizationSettings)
    {
      var fileContent = File.ReadAllText(filePath);
      foreach (var decl in declarations)
      {
        var toReplace = " " + decl.Header + " ";
        var replacerKind = optimizationSettings.UseSharedPtrs ? "std::shared_ptr" : "std::unique_ptr";
        var replacer = toReplace.Replace(decl.Header, replacerKind + "<" + decl.Header + "> ");
        fileContent = fileContent.Replace(toReplace, replacer);
      }
      File.WriteAllText(filePath, fileContent);
    }
  }
}
