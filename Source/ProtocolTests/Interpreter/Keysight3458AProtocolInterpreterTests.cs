using System;
using Domain.UnionTypes;
using Emulator.Command;
using FluentAssertions;
using FunicularSwitch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Protocol.Interpreter;
using Range = Domain.UnionTypes.Range;

// ReSharper disable InconsistentNaming

namespace ProtocolTests.Interpreter
{
    [TestClass]
    public class Keysight3458AProtocolInterpreterTests
    {
        [TestMethod]
        public void Test_SCPI_IDN()
        {
            AssertCommandType<Keysight3458ACommand.Identification_>("*IDN?");
        }

        [TestMethod]
        public void Test_SCPI_Read_Query()
        {
            AssertCommandType<Keysight3458ACommand.Read_>("READ?");
        }

        [TestMethod]
        public void Test_SCPI_Abort_Command()
        {
            AssertCommandType<Keysight3458ACommand.Abort_>("ABORt");
            AssertCommandType<Keysight3458ACommand.Abort_>("ABOR");
        }

        [TestMethod]
        public void Test_SCPI_ConfigureCurrent_Command()
        {
            //Test long form and short form with ac and dc
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONF:CURR:AC").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONF:CURR:DC").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:DC").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            // Test defaults
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONF:CURR:DC").Range.Should().BeOfType<None<Range>>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:DC").Resolution.Should().BeOfType<None<Resolution>>();
            //Test parameters
            //Test range <AUTO DEF MIN MAX>
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC AUTO").Range.AssertOptionType<Range, Range.Auto_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC     AUTO").Range.AssertOptionType<Range, Range.Auto_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC MIN").Range.AssertOptionType<Range, Range.Min_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC     MIN").Range.AssertOptionType<Range, Range.Min_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC MAX").Range.AssertOptionType<Range, Range.Max_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC     MAX").Range.AssertOptionType<Range, Range.Max_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC DEF").Range.AssertOptionType<Range, Range.Def_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC     DEF").Range.AssertOptionType<Range, Range.Def_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC     1").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1.0").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1.0e2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1.0E2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1.0e+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1.0E+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1.0e-2").AssertRange(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1.0E-2").AssertRange(1.0 * Math.Pow(10, -2));
            //Test resolution
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,MIN").Resolution.AssertOptionType<Resolution, Resolution.Min_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,MAX").Resolution.AssertOptionType<Resolution, Resolution.Max_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,DEF").Resolution.AssertOptionType<Resolution, Resolution.Def_>();
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1.0").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1.0e2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1.0E2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1.0e+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1.0E+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1.0e-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.ConfigureCurrent_>("CONFigure:CURRent:AC 1,1.0E-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
            AssertError(" CONF:CURR:AC");
            AssertError("CONF :CURR:AC");
            AssertError("CONF: CURR:AC");
            AssertError("CONF:CURR :AC");
            AssertError("CONF:CURR: AC");
            AssertError("CONF:CURR:AC ");
            AssertError("CONF:CURR:BC");
            AssertError("CONF:CURR:DC .0,.0");
            AssertError("CONF:CURR:DC0, 0");
            AssertError("CONF:CURR:DC -0, -0");
            AssertError("CONF:CURR:DC -1,-1"); //todo implement negative numbers?
            AssertError("CONF:CURR:DC 1.1.1,1.1.1");
        }

        [TestMethod]
        public void Test_SCPI_MeasureCurrent_Query()
        {
            //Test long form and short form with ac and dc
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEAS:CURR:AC?").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC?").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURR:DC?").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:DC?").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            // Test defaults
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:DC?").Range.Should().BeOfType<None<Range>>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:DC?").Resolution.Should().BeOfType<None<Resolution>>();
            //Test parameters
            //Test range <AUTO DEF MIN MAX>
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? AUTO").Range.AssertOptionType<Range, Range.Auto_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? MIN").Range.AssertOptionType<Range, Range.Min_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? MAX").Range.AssertOptionType<Range, Range.Max_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? DEF").Range.AssertOptionType<Range, Range.Def_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1.0").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1.0e2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1.0E2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1.0e+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1.0E+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1.0e-2").AssertRange(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1.0E-2").AssertRange(1.0 * Math.Pow(10, -2));
            //Test resolution
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,MIN").Resolution.AssertOptionType<Resolution, Resolution.Min_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,MAX").Resolution.AssertOptionType<Resolution, Resolution.Max_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,DEF").Resolution.AssertOptionType<Resolution, Resolution.Def_>();
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1.0").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1.0e2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1.0E2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1.0e+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1.0E+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1.0e-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.MeasureCurrent_>("MEASure:CURRent:AC? 1,1.0E-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
        }

        [TestMethod]
        public void Test_SCPI_ConfigureVoltage_Command()
        {
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONF:VOLT:AC").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONF:VOLT:DC").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:DC").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            // Test defaults
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONF:VOLT:DC").Range.Should().BeOfType<None<Range>>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:DC").Resolution.Should().BeOfType<None<Resolution>>();
            //Test parameters
            //Test range <AUTO DEF MIN MAX>
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC AUTO").Range.AssertOptionType<Range, Range.Auto_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC MIN").Range.AssertOptionType<Range, Range.Min_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC MAX").Range.AssertOptionType<Range, Range.Max_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC DEF").Range.AssertOptionType<Range, Range.Def_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1.0").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1.0e2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1.0E2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1.0e+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1.0E+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1.0e-2").AssertRange(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1.0E-2").AssertRange(1.0 * Math.Pow(10, -2));
            //Test resolution
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,MIN").Resolution.AssertOptionType<Resolution, Resolution.Min_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,MAX").Resolution.AssertOptionType<Resolution, Resolution.Max_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,DEF").Resolution.AssertOptionType<Resolution, Resolution.Def_>();
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1.0").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1.0e2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1.0E2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1.0e+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1.0E+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1.0e-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.ConfigureVoltage_>("CONFigure:VOLTage:AC 1,1.0E-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
        }
        
        [TestMethod]
        public void Test_SCPI_MeasureVoltage_Command()
        {
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEAS:VOLT:AC?").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC?").ElectricityType.Should().BeOfType<ElectricityType.AC_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEAS:VOLT:DC?").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:DC?").ElectricityType.Should().BeOfType<ElectricityType.DC_>();
            // Test defaults
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEAS:VOLT:DC?").Range.Should().BeOfType<None<Range>>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:DC?").Resolution.Should().BeOfType<None<Resolution>>();
            //Test parameters
            //Test range <AUTO DEF MIN MAX>
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? AUTO").Range.AssertOptionType<Range, Range.Auto_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? MIN").Range.AssertOptionType<Range, Range.Min_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? MAX").Range.AssertOptionType<Range, Range.Max_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? DEF").Range.AssertOptionType<Range, Range.Def_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1.0").AssertRange(1);
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1.0e2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1.0E2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1.0e+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1.0E+2").AssertRange(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1.0e-2").AssertRange(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1.0E-2").AssertRange(1.0 * Math.Pow(10, -2));
            //Test resolution
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,MIN").Resolution.AssertOptionType<Resolution, Resolution.Min_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,MAX").Resolution.AssertOptionType<Resolution, Resolution.Max_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,DEF").Resolution.AssertOptionType<Resolution, Resolution.Def_>();
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1.0").AssertRange(1).AssertResolution(1);
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1.0e2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1.0E2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1.0e+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1.0E+2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, 2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1.0e-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
            AssertCommandType<Keysight3458ACommand.MeasureVoltage_>("MEASure:VOLTage:AC? 1,1.0E-2").AssertRange(1).AssertResolution(1.0 * Math.Pow(10, -2));
        }
        
        private static T AssertCommandType<T>(string input)
        {
            var commands = new Keysight3458AProtocolInterpreter().GetCommand(input);
            return commands.Match(ok => ok.Should().BeOfType<T>()
                .Which,
                error => throw new Exception(error));
        }
        
        private static void AssertError(string input)
        {
            new Keysight3458AProtocolInterpreter().GetCommand(input).Should().BeOfType<Error<Keysight3458ACommand>>();
        }

    }

    public static class Keysight3458AAssertionExtensions
    {
        public static T AssertRange<T>(this T command, double expected)
            where T : Keysight3458ACommand
        {
            var range = command switch
            {
                Keysight3458ACommand.ConfigureCurrent_ configureCurrent => configureCurrent.Range,
                Keysight3458ACommand.MeasureCurrent_ measureCurrent => measureCurrent.Range,
                Keysight3458ACommand.ConfigureVoltage_ configureVoltage => configureVoltage.Range,
                Keysight3458ACommand.MeasureVoltage_ measureVoltage => measureVoltage.Range,
                _ => throw new Exception("Missing type case in AssertRange")
            };
            range.AssertOptionType<Range, Range.Number_>()
                .Value.Should().Be(expected);
            return command;
        }

        public static T AssertResolution<T>(this T command, double expected)
            where T : Keysight3458ACommand
        {
            var range = command switch
            {
                Keysight3458ACommand.ConfigureCurrent_ configureCurrent => configureCurrent.Resolution,
                Keysight3458ACommand.MeasureCurrent_ measureCurrent => measureCurrent.Resolution,
                Keysight3458ACommand.ConfigureVoltage_ configureVoltage => configureVoltage.Resolution,
                Keysight3458ACommand.MeasureVoltage_ measureVoltage => measureVoltage.Resolution,
                _ => throw new Exception("Missing type case in AssertResolution")
            };
            range.AssertOptionType<Resolution, Resolution.Number_>()
                .Value.Should().Be(expected);
            return command;
        }
    }
}