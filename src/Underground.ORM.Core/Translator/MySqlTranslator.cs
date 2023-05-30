using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using Urderground.ORM.Core.Attributes;

namespace Urderground.ORM.Core.Translator
{
    public class MySqlTranslator
    {
        public MySqlSyntax TranslateToMySqlSyntax(MethodInfo method, object?[] values)
        {
            var functionAttribute = method.GetCustomAttribute<MySqlFunctionScopeAttribute>();

            if (functionAttribute == null)
            {
                throw new NotImplementedException($"Atributo '{nameof(MySqlFunctionScopeAttribute)}' não definido para este método");
            }

            #region Function In Parameters

            var parameters = method.GetParameters();

            var parametersIn = parameters
                .Select(x => new MySqlParameterDbType(x.Name,
                                                 GetDbType(x.ParameterType),
                                                 x.ParameterType)).ToList();

            #endregion

            #region Function Out Parameters

            // TODO: Para tipos Out

            #endregion

            #region Function InOut Parameters

            // TODO: Para tipos Ref

            #endregion

            #region Function Return Parameter

            var returnType = method.ReturnType;
            var mysqlReturnDbType = GetMySqlReturnDbType(returnType);
            
            #endregion

            var csFileContent = File.ReadAllText(functionAttribute.CallerFilePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csFileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var nds = (NamespaceDeclarationSyntax)root.Members[0];
            var cds = (ClassDeclarationSyntax)nds.Members[0];

            List<string> mysqlSyntaxOut = new();

            var parametersInFunction = string.Join(", ", parametersIn.Select(x => $"`{x.Argument}` {x.DbType}"));

            mysqlSyntaxOut.Add($"CREATE FUNCTION `{functionAttribute.Name}` ({parametersInFunction})");
            mysqlSyntaxOut.Add("\n");
            mysqlSyntaxOut.Add($"RETURNS {mysqlReturnDbType.DbType}");
            mysqlSyntaxOut.Add("\n");
            mysqlSyntaxOut.Add("BEGIN");
            mysqlSyntaxOut.Add("\n");

            foreach (var ds in cds.Members)
            {
                if (ds is MethodDeclarationSyntax)
                {
                    var mds = (MethodDeclarationSyntax)ds;
                    var methodName = mds.Identifier.ValueText;

                    if (methodName == method.Name)
                    {
                        // TODO: Desenvolver regras para tradução de C# para MySql

                        ConvertMethodBlockSyntax(mds.Body!, csFileContent, mysqlSyntaxOut);
                        break;
                    }
                }
            }

            mysqlSyntaxOut.Add("END;");

            return new MySqlSyntax(string.Join("", mysqlSyntaxOut), 
                                   mysqlSyntaxOut);
        }

        private void ConvertMethodBlockSyntax(BlockSyntax block,
                                              string csFileContent,
                                              List<string> mysqlSyntaxOut)
        {
            var statements = block.Statements;

            foreach (var statement in statements)
            {
                string codeLine = csFileContent[statement.Span.Start..statement.Span.End];
                string fullCodeLine = csFileContent[statement.FullSpan.Start..statement.FullSpan.End];
                var fullCodeLines = fullCodeLine.TrimEnd().Split("\n")
                                                     .Select(x => x.Trim()).ToList();

                #region Comments

                foreach (var line in fullCodeLines)
                {
                    if (line.StartsWith("//"))
                    {
                        mysqlSyntaxOut.Add("# " + line[2..].Trim());
                        mysqlSyntaxOut.Add("\n");
                    }
                    else if (string.IsNullOrEmpty(line.Trim()))
                        mysqlSyntaxOut.Add("\n");
                }

                #endregion

                var descendants = statement.DescendantNodesAndTokensAndSelf().ToList();

                TranslateStatementToMySql(csFileContent,
                                          descendants,
                                          mysqlSyntaxOut);

            }
        }

        private void TranslateStatementToMySql(string csFileContent,
                                               List<SyntaxNodeOrToken> descendants,
                                               List<string> mysqlSyntaxOut)
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
                    TranslateDeclarationToMySql(csFileContent,
                                                descendants,
                                                mysqlSyntaxOut);
                }

