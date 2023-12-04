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

        SetMasterAddLatch(5, daq.DeviceAlias);
        //daq.WritePort(1, 0b0101);

        //// Open or close the relay.
        //daq.WriteDOChannel(0, relay, close);
    }

    private static void SetMasterAddLatch(byte data, string deviceName)
    {
        if (data > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Maximum is 7");
        }

        using DaqTask masterAddLatchTask = new();
        NIDAQ.DAQmxCreateDOChan(masterAddLatchTask, $"{deviceName}/port1/line3", DllWrapper.DAQmxLineGrouping.ChanPerLine);
        NIDAQ.WriteDOSingleLine(deviceName, 1, 3, false); // setting p1.3 to ON

        using DaqTask writeData = new();
        NIDAQ.DAQmxCreateDOChan(writeData, $"{deviceName}/port0", DllWrapper.DAQmxLineGrouping.ChanForAllLines);
        NIDAQ.DAQmxWriteDigitalU8(writeData, 1, true, 1, DllWrapper.DAQmxDataLayout.GroupByChannel, new[] { data }, out int samplesWritten);

        NIDAQ.WriteDOSingleLine(deviceName, 1, 3, true); // setting p1.3 to OFF
    }
}
