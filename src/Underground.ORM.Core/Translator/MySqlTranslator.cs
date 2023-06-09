﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System.Reflection;
using Underground.ORM.Core.Attributes;
using Underground.ORM.Core.Translator.Parameter;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Token.Block;
using Underground.ORM.Core.Translator.Syntax.Token.Declaration;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;
using Underground.ORM.Core.Translator.Syntax.Token.Statement;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        public MySqlSyntaxBuilt TranslateToFunctionCreateStatementSyntax(MethodInfo method)
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

            MySqlSyntax mysqlSyntaxOut = new();

            mysqlSyntaxOut.Append(
                new CreateStatementToken("CREATE "),
                new FunctionStatementToken("FUNCTION "),
                new NameFunctionStatementToken($"`{functionAttribute.RoutineName}`"),
                new OpenParenthesisToken("(")
                );

            foreach (var token in parametersIn)
            {
                var dbTypeToken = token.DbType.OfType<DbTypeToken>().First();

                mysqlSyntaxOut.Append(new VariableToken($"`{token.Argument}` ", (DbType)dbTypeToken.DbType!));
                mysqlSyntaxOut.AppendRange(token.DbType);
                mysqlSyntaxOut.Append(new CommaToken(", "));
            }
            mysqlSyntaxOut.RemoveLast();

            mysqlSyntaxOut.AppendLine(new CloseParenthesisToken(")"));
            mysqlSyntaxOut.AppendLine(new ReturnsStatementToken("RETURNS "), $"{mysqlReturnDbType.DbType}");
            mysqlSyntaxOut.AppendLine(new BeginBlockToken("BEGIN"));

            foreach (var ds in cds.Members)
            {
                if (ds is MethodDeclarationSyntax)
                {
                    var mds = (MethodDeclarationSyntax)ds;
                    var methodName = mds.Identifier.ValueText;

                    if (methodName == method.Name)
                    {
                        TranslateMethodBlockSyntax(mds.Body!, csFileContent, mysqlSyntaxOut);
                        break;
                    }
                }
            }

            mysqlSyntaxOut.Append(new EndBlockToken("END"), new SemicolonToken(";"));

            return new MySqlSyntaxBuilt(method,
                                        functionAttribute.RoutineName,
                                        mysqlSyntaxOut.ToMySqlString(true), 
                                        mysqlSyntaxOut);
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

        private MySqlSyntax GetDbType(Type returnType)
        {
            MySqlSyntax dbType;

            if (returnType.Name == "List`1")
            {
                dbType = new DbTypeToken(
                    "JSON", DbType.Object);
            }
            else if (returnType == typeof(string))
            {
                dbType = new(
                    new DbTypeToken("VARCHAR", DbType.String),
                    new OpenParenthesisToken("("), "255", new CloseParenthesisToken(")"));
            }
            else if (returnType == typeof(bool))
            {
                dbType = new DbTypeToken("BOOL", DbType.Boolean);
            }
            else if (returnType == typeof(short))
            {
                dbType = new DbTypeToken("SMALLINT", DbType.Int16);
            }
            else if (returnType == typeof(ushort))
            {
                dbType = new(
                    new DbTypeToken("SMALLINT ", DbType.UInt16),
                    new DbTypeToken("UNSIGNED", DbType.UInt16));
            }
            else if (returnType == typeof(int))
            {
                dbType = new DbTypeToken("INT", DbType.Int32);
            }
            else if (returnType == typeof(uint))
            {
                dbType = new(
                    new DbTypeToken("INT ", DbType.UInt32),
                    new DbTypeToken("UNSIGNED", DbType.UInt32));
            }
            else if (returnType == typeof(long))
            {
                dbType = new DbTypeToken("BIGINT", DbType.Int64);
            }
            else if (returnType == typeof(ulong))
            {
                dbType = new(
                    new DbTypeToken("BIGINT ", DbType.UInt64),
                    new DbTypeToken("UNSIGNED", DbType.UInt64));
            }
            else if (returnType == typeof(decimal))
            {
                dbType = new DbTypeToken("DECIMAL", DbType.Decimal);
            }
            else if (returnType == typeof(double))
            {
                dbType = new DbTypeToken("DOUBLE", DbType.Double);
            }
            else if (returnType == typeof(byte[]))
            {
                dbType = new DbTypeToken("LONGBLOB", DbType.Binary);
            }
            else if (returnType == typeof(DateTime))
            {
                dbType = new DbTypeToken("DATETIME", DbType.DateTime);
            }
            else if (returnType == typeof(TimeSpan))
            {
                dbType = new DbTypeToken("TIMESTAMP", DbType.Time);
            }
            else if (returnType == typeof(Guid))
            {
                dbType = new((DbTypeToken)
                    new("CHAR", DbType.StringFixedLength),
                    new OpenParenthesisToken("("), "36", new CloseParenthesisToken(")"));
            }
            else if (returnType == typeof(char))
            {
                dbType = new DbTypeToken("CHAR", DbType.StringFixedLength);
            }
            else
                throw new NotImplementedException();

            return dbType;
        }
    }
}
