using System;

namespace Protocol
{
	public sealed class VisitorTokenHandlerException : Exception
	{
		public VisitorTokenHandlerException(string message) : base(message)
		{
		}
	}
}