using System;
using System.Threading.Tasks;

namespace Domain.UnionTypes
{
    public abstract class MeasurementType
    {
        public static readonly MeasurementType Voltage = new Voltage_();
        public static readonly MeasurementType Current = new Current_();

        public class Voltage_ : MeasurementType
        {
            public Voltage_() : base(UnionCases.Voltage)
            {
            }
        }

        public class Current_ : MeasurementType
        {
            public Current_() : base(UnionCases.Current)
            {
            }
        }

        internal enum UnionCases
        {
            Voltage,
            Current,
        }

        internal UnionCases UnionCase { get; }
        MeasurementType(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(MeasurementType other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MeasurementType)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class MeasurementTypeExtension
    {
        public static T Match<T>(this MeasurementType measurementType, Func<MeasurementType.Voltage_, T> voltage, Func<MeasurementType.Current_, T> current)
        {
            switch (measurementType.UnionCase)
            {
                case MeasurementType.UnionCases.Voltage:
                    return voltage((MeasurementType.Voltage_)measurementType);
                case MeasurementType.UnionCases.Current:
                    return current((MeasurementType.Current_)measurementType);
                default:
                    throw new ArgumentException($"Unknown type derived from MeasurementType: {measurementType.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this MeasurementType measurementType, Func<MeasurementType.Voltage_, Task<T>> voltage, Func<MeasurementType.Current_, Task<T>> current)
        {
            switch (measurementType.UnionCase)
            {
                case MeasurementType.UnionCases.Voltage:
                    return await voltage((MeasurementType.Voltage_)measurementType).ConfigureAwait(false);
                case MeasurementType.UnionCases.Current:
                    return await current((MeasurementType.Current_)measurementType).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from MeasurementType: {measurementType.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<MeasurementType> measurementType, Func<MeasurementType.Voltage_, T> voltage, Func<MeasurementType.Current_, T> current) => (await measurementType.ConfigureAwait(false)).Match(voltage, current);
        public static async Task<T> Match<T>(this Task<MeasurementType> measurementType, Func<MeasurementType.Voltage_, Task<T>> voltage, Func<MeasurementType.Current_, Task<T>> current) => await(await measurementType.ConfigureAwait(false)).Match(voltage, current).ConfigureAwait(false);
    }
}