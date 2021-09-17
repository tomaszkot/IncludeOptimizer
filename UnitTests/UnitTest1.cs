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
    public void TestDeclaration_Simple()
    {
      string declaration = "#include <Person>\r\nPerson m_person ";
      Analyser analyser = new Analyser();
      analyser.Analyse(declaration);
      var found = analyser.Declarations;
      Assert.AreEqual(found.Count, 1);
      Assert.AreEqual(found[0].MemberName, "m_person");
    }

    [TestMethod]
    public void TestDeclaration_Namespace()
    {
      string declaration = "#include <Person>\r\nBL.Person m_person ";
      Analyser analyser = new Analyser();
      analyser.Analyse(declaration);
      var found = analyser.Declarations;
      Assert.AreEqual(found.Count, 1);
      Assert.AreEqual(found[0].MemberName, "m_person");
    }
  }
}
