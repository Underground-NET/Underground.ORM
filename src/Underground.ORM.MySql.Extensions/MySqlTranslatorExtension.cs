using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Underground.ORM.Core;
using Underground.ORM.Core.Syntax.Token.Declaration;
using Underground.ORM.Core.Syntax.Token.Operator;
using Underground.ORM.Core.Syntax.Token.Statement;
using Underground.ORM.Core.Translator.Extension;
using Underground.ORM.MySql.Extensions.Syntax;

namespace Underground.ORM.MySql.Extensions
{
    public static class MySqlTranslatorExtension
    {
        public static void UseMySqlSyntax(this OrmEngine orm)
        {
            orm.MySqlTranslator.ClearAllTranslations();

            orm.MySqlTranslator.AddSyntaxTranslationByAscendant<ExpressionSyntax>(
                (token, kind, parent, ascendant, helper, span, mysql) =>
                {
                    mysql.Append(token.ValueText);
                });

            orm.MySqlTranslator.AddSyntaxTranslationByAscendant<ReturnStatementSyntax>(
                (token, kind, parent, ascendant, helper, span, mysql) =>
                {
                    var expression = token.GetAscendantType<ExpressionSyntax>();

                    if (expression != null)
                    {
                        helper.Translator.RaiseTranslationByAscendant<ExpressionSyntax>
                            (token, kind, parent, ascendant, helper, span, mysql);
                    }
                    else if (kind == SyntaxKind.ReturnKeyword)
                    {
                        mysql.Append(new ReturnsStatementToken("RETURN "));
                    }
                    else if (kind == SyntaxKind.SemicolonToken)
                    {
                        mysql.AppendLine(new SemicolonToken());
                    }
                });

            orm.MySqlTranslator.AddSyntaxTranslationByAscendant<LocalDeclarationStatementSyntax>(
                (token, kind, parent, ascendant, helper, span, mysql) =>
                {
                    var v = helper.GetVariable<DeclareStatementVars>(new());

                    var previousToken = token.GetPreviousToken();
                    var nextToken = token.GetNextToken();
                    var value = token.Value;
                    var valueText = token.ValueText;
                    var text = token.Text;

                    var predefinedTypeSyntax = parent as PredefinedTypeSyntax;

                    if (kind == SyntaxKind.SemicolonToken ||
                        kind == SyntaxKind.CommaToken)
                    {
                        var dbType = v.MySqlDeclare.SelectMany(x => x).OfType<DbTypeToken>().FirstOrDefault();
                        if (dbType is null)
                        {
                            v.MySqlDeclare.Add(v.CurrentDbType!.AsSyntax<MySqlSyntax>()!);
                        }

                        v.MySqlExpression.ToList().ForEach(x => x.Order = 5);
                        v.MySqlDeclare.Add(v.MySqlExpression);

                        var ordered = v.MySqlDeclare.SelectMany(x => x).OrderBy(x => x.Order).ToList();

                        mysql.AppendRange(ordered);
                        mysql.AppendLine(new SemicolonToken());

                        v.DefaultValue = false;

                        v.MySqlExpression = new();
                        v.MySqlDeclare.Clear();

                        return;
                    }

                    var ascendants = token.GetAscendants();
                    var expression = token.GetAscendantType<ExpressionSyntax>();

                    if (v.DefaultValue && expression != null)
                    {
                        helper.Translator.RaiseTranslationByAscendant<ExpressionSyntax>(
                            token, kind, parent, ascendant, helper, span, v.MySqlExpression);

                        return;
                    }

                    if (kind == SyntaxKind.IdentifierToken ||
                        predefinedTypeSyntax != null)
                    {
                        if (nextToken.IsKind(SyntaxKind.DotToken)) return;

                        var identifierNameSyntax = parent as IdentifierNameSyntax;
                        var variableDeclaratorSyntax = parent as VariableDeclaratorSyntax;

                        string tokenType;
                        if (predefinedTypeSyntax != null)
                        {
                            tokenType = predefinedTypeSyntax.Keyword.ValueText;
                        }
                        else
                        {
                            tokenType = token.ValueText;
                        }

                        if (identifierNameSyntax != null || predefinedTypeSyntax != null)
                        {
                            var dbType = helper.Translator.TranslateDbTypeFromToken(tokenType, span);
                            v.CurrentDbType = dbType.First().AsToken<MySqlSyntaxToken>();
                            v.CurrentDbType!.Order = 3;

                            v.MySqlDeclare.Add(v.CurrentDbType.AsSyntax<MySqlSyntax>()!);
                        }

                        if (variableDeclaratorSyntax != null)
                        {
                            v.MySqlDeclare.Add(new DeclareToken("DECLARE ") { Order = 1 }.AsSyntax<MySqlSyntax>()!);
                            v.MySqlDeclare.Add(new VariableToken($"`{tokenType}` ", v.CurrentDbType!.DbType!.Value) { Order = 2 }.AsSyntax<MySqlSyntax>()!);
                        }
                    }

                    if (kind == SyntaxKind.EqualsToken)
                    {
                        v.MySqlDeclare.Add(new DefaultToken("DEFAULT ") { Order = 4 }.AsSyntax<MySqlSyntax>()!);
                        v.DefaultValue = true;
                    }
                });
        }
    }

    internal class DeclareStatementVars
    {
        public List<MySqlSyntax> MySqlDeclare { get; set; } = new();

        public MySqlSyntaxToken? CurrentDbType { get; set; } = null;

        public MySqlSyntax MySqlExpression { get; set; } = new();

        public bool DefaultValue { get; set; } = false;
    }
}
