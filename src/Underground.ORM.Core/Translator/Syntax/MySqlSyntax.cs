using System.Collections;
using System.Linq.Expressions;
using Underground.ORM.Core.Translator.Pretty;
using Underground.ORM.Core.Translator.Syntax.Variable;

namespace Underground.ORM.Core.Translator.Syntax
{
    public class MySqlSyntax : IList<MySqlSyntaxToken>, ICloneable
    {
        readonly MySqlPretty _pretty = new();

        private readonly bool _isReadOnly;

        private List<MySqlSyntaxToken> _list = new();
        private int _syntaxLineNumbers = 0;

        public int SyntaxLineNumbers => _syntaxLineNumbers;

        public MySqlSyntaxToken this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _isReadOnly;

        public override string ToString()
        {
            return string.Join("", _list.Select(x => x.Token + $"{(x.EndLine || x.RightSpace ? " " : "")}"));
        }

        public MySqlSyntax() : this(50)
        {
        }

        public MySqlSyntax(int capacity)
        {
            _list = new(capacity);
        }

        public MySqlSyntax(params string[] items)
        {
            Append(items);
        }

        public MySqlSyntax(params MySqlSyntaxToken[] items)
        {
            AppendRange(items);
        }

        public string ToMySqlString(bool pretty)
        {
            int leftSpacesCount = 0;

            return string.Join("", _list.Select((item, i) =>
            {
                var prevItem = i == 0 ? null : _list[i - 1];
                var nextItem = i == _list.Count - 1 ? null : _list[i + 1];

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

        public void Add(MySqlSyntaxToken item)
        {
            InternalAdd(item, -1, false);
        }

        public void Append(MySqlSyntaxToken item)
        {
            InternalAdd(item, -1, false);
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
                InternalAdd(new MySqlSyntaxToken(tokens[i], lastItem), -1, false);
            }
        }

        private void InternalAdd(MySqlSyntaxToken item, int indexAt, bool insert)
        {
            if (indexAt == _list.Count)
            {
                indexAt = -1;
                insert = false;
            }

            bool add = false;
            if (indexAt == -1)
            {
                add = true;
                indexAt = _list.Count;
            }
            else
                add = false;

            int indexPrev = indexAt - 1;
            int indexNext = indexAt + 1;

            if(add)
                item.SetLineNumber(_syntaxLineNumbers + 1);
            else
                item.SetLineNumber(_list[indexAt].LineNumber);

            int lineNumber = item.LineNumber;

            if (indexAt == 0)
            {
                item.SetStartLine(true);
            }
            else
                item.SetStartLine(_list.Count == 0 || _list[indexPrev].EndLine);

            #region Reference

            if (item is MySqlSyntaxVariableReferenceToken)
            {
                var found = _list.OfType<MySqlSyntaxVariableToken>()
                    .Reverse().FirstOrDefault(x => x.IsVar && x.Token == item.Token);

                if (found is not null)
                {
                    item.Reference = found;
                }
            }

            #endregion

            #region Adjust Previous Item

            if (indexPrev > -1) _list[indexPrev].Next = item;

            #endregion

            #region Adjust Current Item

            item.Previous = indexPrev > -1 ? _list[indexPrev] : null;
            item.Next = _list.Count - 1 >= indexNext ? _list[indexNext] : null;

            #endregion

            #region Adjust Next Item

            if (_list.Count - 1 >= indexNext) _list[indexNext].Previous = item;

            #endregion

            #region ElevatorLevel

            if (item == "(")
            {
                item.ElevatorLevel = item?.Previous?.ElevatorLevel + 1 ?? 0;
            }
            else if (
                item.Previous is not null &&
                item.Previous == ")")
            {
                item.ElevatorLevel = item?.Previous?.ElevatorLevel - 1 ?? 0;
            }
            else
            {
                item.ElevatorLevel = item?.Previous?.ElevatorLevel ?? 0;
            }
            #endregion

            if (insert)
            {
                _list.Insert(indexAt, item!);
            }
            else
            {
                if (add)
                    _list.Add(item!);
                else
                    _list[indexAt] = item!;
            }

            if (item!.EndLine) Interlocked.Increment(ref _syntaxLineNumbers);
        }

        public void AppendRange(MySqlSyntaxToken[] list)
        {
            foreach (var item in list)
                InternalAdd(item, -1, false);
        }

        public void AppendRange(MySqlSyntax list)
        {
            foreach (var item in list)
                InternalAdd(item, -1, false);
        }

        public void AppendRange(IEnumerable<MySqlSyntax> lists)
        {
            foreach (var item in lists.SelectMany(x => x))
                InternalAdd(item, -1, false);
        }

        public void Clear()
        {
            _list.Clear();
            UpdateSyntaxLineNumbers();
        }

        void UpdateSyntaxLineNumbers()
        {
            _syntaxLineNumbers = _list.Count > 0 ? _list[^1].LineNumber : 0;
        }

        public bool Contains(MySqlSyntaxToken item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(MySqlSyntaxToken[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<MySqlSyntaxToken> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(MySqlSyntaxToken item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, MySqlSyntaxToken item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(MySqlSyntaxToken item)
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
            UpdateSyntaxLineNumbers();
        }

        public object Clone()
        {
            MySqlSyntax listClone = (MySqlSyntax)MemberwiseClone();
            listClone._list = _list.Select(x => (MySqlSyntaxToken)x.Clone()).ToList();
            return listClone;
        }

        internal void UpdateReferences(MySqlSyntax mysqlSyntax)
        {
            var variableDeclared = mysqlSyntax.OfType<MySqlSyntaxVariableToken>().Reverse();

            foreach (var item in _list.OfType<MySqlSyntaxVariableReferenceToken>())
            {
                if (item is MySqlSyntaxVariableReferenceToken)
                {
                    var found = variableDeclared.FirstOrDefault(x => x.IsVar && x.Token == item.Token);

                    if (found is not null)
                    {
                        item.Reference = found;
                    }
                }
            }
        }

        internal void ReplaceAt(int i, MySqlSyntaxToken token)
        {
            InternalAdd(token, i, false);
        }

        internal void AppendAt(int i, MySqlSyntaxToken token)
        {
            InternalAdd(token, i, true);
        }

        internal List<int> GetLevels()
        {
            return _list.Select(x => x.ElevatorLevel).Distinct().Reverse().ToList();
        }

        internal IEnumerable<List<MySqlSyntaxToken>> GetGroupsItemsFromLevel(int level)
        {
            var items = _list.Where(x => x.ElevatorLevel == level).ToList();

            int pos = 0;
            int idxOpen = -1;
            int idxClose = -1;

            do
            {
                idxOpen = items.FindIndex(pos, x => x.Token == "(");
                idxClose = items.FindIndex(pos, x => x.Token == ")");

                if (idxOpen > -1 && idxClose > -1)
                {
                    yield return items.Skip(idxOpen).Take(idxClose + 1).ToList();
                    pos = idxClose + 1;
                }

            } while (idxClose > -1);

            if (pos == 0) yield return items;
        }

        public static implicit operator MySqlSyntax(MySqlSyntaxToken item)
        {
            var list = new MySqlSyntax();
            list.Append(item);
            return list;
        }

        public static implicit operator MySqlSyntax(string token)
        {
            return new(token);
        }
    }
}
