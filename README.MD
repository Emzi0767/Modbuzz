# Modbuzz - a Modbus slave emulator for testing purposes.
Modbuzz is a simple utility which emulates a Modbus slave. Currently only RTU is implemented.

## Running the program
The pre-packaged versions of the program support Windows 10 x64 (or newer) and Debian 10 arm64 (or newer). Support is
not guaranteed for other platforms. To run the program on other Linux distributions, consult the
[RCJP.SerialPortStream][0] readme.

The program takes 5 arguments:
1. Serial port (e.g. `COM3` on Windows or `/dev/ttyUSB0` on Linux)
2. Baud rate (e.g. `9600`)
3. Data bits (e.g. `8`, but values 6-8 are allowed)
4. Parity (e.g. `None`, but `None`, `Even`, `Odd`, `Mark`, and `Space` are allowed, not case-sensitive)
5. Stop bits (e.g. `One`, but `One`, `One5`, and `Two` are allowed, not case-sensitive)

It then opens the serial port and starts responding to Modbus requests from a master. The emulated device parameters
are as follows:
- Slave ID: 192 (`0xC0`)
- Registers:
  - Coil:
    - 10000 (`0x2710`, PLC: 10001): Enable random number generator
    - 10001 (`0x2711`, PLC: 10002): Enable logging (currently has no effect)
  - Discrete input:
    - 40000 (`0x9C40`, PLC: 40001): Current random number is greater than the last one
    - 40001 (`0x9C41`, PLC: 40002): Counter value overflowed on last tick
  - Holding register:
    - 20000 (`0x4E20`, PLC: 20001): Random lower bound
    - 20001 (`0x4E21`, PLC: 20002): Random upper bound
    - 20002 (`0x4E22`, PLC: 20003): Random range (if set to non-zero value, will cause the next random to be within
      this much of the last)
    - 20003-20004 (`0x4E23`-`0x4E24`, PLC: 20004-20005), size 2: 32-bit number register
  - Input:
    - 30000 (`0x7530`, PLC: 30001): Last random number generated
    - 30001 (`0x7531`, PLC: 30002): Request counter
    - 30002-30003 (`0x7532`-`0x7533`, PLC: 30003-30004): 32-bit random number

Registers 30000, 30001, 30002, and 30003 are updated on each read of 30000. If the value of 10000 is 0, value of 30000
will not change.

The program discards repeated consecutive reads to the same address. If you want to read a register twice, issue
another read or write command to another register in-between.

## Building the program
You can build the program in 2 flavours: self-contained single-file trimmed .NET application, or a .NET AOT binary. To
deploy as the latter, pass `-p:AOT_RELEASE=1` to `dotnet publish`.

For Linux, you must first compile native libraries for [RJCP.SerialPortStream][0]. Consult the project's readme for
instructions.

To build a deployable executable, use `dotnet publish`. For example:

`dotnet publish -c Release -r linux-arm64 -o publish-linux-arm64/ src/Emzi0767.Modbuzz.Feeder`

The above command will create a Linux AArch64 (ARM64) binary for glibc-based systems. For Runtime IDs for other
platforms, consult [the .NET documentation][1].


[0]: https://github.com/jcurl/RJCP.DLL.SerialPortStream
[1]: https://learn.microsoft.com/en-us/dotnet/core/rid-catalog
