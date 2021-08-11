using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace EmulatorHost.Network
{
    public class DataBuilder
    {
        private readonly ISubject<IDataBuilderParameter> parameters;
        private readonly IObservable<IDataBuilder> dataBuilder;

        public IObservable<string> DataStream => dataBuilder.OfType<ReadyDataBuilder>().Select(readyDataBuilder => readyDataBuilder.Data);

        public DataBuilder()
        {
            parameters = new Subject<IDataBuilderParameter>();
            dataBuilder = parameters.Scan(
                (IDataBuilder)new ReadyDataBuilder(string.Empty),
                (state, trigger) =>
                    state.Apply(trigger));
        }

        public void Append(string input)
        {
            foreach (var parameter in input.ToCharArray().Select(character =>
            {
                const char separatorChar = '\n';
                const char sequenceEscapeChar = '"';

                return character switch
                {
                    separatorChar => new DataBuilderParameters.Separator(separatorChar) as IDataBuilderParameter,
                    sequenceEscapeChar => new DataBuilderParameters.EscapeChar(sequenceEscapeChar) as IDataBuilderParameter,
                    _ => new DataBuilderParameters.Char(character) as IDataBuilderParameter
                };
            }))
            {
                parameters.OnNext(parameter);
            }
        }

        public void Stop()
        {
            parameters.OnCompleted();
        }
    }

    public interface IDataBuilder
    {
        DataBuilderState State { get; }
    }

    public class ReadyDataBuilder : IDataBuilder
    {
        public ReadyDataBuilder(string data)
        {
            Data = data;
        }

        public string Data { get; }
        public DataBuilderState State => DataBuilderState.Ready;

        public ReadingDataBuilder Char(char value)
        {
            return new ReadingDataBuilder(new StringBuilder().Append(value));
        }

        public ReadingDataBuilder Char(DataBuilderParameters.Char parameters)
        {
            return Char(parameters.Value);
        }

        public ReadyDataBuilder Separator(char value)
        {
            return new ReadyDataBuilder(string.Empty);
        }

        public ReadyDataBuilder Separator(DataBuilderParameters.Separator parameters)
        {
            return Separator(parameters.Value);
        }

        public EscapingDataBuilder EscapeChar(char value)
        {
            return new EscapingDataBuilder(new StringBuilder().Append(value));
        }

        public EscapingDataBuilder EscapeChar(DataBuilderParameters.EscapeChar parameters)
        {
            return EscapeChar(parameters.Value);
        }
    }

    public class ReadingDataBuilder : IDataBuilder
    {
        public ReadingDataBuilder(StringBuilder buffer)
        {
            Buffer = buffer;
        }

        public StringBuilder Buffer { get; }
        public DataBuilderState State => DataBuilderState.Reading;

        public ReadingDataBuilder Char(char value)
        {
            Buffer.Append(value);
            return new ReadingDataBuilder(Buffer);
        }

        public ReadingDataBuilder Char(DataBuilderParameters.Char parameters)
        {
            return Char(parameters.Value);
        }

        public EscapingDataBuilder EscapeChar(char value)
        {
            return new EscapingDataBuilder(Buffer.Append(value));
        }

        public EscapingDataBuilder EscapeChar(DataBuilderParameters.EscapeChar parameters)
        {
            return EscapeChar(parameters.Value);
        }

        public ReadyDataBuilder Separator(char value)
        {
            return new ReadyDataBuilder(Buffer.ToString());
        }

        public ReadyDataBuilder Separator(DataBuilderParameters.Separator parameters)
        {
            return Separator(parameters.Value);
        }
    }

    public class EscapingDataBuilder : IDataBuilder
    {
        public EscapingDataBuilder(StringBuilder buffer)
        {
            Buffer = buffer;
        }

        public StringBuilder Buffer { get; }
        public DataBuilderState State => DataBuilderState.Escaping;

        public EscapingDataBuilder Char(char value)
        {
            return new EscapingDataBuilder(Buffer.Append(value));
        }

        public EscapingDataBuilder Char(DataBuilderParameters.Char parameters)
        {
            return Char(parameters.Value);
        }

        public EscapingDataBuilder Separator(char value)
        {
            return new EscapingDataBuilder(Buffer.Append(value));
        }

        public EscapingDataBuilder Separator(DataBuilderParameters.Separator parameters)
        {
            return Separator(parameters.Value);
        }

        public ReadingDataBuilder EscapeChar(char value)
        {
            return new ReadingDataBuilder(Buffer.Append(value));
        }

        public ReadingDataBuilder EscapeChar(DataBuilderParameters.EscapeChar parameters)
        {
            return EscapeChar(parameters.Value);
        }
    }

    public interface IDataBuilderParameter
    {
        DataBuilderTrigger Trigger { get; }
    }

    public static class DataBuilderParameters
    {
        public class Char : IDataBuilderParameter
        {
            public Char(char value)
            {
                Value = value;
            }

            public char Value { get; }
            public DataBuilderTrigger Trigger => DataBuilderTrigger.Char;
        }

        public class EscapeChar : IDataBuilderParameter
        {
            public EscapeChar(char value)
            {
                Value = value;
            }

            public char Value { get; }
            public DataBuilderTrigger Trigger => DataBuilderTrigger.EscapeChar;
        }

        public class Separator : IDataBuilderParameter
        {
            public Separator(char value)
            {
                Value = value;
            }

            public char Value { get; }
            public DataBuilderTrigger Trigger => DataBuilderTrigger.Separator;
        }
    }

    public abstract class DataBuilderState
    {
        public static readonly DataBuilderState Ready = new Ready_();
        public static readonly DataBuilderState Reading = new Reading_();
        public static readonly DataBuilderState Escaping = new Escaping_();

        public class Ready_ : DataBuilderState
        {
            public Ready_() : base(UnionCases.Ready)
            {
            }
        }

        public class Reading_ : DataBuilderState
        {
            public Reading_() : base(UnionCases.Reading)
            {
            }
        }

        public class Escaping_ : DataBuilderState
        {
            public Escaping_() : base(UnionCases.Escaping)
            {
            }
        }

        internal enum UnionCases
        {
            Ready,
            Reading,
            Escaping
        }

        internal UnionCases UnionCase { get; }
        DataBuilderState(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(DataBuilderState other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataBuilderState)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public abstract class DataBuilderTrigger
    {
        public static readonly DataBuilderTrigger Separator = new Separator_();
        public static readonly DataBuilderTrigger EscapeChar = new EscapeChar_();
        public static readonly DataBuilderTrigger Char = new Char_();

        public class Char_ : DataBuilderTrigger
        {
            public Char_() : base(UnionCases.Char)
            {
            }
        }

        public class EscapeChar_ : DataBuilderTrigger
        {
            public EscapeChar_() : base(UnionCases.EscapeChar)
            {
            }
        }

        public class Separator_ : DataBuilderTrigger
        {
            public Separator_() : base(UnionCases.Separator)
            {
            }
        }

        internal enum UnionCases
        {
            Separator,
            EscapeChar,
            Char
        }

        internal UnionCases UnionCase { get; }
        DataBuilderTrigger(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(DataBuilderTrigger other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataBuilderTrigger)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public abstract class DataBuilderTransitionResult
    {
    }

    public class DataBuilderTransition : DataBuilderTransitionResult
    {
        public IDataBuilder Source { get; }
        public IDataBuilder Destination { get; }
        public IDataBuilderParameter Trigger { get; }

        public DataBuilderTransition(IDataBuilder source, IDataBuilder destination, IDataBuilderParameter trigger)
        {
            Source = source;
            Destination = destination;
            Trigger = trigger;
        }
    }

    public class DataBuilderInvalidTrigger : DataBuilderTransitionResult
    {
        public IDataBuilder Source { get; }
        public IDataBuilderParameter Trigger { get; }

        public DataBuilderInvalidTrigger(IDataBuilder source, IDataBuilderParameter trigger)
        {
            Source = source;
            Trigger = trigger;
        }
    }

    public static class DataBuilderExtension
    {
        public static IDataBuilder Apply(this IDataBuilder dataBuilder, IDataBuilderParameter parameter)
        {
            switch (dataBuilder.State.UnionCase)
            {
                case DataBuilderState.UnionCases.Ready:
                    {
                        switch (parameter.Trigger.UnionCase)
                        {
                            case DataBuilderTrigger.UnionCases.Separator:
                                return ((ReadyDataBuilder)dataBuilder).Separator((DataBuilderParameters.Separator)parameter);
                            case DataBuilderTrigger.UnionCases.EscapeChar:
                                return ((ReadyDataBuilder)dataBuilder).EscapeChar((DataBuilderParameters.EscapeChar)parameter);
                            case DataBuilderTrigger.UnionCases.Char:
                                return ((ReadyDataBuilder)dataBuilder).Char((DataBuilderParameters.Char)parameter);
                            default:
                                return dataBuilder;
                        }
                    }

                case DataBuilderState.UnionCases.Reading:
                    {
                        switch (parameter.Trigger.UnionCase)
                        {
                            case DataBuilderTrigger.UnionCases.Char:
                                return ((ReadingDataBuilder)dataBuilder).Char((DataBuilderParameters.Char)parameter);
                            case DataBuilderTrigger.UnionCases.EscapeChar:
                                return ((ReadingDataBuilder)dataBuilder).EscapeChar((DataBuilderParameters.EscapeChar)parameter);
                            case DataBuilderTrigger.UnionCases.Separator:
                                return ((ReadingDataBuilder)dataBuilder).Separator((DataBuilderParameters.Separator)parameter);
                            default:
                                return dataBuilder;
                        }
                    }

                case DataBuilderState.UnionCases.Escaping:
                    {
                        switch (parameter.Trigger.UnionCase)
                        {
                            case DataBuilderTrigger.UnionCases.Char:
                                return ((EscapingDataBuilder)dataBuilder).Char((DataBuilderParameters.Char)parameter);
                            case DataBuilderTrigger.UnionCases.Separator:
                                return ((EscapingDataBuilder)dataBuilder).Separator((DataBuilderParameters.Separator)parameter);
                            case DataBuilderTrigger.UnionCases.EscapeChar:
                                return ((EscapingDataBuilder)dataBuilder).EscapeChar((DataBuilderParameters.EscapeChar)parameter);
                            default:
                                return dataBuilder;
                        }
                    }

                default:
                    throw new ArgumentException($"Unknown type implementing IDataBuilder: {dataBuilder.GetType().Name}");
            }
        }

        public static DataBuilderTransitionResult DoTransition(this IDataBuilder dataBuilder, IDataBuilderParameter parameter)
        {
            switch (dataBuilder.State.UnionCase)
            {
                case DataBuilderState.UnionCases.Ready:
                    {
                        switch (parameter.Trigger.UnionCase)
                        {
                            case DataBuilderTrigger.UnionCases.Separator:
                                return new DataBuilderTransition(dataBuilder, ((ReadyDataBuilder)dataBuilder).Separator((DataBuilderParameters.Separator)parameter), parameter);
                            case DataBuilderTrigger.UnionCases.EscapeChar:
                                return new DataBuilderTransition(dataBuilder, ((ReadyDataBuilder)dataBuilder).EscapeChar((DataBuilderParameters.EscapeChar)parameter), parameter);
                            case DataBuilderTrigger.UnionCases.Char:
                                return new DataBuilderTransition(dataBuilder, ((ReadyDataBuilder)dataBuilder).Char((DataBuilderParameters.Char)parameter), parameter);
                            default:
                                return new DataBuilderInvalidTrigger(dataBuilder, parameter);
                        }
                    }

                case DataBuilderState.UnionCases.Reading:
                    {
                        switch (parameter.Trigger.UnionCase)
                        {
                            case DataBuilderTrigger.UnionCases.Char:
                                return new DataBuilderTransition(dataBuilder, ((ReadingDataBuilder)dataBuilder).Char((DataBuilderParameters.Char)parameter), parameter);
                            case DataBuilderTrigger.UnionCases.EscapeChar:
                                return new DataBuilderTransition(dataBuilder, ((ReadingDataBuilder)dataBuilder).EscapeChar((DataBuilderParameters.EscapeChar)parameter), parameter);
                            case DataBuilderTrigger.UnionCases.Separator:
                                return new DataBuilderTransition(dataBuilder, ((ReadingDataBuilder)dataBuilder).Separator((DataBuilderParameters.Separator)parameter), parameter);
                            default:
                                return new DataBuilderInvalidTrigger(dataBuilder, parameter);
                        }
                    }

                case DataBuilderState.UnionCases.Escaping:
                    {
                        switch (parameter.Trigger.UnionCase)
                        {
                            case DataBuilderTrigger.UnionCases.Char:
                                return new DataBuilderTransition(dataBuilder, ((EscapingDataBuilder)dataBuilder).Char((DataBuilderParameters.Char)parameter), parameter);
                            case DataBuilderTrigger.UnionCases.Separator:
                                return new DataBuilderTransition(dataBuilder, ((EscapingDataBuilder)dataBuilder).Separator((DataBuilderParameters.Separator)parameter), parameter);
                            case DataBuilderTrigger.UnionCases.EscapeChar:
                                return new DataBuilderTransition(dataBuilder, ((EscapingDataBuilder)dataBuilder).EscapeChar((DataBuilderParameters.EscapeChar)parameter), parameter);
                            default:
                                return new DataBuilderInvalidTrigger(dataBuilder, parameter);
                        }
                    }

                default:
                    throw new ArgumentException($"Unknown type implementing IDataBuilder: {dataBuilder.GetType().Name}");
            }
        }

        public static T Match<T>(this IDataBuilder dataBuilder, Func<ReadyDataBuilder, T> ready, Func<ReadingDataBuilder, T> reading, Func<EscapingDataBuilder, T> escaping)
        {
            switch (dataBuilder.State.UnionCase)
            {
                case DataBuilderState.UnionCases.Ready:
                    return ready((ReadyDataBuilder)dataBuilder);
                case DataBuilderState.UnionCases.Reading:
                    return reading((ReadingDataBuilder)dataBuilder);
                case DataBuilderState.UnionCases.Escaping:
                    return escaping((EscapingDataBuilder)dataBuilder);
                default:
                    throw new ArgumentException($"Unknown type derived from IDataBuilder: {dataBuilder.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this IDataBuilder dataBuilder, Func<ReadyDataBuilder, Task<T>> ready, Func<ReadingDataBuilder, Task<T>> reading, Func<EscapingDataBuilder, Task<T>> escaping)
        {
            switch (dataBuilder.State.UnionCase)
            {
                case DataBuilderState.UnionCases.Ready:
                    return await ready((ReadyDataBuilder)dataBuilder).ConfigureAwait(false);
                case DataBuilderState.UnionCases.Reading:
                    return await reading((ReadingDataBuilder)dataBuilder).ConfigureAwait(false);
                case DataBuilderState.UnionCases.Escaping:
                    return await escaping((EscapingDataBuilder)dataBuilder).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from IDataBuilder: {dataBuilder.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<IDataBuilder> dataBuilder, Func<ReadyDataBuilder, T> ready, Func<ReadingDataBuilder, T> reading, Func<EscapingDataBuilder, T> escaping) => (await dataBuilder.ConfigureAwait(false)).Match(ready, reading, escaping);
        public static async Task<T> Match<T>(this Task<IDataBuilder> dataBuilder, Func<ReadyDataBuilder, Task<T>> ready, Func<ReadingDataBuilder, Task<T>> reading, Func<EscapingDataBuilder, Task<T>> escaping) => await(await dataBuilder.ConfigureAwait(false)).Match(ready, reading, escaping).ConfigureAwait(false);

        public static T Match<T>(this IDataBuilderParameter parameter, Func<DataBuilderParameters.Char, T> @char,
            Func<DataBuilderParameters.EscapeChar, T> escapeChar, Func<DataBuilderParameters.Separator, T> separator)
        {
            switch (parameter.Trigger.UnionCase)
            {
                case DataBuilderTrigger.UnionCases.Char:
                    return @char((DataBuilderParameters.Char)parameter);
                case DataBuilderTrigger.UnionCases.EscapeChar:
                    return escapeChar((DataBuilderParameters.EscapeChar)parameter);
                case DataBuilderTrigger.UnionCases.Separator:
                    return separator((DataBuilderParameters.Separator)parameter);
                default:
                    throw new ArgumentException(
                        $"Unknown type derived from IDataBuilderParameter: {parameter.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this IDataBuilderParameter parameter,
            Func<DataBuilderParameters.Char, Task<T>> @char, Func<DataBuilderParameters.EscapeChar, Task<T>> escapeChar,
            Func<DataBuilderParameters.Separator, Task<T>> separator)
        {
            switch (parameter.Trigger.UnionCase)
            {
                case DataBuilderTrigger.UnionCases.Char:
                    return await @char((DataBuilderParameters.Char)parameter).ConfigureAwait(false);
                case DataBuilderTrigger.UnionCases.EscapeChar:
                    return await escapeChar((DataBuilderParameters.EscapeChar)parameter).ConfigureAwait(false);
                case DataBuilderTrigger.UnionCases.Separator:
                    return await separator((DataBuilderParameters.Separator)parameter).ConfigureAwait(false);
                default:
                    throw new ArgumentException(
                        $"Unknown type derived from IDataBuilderParameter: {parameter.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<IDataBuilderParameter> parameter,
            Func<DataBuilderParameters.Char, T> @char, Func<DataBuilderParameters.EscapeChar, T> escapeChar,
            Func<DataBuilderParameters.Separator, T> separator) =>
            (await parameter.ConfigureAwait(false)).Match(@char, escapeChar, separator);

        public static async Task<T> Match<T>(this Task<IDataBuilderParameter> parameter,
            Func<DataBuilderParameters.Char, Task<T>> @char, Func<DataBuilderParameters.EscapeChar, Task<T>> escapeChar,
            Func<DataBuilderParameters.Separator, Task<T>> separator) => await (await parameter.ConfigureAwait(false))
            .Match(@char, escapeChar, separator).ConfigureAwait(false);

        public static T Match<T>(this IDataBuilderParameter parameter, Func<DataBuilderParameters.Separator, T> separator, Func<DataBuilderParameters.EscapeChar, T> escapeChar, Func<DataBuilderParameters.Char, T> @char)
        {
            switch (parameter.Trigger.UnionCase)
            {
                case DataBuilderTrigger.UnionCases.Separator:
                    return separator((DataBuilderParameters.Separator)parameter);
                case DataBuilderTrigger.UnionCases.EscapeChar:
                    return escapeChar((DataBuilderParameters.EscapeChar)parameter);
                case DataBuilderTrigger.UnionCases.Char:
                    return @char((DataBuilderParameters.Char)parameter);
                default:
                    throw new ArgumentException($"Unknown type derived from IDataBuilderParameter: {parameter.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this IDataBuilderParameter parameter, Func<DataBuilderParameters.Separator, Task<T>> separator, Func<DataBuilderParameters.EscapeChar, Task<T>> escapeChar, Func<DataBuilderParameters.Char, Task<T>> @char)
        {
            switch (parameter.Trigger.UnionCase)
            {
                case DataBuilderTrigger.UnionCases.Separator:
                    return await separator((DataBuilderParameters.Separator)parameter).ConfigureAwait(false);
                case DataBuilderTrigger.UnionCases.EscapeChar:
                    return await escapeChar((DataBuilderParameters.EscapeChar)parameter).ConfigureAwait(false);
                case DataBuilderTrigger.UnionCases.Char:
                    return await @char((DataBuilderParameters.Char)parameter).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from IDataBuilderParameter: {parameter.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<IDataBuilderParameter> parameter, Func<DataBuilderParameters.Separator, T> separator, Func<DataBuilderParameters.EscapeChar, T> escapeChar, Func<DataBuilderParameters.Char, T> @char) => (await parameter.ConfigureAwait(false)).Match(separator, escapeChar, @char);
        public static async Task<T> Match<T>(this Task<IDataBuilderParameter> parameter, Func<DataBuilderParameters.Separator, Task<T>> separator, Func<DataBuilderParameters.EscapeChar, Task<T>> escapeChar, Func<DataBuilderParameters.Char, Task<T>> @char) => await(await parameter.ConfigureAwait(false)).Match(separator, escapeChar, @char).ConfigureAwait(false);
    }
}