using IncludeOptimizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTests
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestDeclaration_Expressions()
    {
      string declaration = "BL::Person m_person ";
      var analyser = new Analyser();
      var decl = analyser.ParseMemberDeclaration(declaration);
      Assert.AreEqual(decl.MemberName, "m_person");
      Assert.AreEqual(decl.Type, "BL::Person");
      Assert.AreEqual(decl.Class, "Person");
    }

    [TestMethod]
    public void TestDeclaration_Simple()
    {
      string declaration = "#include <Person>\r\nPerson m_person ";
      var analyser = new Analyser();
      analyser.Analyse(declaration);
      var found = analyser.Declarations;
      Assert.AreEqual(found.Count, 1);
      Assert.AreEqual(found[0].MemberName, "m_person");
      Assert.AreEqual(found[0].Type, "Person");
      Assert.AreEqual(found[0].Class, "Person");
    }

    [TestMethod]
    public void TestDeclaration_Namespace()
    {
      string code = "#include <Person>\r\nBL::Person m_person ";
      var analyser = new Analyser();
      analyser.Analyse(code);
      var found = analyser.Declarations;
      Assert.AreEqual(found.Count, 1);
      Assert.AreEqual(found[0].MemberName, "m_person");
      Assert.AreEqual(found[0].Type, "BL::Person");
      Assert.AreEqual(found[0].Class, "Person");
    }

    [TestMethod]
    public void TestDeclaration_Many()
    {
      string code = "#include <Person>" +
        "\r\n#include <Boss>" +
        "\r\nBL::Person m_person" +
        "\r\nBL::Boss m_boss ";
      var analyser = new Analyser();
      analyser.Analyse(code);
      var found = analyser.Declarations;
      Assert.AreEqual(found.Count, 2);
      Assert.AreEqual(found[0].MemberName, "m_person");
      Assert.AreEqual(found[0].Type, "BL::Person");
      Assert.AreEqual(found[0].Class, "Person");

      Assert.AreEqual(found[1].MemberName, "m_boss");
      Assert.AreEqual(found[1].Type, "BL::Boss");
      Assert.AreEqual(found[1].Class, "Boss");
    }
  }
}
