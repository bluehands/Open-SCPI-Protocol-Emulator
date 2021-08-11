using FluentAssertions;
using FunicularSwitch;

namespace ProtocolTests.Interpreter
{
	public static class AssertionExtensions
	{
		public static TAssert AssertOptionType<TBase, TAssert>(this Option<TBase> option)
		{
			return option.Should().BeOfType<Some<TBase>>().Which.Value.Should().BeOfType<TAssert>().Which;
		}
	}
}