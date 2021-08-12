using System;

namespace Domain.Abstractions
{
    public interface IByteArrayConvertible
    {
        byte[] ToByteArray(Func<string, byte[]> stringByteArrayEncoder);
    }
}