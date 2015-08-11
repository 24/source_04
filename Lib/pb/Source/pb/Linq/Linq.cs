using System;
using System.Linq;
using System.Collections.Generic;

namespace pb.Linq
{
    public enum JoinType
    {
        InnerJoin = 1,                            // result with only intersection of outer and inner
        LeftOuterJoin,                            // result with all outer and intersection between outer and inner
        RightOuterJoin,                           // result with all inner and intersection between outer and inner
        FullOuterJoin,                            // result with all outer and inner
        LeftOuterJoinWithoutInner,                // result with only outer without intersection between outer and inner
        RightOuterJoinWithoutInner,               // result with only inner without intersection between outer and inner
        FullOuterJoinWithoutInner                 // result with only outer and inner without intersection between outer and inner
    }

    public static partial class GlobalExtension
    {
        public static IEnumerable<TResult> zJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, JoinType joinType = JoinType.InnerJoin,
            TOuter outerDefaultValue = default(TOuter), TInner innerDefaultValue = default(TInner), Func<TOuter, bool> outerIsEmpty = null, Func<TInner, bool> innerIsEmpty = null)
        {
            switch (joinType)
            {
                case JoinType.InnerJoin:
                    // from outerValue in outer
                    // join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue)
                    // select resultSelector(outerValue, innerValue);
                    return outer.zGetJoinQuery(inner, outerKeySelector, innerKeySelector, resultSelector);
                case JoinType.LeftOuterJoin:
                    // from outerValue in outer
                    // join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue) into innerGroup
                    // from innerValue2 in innerGroup.DefaultIfEmpty(innerDefaultValue)
                    // select resultSelector(outerValue, innerValue2);
                    return outer.zGetGroupJoinQuery(inner, outerKeySelector, innerKeySelector, resultSelector, innerDefaultValue);
                case JoinType.RightOuterJoin:
                    // from innerValue in inner
                    // join outerValue in outer on innerKeySelector(innerValue) equals outerKeySelector(outerValue) into outerGroup
                    // from outerValue2 in outerGroup.DefaultIfEmpty(outerDefaultValue)
                    // select resultSelector(innerValue, outerValue2);
                    return inner.zGetGroupJoinQuery(outer, innerKeySelector, outerKeySelector, (innerValue, outerValue) => resultSelector(outerValue, innerValue), outerDefaultValue);
                case JoinType.FullOuterJoin:
                    // LeftOuterJoin concat RightOuterJoinWithoutInner
                    // from outerValue in outer
                    // join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue) into innerGroup
                    // from innerValue2 in innerGroup.DefaultIfEmpty(innerDefaultValue)
                    // select resultSelector(outerValue, innerValue2);
                    // concat
                    // from innerValue in inner
                    // join outerValue in outer on innerKeySelector(innerValue) equals outerKeySelector(outerValue) into outerGroup
                    // from outerValue2 in outerGroup.DefaultIfEmpty(outerDefaultValue)
                    // where outerIsEmpty(outerValue2)
                    // select resultSelector(innerValue, outerValue2);
                    IEnumerable<TResult> queryLeft1 = outer.zGetGroupJoinQuery(inner, outerKeySelector, innerKeySelector, resultSelector, innerDefaultValue);
                    IEnumerable<TResult> queryRight1 = inner.zGetGroupJoinQuery(outer, innerKeySelector, outerKeySelector, (innerValue, outerValue) => resultSelector(outerValue, innerValue), outerDefaultValue,
                        outerIsEmpty ?? GetDefaultIsEmpty(outerDefaultValue));
                    return queryLeft1.Concat(queryRight1);
                case JoinType.LeftOuterJoinWithoutInner:
                    // from outerValue in outer
                    // join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue) into innerGroup
                    // from innerValue2 in innerGroup.DefaultIfEmpty(innerDefaultValue)
                    // where innerIsEmpty(innerValue2)
                    // select resultSelector(outerValue, innerValue2);
                    return outer.zGetGroupJoinQuery(inner, outerKeySelector, innerKeySelector, resultSelector, innerDefaultValue,
                        innerIsEmpty ?? GetDefaultIsEmpty(innerDefaultValue));
                case JoinType.RightOuterJoinWithoutInner:
                    // from innerValue in inner
                    // join outerValue in outer on innerKeySelector(innerValue) equals outerKeySelector(outerValue) into outerGroup
                    // from outerValue2 in outerGroup.DefaultIfEmpty(outerDefaultValue)
                    // where outerIsEmpty(outerValue2)
                    // select resultSelector(innerValue, outerValue2);
                    return inner.zGetGroupJoinQuery(outer, innerKeySelector, outerKeySelector, (innerValue, outerValue) => resultSelector(outerValue, innerValue), outerDefaultValue,
                        outerIsEmpty ?? GetDefaultIsEmpty(outerDefaultValue));
                case JoinType.FullOuterJoinWithoutInner:
                    // LeftOuterJoinWithoutInner concat RightOuterJoinWithoutInner
                    // from outerValue in outer
                    // join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue) into innerGroup
                    // from innerValue2 in innerGroup.DefaultIfEmpty(innerDefaultValue)
                    // where innerIsEmpty(innerValue2)
                    // select resultSelector(outerValue, innerValue2);
                    // concat
                    // from innerValue in inner
                    // join outerValue in outer on innerKeySelector(innerValue) equals outerKeySelector(outerValue) into outerGroup
                    // from outerValue2 in outerGroup.DefaultIfEmpty(outerDefaultValue)
                    // where outerIsEmpty(outerValue2)
                    // select resultSelector(innerValue, outerValue2);
                    IEnumerable<TResult> queryLeft2 = outer.zGetGroupJoinQuery(inner, outerKeySelector, innerKeySelector, resultSelector, innerDefaultValue,
                        innerIsEmpty ?? GetDefaultIsEmpty(innerDefaultValue));
                    IEnumerable<TResult> queryRight2 = inner.zGetGroupJoinQuery(outer, innerKeySelector, outerKeySelector, (innerValue, outerValue) => resultSelector(outerValue, innerValue), outerDefaultValue,
                        outerIsEmpty ?? GetDefaultIsEmpty(outerDefaultValue));
                    return queryLeft2.Concat(queryRight2);
                default:
                    throw new PBException("unknow JoinType {0}", joinType);
            }
        }

        private static IEnumerable<TResult> zGetJoinQuery<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return
                from outerValue in outer
                join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue)
                select resultSelector(outerValue, innerValue);
        }

        private static IEnumerable<TResult> zGetGroupJoinQuery<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, TInner innerDefaultValue = default(TInner),
            Func<TInner, bool> innerIsEmpty = null)
        {
            if (innerIsEmpty != null)
                return
                    from outerValue in outer
                    join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue) into innerGroup
                    from innerValue2 in innerGroup.DefaultIfEmpty(innerDefaultValue)
                    where innerIsEmpty(innerValue2)
                    select resultSelector(outerValue, innerValue2);
            else
                return
                    from outerValue in outer
                    join innerValue in inner on outerKeySelector(outerValue) equals innerKeySelector(innerValue) into innerGroup
                    from innerValue2 in innerGroup.DefaultIfEmpty(innerDefaultValue)
                    select resultSelector(outerValue, innerValue2);
        }

        private static Func<T, bool> GetDefaultIsEmpty<T>(T defaultValue)
        {
            return value => EqualityComparer<T>.Default.Equals(value, defaultValue);
        }
    }
}
