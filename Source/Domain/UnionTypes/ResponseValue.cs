using System;
using System.Threading.Tasks;
using Domain.Abstractions;

namespace Domain.UnionTypes
{
    public abstract class ResponseValue : IByteArrayConvertible
    {
        public static ResponseValue String(string value) => new String_(value);
        public abstract byte[] ToByteArray(Func<string, byte[]> stringByteArrayEncoder);

        public class String_ : ResponseValue
        {
            public string Value { get; }
            public String_(string value) : base(UnionCases.String)
            {
                Value = value;
            }

            public override byte[] ToByteArray(Func<string, byte[]> stringByteArrayEncoder)
            {
                return stringByteArrayEncoder(Value);
            }
        }

        internal enum UnionCases
        {
            String
        }

        internal UnionCases UnionCase { get; }
        ResponseValue(UnionCases unionCase) => UnionCase = unionCase;

        public override string ToString() => Enum.GetName(typeof(UnionCases), UnionCase) ?? UnionCase.ToString();


        bool Equals(ResponseValue other) => UnionCase == other.UnionCase;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ResponseValue)obj);
        }

        public override int GetHashCode() => (int)UnionCase;
    }

    public static class ResponseValueExtension
    {
        public static T Match<T>(this ResponseValue responseValue, Func<ResponseValue.String_, T> @string)
        {
            switch (responseValue.UnionCase)
            {
                case ResponseValue.UnionCases.String:
                    return @string((ResponseValue.String_)responseValue);
                default:
                    throw new ArgumentException($"Unknown type derived from ResponseValue: {responseValue.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this ResponseValue responseValue, Func<ResponseValue.String_, Task<T>> @string)
        {
            switch (responseValue.UnionCase)
            {
                case ResponseValue.UnionCases.String:
                    return await @string((ResponseValue.String_)responseValue).ConfigureAwait(false);
                default:
                    throw new ArgumentException($"Unknown type derived from ResponseValue: {responseValue.GetType().Name}");
            }
        }

        public static async Task<T> Match<T>(this Task<ResponseValue> responseValue, Func<ResponseValue.String_, T> @string) => (await responseValue.ConfigureAwait(false)).Match(@string);
        public static async Task<T> Match<T>(this Task<ResponseValue> responseValue, Func<ResponseValue.String_, Task<T>> @string) => await(await responseValue.ConfigureAwait(false)).Match(@string).ConfigureAwait(false);
    }
}