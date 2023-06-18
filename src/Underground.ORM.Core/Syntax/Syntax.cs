using System.Collections;
using Underground.ORM.Core.Pretty;
using Underground.ORM.Core.Syntax.Token.Declaration;
using Underground.ORM.Core.Syntax.Token.Reference;

namespace Underground.ORM.Core.Syntax
{
    public class SyntaxBase : IList<SyntaxTokenBase>, ICloneable
    {
        private readonly SyntaxPretty _pretty = new();

        private readonly bool _isReadOnly;

        private List<SyntaxTokenBase> _list = new();
        private int _syntaxLineNumbers = 0;

        public int SyntaxLineNumbers => _syntaxLineNumbers;

        public SyntaxTokenBase this[int index]
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

        public SyntaxBase() : this(50)
        {
        }

        public SyntaxBase(int capacity)
        {
            _list = new(capacity);
        }

        public SyntaxBase(params SyntaxTokenBase[] items)
        {
            AppendRange(items);
        }

        public virtual string ToMySqlString(bool pretty)
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

        public void Add(SyntaxTokenBase item)
        {
            Append(item);
        }

        public void Append(SyntaxTokenBase item)
        {
            InternalAdd(item, -1, false);
        }

        public void Append(params SyntaxTokenBase[] tokens)
        {
            InternalAppend(tokens, false);
        }

        public void AppendLine(params SyntaxTokenBase[] tokens)
        {
            InternalAppend(tokens, true);
        }

        private void InternalAppend(SyntaxTokenBase[] tokens, bool newLine)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                bool lastItem = newLine && i == tokens.Length - 1;
                if (lastItem) tokens[i].SetEndLine();

                InternalAdd(tokens[i], -1, false);
            }
        }

        private void InternalAdd(SyntaxTokenBase item, int indexAt, bool insert)
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
            int indexNext = insert ? indexAt : indexAt + 1;

            if (add)
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

            if (item is VariableReferenceToken)
            {
                var found = _list.OfType<VariableToken>()
                    .Reverse().FirstOrDefault(x => x.IsVar && x.Token == item.Token);

                if (found is not null)
                {
                    item.Reference = found;
                    item.Is.String = found.IsString;
                }
            }

            #endregion

            #region Adjust Previous Item

            if (indexPrev > -1)
                _list[indexPrev].Next = item;

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
                item.SetElevatorLevel(item?.Previous?.ElevatorLevel + 1 ?? 1);
            }
            else if (
                item.Previous is not null &&
                item.Previous == ")")
            {
                item.SetElevatorLevel(item?.Previous?.ElevatorLevel - 1 ?? 0);
            }
            else
            {
                item.SetElevatorLevel(item?.Previous?.ElevatorLevel ?? 0);
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

        public void AppendRange(SyntaxTokenBase[] list)
        {
            foreach (var item in list)
                InternalAdd(item, -1, false);
        }

        public void AppendRange(List<SyntaxTokenBase> list)
        {
            foreach (var item in list)
                InternalAdd(item, -1, false);
        }

        public void AppendRange(SyntaxBase list)
        {
            foreach (var item in list)
                InternalAdd(item, -1, false);
        }

        public void AppendRange(IEnumerable<SyntaxBase> lists)
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

        public bool Contains(SyntaxTokenBase item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(SyntaxTokenBase[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SyntaxTokenBase> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(SyntaxTokenBase item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, SyntaxTokenBase item)
        {
            AppendAt(index, item);
        }

        public bool Remove(SyntaxTokenBase item)
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

        public virtual object Clone()
        {
            var listClone = (SyntaxBase)MemberwiseClone();
            listClone._list = _list.Select(x => (SyntaxTokenBase)x.Clone()).ToList();
            return listClone;
        }

        public virtual void UpdateReferences(SyntaxBase syntax)
        {
            var variableDeclared = syntax.OfType<VariableToken>().Reverse();

            foreach (var item in _list.OfType<VariableReferenceToken>())
            {
                if (item is VariableReferenceToken)
                {
                    var found = variableDeclared.FirstOrDefault(x => x.IsVar && x.Token == item.Token);

                    if (found is not null)
                    {
                        item.Reference = found;
                        item.Is.String = found.IsString;
                    }
                }
            }
        }

        public virtual void ReplaceAt(int i, SyntaxTokenBase token)
        {
            InternalAdd(token, i, false);
        }

        public virtual void AppendAt(int i, SyntaxTokenBase token)
        {
            InternalAdd(token, i, true);
        }

        public virtual List<int> GetLevels()
        {
            return _list.Select(x => x.ElevatorLevel).Distinct().Reverse().ToList();
        }

        public virtual IEnumerable<List<SyntaxTokenBase>> GetGroupsItemsFromLevel(int level)
        {
            var items = _list.Where(x => x.ElevatorLevel == level).ToList();

            int pos = 0;
            int idxOpen = -1;
            int idxClose = -1;

            do
            {
                idxOpen = items.FindIndex(pos, x => x.Is.OpenParenthesis);
                idxClose = items.FindIndex(pos, x => x.Is.CloseParenthesis);

                if (idxOpen > -1 && idxClose > -1)
                {
                    yield return items.Skip(idxOpen).Take(idxClose + 1).ToList();
                    pos = idxClose + 1;
                }

            } while (idxClose > -1);

            if (pos == 0) yield return items;
        }

        IEnumerator<SyntaxTokenBase> IEnumerable<SyntaxTokenBase>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator SyntaxBase(SyntaxTokenBase item)
        {
            var list = new SyntaxBase();
            list.Append(item);
            return list;
        }

        public static implicit operator SyntaxBase(string token)
        {
            return new SyntaxBase(new SyntaxTokenBase(token));
        }
    }
}
