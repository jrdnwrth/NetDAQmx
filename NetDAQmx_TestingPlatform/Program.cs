using NetDAQmx;
using NetDAQmx.Helpers;

namespace NetDAQmx_TestingPlatform;

internal class Program
{
    /// <summary>
    /// This code was tested on these devices: NI-9481, NI-9485.
    /// </summary>
    static void Main(string[] args)
    {
        var connectedDevices = NIDAQ.GetSystemDevices();
        if (connectedDevices.Length == 0)
        {
            Console.WriteLine("There are no NI Daq Connected to this PC.");
            return;
        }

        uint relay = 7;                 // 0,1,2, or 3 for NI-9481.  0,1,2,3,4,5,6, or 7 for NI-9485.
        bool close = false;             // Set this to false to open the relay.
        NIDAQ daq = new(connectedDevices[0]); // Use NI-MAX to assign this name.

        // Open or close the relay.
        daq.WriteDOChannel(0, relay, close);
        
        // Read whether it is open or closed.
        bool is_closed = daq.IsLineOpen(0, relay);

        // Print the results.
        Console.WriteLine(is_closed ? "Closed" : "Open");
    }
}