                if (ifStatementSyntax != null)
                {
                    TranslateIfToMySql(csFileContent,
                                       ifStatementSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                       mysqlSyntaxOut);
                }

                if (assignmentExpressionSyntax != null)
                {
                    mysqlSyntaxOut.Add("SET ");
                    mysqlSyntaxOut.Add("\n");

                    var left = assignmentExpressionSyntax.Left as ExpressionSyntax;
                    var leftIdentifierNameSyntax = left as IdentifierNameSyntax;


                    mysqlSyntaxOut.Append(" = ");

                    var right = assignmentExpressionSyntax.Right as ExpressionSyntax;
                    var rightDescendents = right.GetAnnotatedNodesAndTokens().ToList();

                    var rightExpression = TranslateExpressionToMySql(csFileContent,
                                                    new List<SyntaxNodeOrToken>() { right });


                }

                if (expressionStatementSyntax != null)
                {

                }

                if (returnStatementSyntax != null)
                {
                    TranslateReturnsToMySql(csFileContent,
                                            returnStatementSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                            mysqlSyntaxOut);
                }
            }
        }

        private void TranslateIfToMySql(string csFileContent,
                                                List<SyntaxNodeOrToken> descendants,
                                                List<string> mysqlSyntaxOut)
        {
            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var ifStatementSyntax = node as IfStatementSyntax;
                var expressionSyntax = node as ExpressionSyntax;

                if (ifStatementSyntax != null)
                {
                    mysqlSyntaxOut.Append("IF(");

                    var condition = ifStatementSyntax.Condition;
                    var conditionDescendants = condition.DescendantNodesAndTokensAndSelf().ToList();
                    var conditionTranslated = TranslateExpressionToMySql(csFileContent, conditionDescendants);

                    mysqlSyntaxOut.Append(string.Join("", conditionTranslated));
                    mysqlSyntaxOut.Add(")THEN");
                    mysqlSyntaxOut.Add("\n");

                    var statementSyntax = ifStatementSyntax.Statement;

                    TranslateStatementToMySql(csFileContent,
                                              statementSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                              mysqlSyntaxOut);

                    var elseSyntax = ifStatementSyntax?.Else;

                    if (elseSyntax != null)
                    {
                        mysqlSyntaxOut.Add("ELSE");
                        mysqlSyntaxOut.Add("\n");

                        TranslateStatementToMySql(csFileContent,
                                                  elseSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                                  mysqlSyntaxOut);

                        mysqlSyntaxOut.Add("END IF");
                        mysqlSyntaxOut.Add("\n");

                    }
                }
                else if (expressionSyntax != null)
                {

                }
                else
                {
                    //if ((token.Parent as IfStatementSyntax) != null) continue;

                    //mysqlStatement.Add(token.ValueText);
                }
            }
        }

        private List<string> TranslateReturnsToMySql(string csFileContent,
                                                     List<SyntaxNodeOrToken> descendants,
                                                     List<string> mysqlSyntaxOut)
        {
            List<string> mysqlStatement = new();

            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var returnStatementSyntax = node as ReturnStatementSyntax;
                var expressionSyntax = node as ExpressionSyntax;

                if (returnStatementSyntax != null)
                {
                    mysqlStatement.Add("RETURN ");
                }
                else if (expressionSyntax != null)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

                    var expTranslated = TranslateExpressionToMySql(csFileContent, descendantExpression);

                    mysqlStatement.AddRange(expTranslated);

                    break;
                }
            }

            mysqlSyntaxOut.Add(string.Join("", mysqlStatement) + ";");
            mysqlSyntaxOut.Add("\n");

            return mysqlStatement;
        }

        private List<string> TranslateDeclarationToMySql(string csFileContent,
                                                         List<SyntaxNodeOrToken> descendants,
                                                         List<string> mysqlSyntaxOut)
        {
            List<string> mysqlStatements = new();

            Dictionary<int, string> mysqlDeclare = new();
            (int Order, List<string>)? mysqlExpression = null;
            (int Order, string) mysqlDbType;

            int variablesCount = 0;
            bool defaultValue = false;

            #region Predefined Type

            var predefinedTypeSyntax = descendants.Select(x => x.AsNode()).OfType<PredefinedTypeSyntax>().FirstOrDefault();
            var predefinedType = descendants.Select(x => x.AsNode()).OfType<VariableDeclarationSyntax>().FirstOrDefault();

            string dbType;
            if (predefinedTypeSyntax != null)
            {
                string contentDeclaration = csFileContent[predefinedTypeSyntax.Span.Start..predefinedTypeSyntax.Span.End];
                dbType = GetDbTypeFromToken(predefinedTypeSyntax.Keyword.Text, contentDeclaration);

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

                dbType = GetDbTypeFromToken(variableTypeName, contentDeclaration);
            }
            else
                throw new NotImplementedException("Predefined type not found");

            mysqlDbType = (3, $" {dbType}");
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

                    mysqlStatements.AddRange(FinalizeDeclarationStatementMySql(mysqlDeclare, mysqlExpression, mysqlDbType, mysqlSyntaxOut));

                    string variableName = variableDeclarator.Identifier.ValueText;

                    mysqlDeclare.Add(1, "DECLARE ");
                    mysqlDeclare.Add(2, $"`{variableName}`");

                    variablesCount++;
                }

                if (equalsValueClauseSyntax != null)
                {
                    defaultValue = true;
                    mysqlDeclare.Add(4, " DEFAULT ");
                }

                if (expressionSyntax != null && defaultValue)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

                    var expTranslated = TranslateExpressionToMySql(csFileContent, descendantExpression);

                    mysqlStatements.AddRange(
                        FinalizeDeclarationStatementMySql(mysqlDeclare,
                                                         (5, expTranslated),
                                                         mysqlDbType,
                                                         mysqlSyntaxOut));

                    defaultValue = false;
                }
            }

            mysqlStatements.AddRange(FinalizeDeclarationStatementMySql(mysqlDeclare, mysqlExpression, mysqlDbType, mysqlSyntaxOut));

            return mysqlStatements;
        }

        private List<string> FinalizeDeclarationStatementMySql(Dictionary<int, string> mysqlDeclare,
                                                              (int Order, List<string>)? mysqlExpression,
                                                              (int Order, string) mysqlDbType,
                                                              List<string> mysqlSyntaxOut)
        {
            if (!mysqlDeclare.Any()) return new();

            mysqlDeclare.Add(mysqlDbType.Order, mysqlDbType.Item2);

            if (mysqlExpression != null)
            {
                mysqlDeclare.Add(mysqlExpression.Value.Order, string.Join("", mysqlExpression.Value.Item2));
            }

            // (1)DECLARE (2)`variable` (3)type (4)DEFAULT (5)expression;
            var mysqlStatement = string.Join("", mysqlDeclare.OrderBy(x => x.Key).Select(x => x.Value));

            mysqlSyntaxOut.Add(mysqlStatement + ";");
            mysqlSyntaxOut.Add("\n");

            mysqlDeclare.Clear();

            return new List<string>() { mysqlStatement };
        }

        private List<string> TranslateExpressionToMySql(string csFileContent,
                                                        List<SyntaxNodeOrToken> descendants)
        {
            List<string> mysqlExpression = new();

            int currentLevel = 0;
            List<ElevatorCastExpression> elevatorCast = new();

            foreach (var item in descendants)
            {
                string contentExpression = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                if (token.Parent != null)
                {
                    var identifierNameSyntax = token.Parent as IdentifierNameSyntax;
                    var literalExpressionSyntax = token.Parent as LiteralExpressionSyntax;
                    var castExpressionSyntax = token.Parent as CastExpressionSyntax;
                    var binaryExpressionSyntax = token.Parent as BinaryExpressionSyntax;
                    var parenthesizedExpressionSyntax = token.Parent as ParenthesizedExpressionSyntax;
                    var predefinedTypeSyntax = token.Parent as PredefinedTypeSyntax;

                    if (identifierNameSyntax != null)
                    {
                        mysqlExpression.Add($"`{token.ValueText}`");
                    }
                    else if (literalExpressionSyntax != null)
                    {
                        mysqlExpression.Add(token.Text);
                    }
                    else if (castExpressionSyntax != null)
                    {
                        if (token.ValueText == ")")
                        {
                            var predefinedType = castExpressionSyntax.Type as PredefinedTypeSyntax;

                            var (Function, Alias) = GetMysqlCastFunctionFromToken(predefinedType!.Keyword.ValueText, contentExpression);

                            elevatorCast.Add(new ElevatorCastExpression()
                            {
                                Level = currentLevel,
                                Cast = castExpressionSyntax,
                                Function = Function,
                                Alias = Alias
                            });

                            mysqlExpression.Add(Function);
                            mysqlExpression.Add("(");
                        }
                    }
                    else if (binaryExpressionSyntax != null)
                    {

                        #region Close parenthese with mysql cast conversion

                        ElevatorCastExpression lastElevator;
                        if (elevatorCast.Any() && currentLevel == (lastElevator = elevatorCast.Last()).Level)
                        {
                            mysqlExpression.Add(BuildRightCastFunction(lastElevator));
                            elevatorCast.RemoveAt(elevatorCast.Count - 1);
                        }
                        #endregion

                        string operatorToken = binaryExpressionSyntax.OperatorToken.ValueText;

                        if (operatorToken == "==")
                        {
                            operatorToken = "=";
                        }

                        mysqlExpression.Add(operatorToken);
                    }
                    else if (parenthesizedExpressionSyntax != null)
                    {
                        string parentheseToken = token.Text;

                        mysqlExpression.Add(parentheseToken);

                        if (parentheseToken == "(")
                        {
                            currentLevel++;
                        }
                        else if (parentheseToken == ")")
                        {
                            currentLevel--;
                        }
                    }
                    else
                    {
                        if (predefinedTypeSyntax?.Parent as CastExpressionSyntax != null)
                        {
                            continue;
                        }

                        // Tokens não tratados podem ser adicionados à expressão
                        mysqlExpression.Add(token.Text);
                    }
                }
            }

            #region Close parentheses with mysql cast conversion
            elevatorCast.ForEach(lastElevator =>
            {
                mysqlExpression.Add(BuildRightCastFunction(lastElevator));
            });
            #endregion

            return mysqlExpression;
        }

        private string BuildRightCastFunction(ElevatorCastExpression lastElevator)
        {
            string castFunction = ")";

            if (lastElevator.Alias != null)
            {
                castFunction = $" AS {lastElevator.Alias})";
            }

            return castFunction;
        }

        private (string Function, string? Alias)
            GetMysqlCastFunctionFromToken(string castType, string contentDeclaration)
        {
            if (castType == "ulong")
            {
                return ("CAST", "UNSIGNED BIGINT");
            }
            else if (castType == "long")
            {
                return ("CAST", "SIGNED BIGINT");
            }
            else if (castType == "uint")
            {
                return ("CAST", "UNSIGNED INT");
            }
            else if (castType == "int")
            {
                return ("CAST", "SIGNED INT");
            }
            else if (castType == "char")
            {
                return ("CHAR", null);
            }
            else
                throw new NotImplementedException($"Tipo de conversão '{castType}' de '{contentDeclaration}' não suportada");
        }

        private string GetDbTypeFromToken(string tokenType,
                                          string contentDeclaration)
        {
            if (tokenType == "var")
            {
                return "JSON";
            }
            else if (tokenType.StartsWith("List<") ||
                     tokenType.StartsWith("IList<") ||
                     tokenType.StartsWith("IEnumerable<") ||
                     tokenType.StartsWith("Enumerable<") ||
                     tokenType.StartsWith("Collection<") ||
                     tokenType.StartsWith("ICollection<") ||
                     tokenType.EndsWith("[]"))
            {
                return "JSON";
            }
            else if (tokenType == "object" || tokenType == "Object" || tokenType == "System.Object")
            {
                return "JSON";
            }
            else if (tokenType == "char" || tokenType == "Char" || tokenType == "System.Char")
            {
                return "CHAR";
            }
            else if (tokenType == "bool" || tokenType == "Boolean" || tokenType == "System.Boolean")
            {
                return "BOOL";
            }
            else if (tokenType == "sbyte" || tokenType == "SByte" || tokenType == "System.SByte")
            {
                return "TINYINT";
            }
            else if (tokenType == "byte" || tokenType == "Byte" || tokenType == "System.Byte")
            {
                return "TINYINT UNSIGNED";
            }
            else if (tokenType == "string" || tokenType == "String" || tokenType == "System.String")
            {
                return "VARCHAR(255)";
            }
            else if (tokenType == "short" || tokenType == "Int16" || tokenType == "System.Int16")
            {
                return "SMALLINT";
            }
            else if (tokenType == "ushort" || tokenType == "UInt16" || tokenType == "System.UInt16")
            {
                return "SMALLINT UNSIGNED";
            }
            else if (tokenType == "int" || tokenType == "Int32" || tokenType == "System.Int32")
            {
                return "INT";
            }
            else if (tokenType == "uint" || tokenType == "UInt32" || tokenType == "System.UInt32")
            {
                return "INT UNSIGNED";
            }
            else if (tokenType == "long" || tokenType == "Int64" || tokenType == "System.Int64")
            {
                return "BIGINT";
            }
            else if (tokenType == "ulong" || tokenType == "UInt64" || tokenType == "System.UInt64")
            {
                return "BIGINT UNSIGNED";
            }
            else if (tokenType == "decimal" || tokenType == "Decimal" || tokenType == "System.Decimal")
            {
                return "DECIMAL";
            }
            else if (tokenType == "double" || tokenType == "Double" || tokenType == "System.Double")
            {
                return "DOUBLE";
            }
            else if (tokenType == "float" || tokenType == "Single" || tokenType == "System.Single")
            {
                return "FLOAT";
            }
            else
                throw new NotImplementedException($"Tipo de declaração '{contentDeclaration}' não suportada");

        }

        private MySqlParameterDbType GetMySqlReturnDbType(Type returnType)
        {
            var dbType = MySqlParameterDbType.Factory.Create();

            dbType.DbType = GetDbType(returnType);
            dbType.FromType = returnType;

            if (returnType.IsGenericType)
            {
                foreach (var genericType in returnType.GenericTypeArguments)
                {
                    dbType.AddSubType(genericType.Name, GetDbType(genericType), genericType);
                }
            }

            return dbType;
        }

        private string GetDbType(Type returnType)
        {
            string dbType;

            if (returnType.Name == "List`1")
            {
                dbType = "JSON";
            }
            else if (returnType == typeof(string))
            {
                dbType = "VARCHAR(255)";
            }
            else if (returnType == typeof(bool))
            {
                dbType = "TINYINT";
            }
            else if (returnType == typeof(short))
            {
                dbType = "SMALLINT";
            }
            else if (returnType == typeof(int))
            {
                dbType = "INT";
            }
            else if (returnType == typeof(long))
            {
                dbType = "BIGINT";
            }
            else if (returnType == typeof(decimal))
            {
                dbType = "DECIMAL";
            }
            else if (returnType == typeof(double))
            {
                dbType = "DECIMAL";
            }
            else if (returnType == typeof(byte[]))
            {
                dbType = "LONGBLOB";
            }
            else if (returnType == typeof(DateTime))
            {
                dbType = "DATETIME";
            }
            else if (returnType == typeof(TimeSpan))
            {
                dbType = "TIMESTAMP";
            }
            else if (returnType == typeof(Guid))
            {
                dbType = "CHAR(36)";
            }
            else if (returnType == typeof(char))
            {
                dbType = "CHAR";
            }
            else
                throw new NotImplementedException();

            return dbType;
        }
    }
}
