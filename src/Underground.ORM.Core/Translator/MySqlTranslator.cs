using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using Urderground.ORM.Core.Attributes;
using Urderground.ORM.Core.Translator.CastExpression;
using Urderground.ORM.Core.Translator.List;
using Urderground.ORM.Core.Translator.Parameter;

namespace Urderground.ORM.Core.Translator
{
    public class MySqlTranslator
    {
        public MySqlSyntaxBuilt TranslateToFunctionCreateSyntax(MethodInfo method)
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

            MySqlSyntaxList mysqlSyntaxOut = new();

            var parametersInFunction = string.Join(", ", parametersIn.Select(x => $"`{x.Argument}` {x.DbType}"));

            mysqlSyntaxOut.AppendLine("CREATE ", "FUNCTION ", $"`{functionAttribute.RoutineName}`", "(", $"{parametersInFunction}", ")");
            mysqlSyntaxOut.AppendLine("RETURNS ", $"{mysqlReturnDbType.DbType}");
            mysqlSyntaxOut.AppendLine("BEGIN");

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

            mysqlSyntaxOut.Append("END", ";");

            return new MySqlSyntaxBuilt(method,
                                        functionAttribute.RoutineName,
                                        string.Join("", mysqlSyntaxOut.Select(x => x.Token + (x.NewLine ? "\n" : ""))), 
                                        mysqlSyntaxOut);
        }

        private void ConvertMethodBlockSyntax(BlockSyntax block,
                                              string csFileContent,
                                              MySqlSyntaxList mysqlSyntaxOut)
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
                        mysqlSyntaxOut.AppendLine("# " + line[2..].Trim());
                    }
                    else if (string.IsNullOrEmpty(line.Trim()))
                        mysqlSyntaxOut.AppendLine();
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
                    mysqlSyntaxOut.AppendLine("SET ");

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
                                                MySqlSyntaxList mysqlSyntaxOut)
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
                    mysqlSyntaxOut.Append("IF", "(");

                    var condition = ifStatementSyntax.Condition;
                    var conditionDescendants = condition.DescendantNodesAndTokensAndSelf().ToList();
                    var conditionTranslated = TranslateExpressionToMySql(csFileContent, conditionDescendants);

                    mysqlSyntaxOut.AppendRange(conditionTranslated);
                    mysqlSyntaxOut.AppendLine(")", "THEN");

                    var statementSyntax = ifStatementSyntax.Statement;

                    TranslateStatementToMySql(csFileContent,
                                              statementSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                              mysqlSyntaxOut);

                    var elseSyntax = ifStatementSyntax?.Else;

                    if (elseSyntax != null)
                    {
                        mysqlSyntaxOut.AppendLine("ELSE");

                        TranslateStatementToMySql(csFileContent,
                                                  elseSyntax.DescendantNodesAndTokensAndSelf().ToList(),
                                                  mysqlSyntaxOut);

                        mysqlSyntaxOut.AppendLine("END IF");

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

        private MySqlSyntaxList TranslateReturnsToMySql(string csFileContent,
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

                    var expTranslated = TranslateExpressionToMySql(csFileContent, descendantExpression);

                    mysqlStatement.AppendRange(expTranslated);

                    break;
                }
            }

            mysqlSyntaxOut.AppendRange(mysqlStatement);
            mysqlSyntaxOut.AppendLine(";");

            return mysqlStatement;
        }

        private void TranslateDeclarationToMySql(string csFileContent,
                                                 List<SyntaxNodeOrToken> descendants,
                                                 MySqlSyntaxList mysqlSyntaxOut)
        {
            Dictionary<int, MySqlSyntaxList> mysqlDeclare = new();
            (int Order, MySqlSyntaxList)? mysqlExpression = null;
            (int Order, MySqlSyntaxItem) mysqlDbType;

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

                    FinalizeDeclarationStatementMySql(mysqlDeclare,
                                                      mysqlExpression,
                                                      mysqlDbType,
                                                      mysqlSyntaxOut);

                    string variableName = variableDeclarator.Identifier.ValueText;

                    mysqlDeclare.Add(1, new() { "DECLARE " });
                    mysqlDeclare.Add(2, new() { $"`{variableName}`" });

                    variablesCount++;
                }

                if (equalsValueClauseSyntax != null)
                {
                    defaultValue = true;
                    mysqlDeclare.Add(4, new() { " DEFAULT " });
                }

                if (expressionSyntax != null && defaultValue)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();

                    var expTranslated = TranslateExpressionToMySql(csFileContent,
                                                                   descendantExpression);

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

        private void FinalizeDeclarationStatementMySql(Dictionary<int, MySqlSyntaxList> mysqlDeclare,
                                                      (int Order, MySqlSyntaxList)? mysqlExpression,
                                                      (int Order, MySqlSyntaxItem) mysqlDbType,
                                                      MySqlSyntaxList mysqlSyntaxOut)
        {
            if (!mysqlDeclare.Any()) return;

            mysqlDeclare.Add(mysqlDbType.Order, new() { mysqlDbType.Item2 });

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

        private MySqlSyntaxList TranslateExpressionToMySql(string csFileContent,
                                                           List<SyntaxNodeOrToken> descendants)
        {
            MySqlSyntaxList mysqlExpression = new();

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
                        mysqlExpression.Append($"`{token.ValueText}`");
                    }
                    else if (literalExpressionSyntax != null)
                    {
                        mysqlExpression.Append(token.Text);
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

                            mysqlExpression.Append(Function);
                            mysqlExpression.Append("(");
                        }
                    }
                    else if (binaryExpressionSyntax != null)
                    {

                        #region Close parenthese with mysql cast conversion

                        ElevatorCastExpression lastElevator;
                        if (elevatorCast.Any() && currentLevel == (lastElevator = elevatorCast.Last()).Level)
                        {
                            mysqlExpression.Append(BuildRightCastFunction(lastElevator));
                            elevatorCast.RemoveAt(elevatorCast.Count - 1);
                        }
                        #endregion

                        string operatorToken = binaryExpressionSyntax.OperatorToken.ValueText;

                        if (operatorToken == "==")
                        {
                            operatorToken = "=";
                        }

                        mysqlExpression.Append(operatorToken);
                    }
                    else if (parenthesizedExpressionSyntax != null)
                    {
                        string parentheseToken = token.Text;

                        mysqlExpression.Append(parentheseToken);

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
                        mysqlExpression.Append(token.Text);
                    }
                }
            }

            #region Close parentheses with mysql cast conversion
            elevatorCast.ForEach(lastElevator =>
            {
                mysqlExpression.Append(BuildRightCastFunction(lastElevator));
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
