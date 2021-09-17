using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IncludeOptimizer
{

  public class OptimizationSettings
  {
    public bool UseSharedPtrs { get; set; }
  }

  public class Declaration
  {
    public string Header { get; set; }//e.g.#include "Person.h"
    public string Body { get; set; }//e.g. BL::Person m_persons;
    public List<string> Namespaces { get; set; }//e.g. BL
    public string Type { get; set; } //e.g. BL::Person
    public string Class { get; set; } //e.g. Person
    public string MemberName //e.g. m_persons
    {
      get;set;

    }
  }

  public class Analyser
  {
    public const string MemberDeclarationRegex = @"(?<type>.*)\s(?<memberName>\w+)";

    string fileContent;
    string[] splittedFileContent;

    List<string> knownHeaders = new List<string>() { 
    "memory", "vector", "string", 
    };

    List<string> customHeaders = new List<string>();
    List<Declaration> declarations = new List<Declaration>();

    public List<Declaration> Declarations { get => declarations; set => declarations = value; }

    public void Analyse(string filePath, OptimizationSettings optimizationSettings)
    {
      fileContent = File.ReadAllText(filePath);
      Analyse(fileContent);
    }

    public void Analyse(string fileContent)
    {
      splittedFileContent = fileContent
              .Split("\r\n".ToCharArray())
              .Where(i => !i.StartsWith("//")  && i.Trim().Any())
              .ToArray();

      Analyse();
    }

    private void Analyse()
    {
      customHeaders = FindCustomHeaders(splittedFileContent);
      Declarations = FindDeclarations(customHeaders);
      Debug.WriteLine("end!");
    }

    public Declaration ParseMemberDeclaration(string declarationLine)
    {
      var matches = Regex.Matches(declarationLine, MemberDeclarationRegex);
      
      var decl = new Declaration();
      decl.Body = declarationLine;
      decl.Header = "";
      if (matches.Count > 0)
      {
        if (matches[0].Groups.Count > 2)
        {
          decl.Type = matches[0].Groups[1].Value;
          decl.Class = decl.Type.Split("::".ToCharArray()).Last();
          decl.MemberName = matches[0].Groups[2].Value;
        }
      }
      return decl;
    }

    public List<Declaration> FindDeclarations(List<string> customHeaders)
    {
      var declarations = new List<Declaration>();
      foreach (var header in customHeaders)
      {
        foreach (var line in splittedFileContent)
        {
          var lineToProcess = line;
          if (lineToProcess.Contains(header+ " "))
          {
            var dec = ParseMemberDeclaration(lineToProcess);
            dec.Header = header;
            declarations.Add(dec);
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
