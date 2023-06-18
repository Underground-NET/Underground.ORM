using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Underground.ORM.Core.Syntax;
using Underground.ORM.Core.Translator.Extension;

namespace Underground.ORM.Core.Translator
{
    public class ActionVar
    {
        public Action<SyntaxToken,
                      SyntaxKind,
                      CSharpSyntaxNode?,
                      CSharpSyntaxNode?,
                      TranslatorHelper,
                      string,
                      SyntaxBase> Act
        { get; set; }
    }

    public class TranslatorHelper
    {
        private readonly SqlTranslator _translator;

        public SqlTranslator Translator => _translator;

        public TranslatorHelper(SqlTranslator translator)
        {
            _translator = translator;
        }

        public Dictionary<Type, object?> Variables { get; set; } = new();

        public void AddVariable<T>(T value)
        {
            Variables[typeof(T)] = value;
        }

        public T GetVariable<T>(T initialization) 
        {
            var value = Variables.GetValueOrDefault(typeof(T))!;

            if (value is null)
            {
                AddVariable(initialization);
                value = initialization;
            }

            return (T)value!;
        }
    }

    public partial class SqlTranslator
    {
        public readonly Dictionary<SyntaxToken, ActionVar> TokenTranslation = new();
        public readonly Dictionary<Type, ActionVar> KindTranslation = new();
        public readonly Dictionary<Type, ActionVar> ParentTranslation = new();
        public readonly Dictionary<Type, ActionVar> AscendantTranslation = new();

        public void ClearAllTranslations()
        {
            AscendantTranslation.Clear();
            TokenTranslation.Clear();
            KindTranslation.Clear();
            ParentTranslation.Clear();
        }

        public void AddSyntaxTranslationByToken(SyntaxToken syntaxToken,
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, TranslatorHelper, string, SyntaxBase> action)
        {
            TokenTranslation.Add(syntaxToken, new ActionVar() { Act = action });
        }

        public void AddSyntaxTranslationByKind<TSyntaxKind>(
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, TranslatorHelper, string, SyntaxBase> action) where TSyntaxKind : Enum
        {
            KindTranslation.Add(typeof(TSyntaxKind), new ActionVar() { Act = action });
        }

        public void AddSyntaxTranslationByParent<TSyntaxNode>(
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, TranslatorHelper, string, SyntaxBase> action) where TSyntaxNode : CSharpSyntaxNode
        {
            ParentTranslation.Add(typeof(TSyntaxNode), new ActionVar() { Act = action });
        }

        public void AddSyntaxTranslationByAscendant<TSyntaxNode>(
            Action<SyntaxToken, SyntaxKind, CSharpSyntaxNode?, CSharpSyntaxNode?, TranslatorHelper, string, SyntaxBase> action) where TSyntaxNode : CSharpSyntaxNode
        {
            AscendantTranslation.Add(typeof(TSyntaxNode), new ActionVar() { Act = action });
        }

        public void RaiseTranslationByAscendant<TSyntaxNode>(SyntaxToken token, SyntaxKind kind, CSharpSyntaxNode? parent, CSharpSyntaxNode? ascendant, TranslatorHelper vars, string span, SyntaxBase mysql)
        {
            AscendantTranslation[typeof(TSyntaxNode)].Act(token, kind, parent, ascendant, vars, span, mysql);
        }

        private void TranslateStatement(string csFileContent,
                                        IEnumerable<SyntaxToken> csharpTokens,
                                        string fullSpan,
                                        SyntaxBase mysqlSyntaxOut)
        {
            TranslatorHelper helper = new(this);

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
                                   helper,
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
                                    helper,
                                    span,
                                    mysqlSyntaxOut);
                }
            }
        }
    }
}
