using System.Globalization;
using Domain.UnionTypes;
using FunicularSwitch;
using ProtocolParser.Keysight3458A;

namespace Protocol.Keysight3458
{
    public class Keysight3458AVoltageParameterVisitor : Keysight3458ASCPIBaseVisitor<(Option<Range>, Option<Resolution>)>
    {
        public override (Option<Range>, Option<Resolution>) VisitVoltageParameters(Keysight3458ASCPIParser.VoltageParametersContext context)
        {
            var range = Option<Range>.None;
            if (context.range != null)
            {
                var text = context.range.Text;
                range = context.range.Type switch
                {
                    Keysight3458ASCPILexer.Number => Option.Some(Range.Number(ToDouble(context.range.Text))),
                    Keysight3458ASCPILexer.AUTO => Option.Some(Range.Auto),
                    Keysight3458ASCPILexer.MIN => Option.Some(Range.Min),
                    Keysight3458ASCPILexer.MAX => Option.Some(Range.Max),
                    Keysight3458ASCPILexer.DEF => Option.Some(Range.Def),
                    _ => Option<Range>.None
                };
            }

            var resolution = Option.None<Resolution>();
            if (context.resolution != null)
            {
                resolution = context.resolution.Type switch
                {
                    Keysight3458ASCPILexer.Number => Option.Some(Resolution.Number(ToDouble(context.resolution.Text))),
                    Keysight3458ASCPILexer.MIN => Option.Some(Resolution.Min),
                    Keysight3458ASCPILexer.MAX => Option.Some(Resolution.Max),
                    Keysight3458ASCPILexer.DEF => Option.Some(Resolution.Def),
                    _ => Option<Resolution>.None
                };
            }

            return (range, resolution);
        }

        private static double ToDouble(string input)
        {
            return decimal.ToDouble(decimal.Parse(input, NumberStyles.Float, new CultureInfo("en-US")));
        }

        protected override (Option<Range>, Option<Resolution>) DefaultResult { get; } =
            (Option<Range>.None, Option<Resolution>.None);
    }
}