grammar Keysight34465ASCPI;

// ------------------------------------ Entry ------------- 

command
    : identificationQuery EOF
    | readQuery EOF
    | abortCommand EOF
    | configureCurrentCommand EOF
    | measureCurrentQuery EOF
    | configureVoltageCommand EOF
    | measureVoltageQuery EOF
    | displayTextCommand EOF
    | displayTextClearCommand EOF
    | senseVoltageImpedanceCommand EOF
    ;


// ------------------------------------ Base Command/Queries ------------- 
identificationQuery
    : '*IDN?';

readQuery
    : 'READ?';

abortCommand
    : 'ABOR'
    | 'ABORt';

// ------------------------------------ Advanced Commands/Queries -------------  

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
    
displayTextCommand
	: DisplayText Space QuotedString;
	
displayTextClearCommand
	: DisplayTextClear;

senseVoltageImpedanceCommand
	: SenseVoltageImpedance bool=(AutoTRUE | AutoFALSE);
    
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
	
DisplayText
	: DisplaySubsystem Text Data?;

DisplayTextClear
	: DisplaySubsystem Text Clear;

SenseVoltageImpedance
	: SenseVoltageSubsystem (Colon DC)? Impedance;

// ------------------------------------ Subsystems -------------

fragment ConfigureSubsystem
    : 'CONFigure'
    | 'CONF';
    
fragment MeasureSubsystem
    : 'MEASure'
    | 'MEAS';

fragment DisplaySubsystem
	: 'DISP'
	| 'DISPlay';
	
fragment SenseVoltageSubsystem
	: Sense? 'VOLT'
	| Sense? 'VOLTage';

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

// ------------------------------------ ThierdLevel Subystems -------------

fragment Data
	: Colon 'DATA';

fragment Clear
	: Colon 'CLE'
	| Colon 'CLEar';
	
// ------------------------------------ Subsystem Fragments -------------

fragment Sense
	: 'SENS' Colon
	| 'SENSe' Colon;

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

// ------------------------------------ Auto Bool -------------

AutoTRUE: Colon AUTO Space ('ON' | '1');
AutoFALSE: Colon AUTO Space ('OFF' | '0');

// ------------------------------------ Number ------------- 
Number
    : DecimalDigits
    | DecimalDigits '.' DecimalDigits DecimalExponent?;

fragment DecimalExponent : ('e' | 'E' | 'e+' | 'E+' | 'e-' | 'E-') ('0'..'9')+;
fragment DecimalDigits   : ('0'..'9')+;

// ------------------------------------ QuotedString ------------- 
QuotedString
	: DoubleQuote AnyLazy DoubleQuote;

fragment DoubleQuote: '"';
fragment AnyLazy: .*?;



