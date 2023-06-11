using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core.Translator.Extension;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Token.Declaration;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator2
    {
        private void TranslateDeclareStatement(string csFileContent,
                                               IEnumerable<SyntaxToken> csharpTokens,
                                               MySqlSyntax mysqlSyntaxOut)
        {
            MySqlSyntax declare = new();

            foreach (SyntaxToken token in csharpTokens)
            {
                var previousToken = token.GetPreviousToken();
                var nextToken = token.GetNextToken();
                var kind = token.Kind();
                var span = csFileContent[token.Span.Start..token.Span.End];
                var fullSpan = csFileContent[token.FullSpan.Start..token.FullSpan.End];
                var value = token.Value;
                var valueText = token.ValueText;
                var text = token.Text;

                var predefinedTypeSyntax = token.GetAscendantType<PredefinedTypeSyntax>();
                var variableDeclarationSyntax = token.GetAscendantType<VariableDeclarationSyntax>();
                var identifierNameSyntax = token.GetAscendantType<IdentifierNameSyntax>();
                var nullableTypeSyntax = token.GetAscendantType<NullableTypeSyntax>();
                var qualifiedNameSyntax = token.GetAscendantType<QualifiedNameSyntax>();
                var expressionSyntax = token.GetAscendantType<ExpressionSyntax>();
                var equalsValueClauseSyntax = token.GetAscendantType<EqualsValueClauseSyntax>();

                if (variableDeclarationSyntax != null)
                {
                    foreach (var variable in variableDeclarationSyntax.Variables)
                    {
                        declare.Append(new DeclareToken("DECLARE "));
                        declare.Append(new VariableToken($"`{variable.Identifier.ValueText}` "));
                        
                        declare.AppendLine(new SemicolonToken(";"));
                    }
                }
            }
        }

        //private void TranslateDeclareStatement(string csFileContent,
        //                                       List<SyntaxToken> csharpTokens,
        //                                       MySqlSyntax mysqlSyntaxOut)
        //{
        //    Dictionary<int, MySqlSyntax> mysqlDeclare = new();
        //    (int Order, MySqlSyntax)? mysqlExpression = null;
        //    (int Order, MySqlSyntax) mysqlDbType;

        //    int variablesCount = 0;
        //    bool defaultValue = false;

        //    #region Predefined Type



        //    var predefinedTypeSyntax = csharpTokens.Select(x => x.AsNode()).OfType<PredefinedTypeSyntax>().FirstOrDefault();
        //    var predefinedType = csharpTokens.Select(x => x.AsNode()).OfType<VariableDeclarationSyntax>().FirstOrDefault();

        //    MySqlSyntax dbType;
        //    if (predefinedTypeSyntax != null)
        //    {
        //        string contentDeclaration = csFileContent[predefinedTypeSyntax.Span.Start..predefinedTypeSyntax.Span.End];
        //        dbType = TranslateDbTypeFromToken(predefinedTypeSyntax.Keyword.Text, contentDeclaration);

        //    }
        //    else if (predefinedType != null)
        //    {
        //        string contentDeclaration = csFileContent[predefinedType.Span.Start..predefinedType.Span.End];

        //        var identifierNameSyntax = predefinedType.Type as IdentifierNameSyntax;
        //        var nullableTypeSyntax = predefinedType.Type as NullableTypeSyntax;
        //        var qualifiedNameSyntax = predefinedType.Type as QualifiedNameSyntax;

        //        string variableTypeName;
        //        if (nullableTypeSyntax != null)
        //        {
        //            variableTypeName = csFileContent[nullableTypeSyntax.Span.Start..nullableTypeSyntax.Span.End].TrimEnd('?');
        //        }
        //        else if (qualifiedNameSyntax != null)
        //        {
        //            variableTypeName = csFileContent[qualifiedNameSyntax!.Span.Start..qualifiedNameSyntax.Span.End];
        //        }
        //        else
        //            variableTypeName = csFileContent[identifierNameSyntax!.Span.Start..identifierNameSyntax.Span.End];

        //        dbType = TranslateDbTypeFromToken(variableTypeName, contentDeclaration);
        //    }
        //    else
        //        throw new NotImplementedException("Predefined type not found");

        //    mysqlDbType = (3, dbType);
        //    #endregion

        //    foreach (var csharpToken in csharpTokens)
        //    {
        //        string contentDeclaration = csFileContent[csharpToken.Span.Start..csharpToken.Span.End];

        //        var variableDeclarator = csharpToken.GetParentType<VariableDeclaratorSyntax>();
        //        var equalsValueClauseSyntax = csharpToken.GetParentType<EqualsValueClauseSyntax>();
        //        var expressionSyntax = csharpToken.GetParentType<ExpressionSyntax>();
        //        var identifierNameSyntax = csharpToken.GetParentType<IdentifierNameSyntax>();

        //        if (variableDeclarator != null)
        //        {
        //            defaultValue = false;

        //            FinalizeDeclarationStatementMySql(mysqlDeclare,
        //                                              mysqlExpression,
        //                                              mysqlDbType,
        //                                              mysqlSyntaxOut);

        //            string variableName = variableDeclarator.Identifier.ValueText;

        //            mysqlDeclare.Add(1, new DeclareToken("DECLARE "));
        //            mysqlDeclare.Add(2, new VariableToken($"`{variableName}` "));

        //            variablesCount++;
        //        }

        //        if (equalsValueClauseSyntax != null)
        //        {
        //            defaultValue = true;
        //            mysqlDeclare.Add(4, new DefaultToken("DEFAULT "));
        //        }

        //        if (expressionSyntax != null && defaultValue)
        //        {
        //            var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

        //            var expTranslated = TranslateExpressionStatement(csFileContent,
        //                                                             descendantExpression,
        //                                                             mysqlSyntaxOut);

        //            FinalizeDeclarationStatementMySql(mysqlDeclare,
        //                                             (5, expTranslated),
        //                                             mysqlDbType,
        //                                             mysqlSyntaxOut);

        //            defaultValue = false;
        //        }
        //    }

        //    FinalizeDeclarationStatementMySql(mysqlDeclare,
        //                                      mysqlExpression,
        //                                      mysqlDbType,
        //                                      mysqlSyntaxOut);
        //}

        //private void FinalizeDeclarationStatementMySql(Dictionary<int, MySqlSyntax> mysqlDeclare,
        //                                              (int Order, MySqlSyntax)? mysqlExpression,
        //                                              (int Order, MySqlSyntax) mysqlDbType,
        //                                              MySqlSyntax mysqlSyntaxOut)
        //{
        //    if (!mysqlDeclare.Any()) return;

        //    var dbTypeToken = mysqlDbType.Item2.OfType<DbTypeToken>().First();
        //    ((VariableToken)mysqlDeclare[2][0]).SetDbType(dbTypeToken);

        //    mysqlDeclare.Add(mysqlDbType.Order, (MySqlSyntax)mysqlDbType.Item2.Clone());

        //    if (mysqlExpression != null)
        //    {
        //        mysqlDeclare.Add(mysqlExpression.Value.Order, mysqlExpression.Value.Item2);
        //    }

        //    // (1)DECLARE (2)`variable` (3)type (4)DEFAULT (5)expression;
        //    var mysqlStatement = mysqlDeclare.OrderBy(x => x.Key).Select(x => x.Value).ToList();

        //    mysqlSyntaxOut.AppendRange(mysqlStatement);
        //    mysqlSyntaxOut.AppendLine(new SemicolonToken(";"));

        //    mysqlDeclare.Clear();
        //}
    }
}
