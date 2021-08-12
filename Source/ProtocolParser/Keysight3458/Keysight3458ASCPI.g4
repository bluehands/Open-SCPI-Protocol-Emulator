grammar Keysight3458ASCPI; // Keysight3458A

command
	: identificationQuery EOF
	| readQuery EOF
	| abortCommand EOF
	| configureCurrentCommand EOF
	| measureCurrentQuery EOF
	| configureVoltageCommand EOF
	| measureVoltageQuery EOF
	;

// ------------------------------------ Base Command/Queries ------------- 
identificationQuery
	: '*IDN?';

readQuery
	: 'READ?';

abortCommand
	: 'ABOR'
	| 'ABORt';

configureCurrentCommand
	: ConfigureCurrent electricityType=(AC | DC)
	| ConfigureCurrent electricityType=(AC | DC) Space currentParameters;

configureVoltageCommand
	: ConfigureVoltage electricityType=(AC | DC)
	| ConfigureVoltage electricityType=(AC | DC) Space voltageParameters;
	
measureCurrentQuery
	: MeasureCurrent electricityType=(AC | DC) QuestionMark
	| MeasureCurrent electricityType=(AC | DC) QuestionMark Space currentParameters;
	
measureVoltageQuery
	: MeasureVoltage electricityType=(AC | DC) QuestionMark
	| MeasureVoltage electricityType=(AC | DC) QuestionMark Space voltageParameters;

// ------------------------------------- Parameters ------------- 

currentParameters
	: range=(Number | AUTO | MIN | MAX | DEF)
	| range=(Number | AUTO | MIN | MAX | DEF) CommaSeparator resolution=(Number | MIN | MAX | DEF);

voltageParameters
	: range=(Number | AUTO | MIN | MAX | DEF)
	| range=(Number | AUTO | MIN | MAX | DEF) CommaSeparator resolution=(Number | MIN | MAX | DEF);

// ------------------------------------ Command/Query prefixes ------------- 

ConfigureVoltage
	: ConfigureSubsystem Voltage Colon
	| ConfigureSubsystem Colon;
	
ConfigureCurrent
	: ConfigureSubsystem Current Colon;

MeasureCurrent
	: MeasureSubsystem Current Colon; 
	
MeasureVoltage
	: MeasureSubsystem Voltage Colon;

// ------------------------------------ Subsystems -------------

fragment ConfigureSubsystem
	: 'CONFigure'
	| 'CONF';
	
fragment MeasureSubsystem
	: 'MEASure'
	| 'MEAS';

// ------------------------------------ SecoundLevel Subystems -------------

fragment Current
	: Colon 'CURR'
	| Colon 'CURRent';
	
fragment Voltage
	: Colon 'VOLTage'
	| Colon 'VOLT';
	
fragment Text
	: Colon 'TEXT';
	
fragment Impedance
	: Colon 'IMP'
	| Colon 'IMPedance';

// ------------------------------------ Tokens ------------- 

AC: 'AC';
DC: 'DC';

AUTO: 'AUTO';
MIN: 'MIN';
MAX: 'MAX';
DEF: 'DEF';

fragment SingleSpace: ' ';
Space: SingleSpace+;

fragment Comma: ',';
CommaSeparator: Space? Comma Space?;

QuestionMark: '?';

fragment Colon
	: ':';

// ------------------------------------ Number ------------- 
Number
	: DecimalDigits
	| DecimalDigits '.' DecimalDigits DecimalExponent?;

fragment DecimalExponent : ('e' | 'E' | 'e+' | 'E+' | 'e-' | 'E-') ('0'..'9')+;
fragment DecimalDigits   : ('0'..'9')+;
