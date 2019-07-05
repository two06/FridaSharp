# FridaSharp
C# Wrapper around frida.dll

## Building Frida.dll
Follow the the instructions [here](https://www.frida.re/docs/building/). Building in release mode should produce smaller files (configurable in the solution build properties menu).

## Building FridaSharp
Load the project in Visual Studio and build. 

## Usage
Run the FridaSharp exectuable and provide the path the JS file containing your instrumentation and the process name to hook.
