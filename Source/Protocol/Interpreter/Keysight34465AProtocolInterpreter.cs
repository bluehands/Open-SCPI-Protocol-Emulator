// ReSharper disable InconsistentNaming

using Antlr4.Runtime;
using Emulator.Command;
using FunicularSwitch;
using Protocol.Keysight34465A;
using ProtocolParser.Keysight34465A;

namespace Protocol.Interpreter
{
	public sealed class Keysight34465AProtocolInterpreter : IProtocolInterpreter<Keysight34465ACommand>
	{
		public Result<Keysight34465ACommand> GetCommand(string input)
		{
			var inputStream = new AntlrInputStream(input);
			var lexer = new Keysight34465ASCPILexer(inputStream);
			lexer.RemoveErrorListeners();
			var lexerErrorListener = new ResultErrorListener<int>();
			lexer.AddErrorListener(lexerErrorListener);
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new Keysight34465ASCPIParser(tokenStream);
			parser.RemoveErrorListeners();
			var parserErrorListener = new ResultErrorListener<IToken>();
			parser.AddErrorListener(parserErrorListener);
			var commandContext = parser.command();
			return lexerErrorListener.StatusResult.Aggregate(parserErrorListener.StatusResult)
				.Bind(_ =>
				{
					try
					{
						return new Keysight34465ACommandVisitor().VisitCommand(commandContext);
					}
					catch (VisitorTokenHandlerException e)
					{
						return Result.Error<Keysight34465ACommand>(e.ToString());
					}
				});
		}
	}
}