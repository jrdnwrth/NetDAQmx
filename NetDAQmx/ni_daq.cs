using System.Runtime.InteropServices;
using System.Text;

namespace NetDAQmx;

/// <summary>
/// The `Daq` functions usually operate on a "Task" handle.  This handle should be closed when finished.
/// To ensure that it will be closed, we created this IDisposable object.
/// </summary>
public class Daq_Task : IDisposable
{
    public IntPtr handle;
    public Daq_Task()
    {
        var status = Dll_Wrapper.DAQmxCreateTask("", out handle);
        Daq.throw_error(status);
    }

    /// <summary>
    /// Fun Fact!  Dispose is not called in debug mode.  However, it does work when we 'Start Without Debugging'
    /// https://stackoverflow.com/questions/518352/does-dispose-still-get-called-when-exception-is-thrown-inside-of-a-using-stateme
    /// </summary>
    public void Dispose()
    {
        var status = Dll_Wrapper.DAQmxClearTask(handle);
        Daq.throw_error(status);
    }
}

/// <summary>
/// This would not be much of a library if the user had to call the raw DLL functions.
/// This class provides more user-friendly functions for interacting with NI-DAQ.
/// </summary>
public class Daq
{
    public static void actuate_relay(string device_alias, bool close, int channel)
    {
        string identifier = device_alias + @"/port0/line" + channel;

        using (var task = new Daq_Task())
        {
            var status = Dll_Wrapper.DAQmxCreateDOChan(task.handle, identifier, "", 0);
            throw_error(status);

            byte[] data = new byte[] { close ? (byte)1 : (byte)0 };
            int written;

            status = Dll_Wrapper.DAQmxWriteDigitalLines(task.handle, 1, true, 10.0, false, data, out written, IntPtr.Zero);
            throw_error(status);
        }
    }

    public static bool is_relay_closed(string device_alias, int channel)
    {
        string identifier = device_alias + @"/port0/line" + channel;

        using (var task = new Daq_Task())
        {
            var status = Dll_Wrapper.DAQmxCreateDOChan(task.handle, identifier, "", 0);
            throw_error(status);

            byte[] data = new byte[8];

            int samples_per_channel_read;
            int bytes_per_channel;

            status = Dll_Wrapper.DAQmxReadDigitalLines(task.handle, 1, 10.0, false, data, (uint)data.Count(), out samples_per_channel_read, out bytes_per_channel, IntPtr.Zero);
            throw_error(status);

            return data[0] == 1;
        }
    }

    /// <summary>
    /// Handle errors here using DAQmxGetErrorString (or try DAQmxGetExtendedErrorInfo).
    /// </summary>
    public static void throw_error(int code)
    {
        // No problems
        if (code == 0)
            return;

        // Give us a message for that error code.
        var error = new StringBuilder(2000);
        Dll_Wrapper.DAQmxGetErrorString(code, error, 2000);
        if (error.ToString().Trim().Length > 0)
            throw new Exception(error.ToString());

        // No message? Then just throw the error code.
        throw new Exception(code + "");
    }
}
