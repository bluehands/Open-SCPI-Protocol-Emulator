using Antlr4.Runtime.Misc;
using Domain.UnionTypes;
using Emulator.Command;
using FunicularSwitch;
using ProtocolParser.Keysight3458A;

namespace Protocol.Keysight3458
{
    public class Keysight3458ACommandVisitor : Keysight3458ASCPIBaseVisitor<Result<Keysight3458ACommand>>
    {
        public override Result<Keysight3458ACommand> VisitCommand(Keysight3458ASCPIParser.CommandContext context)
        {
            // skip EOF
            return context.GetChild(0).Accept(this);
        }

        public override Result<Keysight3458ACommand> VisitAbortCommand([NotNull] Keysight3458ASCPIParser.AbortCommandContext context)
        {
            return Keysight3458ACommand.Abort;
        }

        public override Result<Keysight3458ACommand> VisitIdentificationQuery(
            [NotNull] Keysight3458ASCPIParser.IdentificationQueryContext context)
        {
            return Keysight3458ACommand.Identification;
        }

        public override Result<Keysight3458ACommand> VisitReadQuery([NotNull] Keysight3458ASCPIParser.ReadQueryContext context)
        {
            return Keysight3458ACommand.Read;
        }

        public override Result<Keysight3458ACommand> VisitConfigureCurrentCommand(
            [NotNull] Keysight3458ASCPIParser.ConfigureCurrentCommandContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight3458ASCPILexer.AC => ElectricityType.AC,
                Keysight3458ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };
            var (rage, resolution) = context.Accept(new Keysight3458ACurrentParameterVisitor());
            return Keysight3458ACommand.ConfigureCurrent(electricityType, rage, resolution);
        }

        public override Result<Keysight3458ACommand> VisitConfigureVoltageCommand(Keysight3458ASCPIParser.ConfigureVoltageCommandContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight3458ASCPILexer.AC => ElectricityType.AC,
                Keysight3458ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };
            var (rage, resolution) = context.Accept(new Keysight3458AVoltageParameterVisitor());
            return Keysight3458ACommand.ConfigureVoltage(electricityType, rage, resolution);
        }


        public override Result<Keysight3458ACommand> VisitMeasureCurrentQuery(
            [NotNull] Keysight3458ASCPIParser.MeasureCurrentQueryContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight3458ASCPILexer.AC => ElectricityType.AC,
                Keysight3458ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };


            var (rage, resolution) = context.Accept(new Keysight3458ACurrentParameterVisitor());
            return Keysight3458ACommand.MeasureCurrent(electricityType, rage, resolution);
        }

        public override Result<Keysight3458ACommand> VisitMeasureVoltageQuery(Keysight3458ASCPIParser.MeasureVoltageQueryContext context)
        {
            var electricityType = context.electricityType.Type switch
            {
                Keysight3458ASCPILexer.AC => ElectricityType.AC,
                Keysight3458ASCPILexer.DC => ElectricityType.DC,
                _ => throw new VisitorTokenHandlerException("Wrong electricity type")
            };
            var (rage, resolution) = context.Accept(new Keysight3458AVoltageParameterVisitor());
            return Keysight3458ACommand.MeasureVoltage(electricityType, rage, resolution);
        }

        protected override Result<Keysight3458ACommand> DefaultResult { get; } =
            Result.Error<Keysight3458ACommand>("Default is no valid command");
    }
}