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
    public string ApplyToString(string input, Analyser analyser, OptimizationSettings optimizationSettings, bool implFile)
    {
      var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
      var result = new List<string>();
      int lastIncludeIndex = 0;
      int index = 0;
      var replaced = false;

      foreach (var line in lines)
      {
        var res = line;

        if (implFile)
        {
          if (line.Trim().Any())
          {
            res = line;
            foreach (var decl in analyser.Declarations)
            {
              if (line.Contains(decl.MemberName+"."))
              {
                res = ConvertMemberUsage(decl.MemberName, res);
              }
            }
          }
        }
        else
        {
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
        }

        index++;
        result.Add(res);
      }

      if (replaced)
      {
        result.InsertRange(lastIncludeIndex+1, analyser.IncludesToAdd);
      }
      return string.Join("\r\n", result);
    }

    public void Apply(string inputFilePath, string outputFilePath, Analyser analyser, OptimizationSettings optimizationSettings)
    {
      var fileContent = File.ReadAllText(inputFilePath);
      var output = ApplyToString(fileContent, analyser, optimizationSettings, false);
      File.WriteAllText(outputFilePath, output);

      var inputCppFilePath = inputFilePath.Replace(".h", ".cpp");
      if (File.Exists(inputCppFilePath))
      {
        fileContent = File.ReadAllText(inputCppFilePath);
        output = ApplyToString(fileContent, analyser, optimizationSettings, true);
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
