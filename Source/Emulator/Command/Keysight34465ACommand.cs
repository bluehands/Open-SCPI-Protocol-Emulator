using System;
using System.Threading.Tasks;
using Domain.UnionTypes;
using FunicularSwitch;
using Range = Domain.UnionTypes.Range;

namespace Emulator.Command
{
    public abstract class Keysight34465ACommand : ICommand
    {
        public static readonly Keysight34465ACommand Identification = new Identification_();
        public static readonly Keysight34465ACommand Read = new Read_();
        public static readonly Keysight34465ACommand Abort = new Abort_();
        public static readonly Keysight34465ACommand Initiate = new Initiate_();
        public static readonly Keysight34465ACommand Fetch = new Fetch_();

        public static Keysight34465ACommand ConfigureCurrent(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) => new ConfigureCurrent_(electricityType, range, resolution);
        public static Keysight34465ACommand MeasureCurrent(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) => new MeasureCurrent_(electricityType, range, resolution);
        public static Keysight34465ACommand ConfigureVoltage(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) => new ConfigureVoltage_(electricityType, range, resolution);
        public static Keysight34465ACommand MeasureVoltage(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) => new MeasureVoltage_(electricityType, range, resolution);
        public static Keysight34465ACommand DisplayText(string text) => new DisplayText_(text);

        public static readonly Keysight34465ACommand ClearDisplay = new ClearDisplay_();

        public static Keysight34465ACommand SetImpedance(Impedance impedance) => new SetImpedance_(impedance);

        public class Identification_ : Keysight34465ACommand
        {
            public Identification_() : base(UnionCases.Identification)
            {
            }
        }

        public class Read_ : Keysight34465ACommand
        {
            public Read_() : base(UnionCases.Read)
            {
            }
        }

        public class Abort_ : Keysight34465ACommand
        {
            public Abort_() : base(UnionCases.Abort)
            {
            }
        }

        public class Initiate_ : Keysight34465ACommand
        {
            public Initiate_() : base(UnionCases.Initiate)
            {
            }
        }

        public class Fetch_ : Keysight34465ACommand
        {
            public Fetch_() : base(UnionCases.Fetch)
            {
            }
        }

        public class ConfigureCurrent_ : Keysight34465ACommand
        {
            public ElectricityType ElectricityType { get; }
            public Option<Range> Range { get; }
            public Option<Resolution> Resolution { get; }
            public ConfigureCurrent_(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) : base(UnionCases.ConfigureCurrent)
            {
                ElectricityType = electricityType;
                Range = range;
                Resolution = resolution;

            }
        }

        public class MeasureCurrent_ : Keysight34465ACommand
        {
            public ElectricityType ElectricityType { get; }
            public Option<Range> Range { get; }
            public Option<Resolution> Resolution { get; }
            public MeasureCurrent_(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) : base(UnionCases.MeasureCurrent)
            {
                ElectricityType = electricityType;
                Range = range;
                Resolution = resolution;
            }
        }

        public class ConfigureVoltage_ : Keysight34465ACommand
        {
            public ElectricityType ElectricityType { get; }
            public Option<Range> Range { get; }
            public Option<Resolution> Resolution { get; }
            public ConfigureVoltage_(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) : base(UnionCases.ConfigureVoltage)
            {
                ElectricityType = electricityType;
                Range = range;
                Resolution = resolution;
            }
        }

        public class MeasureVoltage_ : Keysight34465ACommand
        {
            public ElectricityType ElectricityType { get; }
            public Option<Range> Range { get; }
            public Option<Resolution> Resolution { get; }
            public MeasureVoltage_(ElectricityType electricityType, Option<Range> range, Option<Resolution> resolution) : base(UnionCases.MeasureVoltage)
            {
                ElectricityType = electricityType;
                Range = range;
                Resolution = resolution;
            }
        }

        public class DisplayText_ : Keysight34465ACommand
        {

            public string Text { get; }
            public DisplayText_(string text) : base(UnionCases.DisplayText)
            {
                Text = text;
            }
        }

        public class SetImpedance_ : Keysight34465ACommand
        {

            public Impedance Impedance { get; }
            public SetImpedance_(Impedance impedance) : base(UnionCases.SetImpedance)
            {
                Impedance = impedance;
            }
        }

