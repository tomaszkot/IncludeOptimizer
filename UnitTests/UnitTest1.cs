using IncludeOptimizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
  [TestClass]
  public class UnitTest1
  {
    List<Declaration> GetDeclarations(string code, int count)
    {
      var analyser = new Analyser();
      analyser.Analyse(code);
      var found = analyser.Declarations;
      Assert.AreEqual(found.Count, count);
      return found;
    }

    string Convert(string input, bool implFile)
    {
      var optimizationSettings = new OptimizationSettings();
      var analyser = new Analyser();
      analyser.Analyse(input);
      var applicator = new Applicator();
      return applicator.ApplyToString(input, analyser, implFile);
    }

    Declaration ParseCode(string code)
    {
      var res = GetDeclarations(code, 1);
      return res.Any() ? res[0] : null;
    }

    Declaration ParseDeclaration(string declarationLine)
    {
      var analyser = new Analyser();
      var decl = analyser.ParseMemberDeclaration(declarationLine);
      Assert.IsNotNull(decl);
      return decl;
    }
        
    [TestMethod]
    public void TestInclude_Expressions()
    {
      var analyser = new Analyser();
      var inc = analyser.ParseIncludeContent("#include \"Model.h\"");
      Assert.AreEqual(inc, "Model.h");
    }

    [TestMethod]
    public void TestDeclaration_Expressions()
    {
      var decl = ParseDeclaration("BL::Person m_person;");
      Assert.AreEqual(decl.MemberName, "m_person");
      Assert.AreEqual(decl.Type, "BL::Person");
      Assert.AreEqual(decl.Class, "Person");
    }

    [TestMethod]
    public void Test_ImplFileMemberUsage()
    {
      var applicator = new Applicator();
      var res = applicator.ConvertMemberUsage("m_person", "m_person.foo();");
      Assert.AreEqual(res, "m_person->foo();");
    }

    [TestMethod]
    public void Test_ImplFile()
    {
      var header = "#include <Person>\r\nBL::Person m_person; ";
      
      var optimizationSettings = new OptimizationSettings();
      var analyser = new Analyser();
      analyser.Analyse(header);
      
      var applicator = new Applicator();
      var result = applicator.ApplyToString("m_person.foo();", analyser, true);
      Assert.AreEqual(result, "m_person->foo();");
    }

    [TestMethod]
    public void TestDeclaration_Simple()
    {
      var decl = ParseCode("#include <Person>\r\nPerson m_person ;");
      Assert.AreEqual(decl.MemberName, "m_person");
      Assert.AreEqual(decl.Type, "Person");
      Assert.AreEqual(decl.Class, "Person");
    }

    [TestMethod]
    public void TestDeclaration_Namespace()
    {
      var decl = ParseCode("#include <Person>\r\nBL::Person m_person; ");
      Assert.AreEqual(decl.MemberName, "m_person");
      Assert.AreEqual(decl.Type, "BL::Person");
      Assert.AreEqual(decl.Class, "Person");
    }

    [TestMethod]
    public void TestDeclaration_Replace()
    {
      string code = @"#include <Person>
BL::Person m_person;";
      var result = Convert(code, false);

      string expectedResult = @"#include <memory>
class BL::Person;
std::shared_ptr<BL::Person> m_person;";

      Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void TestDeclaration_Many()
    {
      string code = @"#include <Person>
#include <Boss>
BL::Person m_person;
BL::Boss m_boss; ";
      var decls = GetDeclarations(code, 2);
      Assert.AreEqual(decls[0].MemberName, "m_person");
      Assert.AreEqual(decls[0].Type, "BL::Person");
      Assert.AreEqual(decls[0].Class, "Person");

      Assert.AreEqual(decls[1].MemberName, "m_boss");
      Assert.AreEqual(decls[1].Type, "BL::Boss");
      Assert.AreEqual(decls[1].Class, "Boss");

      var result =  Convert(code, false);

      string expectedResult = @"#include <memory>
class BL::Person;
class BL::Boss;
std::shared_ptr<BL::Person> m_person;
std::shared_ptr<BL::Boss> m_boss; ";

      Assert.AreEqual(expectedResult, result);
    }
  }
}
