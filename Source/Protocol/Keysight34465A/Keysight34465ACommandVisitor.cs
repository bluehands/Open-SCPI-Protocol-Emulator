using Antlr4.Runtime.Misc;
using Domain.UnionTypes;
using Emulator.Command;
using FunicularSwitch;
using ProtocolParser.Keysight34465A;

namespace Protocol.Keysight34465A
{
    public class Keysight34465ACommandVisitor : Keysight34465ASCPIBaseVisitor<Result<Keysight34465ACommand>>
    {
        public override Result<Keysight34465ACommand> VisitCommand(Keysight34465ASCPIParser.CommandContext context)
        {
            // skip EOF
            return context.GetChild(0).Accept(this);
        }

        public override Result<Keysight34465ACommand> VisitAbortCommand([NotNull] Keysight34465ASCPIParser.AbortCommandContext context)
        {
            return Keysight34465ACommand.Abort;
        }

        public override Result<Keysight34465ACommand> VisitIdentificationQuery(
            [NotNull] Keysight34465ASCPIParser.IdentificationQueryContext context)
        {
            return Keysight34465ACommand.Identification;
        }

        public override Result<Keysight34465ACommand> VisitReadQuery([NotNull] Keysight34465ASCPIParser.ReadQueryContext context)
        {
            return Keysight34465ACommand.Read;
        }

        public override Result<Keysight34465ACommand> VisitConfigureCurrentCommand(
            [NotNull] Keysight34465ASCPIParser.ConfigureCurrentCommandContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight34465ASCPILexer.AC => ElectricityType.AC,
                Keysight34465ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };
            var (rage, resolution) = context.Accept(new Keysight34465ACurrentParameterVisitor());
            return Keysight34465ACommand.ConfigureCurrent(electricityType, rage, resolution);
        }

        public override Result<Keysight34465ACommand> VisitConfigureVoltageCommand(Keysight34465ASCPIParser.ConfigureVoltageCommandContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight34465ASCPILexer.AC => ElectricityType.AC,
                Keysight34465ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };
            var (rage, resolution) = context.Accept(new Keysight34465AVoltageParameterVisitor());
            return Keysight34465ACommand.ConfigureVoltage(electricityType, rage, resolution);
        }


        public override Result<Keysight34465ACommand> VisitMeasureCurrentQuery(
            [NotNull] Keysight34465ASCPIParser.MeasureCurrentQueryContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight34465ASCPILexer.AC => ElectricityType.AC,
                Keysight34465ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };


            var (rage, resolution) = context.Accept(new Keysight34465ACurrentParameterVisitor());
            return Keysight34465ACommand.MeasureCurrent(electricityType, rage, resolution);
        }

        public override Result<Keysight34465ACommand> VisitMeasureVoltageQuery(Keysight34465ASCPIParser.MeasureVoltageQueryContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight34465ASCPILexer.AC => ElectricityType.AC,
                Keysight34465ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };
            var (rage, resolution) = context.Accept(new Keysight34465AVoltageParameterVisitor());
            return Keysight34465ACommand.MeasureVoltage(electricityType, rage, resolution);
        }


        public override Result<Keysight34465ACommand> VisitDisplayTextCommand(Keysight34465ASCPIParser.DisplayTextCommandContext context)
        {
            var text = context.QuotedString().GetText();
            return Keysight34465ACommand.DisplayText(text.Substring(1, text.Length - 2));
        }

        public override Result<Keysight34465ACommand> VisitDisplayTextClearCommand(Keysight34465ASCPIParser.DisplayTextClearCommandContext context)
        {
            return Keysight34465ACommand.ClearDisplay;
        }

        public override Result<Keysight34465ACommand> VisitSenseVoltageImpedanceCommand(
            Keysight34465ASCPIParser.SenseVoltageImpedanceCommandContext context)
        {
            var impedance = context.@bool.Type switch
            {
                Keysight34465ASCPILexer.AutoTRUE => Impedance.High,
                Keysight34465ASCPILexer.AutoFALSE => Impedance.Low,
                _ => throw new VisitorTokenHandlerException("Unknown boolean type in sense voltage impedance command")
            };

            return Keysight34465ACommand.SetImpedance(impedance);
        }

        protected override Result<Keysight34465ACommand> DefaultResult { get; } =
            Result.Error<Keysight34465ACommand>("Default is no valid command");
    }
}