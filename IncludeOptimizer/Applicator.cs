using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IncludeOptimizer
{
  public class Applicator
  {
    public void Apply(string filePath, List<string> declarations)
    {
      var fileContent = File.ReadAllText(filePath);
      foreach (var decl in declarations)
        fileContent = fileContent.Replace(" " + decl + " ", " std::shared_ptr<"+ decl+"> ");

      File.WriteAllText(filePath, fileContent);
    }
  }
}
