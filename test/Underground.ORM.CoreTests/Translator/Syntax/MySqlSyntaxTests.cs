using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Tests
{
    [TestClass()]
    public class MySqlSyntaxTests
    {
        readonly MySqlSyntax _syntax = new();

        [TestMethod()]
        public void AppendLineATest()
        {
            _syntax.AppendLine("2", "+", "(", "1", "+", "1", "-", "(", "5", "*", "3", ")", ")");

            SyntaxIntegrityTest(_syntax, 1);
        }

        [TestMethod()]
        public void AppendLineBTest()
        {
            _syntax.AppendLine("(", "1", "+", "1", "-", "(", "5", "*", "3", ")", ")");

            SyntaxIntegrityTest(_syntax, 1);
        }

        [TestMethod()]
        public void AppendLineCTest()
        {
            _syntax.AppendLine("(", "(", "(", "(", "1", "+", "1", ")", ")", ")", ")");
            
            SyntaxIntegrityTest(_syntax, 1);
        }

        [TestMethod()]
        public void AppendLineDTest()
        {
            _syntax.AppendLine("1", "+", "(", "1", "-", "1", ")", "*", "5", "+", "(", "2", "+", "2", "+", "3", ")");

            SyntaxIntegrityTest(_syntax, 1);
        }

        [TestMethod()]
        public void AppendMultiLineTest()
        {
            _syntax.AppendLine("1", "+", "(");
            _syntax.AppendLine("1", "-", "1", ")");
            _syntax.AppendLine("*", "5", "+", "("); 
            _syntax.AppendLine("2", "+", "2", "+", "3", ")");

            SyntaxIntegrityTest(_syntax, 4);
        }

        [TestMethod()]
        public void ReplaceAtTest()
        {
            _syntax.AppendLine("1", "+", "(");
            _syntax.AppendLine("1", "-", "1", ")");
            _syntax.AppendLine("*", "5", "+", "(");
            _syntax.AppendLine("2", "+", "2", "+", "3", ")");

            _syntax.ReplaceAt(1, "-");
            _syntax.ReplaceAt(3, "2");
            _syntax.ReplaceAt(7, "+");
            _syntax.ReplaceAt(11, "4");

            SyntaxIntegrityTest(_syntax, 4);
        }

        [TestMethod()]
        public void AppendAtTest()
        {
            _syntax.AppendLine("1", "+", "(");
            _syntax.AppendLine("1", "-", "1", ")");
            _syntax.AppendLine("*", "5", "+", "(");
            _syntax.AppendLine("2", "+", "2", "+", "3", ")");

            _syntax.AppendAt(3, "33");
            _syntax.AppendAt(4, "+");
            _syntax.AppendAt(5, "22");
            _syntax.AppendAt(6, "+");

            SyntaxIntegrityTest(_syntax, 4);
        }

        private static void SyntaxIntegrityTest(MySqlSyntax syntax, 
                                                int syntaxLineNumbers)
        {
            int currentLineNumber = 1;
            int elevatorLevel = 0;

            for (int i = 0; i < syntax.Count; i++)
            {
                MySqlSyntaxToken? previous = i > 0 ? syntax[i - 1] : null;
                MySqlSyntaxToken current = syntax[i];
                MySqlSyntaxToken? next = i < syntax.Count - 1 ? syntax[i + 1] : null;

                if (current.Token == "(") elevatorLevel++;

                Assert.AreEqual(currentLineNumber, current.LineNumber);

                if (i == 0)
                {
                    Assert.IsTrue(current.StartLine);
                    Assert.IsFalse(current.EndLine);
                }
                else if (i == syntax.Count - 1)
                {
                    Assert.IsTrue(current.EndLine);
                    Assert.IsFalse(current.StartLine);
                }
                else
                {
                    if (syntaxLineNumbers == 1)
                    {
                        Assert.IsFalse(current.StartLine);
                        Assert.IsFalse(current.EndLine);
                    }
                    else
                    {
                        if (current.EndLine) currentLineNumber++;
                    }
                }

                Assert.IsTrue(current.NotDefinedToken);
                Assert.IsFalse(current.RightSpace);

                Assert.AreEqual(elevatorLevel, current.ElevatorLevel);
                Assert.AreEqual(current.Previous, previous);
                Assert.AreEqual(current.Next, next);

                if (current.Token == ")") elevatorLevel--;
            }

            Assert.AreEqual(syntaxLineNumbers, syntax.SyntaxLineNumbers);
        }
    }
}