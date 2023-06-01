using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Urderground.ORM.Core.Translator.List;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private MySqlSyntaxList TranslateReturnStatement(string csFileContent,
                                                         List<SyntaxNodeOrToken> descendants,
                                                         MySqlSyntaxList mysqlSyntaxOut)
        {
            MySqlSyntaxList mysqlStatement = new();

            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var returnStatementSyntax = node as ReturnStatementSyntax;
                var expressionSyntax = node as ExpressionSyntax;

                if (returnStatementSyntax != null)
                {
                    mysqlStatement.Append("RETURN ");
                }
                else if (expressionSyntax != null)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

                    var expTranslated = TranslateExpressionStatement(csFileContent, descendantExpression);

                    mysqlStatement.AppendRange(expTranslated);

                    break;
                }
            }

            mysqlSyntaxOut.AppendRange(mysqlStatement);
            mysqlSyntaxOut.AppendLine(";");

            return mysqlStatement;
        }
    }
}