        public class ClearDisplay_ : Keysight34465ACommand
        {
            public ClearDisplay_() : base(UnionCases.ClearDisplay)
            {
            }
        }

        internal enum UnionCases
        {
            Identification,
            Read,
            Abort,
            Initiate,
            Fetch,
            ConfigureCurrent,
            MeasureCurrent,
            ConfigureVoltage,
            MeasureVoltage,
            DisplayText,
            ClearDisplay,
            SetImpedance
        }

        internal UnionCases UnionCase { get; }
        Keysight34465ACommand(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(Keysight34465ACommand other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Keysight34465ACommand)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class Keysight34465ACommandExtension
    {
        public static T Match<T>(this Keysight34465ACommand command34465A, Func<Keysight34465ACommand.Identification_, T> identification, Func<Keysight34465ACommand.Read_, T> read, Func<Keysight34465ACommand.Abort_, T> abort, Func<Keysight34465ACommand.Initiate_, T> initiate, Func<Keysight34465ACommand.Fetch_, T> fetch, Func<Keysight34465ACommand.ConfigureCurrent_, T> configureCurrent, Func<Keysight34465ACommand.MeasureCurrent_, T> measureCurrent, Func<Keysight34465ACommand.ConfigureVoltage_, T> configureVoltage, Func<Keysight34465ACommand.MeasureVoltage_, T> measureVoltage, Func<Keysight34465ACommand.DisplayText_, T> displayText, Func<Keysight34465ACommand.ClearDisplay_, T> clearDisplay, Func<Keysight34465ACommand.SetImpedance_, T> setImpedance)
        {
            switch (command34465A.UnionCase)
            {
                case Keysight34465ACommand.UnionCases.Identification:
                    return identification((Keysight34465ACommand.Identification_)command34465A);
                case Keysight34465ACommand.UnionCases.Read:
                    return read((Keysight34465ACommand.Read_)command34465A);
                case Keysight34465ACommand.UnionCases.Abort:
                    return abort((Keysight34465ACommand.Abort_)command34465A);
                case Keysight34465ACommand.UnionCases.Initiate:
                    return initiate((Keysight34465ACommand.Initiate_)command34465A);
                case Keysight34465ACommand.UnionCases.Fetch:
                    return fetch((Keysight34465ACommand.Fetch_)command34465A);
                case Keysight34465ACommand.UnionCases.ConfigureCurrent:
                    return configureCurrent((Keysight34465ACommand.ConfigureCurrent_)command34465A);
                case Keysight34465ACommand.UnionCases.MeasureCurrent:
                    return measureCurrent((Keysight34465ACommand.MeasureCurrent_)command34465A);
                case Keysight34465ACommand.UnionCases.ConfigureVoltage:
                    return configureVoltage((Keysight34465ACommand.ConfigureVoltage_)command34465A);
                case Keysight34465ACommand.UnionCases.MeasureVoltage:
                    return measureVoltage((Keysight34465ACommand.MeasureVoltage_)command34465A);
                case Keysight34465ACommand.UnionCases.DisplayText:
                    return displayText((Keysight34465ACommand.DisplayText_)command34465A);
                case Keysight34465ACommand.UnionCases.ClearDisplay:
                    return clearDisplay((Keysight34465ACommand.ClearDisplay_)command34465A);
                case Keysight34465ACommand.UnionCases.SetImpedance:
                    return setImpedance((Keysight34465ACommand.SetImpedance_)command34465A);
                default:
                    throw new ArgumentException($"Unknown type derived from Keysight34465ACommand: {command34465A.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Keysight34465ACommand command34465A, Func<Keysight34465ACommand.Identification_, Task<T>> identification, Func<Keysight34465ACommand.Read_, Task<T>> read, Func<Keysight34465ACommand.Abort_, Task<T>> abort, Func<Keysight34465ACommand.Initiate_, Task<T>> initiate, Func<Keysight34465ACommand.Fetch_, Task<T>> fetch, Func<Keysight34465ACommand.ConfigureCurrent_, Task<T>> configureCurrent, Func<Keysight34465ACommand.MeasureCurrent_, Task<T>> measureCurrent, Func<Keysight34465ACommand.ConfigureVoltage_, Task<T>> configureVoltage, Func<Keysight34465ACommand.MeasureVoltage_, Task<T>> measureVoltage, Func<Keysight34465ACommand.DisplayText_, Task<T>> displayText, Func<Keysight34465ACommand.ClearDisplay_, Task<T>> clearDisplay, Func<Keysight34465ACommand.SetImpedance_, Task<T>> setImpedance)
        {
            switch (command34465A.UnionCase)
            {
                case Keysight34465ACommand.UnionCases.Identification:
                    return await identification((Keysight34465ACommand.Identification_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.Read:
                    return await read((Keysight34465ACommand.Read_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.Abort:
                    return await abort((Keysight34465ACommand.Abort_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.Initiate:
                    return await initiate((Keysight34465ACommand.Initiate_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.Fetch:
                    return await fetch((Keysight34465ACommand.Fetch_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.ConfigureCurrent:
                    return await configureCurrent((Keysight34465ACommand.ConfigureCurrent_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.MeasureCurrent:
                    return await measureCurrent((Keysight34465ACommand.MeasureCurrent_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.ConfigureVoltage:
                    return await configureVoltage((Keysight34465ACommand.ConfigureVoltage_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.MeasureVoltage:
                    return await measureVoltage((Keysight34465ACommand.MeasureVoltage_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.DisplayText:
                    return await displayText((Keysight34465ACommand.DisplayText_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.ClearDisplay:
                    return await clearDisplay((Keysight34465ACommand.ClearDisplay_)command34465A).ConfigureAwait(false);
                case Keysight34465ACommand.UnionCases.SetImpedance:
                    return await setImpedance((Keysight34465ACommand.SetImpedance_)command34465A).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from Keysight34465ACommand: {command34465A.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<Keysight34465ACommand> command, Func<Keysight34465ACommand.Identification_, T> identification, Func<Keysight34465ACommand.Read_, T> read, Func<Keysight34465ACommand.Abort_, T> abort, Func<Keysight34465ACommand.Initiate_, T> initiate, Func<Keysight34465ACommand.Fetch_, T> fetch, Func<Keysight34465ACommand.ConfigureCurrent_, T> configureCurrent, Func<Keysight34465ACommand.MeasureCurrent_, T> measureCurrent, Func<Keysight34465ACommand.ConfigureVoltage_, T> configureVoltage, Func<Keysight34465ACommand.MeasureVoltage_, T> measureVoltage, Func<Keysight34465ACommand.DisplayText_, T> displayText, Func<Keysight34465ACommand.ClearDisplay_, T> clearDisplay, Func<Keysight34465ACommand.SetImpedance_, T> setImpedance) => (await command.ConfigureAwait(false)).Match(identification, read, abort, initiate, fetch, configureCurrent, measureCurrent, configureVoltage, measureVoltage, displayText, clearDisplay, setImpedance);
        public static async Task<T> Match<T>(this Task<Keysight34465ACommand> command, Func<Keysight34465ACommand.Identification_, Task<T>> identification, Func<Keysight34465ACommand.Read_, Task<T>> read, Func<Keysight34465ACommand.Abort_, Task<T>> abort, Func<Keysight34465ACommand.Initiate_, Task<T>> initiate, Func<Keysight34465ACommand.Fetch_, Task<T>> fetch, Func<Keysight34465ACommand.ConfigureCurrent_, Task<T>> configureCurrent, Func<Keysight34465ACommand.MeasureCurrent_, Task<T>> measureCurrent, Func<Keysight34465ACommand.ConfigureVoltage_, Task<T>> configureVoltage, Func<Keysight34465ACommand.MeasureVoltage_, Task<T>> measureVoltage, Func<Keysight34465ACommand.DisplayText_, Task<T>> displayText, Func<Keysight34465ACommand.ClearDisplay_, Task<T>> clearDisplay, Func<Keysight34465ACommand.SetImpedance_, Task<T>> setImpedance) => await(await command.ConfigureAwait(false)).Match(identification, read, abort, initiate, fetch, configureCurrent, measureCurrent, configureVoltage, measureVoltage, displayText, clearDisplay, setImpedance).ConfigureAwait(false);
    }
}