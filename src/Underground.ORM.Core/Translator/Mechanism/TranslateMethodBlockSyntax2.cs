using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Syntax;
using Underground.ORM.Core.Syntax.Token.Marker;

namespace Underground.ORM.Core.Translator
{
    public partial class SqlTranslator
    {
        
        private void TranslateMethodBlockSyntax(BlockSyntax block,
                                                string csFileContent,
                                                SyntaxBase mysqlSyntaxOut)
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
                        mysqlSyntaxOut.AppendLine(new CommentLineToken("# " + line[2..].Trim()));
                    }
                    else if (string.IsNullOrEmpty(line.Trim()))
                        mysqlSyntaxOut.AppendLine("");
                }

                #endregion

                var csharpTokens = statement.DescendantTokens();

                TranslateStatement(csFileContent,
                                   csharpTokens,
                                   fullCodeLine,
                                   mysqlSyntaxOut);
            }
        }
    }
}
