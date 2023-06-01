using Microsoft.CodeAnalysis.CSharp.Syntax;
using Urderground.ORM.Core.Translator.Syntax;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private void TranslateMethodBlockSyntax(BlockSyntax block,
                                                string csFileContent,
                                                MySqlSyntax mysqlSyntaxOut)
        {
            var statements = block.Statements;

            foreach (var statement in statements)
            {
                string codeLine = csFileContent[statement.Span.Start..statement.Span.End];
                string fullCodeLine = csFileContent[statement.FullSpan.Start..statement.FullSpan.End];
                var fullCodeLines = fullCodeLine.TrimEnd().Split("\n")
                                                .Select(x => x.Trim())
                                                .ToList();

                #region Comments

                foreach (var line in fullCodeLines)
                {
                    if (line.StartsWith("//"))
                    {
                        mysqlSyntaxOut.AppendLine("# " + line[2..].Trim());
                    }
                    else if (string.IsNullOrEmpty(line.Trim()))
                        mysqlSyntaxOut.AppendLine();
                }

                #endregion

                var descendants = statement.DescendantNodesAndTokensAndSelf().ToList();

                TranslateStatements(csFileContent,
                                    descendants,
                                    mysqlSyntaxOut);

            }
        }
    }
}
