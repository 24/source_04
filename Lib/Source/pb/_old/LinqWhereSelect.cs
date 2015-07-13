using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pb.Linq
{
    public static partial class GlobalExtension
    {
        public static IEnumerable<T2> zWhereSelect<T1, T2>(this IEnumerable<T1> source, Func<T1, T2> filter)
        {
            return new LinqWhereSelect<T1, T2>(source, filter);
        }
    }

    public class LinqWhereSelect<T1, T2> : IEnumerable<T2>, IEnumerator<T2>
    {
        private IEnumerable<T1> _source = null;
        private Func<T1, T2> _filter = null;
        private IEnumerator<T1> _enumerator = null;
        private T2 _current;

        public LinqWhereSelect(IEnumerable<T1> source, Func<T1, T2> filter)
        {
            _source = source;
            _filter = filter;
            _enumerator = _source.GetEnumerator();
            //Reset();
        }

        public IEnumerator<T2> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T2 Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _current; }
        }

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                _current = _filter(_enumerator.Current);
                if (_current != null)
                    return true;
            }
            return false;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }
    }
}
