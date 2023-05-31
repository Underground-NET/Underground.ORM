using System.Collections;

namespace Urderground.ORM.Core.Translator.List
{
    public class MySqlSyntaxList : IList<MySqlSyntaxItem>
    {
        private readonly bool _isReadOnly;

        private readonly List<MySqlSyntaxItem> _list;
        private int _syntaxLineNumbers = 0;

        public int SyntaxLineNumbers => _syntaxLineNumbers;

        public MySqlSyntaxItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _isReadOnly;

        public MySqlSyntaxList() : this(50)
        {
        }

        public MySqlSyntaxList(int capacity)
        {
            _list = new(capacity);
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
            _list.Add(item);

            if (item.NewLine) Interlocked.Increment(ref _syntaxLineNumbers);
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
    }
}
