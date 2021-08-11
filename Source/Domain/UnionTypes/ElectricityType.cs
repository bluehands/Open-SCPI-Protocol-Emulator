using System;
using System.Threading.Tasks;

namespace Domain.UnionTypes
{
    public abstract class ElectricityType
    {
        public static readonly ElectricityType AC = new AC_();
        public static readonly ElectricityType DC = new DC_();

        public class AC_ : ElectricityType
        {
            public AC_() : base(UnionCases.AC)
            {
            }
        }

        public class DC_ : ElectricityType
        {
            public DC_() : base(UnionCases.DC)
            {
            }
        }

        internal enum UnionCases
        {
            AC,
            DC,
        }

        internal UnionCases UnionCase { get; }
        ElectricityType(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(ElectricityType other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ElectricityType)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class ElectricityTypeExtension
    {
        public static T Match<T>(this ElectricityType electricityType, Func<ElectricityType.AC_, T> aC, Func<ElectricityType.DC_, T> dC)
        {
            switch (electricityType.UnionCase)
            {
                case ElectricityType.UnionCases.AC:
                    return aC((ElectricityType.AC_)electricityType);
                case ElectricityType.UnionCases.DC:
                    return dC((ElectricityType.DC_)electricityType);
                default:
                    throw new ArgumentException($"Unknown type derived from ElectricityType: {electricityType.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this ElectricityType electricityType, Func<ElectricityType.AC_, Task<T>> aC, Func<ElectricityType.DC_, Task<T>> dC)
        {
            switch (electricityType.UnionCase)
            {
                case ElectricityType.UnionCases.AC:
                    return await aC((ElectricityType.AC_)electricityType).ConfigureAwait(false);
                case ElectricityType.UnionCases.DC:
                    return await dC((ElectricityType.DC_)electricityType).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from ElectricityType: {electricityType.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<ElectricityType> electricityType, Func<ElectricityType.AC_, T> aC, Func<ElectricityType.DC_, T> dC) => (await electricityType.ConfigureAwait(false)).Match(aC, dC);
        public static async Task<T> Match<T>(this Task<ElectricityType> electricityType, Func<ElectricityType.AC_, Task<T>> aC, Func<ElectricityType.DC_, Task<T>> dC) => await(await electricityType.ConfigureAwait(false)).Match(aC, dC).ConfigureAwait(false);
    }
}