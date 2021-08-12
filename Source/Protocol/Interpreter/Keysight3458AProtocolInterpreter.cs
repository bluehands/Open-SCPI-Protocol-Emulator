using System;
using Antlr4.Runtime;
using Emulator.Command;
using FunicularSwitch;
using Protocol.Keysight3458;
using ProtocolParser.Keysight3458A;

namespace Protocol.Interpreter
{
	public class Keysight3458AProtocolInterpreter : IProtocolInterpreter<Keysight3458ACommand>
	{
		public Result<Keysight3458ACommand> GetCommand(string input)
		{
			var inputStream = new AntlrInputStream(input);
			var lexer = new Keysight3458ASCPILexer(inputStream);
			lexer.RemoveErrorListeners();
			var lexerErrorListener = new ResultErrorListener<int>();
			lexer.AddErrorListener(lexerErrorListener);
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new Keysight3458ASCPIParser(tokenStream);
			parser.RemoveErrorListeners();
			var parserErrorListener = new ResultErrorListener<IToken>();
			parser.AddErrorListener(parserErrorListener);
			var commandContext = parser.command();
			return lexerErrorListener.StatusResult.Aggregate(parserErrorListener.StatusResult)
				.Bind(_ =>
				{
					try
					{
						return new Keysight3458ACommandVisitor().VisitCommand(commandContext);
					}
					catch (Exception e)
					{
						return Result.Error<Keysight3458ACommand>(e.ToString());
					}
				});
		}
	}
}