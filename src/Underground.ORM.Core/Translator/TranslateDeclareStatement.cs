using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Urderground.ORM.Core.Translator.Syntax;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private void TranslateDeclareStatement(string csFileContent,
                                               List<SyntaxNodeOrToken> descendants,
                                               MySqlSyntax mysqlSyntaxOut)
        {
            Dictionary<int, MySqlSyntax> mysqlDeclare = new();
            (int Order, MySqlSyntax)? mysqlExpression = null;
            (int Order, MySqlSyntax) mysqlDbType;

            int variablesCount = 0;
            bool defaultValue = false;

            #region Predefined Type

            var predefinedTypeSyntax = descendants.Select(x => x.AsNode()).OfType<PredefinedTypeSyntax>().FirstOrDefault();
            var predefinedType = descendants.Select(x => x.AsNode()).OfType<VariableDeclarationSyntax>().FirstOrDefault();

            MySqlSyntax dbType;
            if (predefinedTypeSyntax != null)
            {
                string contentDeclaration = csFileContent[predefinedTypeSyntax.Span.Start..predefinedTypeSyntax.Span.End];
                dbType = TranslateDbTypeFromToken(predefinedTypeSyntax.Keyword.Text, contentDeclaration);

            }
            else if (predefinedType != null)
            {
                string contentDeclaration = csFileContent[predefinedType.Span.Start..predefinedType.Span.End];

                var identifierNameSyntax = predefinedType.Type as IdentifierNameSyntax;
                var nullableTypeSyntax = predefinedType.Type as NullableTypeSyntax;

                string variableTypeName = "";

                if (nullableTypeSyntax != null)
                    variableTypeName = csFileContent[nullableTypeSyntax.Span.Start..nullableTypeSyntax.Span.End].TrimEnd('?');
                else
                    variableTypeName = csFileContent[identifierNameSyntax!.Span.Start..identifierNameSyntax.Span.End];

                dbType = TranslateDbTypeFromToken(variableTypeName, contentDeclaration);
            }
            else
                throw new NotImplementedException("Predefined type not found");

            mysqlDbType = (3, dbType);
            #endregion

            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var variableDeclarator = item.AsNode() as VariableDeclaratorSyntax;
                var equalsValueClauseSyntax = item.AsNode() as EqualsValueClauseSyntax;
                var expressionSyntax = item.AsNode() as ExpressionSyntax;
                var identifierNameSyntax = item.AsNode() as IdentifierNameSyntax;

                if (variableDeclarator != null)
                {
                    defaultValue = false;

                    FinalizeDeclarationStatementMySql(mysqlDeclare,
                                                      mysqlExpression,
                                                      mysqlDbType,
                                                      mysqlSyntaxOut);

                    string variableName = variableDeclarator.Identifier.ValueText;

                    mysqlDeclare.Add(1, new("DECLARE "));
                    mysqlDeclare.Add(2, new($"`{variableName}` "));

                    variablesCount++;
                }

                if (equalsValueClauseSyntax != null)
                {
                    defaultValue = true;
                    mysqlDeclare.Add(4, new("DEFAULT "));
                }

                if (expressionSyntax != null && defaultValue)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

                    var expTranslated = TranslateExpressionStatement(csFileContent,
                                                                     descendantExpression,
                                                                     mysqlSyntaxOut);

                    FinalizeDeclarationStatementMySql(mysqlDeclare,
                                                     (5, expTranslated),
                                                     mysqlDbType,
                                                     mysqlSyntaxOut);

                    defaultValue = false;
                }
            }

            FinalizeDeclarationStatementMySql(mysqlDeclare,
                                              mysqlExpression,
                                              mysqlDbType,
                                              mysqlSyntaxOut);
        }

        private void FinalizeDeclarationStatementMySql(Dictionary<int, MySqlSyntax> mysqlDeclare,
                                                      (int Order, MySqlSyntax)? mysqlExpression,
                                                      (int Order, MySqlSyntax) mysqlDbType,
                                                      MySqlSyntax mysqlSyntaxOut)
        {
            if (!mysqlDeclare.Any()) return;

            mysqlDeclare.Add(mysqlDbType.Order, (MySqlSyntax)mysqlDbType.Item2.Clone());

            if (mysqlExpression != null)
            {
                mysqlDeclare.Add(mysqlExpression.Value.Order, mysqlExpression.Value.Item2);
            }

            // (1)DECLARE (2)`variable` (3)type (4)DEFAULT (5)expression;
            var mysqlStatement = mysqlDeclare.OrderBy(x => x.Key).Select(x => x.Value).ToList();

            mysqlSyntaxOut.AppendRange(mysqlStatement);
            mysqlSyntaxOut.AppendLine(";");

            mysqlDeclare.Clear();
        }
    }
}
