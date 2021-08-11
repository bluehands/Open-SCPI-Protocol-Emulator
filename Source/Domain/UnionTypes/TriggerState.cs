using System;
using System.Threading.Tasks;

namespace Domain.UnionTypes
{
    public abstract class TriggerState
    {
        public static readonly TriggerState Idle = new Idle_();
        public static readonly TriggerState WaitForTrigger = new WaitForTrigger_();

        public class Idle_ : TriggerState
        {
            public Idle_() : base(UnionCases.Idle)
            {
            }
        }

        public class WaitForTrigger_ : TriggerState
        {
            public WaitForTrigger_() : base(UnionCases.WaitForTrigger)
            {
            }
        }

        internal enum UnionCases
        {
            Idle,
            WaitForTrigger,
        }

        internal UnionCases UnionCase { get; }
        TriggerState(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();
        bool Equals(TriggerState other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TriggerState)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class TriggerStateExtension
    {
        public static T Match<T>(this TriggerState triggerState, Func<TriggerState.Idle_, T> idle, Func<TriggerState.WaitForTrigger_, T> waitForTrigger)
        {
            switch (triggerState.UnionCase)
            {
                case TriggerState.UnionCases.Idle:
                    return idle((TriggerState.Idle_)triggerState);
                case TriggerState.UnionCases.WaitForTrigger:
                    return waitForTrigger((TriggerState.WaitForTrigger_)triggerState);
                default:
                    throw new ArgumentException($"Unknown type derived from TriggerState: {triggerState.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this TriggerState triggerState, Func<TriggerState.Idle_, Task<T>> idle, Func<TriggerState.WaitForTrigger_, Task<T>> waitForTrigger)
        {
            switch (triggerState.UnionCase)
            {
                case TriggerState.UnionCases.Idle:
                    return await idle((TriggerState.Idle_)triggerState).ConfigureAwait(false);
                case TriggerState.UnionCases.WaitForTrigger:
                    return await waitForTrigger((TriggerState.WaitForTrigger_)triggerState).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from TriggerState: {triggerState.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<TriggerState> triggerState, Func<TriggerState.Idle_, T> idle, Func<TriggerState.WaitForTrigger_, T> waitForTrigger) => (await triggerState.ConfigureAwait(false)).Match(idle, waitForTrigger);
        public static async Task<T> Match<T>(this Task<TriggerState> triggerState, Func<TriggerState.Idle_, Task<T>> idle, Func<TriggerState.WaitForTrigger_, Task<T>> waitForTrigger) => await(await triggerState.ConfigureAwait(false)).Match(idle, waitForTrigger).ConfigureAwait(false);
    }
}