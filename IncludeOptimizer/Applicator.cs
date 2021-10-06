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
    enum Mode { Unset, SingleDeclaration, ManyDeclarations }
    Mode mode = Mode.SingleDeclaration;
    Analyser analyser;
    List<string> forwardDeclarations = new List<string>();
    int currentLineIndex = 0;
    int ctorIndex = 0;
    bool applied = false;
    List<Declaration> declarations;
    List<string> collectionCreations = new List<string>();
    public bool Applied { get => applied; set => applied = value; }

    public string ApplyToString(string input, Analyser analyser, bool implFile)
    {
      if (!analyser.Declarations.Any())
        return input;
            

      for (currentDeclIndex = 0; currentDeclIndex < analyser.Declarations.Count; currentDeclIndex++)
      {
        //declarations = analyser.Declarations.ToList();
        //if (mode == Mode.SingleDeclaration)
        //{
        //  declarations = new List<Declaration>();
        //  declarations.Add(analyser.Declarations[0]);
        //}
        int lastIncludeIndex = 0;
        declarations = new List<Declaration>();
        declarations.Add(analyser.Declarations[currentDeclIndex]);

        currentLineIndex = 0;
        this.analyser = analyser;

        var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        var result = new List<string>();
        
        applied = false;

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
            add = ApplyToHeader(line, ref lineToAdd);
          }

          var isInc = Analyser.IsIncludeLine(line);
          if (isInc && add)
          {
            lastIncludeIndex = currentLineIndex;
          }

          if (add)
            result.Add(lineToAdd);

          currentLineIndex++;
        }

        if (applied && currentDeclIndex == analyser.Declarations.Count-1)
        {
          if (implFile)
          {
            if (collectionCreations.Any())
              result.InsertRange(ctorIndex + 2, collectionCreations);
            result.InsertRange(lastIncludeIndex, analyser.ImplIncludesToAdd.Distinct());
          }
          else
          {
            result.InsertRange(lastIncludeIndex, analyser.HeaderIncludesToAdd.Distinct());
            var nextInd = lastIncludeIndex + analyser.HeaderIncludesToAdd.Length;
            result.InsertRange(nextInd, forwardDeclarations.Distinct());
          }
          //result.Insert(nextInd+forwardDeclarations.Count, Environment.NewLine);
        }
        input = string.Join(Environment.NewLine, result);
      }

      return input;
    }

    private bool ApplyToHeader(string line, ref string res)
    {
      bool add = true;
      var isInc = Analyser.IsIncludeLine(line);

      if (line.Trim().Any() && !Analyser.IsClassDefLine(line))
      {
        foreach (var decl in declarations)
        {
          if (isInc && line.Contains(decl.Header))
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
          else if (line == decl.Body)//line.Contains(decl.Header))
          {
            var toReplace = " " + decl.Header + " ";
            var replacerKind = GetReplacingPointer();
            if (!decl.IsCollection)
              res = line.Replace(decl.Type, replacerKind + "<" + decl.Type + ">");
            else
              res = GetReplacer(decl, true) + decl.MemberName + ";";

            applied = true;
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
      if (decl.CollectionType.Any())
        replacer += "<" + decl.CollectionType;

      replacer += "<" + decl.Type + ">";
      if (decl.CollectionType.Any())
        replacer += ">";

      if(definition)
        replacer += " ";
      return replacer;
    }

    private string ApplyToImplFile(string line)
    {
      var res = "";
      var collectionCreation = "";
      if (line.Trim().Any())
      {
        res = line;
          
        foreach (var decl in declarations)
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
            applied = true;
          }
        }
      }

      return res;
    }

    bool IsCtor(string line, string type)
    {
      return line.Contains(type+"::"+ type);
    }
    int currentDeclIndex = -1;
    public void Apply(string inputFilePath, string outputFilePath, Analyser analyser, OptimizationSettings optimizationSettings)
    {
      this.analyser = analyser;
      var headerFileContent = File.ReadAllText(inputFilePath);
      var outputHeaderFilePath = outputFilePath;
      var outputCppFilePath = outputHeaderFilePath.Replace(".h", ".cpp");
      var inputCppFilePath = inputFilePath.Replace(".h", ".cpp");
      var cppFileContent = "";
      
      if (File.Exists(inputCppFilePath))
      {
        cppFileContent = File.ReadAllText(inputCppFilePath);
      }

      //for (currentDeclIndex = 0; currentDeclIndex < analyser.Declarations.Count; currentDeclIndex++)
      {
        headerFileContent = ApplyToString(headerFileContent, analyser, false);
        cppFileContent = ApplyToString(cppFileContent, analyser, true);
      }

      File.WriteAllText(outputHeaderFilePath, headerFileContent);
      File.WriteAllText(outputCppFilePath, cppFileContent);
    }

    public string ConvertMemberUsage(string member, string memberUsage)
    {
      return memberUsage.Replace(member+".", member+"->");
    }
  }
}
