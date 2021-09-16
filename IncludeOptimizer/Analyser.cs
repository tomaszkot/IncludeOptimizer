using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace IncludeOptimizer
{
  public class Analyser
  {
    string fileContent;
    string[] splittedFileContent;

    List<string> knownHeaders = new List<string>() { 
    "memory", "vector", "string", 
    };

    List<string> customHeaders = new List<string>();
    List<string> declarations = new List<string>();

    public List<string> Declarations { get => declarations; set => declarations = value; }

    public void Analyse(string filePath)
    {
      fileContent = File.ReadAllText(filePath);
      splittedFileContent = fileContent
        .Split("\r\n".ToCharArray())
        .Where(i=> i.Trim().Any())
        .ToArray();
      
      customHeaders = FindCustomHeaders(splittedFileContent);
      var declarations = FindDeclarationsInFile(customHeaders);
      Debug.WriteLine("end!");
    }

    private List<string> FindDeclarationsInFile(List<string> customHeaders)
    {
      var declarations = new List<string>();
      foreach (var line in splittedFileContent)
      {
        var lineToProcess = line;//.Trim();
        foreach (var header in customHeaders)
        {
          if (lineToProcess.Contains(" "+header+ " "))
          {
            declarations.Add(lineToProcess);
          }
        }
      }

      return declarations;
    }

    private List<string> FindCustomHeaders(string[] fileContent)
    {
      var customHeaders = new List<string>();
      foreach (var line in fileContent)
      {
        var lineToProcess = line.Trim();
        if (lineToProcess.StartsWith("#include"))
        {
          var incContent = GetIncludeContent(lineToProcess);
          if (!knownHeaders.Contains(incContent))
          {
            var includeBody = incContent.Replace(".h", "").Replace("\"", "");
            customHeaders.Add(includeBody);
            int k = 0;
            k++;
          }
        }
      }

      return customHeaders;
    }

    private string GetIncludeContent(string fullIncludeLine)
    {
      var line = fullIncludeLine.Replace("#include", "");
      line = line.Replace("<", "");
      line = line.Replace(">", "");
      return line.Trim();
    }
  }
}
