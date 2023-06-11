using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Underground.ORM.Core.Translator.Extension;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Urderground.ORM.Core.Translator
{
    public class ActionVar
    {
        public Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, Vars, string, MySqlSyntax> Act { get; set; }
    }


    public class Vars 
    {
        public Dictionary<Type, object?> Variables { get; set; } = new();

        public void AddVariable<T>(T value)
        {
            Variables[typeof(T)] = value;
        }

        public T GetVariable<T>(T iniatize) 
        {
            var value = Variables.GetValueOrDefault(typeof(T))!;

            if (value is null)
            {
                AddVariable(iniatize);
                value = iniatize;
            }

            return (T)value!;
        }
    }

    public partial class MySqlTranslator2
    {
        public readonly Dictionary<SyntaxToken, ActionVar> TokenTranslation = new();
        public readonly Dictionary<Type, ActionVar> KindTranslation = new();
        public readonly Dictionary<Type, ActionVar> ParentTranslation = new();
        public readonly Dictionary<Type, ActionVar> AscendantTranslation = new();

        internal void ClearAll()
        {
            AscendantTranslation.Clear();
            TokenTranslation.Clear();
            KindTranslation.Clear();
            ParentTranslation.Clear();
        }

        public void AddSyntaxTranslationByToken(SyntaxToken syntaxToken,
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, Vars, string, MySqlSyntax> action)
        {
            TokenTranslation.Add(syntaxToken, new ActionVar() { Act = action });
        }

        public void AddSyntaxTranslationByKind<TSyntaxKind>(
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, Vars, string, MySqlSyntax> action) where TSyntaxKind : Enum
        {
            KindTranslation.Add(typeof(TSyntaxKind), new ActionVar() { Act = action });
        }

        public void AddSyntaxTranslationByParent<TSyntaxNode>(
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, Vars, string, MySqlSyntax> action) where TSyntaxNode : CSharpSyntaxNode
        {
            ParentTranslation.Add(typeof(TSyntaxNode), new ActionVar() { Act = action });
        }

        public void AddSyntaxTranslationByAscendant<TSyntaxNode>(
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, Vars, string, MySqlSyntax> action) where TSyntaxNode : CSharpSyntaxNode
        {
            AscendantTranslation.Add(typeof(TSyntaxNode), new ActionVar() { Act = action });
        }

        public void RaiseTranslationByAscendant<TSyntaxNode>(SyntaxToken token, SyntaxKind kind, CSharpSyntaxNode? parent, CSharpSyntaxNode? ascendant, Vars vars, string span, MySqlSyntax mysql)
        {
            AscendantTranslation[typeof(TSyntaxNode)].Act(token, kind, parent, ascendant, vars, span, mysql);
        }

        private void TranslateStatement(string csFileContent,
                                        IEnumerable<SyntaxToken> csharpTokens,
                                        string fullSpan,
                                        MySqlSyntax mysqlSyntaxOut)
        {
            Vars vars = new();

            foreach (SyntaxToken token in csharpTokens)
            {
                var kind = token.Kind();
                var parent = token.Parent;
                var ascendants = token.GetAscendants();

                foreach (SyntaxNode ascendant in ascendants)
                {
                    if (AscendantTranslation.TryGetValue(ascendant.GetType(),
                                                         out ActionVar? action))
                    {
                        var span = csFileContent[ascendant.Span.Start..ascendant.Span.End];

                        action.Act(token,
                                   kind,
                                   (CSharpSyntaxNode?)parent,
                                   (CSharpSyntaxNode?)ascendant,
                                   vars,
                                   span,
                                   mysqlSyntaxOut);
                    }
                }

                if (TokenTranslation.TryGetValue(token,
                                                 out ActionVar? actionToken))
                {
                    var span = csFileContent[token.Span.Start..token.Span.End];

                    actionToken.Act(token,
                                    kind,
                                    (CSharpSyntaxNode?)parent,
                                    (CSharpSyntaxNode?)null,
                                    vars,
                                    span,
                                    mysqlSyntaxOut);
                }
            }
        }
    }
}
