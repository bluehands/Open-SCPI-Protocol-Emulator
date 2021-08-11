using System;
using System.Threading.Tasks;

namespace Domain.UnionTypes
{
    public abstract class Range
    {
        public static Range Number(double value) => new Number_(value);

        public static readonly Range Auto = new Auto_();
        public static readonly Range Min = new Min_();
        public static readonly Range Max = new Max_();
        public static readonly Range Def = new Def_();

        public class Number_ : Range
        {
            public double Value { get; }
            public Number_(double value) : base(UnionCases.Number)
            {
                Value = value;
            }
        }

        public class Auto_ : Range
        {
            public Auto_() : base(UnionCases.Auto)
            {
            }
        }

        public class Min_ : Range
        {
            public Min_() : base(UnionCases.Min)
            {
            }
        }

        public class Max_ : Range
        {
            public Max_() : base(UnionCases.Max)
            {
            }
        }

        public class Def_ : Range
        {
            public Def_() : base(UnionCases.Def)
            {
            }
        }

        internal enum UnionCases
        {
            Number,
            Auto,
            Min,
            Max,
            Def
        }

        internal UnionCases UnionCase { get; }
        Range(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(Range other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Range)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class RangeExtension
    {
        public static T Match<T>(this Range range, Func<Range.Number_, T> number, Func<Range.Auto_, T> auto, Func<Range.Min_, T> min, Func<Range.Max_, T> max, Func<Range.Def_, T> def)
        {
            switch (range.UnionCase)
            {
                case Range.UnionCases.Number:
                    return number((Range.Number_)range);
                case Range.UnionCases.Auto:
                    return auto((Range.Auto_)range);
                case Range.UnionCases.Min:
                    return min((Range.Min_)range);
                case Range.UnionCases.Max:
                    return max((Range.Max_)range);
                case Range.UnionCases.Def:
                    return def((Range.Def_)range);
                default:
                    throw new ArgumentException($"Unknown type derived from Range: {range.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Range range, Func<Range.Number_, Task<T>> number, Func<Range.Auto_, Task<T>> auto, Func<Range.Min_, Task<T>> min, Func<Range.Max_, Task<T>> max, Func<Range.Def_, Task<T>> def)
        {
            switch (range.UnionCase)
            {
                case Range.UnionCases.Number:
                    return await number((Range.Number_)range).ConfigureAwait(false);
                case Range.UnionCases.Auto:
                    return await auto((Range.Auto_)range).ConfigureAwait(false);
                case Range.UnionCases.Min:
                    return await min((Range.Min_)range).ConfigureAwait(false);
                case Range.UnionCases.Max:
                    return await max((Range.Max_)range).ConfigureAwait(false);
                case Range.UnionCases.Def:
                    return await def((Range.Def_)range).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from Range: {range.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<Range> range, Func<Range.Number_, T> number, Func<Range.Auto_, T> auto, Func<Range.Min_, T> min, Func<Range.Max_, T> max, Func<Range.Def_, T> def) => (await range.ConfigureAwait(false)).Match(number, auto, min, max, def);
        public static async Task<T> Match<T>(this Task<Range> range, Func<Range.Number_, Task<T>> number, Func<Range.Auto_, Task<T>> auto, Func<Range.Min_, Task<T>> min, Func<Range.Max_, Task<T>> max, Func<Range.Def_, Task<T>> def) => await(await range.ConfigureAwait(false)).Match(number, auto, min, max, def).ConfigureAwait(false);
    }
}