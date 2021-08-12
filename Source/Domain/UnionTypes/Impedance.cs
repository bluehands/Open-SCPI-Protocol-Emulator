using System;
using System.Threading.Tasks;

namespace Domain.UnionTypes
{
    public abstract class Impedance
    {
        public static readonly Impedance High = new High_();
        public static readonly Impedance Low = new Low_();

        public class High_ : Impedance
        {
            public High_() : base(UnionCases.High)
            {
            }
        }

        public class Low_ : Impedance
        {
            public Low_() : base(UnionCases.Low)
            {
            }
        }

        internal enum UnionCases
        {
            High,
            Low
        }

        internal UnionCases UnionCase { get; }
        Impedance(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(Impedance other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Impedance)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class ImpedanceExtension
    {
        public static T Match<T>(this Impedance impedance, Func<Impedance.High_, T> high, Func<Impedance.Low_, T> low)
        {
            switch (impedance.UnionCase)
            {
                case Impedance.UnionCases.High:
                    return high((Impedance.High_)impedance);
                case Impedance.UnionCases.Low:
                    return low((Impedance.Low_)impedance);
                default:
                    throw new ArgumentException($"Unknown type derived from Impedance: {impedance.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Impedance impedance, Func<Impedance.High_, Task<T>> high, Func<Impedance.Low_, Task<T>> low)
        {
            switch (impedance.UnionCase)
            {
                case Impedance.UnionCases.High:
                    return await high((Impedance.High_)impedance).ConfigureAwait(false);
                case Impedance.UnionCases.Low:
                    return await low((Impedance.Low_)impedance).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from Impedance: {impedance.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<Impedance> impedance, Func<Impedance.High_, T> high, Func<Impedance.Low_, T> low) => (await impedance.ConfigureAwait(false)).Match(high, low);
        public static async Task<T> Match<T>(this Task<Impedance> impedance, Func<Impedance.High_, Task<T>> high, Func<Impedance.Low_, Task<T>> low) => await(await impedance.ConfigureAwait(false)).Match(high, low).ConfigureAwait(false);
    }
}