using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Syntax;
using Underground.ORM.Core.Syntax.Token.Operator;
using Underground.ORM.Core.Syntax.Token.Statement;

namespace Underground.ORM.Core.Translator
{
    public partial class SqlTranslator
    {
        private SyntaxBase TranslateReturnStatement(string csFileContent,
                                                     List<SyntaxNodeOrToken> descendants,
                                                     SyntaxBase mysqlSyntaxOut)
        {
            SyntaxBase statement = new();

            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var returnStatementSyntax = node as ReturnStatementSyntax;
                var expressionSyntax = node as ExpressionSyntax;

                if (returnStatementSyntax != null)
                {
                    statement.Append(new ReturnStatementToken("RETURN "));
                }
                else if (expressionSyntax != null)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

                    var expTranslated = TranslateExpressionStatement(csFileContent, 
                                                                     descendantExpression,
                                                                     mysqlSyntaxOut);

                    statement.AppendRange(expTranslated);

                    break;
                }
            }

            mysqlSyntaxOut.AppendRange(statement);
            mysqlSyntaxOut.AppendLine(new SemicolonToken());

            return statement;
        }
    }
}
