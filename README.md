# MT4_To_CSharp_Bridge_Using_ZeroMQ
A prototype for connecting unmanaged code in Metatrader 4 (MQL) to managed code (C#) in modern .NET (6+) using ZeroMQ

## MQL4 Example
### Dependencies
mql-zmq https://github.com/GroM/mql-zmq

### Installation in MT4
Copy from `mql-zmq/Include` directories `zmq` and `mql` into MT4 `MQL4/Include`
Copy files from `mql-zmq/Libraries` into MT4 `MQL4/Libraries`
Copy `BridgeClient.mq4` into `MQL4/Experts` folder

### Usage
Sending to server is done through function
`bool SendCommand(ZmqMsg& response, string& data[])`

Commands (messages to server) are structured as array of strings.
First element is the name of command to execute.
Next ones are optional and are arguments for this command. String representation should be parsable into that type.

Response is `ZmqMsg` type from library. There's also function `ShowResponse` which displays received response.
response.getData() is a way to access returned value from server.

## Server part
Solution is divided into 4 projects.
- Library2Expose is a simple project that is used as example for exposing
- BridgeLibrary is the core
- BridgeConsole is a simple CLI acting as server
- BridgeGenerator is a simple generator which can prepare `MethodAdapters.cs` and `MethodAdaptersList.cs` (those inside project was generated using it).

### Defining commands
To execute commands from external libraries (which could be even without source code - managed dll) there's some kind of adapter.
Command is defined as delegate taking array of string and returning string. 
`public delegate string Cmd(string[] input)`
Argument `input` is a list of arguments for a method from an external library. 
Return value is a result from a method from an external library.

#### Default behavior from generator
Generator is going through all public types defined in dll. For this types gets all exported methods (instance and static, which are public), for which it generates adapter function (command).
Gets argument types and prepares code which parses string into that type (uses `TryParse`). In case of errors, returns messages starting with `ERR` which describes problem.
Then executes function and return value from this function is converted to strinng (uses `ToString`). There's a special case for `void` return type - in that case return string is 'OK'.

### List of commands
There're 2 types of commands base and custom (external), which are holden in dictionaries.
When receiving command it checks that this command was defined. Check order is following: base, custom.
When it finds it executes this command. In case there's no command with that name returns to client `ERR: Unknown command`.
