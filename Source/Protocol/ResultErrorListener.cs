using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using FunicularSwitch;
using FunicularSwitch.Extensions;

// ReSharper disable HeapView.PossibleBoxingAllocation

namespace Protocol
{
    public class ResultErrorListener<T> : IAntlrErrorListener<T>
    {
        private List<Result<Unit>> Results { get;} = new() { Result.Ok(No.Thing)};

        public Result<Unit> StatusResult => Results.Aggregate("\n").Map(res => res.FirstOrDefault());
        
        public void SyntaxError(TextWriter output, IRecognizer recognizer, T offendingSymbol, int line, int charPositionInLine,
            string msg, RecognitionException e)
        {
            Results.Add(Result.Error<Unit>($"{msg}: {offendingSymbol}, {line.ToString()}, {charPositionInLine.ToString()}, ErrorType: {e?.GetType().BeautifulName()}"));
        }
    }
}