
# Open-SCPI-Protocol-Emulator

The Open-SCPI-Protocol-Emulator can be used to emulate Keysight measurement devices by implementing the SCPI protocol using TCP to enable integration tests in continuous integration pipelines or to allow developers to write software for Keysight devices without owning an expensive device. 

## Table of contents

- [Open-SCPI-Protocol-Emulator](#open-scpi-protocol-emulator)
  - [Table of contents](#table-of-contents)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Usage](#usage)
    - [Example configuration that starts two devices:](#example-configuration-that-starts-two-devices)
    - [Start from console with parameters](#start-from-console-with-parameters)
    - [Start from console without parameters](#start-from-console-without-parameters)
  - [Adding a new measurement device](#adding-a-new-measurement-device)
  - [Architecture](#architecture)
    - [Overview](#overview)
      - [EmulatorHost layer](#emulatorhost-layer)
      - [Protocol layer](#protocol-layer)
      - [ProtocolParser layer](#protocolparser-layer)
      - [Emulator layer](#emulator-layer)
      - [Domain layer](#domain-layer)
    - [Dependency graph](#dependency-graph)
    - [Control flow / data flow:](#control-flow--data-flow)
  - [Supported instruments/commands](#supported-instrumentscommands)
  - [Contributing](#contributing)
  - [Authors](#authors)
  - [License](#license)
  - [Acknowledgments](#acknowledgments)
  - [Todos](#todos)

## Prerequisites
- Visual Studio 2019
  - Plugin: [Switchyard](https://marketplace.visualstudio.com/items?itemName=bluehands.Switchyard)
  - Plugin: [AntlrVSIX](https://marketplace.visualstudio.com/items?itemName=KenDomino.AntlrVSIX) (very buggy; consider using the Intellij plugin instead: [antlr.org/tools](https://www.antlr.org/tools.html))
- Optional:
  - Rider with the ANTLR Intellij plugin
- .NET 5
- NuGet
- Git

## Installation

To install run:

```bash
  git clone https://github.com/bluehands/Open-SCPI-Protocol-Emulator.git
```

To restore NuGet packages navigate into the source directory and run:

```bash
  dotnet restore 
```

To build navigate into the source directory and run:

```bash
  dotnet build EmulatorHost.sln
```

## Usage

There are two primary ways to start the application:
1. Pass the configuration (devicesettings.json) from the console as an escaped json
2. Configure the devices inside the devicesettings.json and start the application parameterless (passing the devicesettings.json as parameter overwrites the file configuration)

Only devices with a valid and present configuration will be started. The root nodes for every registered device must be present inside the devicesettings.json but can be empty. Devices with empty root nodes do not start. 

### Example configuration that starts two devices:

```json
{
  "Keysight34465AConfiguration": {
    "Ip": "127.0.0.1",
    "Port": 5025,
    "Identification": "Keysight Technologies,34465A,21-BH-34465-EMULATOR,0.01.00-01.00-01.00-01.00-01-01",
    "VoltageInterferenceFactors": [
      0.0,
      0.001,
      0.002,
      0.001,
      0.0,
      -0.001,
      -0.002,
      -0.001
    ],
    "CurrentInterferenceFactors": [
      0.0,
      0.0001,
      0.0002,
      0.0001,
      0.0,
      -0.0001,
      -0.0002,
      -0.0001
    ],
    "LowImpedanceInterferenceMultiplier": 2.0,
    "HighImpedanceInterferenceMultiplier": 1.0,
    "CurrentRangeAuto": 1E-05,
    "CurrentRangeMin": 1E-05,
    "CurrentRangeMax": 1E-05,
    "CurrentRangeDef": 1E-05,
    "VoltageRangeAuto": 300.0,
    "VoltageRangeMin": 100.0,
    "VoltageRangeMax": 1000.0,
    "VoltageRangeDef": 500.0
  },
  "Keysight3458AConfiguration": {
    "Ip": "127.0.0.1",
    "Port": 5026,
    "Identification": "Keysight Technologies, 3458A-EMULATOR,0.01.00-01.00-01.00-01.00-01-01",
    "VoltageInterferenceFactors": [
      0.0,
      0.001,
      0.002,
      0.001,
      0.0,
      -0.001,
      -0.002,
      -0.001
    ],
    "CurrentInterferenceFactors": [
      0.0,
      0.0001,
      0.0002,
      0.0001,
      0.0,
      -0.0001,
      -0.0002,
      -0.0001
    ],
    "CurrentRangeAuto": 1E-05,
    "CurrentRangeMin": 1E-05,
    "CurrentRangeMax": 1E-05,
    "CurrentRangeDef": 1E-05,
    "VoltageRangeAuto": 300.0,
    "VoltageRangeMin": 100.0,
    "VoltageRangeMax": 1000.0,
    "VoltageRangeDef": 500.0
  }
}
```

### Start from console with parameters

Escape the devicesettings.json and then run:

```bash
  EmulatorHost.exe "{\r\n  \"Keysight34465AConfiguration\": {\r\n    \"Ip\": \"127.0.0.1\",\r\n    \"Port\": 5025,\r\n    \"Identification\": \"Keysight Technologies,34465A,21-BH-34465-EMULATOR,0.01.00-01.00-01.00-01.00-01-01\",\r\n    \"VoltageInterferenceFactors\": [\r\n      0.0,\r\n      0.001,\r\n      0.002,\r\n      0.001,\r\n      0.0,\r\n      -0.001,\r\n      -0.002,\r\n      -0.001\r\n    ],\r\n    \"CurrentInterferenceFactors\": [\r\n      0.0,\r\n      0.0001,\r\n      0.0002,\r\n      0.0001,\r\n      0.0,\r\n      -0.0001,\r\n      -0.0002,\r\n      -0.0001\r\n    ],\r\n    \"LowImpedanceInterferenceMultiplier\": 2.0,\r\n    \"HighImpedanceInterferenceMultiplier\": 1.0,\r\n    \"CurrentRangeAuto\": 1E-05,\r\n    \"CurrentRangeMin\": 1E-05,\r\n    \"CurrentRangeMax\": 1E-05,\r\n    \"CurrentRangeDef\": 1E-05,\r\n    \"VoltageRangeAuto\": 300.0,\r\n    \"VoltageRangeMin\": 100.0,\r\n    \"VoltageRangeMax\": 1000.0,\r\n    \"VoltageRangeDef\": 500.0\r\n  },\r\n  \"Keysight3458AConfiguration\": {\r\n    \"Ip\": \"127.0.0.1\",\r\n    \"Port\": 5026,\r\n    \"Identification\": \"Keysight Technologies, 3458A-EMULATOR,0.01.00-01.00-01.00-01.00-01-01\",\r\n    \"VoltageInterferenceFactors\": [\r\n      0.0,\r\n      0.001,\r\n      0.002,\r\n      0.001,\r\n      0.0,\r\n      -0.001,\r\n      -0.002,\r\n      -0.001\r\n    ],\r\n    \"CurrentInterferenceFactors\": [\r\n      0.0,\r\n      0.0001,\r\n      0.0002,\r\n      0.0001,\r\n      0.0,\r\n      -0.0001,\r\n      -0.0002,\r\n      -0.0001\r\n    ],\r\n    \"CurrentRangeAuto\": 1E-05,\r\n    \"CurrentRangeMin\": 1E-05,\r\n    \"CurrentRangeMax\": 1E-05,\r\n    \"CurrentRangeDef\": 1E-05,\r\n    \"VoltageRangeAuto\": 300.0,\r\n    \"VoltageRangeMin\": 100.0,\r\n    \"VoltageRangeMax\": 1000.0,\r\n    \"VoltageRangeDef\": 500.0\r\n  }\r\n}"
```

### Start from console without parameters

Edit the devicesettings.json and then run:

```bash
  EmulatorHost.exe 
```

## Adding a new measurement device

To add another measurement device the following steps are required:

1. Create a new device inside *Domain/YourDevice* that implements the desired functionalities and states. Consider extending the *Domain/KeysightBase/KeysightDeviceBase* and using or creating new *UnionTypes*. Use the [Switchyard](https://github.com/bluehands/Switchyard) project to genrate *UnionTypes* from enums. You can download the VS plugin from here: [Switchyard](https://marketplace.visualstudio.com/items?itemName=bluehands.Switchyard).
2. Create a configuration scheme for the new device that implements the *Domain/Abstractions/IDeviceConfiguration* interface inside *Domain/YourDevice* and add your device configuration class to the *EmulatorHost/Configuration/DeviceConfigurations* class.
3. Create a new device controller inside the *Emulator/Controller* project for the new measurement device that implements the *Emulator/Controller/IDeviceController\<TDeviceCommand\>* where *TDeviceCommand* is the parsed representation of an SCPI command.
4. Create a new command UnionType inside *Emulator/Command*
5. Create a new parser grammar for the desired (SCPI) inside *ProtocolParser/YourDevice* commands and build the parser. Check the XML project file to see how to configure the MSBuild to generate an ANTLR parser. [antlr.org](https://www.antlr.org/). Also consider installing a syntax highlighting plugin for ANTLR v4 grammars (available for Rider and VS; Note: the Rider plugin is much better!).
6. Create a new visitor for the parser / parser grammar inside *Protocol/YourDevice*
7. Create a new protocol interpreter inside *Protocol/Interpreter* that implements the *Protocol/Interpreter/IProtocolInterpreter*
8. Register your created services inside the *EmulatorHost/Program* by using the *EmulatorHost/HostedDeviceService*. Follow the existing scheme.
9. Have fun!

## Architecture

The architecture of the application follows the rules of an [Onion Architekture](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/).
The basic rule is that no layer must reference an outer layer of the onion structure but outer layer may reference inner layers.

### Overview

The following sections describe the layers from the outside toward the inside of the onion architecture, which can also be seen in the dependency graph.

#### EmulatorHost layer

The outer layer ist the *EmulatorHost* layer. It composes all other layers together and is responsible for the startup and configuration of the application. 
It also contains the dependency injection system, which means that new components, services or devices are registered there.

Project structure:

- *EmulatorHost*
  - *EmulatorHostService*: composes all parts for a device together and needs to be registered in the DI container
  - *Configuration*
    - Contains the configuration deserializer and *DeviceConfiguration* schema
  - *Network*
    - Contains the *TCPServer* and a fsm for escaping the protocol separator

#### Protocol layer

The *Protocol* layer is responsible for interpreting and creating command objects from the incoming sequences, that can be passed to the controllers inside the *Emulator* layer.
The *Protocol* layer references the *ProtocolParser* layer, which contains the ANTLR SCPI parsers.

Project structure:

- *Protocol*
  - *Interpreter*
    - Contains the device protocol interpreters
  - *Keysight34465A*
    - Contains AST visitors for the Keysight34465A SCPI parser
  - *Keysight3458*
    - Contains AST visitors for the Keysight3458 SCPI parser

#### ProtocolParser layer

The *ProtocolParser* layer contains the ANTLR SCPI grammar and the resulting generated parser for each device.

Project structure:

- *ProtocolParser*
  - *Keysight34465A*
    - Contains grammar and generated parser for the Keysight34465A device
  - *Keysight3458*
    - Contains grammar and generated parser for the Keysight3458 device

#### Emulator layer

The *Emulator* layer is responsible for composing the functionalities of the *Domain* layer and contains a controller for every device, that accepts device specific commands and executes them. A controller also returns a execution result.

Project structure:

- *Emulator*
  - *Command*
    - Contains the commands an some execution logic for commands. The *CommandExecutor* is used to buffer the data output from the execution and provide it to the socket server.
  - *Controller*
    - Contains the controllers that compose the domain logic

#### Domain layer

The *Domain* layer contains the logic for the behavior of a device and some domain entities.

Project structure:

- *Domain*
  - *Abstractions*
    - Contains Interfaces for the configuration and SocketServer
  - *Keysight34465A*
    - Contains the logic for the device and configuration
  - *Keysight3458A*
    - Contains the logic for the device and configuration
  - *KeysightBase*
    - Contains some shared logic
  - *UnionTypes*
    - Contains the domain entities used by the devices and commands

### Dependency graph

![graph](https://github.com/bluehands/Open-SCPI-Protocol-Emulator/blob/develop/Docs/EmulatorArchitecture.png)

### Control flow / data flow:

1. A TCP port is opened for every device by the *HostedDeviceService*
2. The *TCPServer* accepts requests and splits them at the SCPI separator "\n" using an fsm
3. The *HostedDeviceService* subscribes to the *TCPServer* and passes incoming sequences to the *ProtocolInterpreter*
4. The *ProtocolInterpreter* passes the sequence to the device specific parser
5. The *Parser* generates an syntax tree which is passed to the device specific *Visitor* by the *ProtocolInterpreter*
6. The *Visitor* visits the syntax tree and generates a device specific command containing parameters
7. The *HostedDeviceService* passes the command to a *CommandExecutor* that contains an output queue. *TCPServer* of a device polls this output queue and sends the data just like a real device
8. The *CommandExecutor* passes the command together with the output queue to the device specific *Controller*
9. The *Controller* matches the command depending on the type and calls the domain logic of a device. The *Controller* also pushes output values generated by the domain to the output queue
10. The *Device* accepts the parameters of a command and applies state changes. The device also generates the output values that are pushed to the output queue
11. The *HostedDeviceService* writes the execution result of a *Controller* to the console
12. The *TCPServer* server as already mentioned pools the output queue and sends the generated values

## Supported instruments/commands

Long form:

- Keysight-34465A
  - *IDN?
  - READ?
  - ABORt
  - CONFigure:CURRent:AC
  - CONFigure:CURRent:DC
  - MEASure:CURRent:AC?
  - MEASure:CURRent:DC?
  - CONFigure:VOLTage:AC
  - CONFigure:VOLTage:DC
  - MEASure:VOLTage:AC?
  - MEASure:VOLTage:DC?
  - DISPlay:TEXT
  - DISPlay:TEXT:CLEar
  - SENSe:VOLTage:IMPedance:AUTO
- Keysight-3458
  - *IDN?
  - READ?
  - ABORt
  - CONFigure:CURRent:AC
  - CONFigure:CURRent:DC
  - MEASure:CURRent:AC?
  - MEASure:CURRent:DC?
  - CONFigure:VOLTage:AC
  - CONFigure:VOLTage:DC
  - MEASure:VOLTage:AC?
  - MEASure:VOLTage:DC?

## Contributing

We're looking forward for your pull requests.

## Authors

[bluehands.de](bluehands.de)

## License

MIT License

## Acknowledgments

- [antlr.org](https://www.antlr.org/)
- [github antlr4](https://github.com/antlr/antlr4)
- [ANTLR Handout TU-Darmstadt](https://www.esa.informatik.tu-darmstadt.de/archive/twiki/pub/Lectures/Compiler112De/antlr-handout.pdf)
- [Onion Architekture](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)

## Todos

- Transform the ANTLR grammar to a general grammar that describes the SCPI protocol as the current grammar only supports a small subset of the protocol, which is not quite good extensible.
- Produce an AST instead of just a CST to remove duplicate visitors for every device.
- Extend the functionalities of the existing devices (only command used for our purpose are implemented)
- Extend the tests