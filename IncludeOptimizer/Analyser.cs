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
    public bool UseSharedPtrs { get; set; } = true;
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

    public override string ToString()
    {
      return Body;
    }
  }

  public class Analyser
  {
    public const string TypeDeclaration = @"(?<type>[\w:]+)\s+";
    public const string MemberDeclarationRegex = TypeDeclaration+@"(?<memberName>\w+)\s*;";
    public const string IncludeDeclarationRegexGTLT = @"\s*#include\s*<(?<type>.*)\s*>";
    public const string IncludeDeclarationRegexQuoted = "\\s*#include\\s*\"(?<type>.*)\\s*\"";

    public string ResultsToString()
    {
      return string.Join("\r\n", Declarations.Select(i => i.ToString()));
    }

    string fileContent;
    string[] splittedFileContent;
    string[] includesToAdd = new string[0];

    List<string> knownHeaders = new List<string>() { "memory", "vector", "string", "set", "memory", "algorithm"};

    List<string> customHeaders = new List<string>();
    
    public List<Declaration> Declarations { get ; set ; }
    public string[] IncludesToAdd { get => includesToAdd; set => includesToAdd = value; }

    OptimizationSettings optimizationSettings;

    public void Analyse(string filePath, OptimizationSettings optimizationSettings)
    {
      this.optimizationSettings = optimizationSettings;
      fileContent = File.ReadAllText(filePath);
      Analyse(fileContent);
    }

    public void Analyse(string fileContent)
    {
      this.fileContent = fileContent;
      splittedFileContent = fileContent
              .Split("\r\n".ToCharArray())
              .Where(i => !i.Trim().StartsWith("//")  && i.Trim().Any())
              .ToArray();

      Analyse();
    }

    private void Analyse()
    {
      customHeaders = FindCustomHeaders(splittedFileContent);
      Declarations = FindDeclarations(customHeaders);
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
          decl.Type = matches[0].Groups[1].Value.Trim();
          decl.Class = decl.Type.Split("::".ToCharArray()).Last().Trim();
          decl.MemberName = matches[0].Groups[2].Value.Trim();
        }
      }
      return decl;
    }

    public List<Declaration> FindDeclarations(List<string> customHeaders)
    {
      var declarations = new List<Declaration>();

      List<string> includesToAdd = new List<string>() 
      { 
        "#include <memory>" 
      };
      foreach (var header in customHeaders)
      {
        foreach (var line in splittedFileContent)
        {
          var isInc = IsIncludeLine(line);
          if (isInc && line.Contains("<memory>"))
            includesToAdd.Remove("memory");
          if (!isInc && !IsClassDefLine(line) && line.Contains(header))
          {
            var dec = ParseMemberDeclaration(line);
            if (!string.IsNullOrEmpty(dec.Type))
            {
              dec.Header = header;
              declarations.Add(dec);
            }
          }
        }
      }
      this.includesToAdd = includesToAdd.ToArray();
      return declarations;
    }
        
    private List<string> FindCustomHeaders(string[] fileContent)
    {
      var customHeaders = new List<string>();
      foreach (var line in fileContent)
      {
        var lineToProcess = line.Trim();
        if (IsIncludeLine(lineToProcess))
        {
          var incContent = ParseIncludeContent(lineToProcess);
          if (!knownHeaders.Contains(incContent))
          {
            var includeBody = incContent.Replace(".h", "").Replace("\"", "");
            customHeaders.Add(includeBody);
          }
        }
      }

      return customHeaders;
    }

    public static bool IsIncludeLine(string lineToProcess)
    {
      return lineToProcess.Trim().StartsWith("#include");
    }

    public static bool IsClassDefLine(string lineToProcess)
    {
      return lineToProcess.Trim().Contains("class ");
    }

    public string ParseIncludeContent(string fullIncludeLine)
    {
      var matches = Regex.Matches(fullIncludeLine, IncludeDeclarationRegexGTLT);
      if (matches.Count > 0 && matches[0].Groups.Count > 1)
      {
        return matches[0].Groups[1].Value.Trim();
      }

      matches = Regex.Matches(fullIncludeLine, IncludeDeclarationRegexQuoted);
      if (matches.Count > 0 && matches[0].Groups.Count > 1)
      {
        return matches[0].Groups[1].Value.Trim();
      }

      return fullIncludeLine;
    }
  }
}
