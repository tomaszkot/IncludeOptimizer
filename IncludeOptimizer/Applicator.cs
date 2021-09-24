﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IncludeOptimizer
{
  public class Applicator
  {
    Analyser analyser;
    List<string> forwardDeclarations = new List<string>();

    public string ApplyToString(string input, Analyser analyser, bool implFile)
    {
      this.analyser = analyser;

      var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
      var result = new List<string>();
      int lastIncludeIndex = 0;
      int index = 0;
      var replaced = false;
      
      foreach (var line in lines)
      {
        var lineToAdd = line;
        var add = true;

        if (implFile)
        {
          lineToAdd = ApplyToImplFile(line);
        }
        else
        {
          add = ApplyToHeader(line, ref replaced, ref lineToAdd);
        }
        
        var isInc = Analyser.IsIncludeLine(line);
        if (isInc && add)
        {
          lastIncludeIndex = index;
        }

        index++;
        if(add)
          result.Add(lineToAdd);
      }

      if (replaced)
      {
        result.InsertRange(lastIncludeIndex, analyser.IncludesToAdd);
        var nextInd = lastIncludeIndex + analyser.IncludesToAdd.Length;
        result.InsertRange(nextInd, forwardDeclarations);
        //result.Insert(nextInd+forwardDeclarations.Count, Environment.NewLine);
      }
      return string.Join(Environment.NewLine, result);
    }

    private bool ApplyToHeader(string line, ref bool replaced, ref string res)
    {
      bool add = true;
      var isInc = Analyser.IsIncludeLine(line);

      if (line.Trim().Any() && !Analyser.IsClassDefLine(line))
      {
        foreach (var decl in analyser.Declarations)
        {
          if (line.Contains(decl.Header))
          {
            if (isInc)
            {
              var fd = "class " + decl.Type + ";";
              forwardDeclarations.Add(fd);
              add = false;
              break;
            }
            var toReplace = " " + decl.Header + " ";
            var replacerKind = analyser.OptimizationSettings.UseSharedPtrs ? "std::shared_ptr" : "std::unique_ptr";
            res = line.Replace(decl.Type, replacerKind + "<" + decl.Type + ">");

            replaced = true;
            break;
          }
        }
      }

      

      return add;
    }

    private string ApplyToImplFile(string line)
    {
      var res = "";
      if (line.Trim().Any())
      {
        res = line;
        foreach (var decl in analyser.Declarations)
        {
          if (line.Contains(decl.MemberName + "."))
          {
            res = ConvertMemberUsage(decl.MemberName, res);
          }
        }
      }

      return res;
    }

    public void Apply(string inputFilePath, string outputFilePath, Analyser analyser, OptimizationSettings optimizationSettings)
    {
      var fileContent = File.ReadAllText(inputFilePath);
      var output = ApplyToString(fileContent, analyser, false);
      File.WriteAllText(outputFilePath, output);

      var inputCppFilePath = inputFilePath.Replace(".h", ".cpp");
      if (File.Exists(inputCppFilePath))
      {
        fileContent = File.ReadAllText(inputCppFilePath);
        output = ApplyToString(fileContent, analyser, true);
        var outputCppFilePath = outputFilePath.Replace(".h", ".cpp");
        File.WriteAllText(outputCppFilePath, output);
      }
    }

    public string ConvertMemberUsage(string member, string memberUsage)
    {
      return memberUsage.Replace(member+".", member+"->");
    }
  }
}
