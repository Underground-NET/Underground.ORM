using Microsoft.CodeAnalysis;
using System.Reflection;
using Underground.ORM.Core.Syntax;

namespace Underground.ORM.Core.Translator.Extension
{
    public static class TranslatorExtension
    {
        public static T? AsSyntax<T>(
            this SyntaxTokenBase? token) where T : SyntaxBase
        {
            var type = typeof(T);
            var instance = Activator.CreateInstance(type);

            var typeInst = instance!.GetType();
            var prop = typeInst.GetMethod("Append", new[] { typeof(SyntaxTokenBase) });
            prop!.Invoke(instance, new[] { token });

            return (T?)instance;
        }

        public static T? AsToken<T>(
            this SyntaxTokenBase? token) where T : SyntaxTokenBase
        {
            if (token is null) return default;

            var type = typeof(T);
            var baseType = token.GetType();
            var tokenProp = baseType.GetProperty(nameof(token.Token));
            var tokenValue = tokenProp?.GetValue(token, null);

            var instance = Activator.CreateInstance(type, tokenValue);

            FieldInfo[] fields = type.GetFields();
            foreach (var field in fields)
            {
                var value = field.GetValue(token);
                field.SetValue(instance, value);
            }

            PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(token, null);
                property.SetValue(instance, value, null);
            }

            return (T?)instance;
        }

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