using Microsoft.CodeAnalysis;

namespace Underground.ORM.Core.Translator.Extension
{
    public static class TranslatorExtension
    {
        public static T? GetAscendantType<T>(this SyntaxToken token) where T : SyntaxNode
        {
            var parent = token.Parent;

            while (parent != null)
            {
                if (parent == null) break;
                if (parent is T) break;

                parent = parent?.Parent;
            }

            return (T?)parent;
        }

        public static SyntaxNode? GetAscendantType(this SyntaxToken token, 
                                                   Type nodeType)
        {
            var parent = token.Parent;

            while (parent != null)
            {
                if (parent == null) break;
                if (parent.GetType() == nodeType) break;
                
                parent = parent?.Parent;
            }

            return parent;
        }

        public static IEnumerable<SyntaxNode> GetAscendants(this SyntaxToken token)
        {
            var parent = token.Parent;

            while (parent != null)
            {
                yield return parent;

                parent = parent?.Parent;

                if (parent == null) break;
            }
        }
    }
}
