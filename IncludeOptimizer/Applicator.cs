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
    Analyser analyser;
    List<string> forwardDeclarations = new List<string>();
    int currentLineIndex = 0;
    int ctorIndex = 0;

    public string ApplyToString(string input, Analyser analyser, bool implFile)
    {
      currentLineIndex = 0;
      this.analyser = analyser;

      var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
      var result = new List<string>();
      int lastIncludeIndex = 0;
      
      var replaced = false;
      
      foreach (var line in lines)
      {
        var lineToAdd = line;
        var add = true;

        if (implFile)
        {
          lineToAdd = ApplyToImplFile(line, ref replaced);
        }
        else
        {
          add = ApplyToHeader(line, ref replaced, ref lineToAdd);
        }
        
        var isInc = Analyser.IsIncludeLine(line);
        if (isInc && add)
        {
          lastIncludeIndex = currentLineIndex;
        }
                
        if(add)
          result.Add(lineToAdd);

        currentLineIndex++;
      }

      if (replaced)
      {
        if (implFile)
        {
          if(collectionCreations.Any())
            result.InsertRange(ctorIndex+2, collectionCreations);
          result.InsertRange(lastIncludeIndex, analyser.ImplIncludesToAdd);
        }
        else
        {
          result.InsertRange(lastIncludeIndex, analyser.HeaderIncludesToAdd);
          var nextInd = lastIncludeIndex + analyser.HeaderIncludesToAdd.Length;
          result.InsertRange(nextInd, forwardDeclarations);
        }
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
              var fd = "";// 
              foreach (var namesp in decl.Namespaces)
              {
                fd += "namespace " + namesp + "{";
              }
              fd += "class " + decl.Class + ";";
              foreach (var namesp in decl.Namespaces)
                fd += "}";

              fd += ";";

              forwardDeclarations.Add(fd);
              add = false;
              break;
            }
            var toReplace = " " + decl.Header + " ";
            var replacerKind = GetReplacingPointer();
            if (!decl.IsCollection)
              res = line.Replace(decl.Type, replacerKind + "<" + decl.Type + ">");
            else
              res = GetReplacer(decl, true) + decl.MemberName + ";";

            replaced = true;
            break;
          }
        }
      }

      

      return add;
    }

    private string GetReplacingPointer()
    {
      return analyser.OptimizationSettings.UseSharedPtrs ? "std::shared_ptr" : "std::unique_ptr";
    }

    private string GetPointerCreation()
    {
      return analyser.OptimizationSettings.UseSharedPtrs ? "std::make_shared" : "std::make_unique";
    }

    public string GetReplacer(Declaration decl, bool definition)
    {
      var replacer = definition ? GetReplacingPointer() : GetPointerCreation();
      return replacer + "<" + decl.CollectionType + "<" + decl.Type + ">> ";
    }

    List<string> collectionCreations = new List<string>();
    private string ApplyToImplFile(string line, ref bool replaced)
    {
      var res = "";
      var collectionCreation = "";
      if (line.Trim().Any())
      {
        res = line;
          
        foreach (var decl in analyser.Declarations)
        {
          var isCtor = IsCtor(line, analyser.ClassName);
          if (isCtor)
          {
            collectionCreation = decl.MemberName + " = " + GetReplacer(decl, false) + "();";
            collectionCreations.Add(collectionCreation);
            ctorIndex = currentLineIndex;
          }
          //else if (collectionCreation.Any())
          //{
          //  line += collectionCreation;
          //  collectionCreation = "";
          //}

          if (line.Contains(decl.MemberName + "."))
          {
            res = ConvertMemberUsage(decl.MemberName, res);
            replaced = true;
          }
        }
      }

      return res;
    }

    bool IsCtor(string line, string type)
    {
      return line.Contains(type+"::"+ type);
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
