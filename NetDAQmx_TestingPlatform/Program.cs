using NetDAQmx;
using System;

namespace NetDAQmx_TestingPlatform;

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
