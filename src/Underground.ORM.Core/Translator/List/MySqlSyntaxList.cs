using System.Collections;
using Urderground.ORM.Core.Translator.Pretty;

namespace Urderground.ORM.Core.Translator.List
{
    public class MySqlSyntaxList : IList<MySqlSyntaxItem>, ICloneable
    {
        readonly MySqlPretty _pretty = new();

        private readonly bool _isReadOnly;

        private List<MySqlSyntaxItem> _list = new ();
        private int _syntaxLineNumbers = 0;

        public int SyntaxLineNumbers => _syntaxLineNumbers;

        public MySqlSyntaxItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _isReadOnly;

        public override string ToString()
        {
            return string.Join("", _list.Select(x => x.Token + $"{(x.EndLine || x.RightSpace ? " ": "")}"));
        }

        public MySqlSyntaxList() : this(50)
        {
        }

        public MySqlSyntaxList(int capacity)
        {
            _list = new(capacity);
        }

        public MySqlSyntaxList(params string[] items)
        {
            Append(items);
        }

        public string ToMySqlString(bool pretty)
        {
            int leftSpacesCount = 0;

            return string.Join("", _list.Select((item, i) =>
            {
                var prevItem = i == 0 ? null : _list[i - 1];
                var nextItem = i == _list.Count -1 ? null : _list[i + 1];

                string token = item.Token;

                if (pretty)
                {
                    var addTabBeforeReturn = _pretty.AddTabBeforeReturn.FirstOrDefault(x => x == token);
                    var addTabAfterReturn = _pretty.AddTabAfterReturn.FirstOrDefault(x => x == token);
                    var remTabBeforeReturn = _pretty.RemTabBeforeReturn.FirstOrDefault(x => x == token);
                    var remTabAfterReturn = _pretty.RemTabAfterReturn.FirstOrDefault(x => x == token);

                    if (addTabBeforeReturn != null) leftSpacesCount += 4;
                    if (remTabBeforeReturn != null) leftSpacesCount -= 4;

                    if (item.StartLine)
                    {
                        token = token.Insert(0, string.Concat(Enumerable.Repeat(' ', leftSpacesCount)));
                    }

                    if (addTabAfterReturn != null) leftSpacesCount += 4;
                    if (remTabAfterReturn != null) leftSpacesCount -= 4;
                }

                if (item.RightSpace && (nextItem?.Token ?? "") != ";") token += " ";
                if (item.EndLine) token += "\n";

                return token;
            }));
        }

        public void Add(MySqlSyntaxItem item)
        {
            InternalAdd(item);
        }

        public void Append(MySqlSyntaxItem item)
        {
            InternalAdd(item);
        }

        public void Append(params string[] tokens)
        {
            InternalAppend(tokens, false);
        }

        public void AppendLine(params string[] tokens)
        {
            InternalAppend(tokens, true);
        }

        private void InternalAppend(string[] tokens, bool newLine)
        {
            if (tokens.Length == 0)
            {
                tokens = new[] { "" };
            }

            for (int i = 0; i < tokens.Length; i++)
            {
                bool lastItem = newLine && i == tokens.Length - 1;
                InternalAdd(new MySqlSyntaxItem(tokens[i], lastItem));
            }
        }

        private void InternalAdd(MySqlSyntaxItem item)
        {
            item.SetLineNumber(_syntaxLineNumbers + 1);
            item.SetStartLine(_list.Count == 0 || _list[^1].EndLine);

            _list.Add(item);

            if (item.EndLine) Interlocked.Increment(ref _syntaxLineNumbers);
        }

        public void AppendRange(MySqlSyntaxList list)
        {
            foreach (var item in list)
                InternalAdd(item);
        }

        public void AppendRange(IEnumerable<MySqlSyntaxList> lists)
        {
            foreach (var item in lists.SelectMany(x => x))
                InternalAdd(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(MySqlSyntaxItem item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(MySqlSyntaxItem[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<MySqlSyntaxItem> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(MySqlSyntaxItem item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, MySqlSyntaxItem item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(MySqlSyntaxItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void RemoveLast()
        {
            _list.RemoveAt(_list.Count - 1);
            _syntaxLineNumbers = _list[^1].LineNumber;
        }

        public object Clone()
        {
            MySqlSyntaxList listClone = (MySqlSyntaxList)MemberwiseClone();
            listClone._list = _list.Select(x => (MySqlSyntaxItem)x.Clone()).ToList();
            return listClone;
        }

        public static implicit operator MySqlSyntaxList(MySqlSyntaxItem item)
        {
            return new MySqlSyntaxList() { item };
        }

        public static implicit operator MySqlSyntaxList(string token)
        {
            return new(token);
        }
    }
}
