using System;
using System.Threading.Tasks;

namespace Domain.UnionTypes
{
    public abstract class DisplayState
    {
        public static DisplayState DisplayText(string textValue) => new DisplayText_(textValue);

        public static readonly DisplayState Hidden = new Hidden_();

        public class DisplayText_ : DisplayState
        {
            public string TextValue { get; }
            public DisplayText_(string textValue) : base(UnionCases.DisplayText)
            {
                TextValue = textValue;
            }
        }

        public class Hidden_ : DisplayState
        {
            public Hidden_() : base(UnionCases.Hidden)
            {
            }
        }

        internal enum UnionCases
        {
            DisplayText,
            Hidden,
        }

        internal UnionCases UnionCase { get; }
        DisplayState(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(DisplayState other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DisplayState)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class DisplayStateExtension
    {
        public static T Match<T>(this DisplayState displayState, Func<DisplayState.DisplayText_, T> displayText, Func<DisplayState.Hidden_, T> hidden)
        {
            switch (displayState.UnionCase)
            {
                case DisplayState.UnionCases.DisplayText:
                    return displayText((DisplayState.DisplayText_)displayState);
                case DisplayState.UnionCases.Hidden:
                    return hidden((DisplayState.Hidden_)displayState);
                default:
                    throw new ArgumentException($"Unknown type derived from DisplayState: {displayState.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this DisplayState displayState, Func<DisplayState.DisplayText_, Task<T>> displayText, Func<DisplayState.Hidden_, Task<T>> hidden)
        {
            switch (displayState.UnionCase)
            {
                case DisplayState.UnionCases.DisplayText:
                    return await displayText((DisplayState.DisplayText_)displayState).ConfigureAwait(false);
                case DisplayState.UnionCases.Hidden:
                    return await hidden((DisplayState.Hidden_)displayState).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from DisplayState: {displayState.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<DisplayState> displayState, Func<DisplayState.DisplayText_, T> displayText, Func<DisplayState.Hidden_, T> hidden) => (await displayState.ConfigureAwait(false)).Match(displayText, hidden);
        public static async Task<T> Match<T>(this Task<DisplayState> displayState, Func<DisplayState.DisplayText_, Task<T>> displayText, Func<DisplayState.Hidden_, Task<T>> hidden) => await(await displayState.ConfigureAwait(false)).Match(displayText, hidden).ConfigureAwait(false);
    }
}