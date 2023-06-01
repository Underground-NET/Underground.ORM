using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Urderground.ORM.Core.Translator.List;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private void TranslateStatements(string csFileContent,
                                         List<SyntaxNodeOrToken> descendants,
                                         MySqlSyntaxList mysqlSyntaxOut)
        {
            foreach (var item in descendants)
            {
                var variableDeclaration = item.AsNode() as VariableDeclarationSyntax;
                var returnStatementSyntax = item.AsNode() as ReturnStatementSyntax;
                var ifStatementSyntax = item.AsNode() as IfStatementSyntax;
                var expressionSyntax = item.AsNode() as ExpressionSyntax;
                var expressionStatementSyntax = item.AsNode() as ExpressionStatementSyntax;
                var assignmentExpressionSyntax = item.AsNode() as AssignmentExpressionSyntax;

                if (variableDeclaration != null)
                {
                    TranslateDeclareStatement(csFileContent,
                                              descendants,
                                              mysqlSyntaxOut);
                }

                if (ifStatementSyntax != null)
                {
                    TranslateIfElseStatement(csFileContent,
                                             ifStatementSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                             mysqlSyntaxOut);
                }

                if (assignmentExpressionSyntax != null)
                {
                    mysqlSyntaxOut.AppendLine("SET ");

                    var left = assignmentExpressionSyntax.Left as ExpressionSyntax;
                    var leftIdentifierNameSyntax = left as IdentifierNameSyntax;


                    mysqlSyntaxOut.Append(" = ");

                    var right = assignmentExpressionSyntax.Right as ExpressionSyntax;
                    var rightDescendents = right.GetAnnotatedNodesAndTokens().ToList();

                    var rightExpression = TranslateExpressionStatement(csFileContent,
                                                                       new List<SyntaxNodeOrToken>() { right });


                }

                if (expressionStatementSyntax != null)
                {

                }

                if (returnStatementSyntax != null)
                {
                    TranslateReturnStatement(csFileContent,
                                             returnStatementSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                             mysqlSyntaxOut);
                }
            }
        }
    }
}
