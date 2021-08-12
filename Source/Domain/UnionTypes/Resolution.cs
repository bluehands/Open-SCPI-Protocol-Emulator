using System;
using System.Threading.Tasks;

namespace Domain.UnionTypes
{
    public abstract class Resolution
    {
        public static Resolution Number(double value) => new Number_(value);

        public static readonly Resolution Min = new Min_();
        public static readonly Resolution Max = new Max_();
        public static readonly Resolution Def = new Def_();

        public class Number_ : Resolution
        {
            public double Value { get; }
            public Number_(double value) : base(UnionCases.Number)
            {
                Value = value;
            }
        }

        public class Min_ : Resolution
        {
            public Min_() : base(UnionCases.Min)
            {
            }
        }

        public class Max_ : Resolution
        {
            public Max_() : base(UnionCases.Max)
            {
            }
        }

        public class Def_ : Resolution
        {
            public Def_() : base(UnionCases.Def)
            {
            }
        }

        internal enum UnionCases
        {
            Number,
            Min,
            Max,
            Def,
        }

        internal UnionCases UnionCase { get; }
        Resolution(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(Resolution other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Resolution)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class ResolutionExtension
    {
        public static T Match<T>(this Resolution resolution, Func<Resolution.Number_, T> number, Func<Resolution.Min_, T> min, Func<Resolution.Max_, T> max, Func<Resolution.Def_, T> def)
        {
            switch (resolution.UnionCase)
            {
                case Resolution.UnionCases.Number:
                    return number((Resolution.Number_)resolution);
                case Resolution.UnionCases.Min:
                    return min((Resolution.Min_)resolution);
                case Resolution.UnionCases.Max:
                    return max((Resolution.Max_)resolution);
                case Resolution.UnionCases.Def:
                    return def((Resolution.Def_)resolution);
                default:
                    throw new ArgumentException($"Unknown type derived from Resolution: {resolution.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Resolution resolution, Func<Resolution.Number_, Task<T>> number, Func<Resolution.Min_, Task<T>> min, Func<Resolution.Max_, Task<T>> max, Func<Resolution.Def_, Task<T>> def)
        {
            switch (resolution.UnionCase)
            {
                case Resolution.UnionCases.Number:
                    return await number((Resolution.Number_)resolution).ConfigureAwait(false);
                case Resolution.UnionCases.Min:
                    return await min((Resolution.Min_)resolution).ConfigureAwait(false);
                case Resolution.UnionCases.Max:
                    return await max((Resolution.Max_)resolution).ConfigureAwait(false);
                case Resolution.UnionCases.Def:
                    return await def((Resolution.Def_)resolution).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from Resolution: {resolution.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<Resolution> resolution, Func<Resolution.Number_, T> number, Func<Resolution.Min_, T> min, Func<Resolution.Max_, T> max, Func<Resolution.Def_, T> def) => (await resolution.ConfigureAwait(false)).Match(number, min, max, def);
        public static async Task<T> Match<T>(this Task<Resolution> resolution, Func<Resolution.Number_, Task<T>> number, Func<Resolution.Min_, Task<T>> min, Func<Resolution.Max_, Task<T>> max, Func<Resolution.Def_, Task<T>> def) => await(await resolution.ConfigureAwait(false)).Match(number, min, max, def).ConfigureAwait(false);
    }
}