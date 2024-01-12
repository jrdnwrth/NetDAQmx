# UPDATE!

The NetDAQmx repository has moved!  It now lives here:

[https://github.com/BenMendel/NetDAQmx] (https://github.com/BenMendel/NetDAQmx)

# NetDAQmx

This project provides a minimalistic C# wrapper around the NI-DAQmx driver.

## Example

```C#
using NetDAQmx;

internal class Program
{
	/// <summary>
	/// This code was tested on these devices: NI-9481, NI-9485.
	/// </summary>
	static void Main(string[] args)
	{
		int relay = 1;                // 0,1,2, or 3 for NI-9481.  0,1,2,3,4,5,6, or 7 for NI-9485.
		bool close = true;            // Set this to false to open the relay.
		string device_name = "Dev1";  // Use NI-MAX to assign this name.

		// Open or close the relay.
		Daq.actuate_relay(device_name, close, relay);

		// Read whether it is open or closed.
		bool is_closed = Daq.is_relay_closed(device_name, relay);

		// Print the results.
		Console.WriteLine(is_closed ? "Closed" : "Open" );
	}
}
```


## Tips for Further Development

* The NI-DAQmx dll is actually named `nicaiu.dll`.
* The NIDAQmx.h file defines all the function signatures for NI-DAQ.
* I would have included it in this project except that it is copyright by National Instruments.
* You can find NIDAQmx.h in either of the following directories:
    * C:\Program Files (x86)\National Instruments\Shared\ExternalCompilerSupport\C\include
    * C:\Program Files (x86)\National Instruments\NI-DAQ\DAQmx ANSI C Dev\include
* The fastest way to implement more functions is to copy them to ChatGPT and ask it to write the C# P/Invoke functions.
* Fun Fact: The Python library `pydaqmx` actually finds this file on your computer and dynamically 
builds all the functions from it.

