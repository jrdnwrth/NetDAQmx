using System.Runtime.InteropServices;
using System.Text;

namespace NetDAQmx;

public class Dll_Wrapper
{
    private const string DllPath = @"nicaiu.dll";

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxCreateTask(string taskName, out IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxCreateDOChan(IntPtr taskHandle, string lines, string nameToAssignToLines, int lineGrouping);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxWriteDigitalLines(IntPtr taskHandle, int numSampsPerChan, bool autoStart, double timeout, bool dataLayout, byte[] writeArray, out int sampsPerChanWritten, IntPtr reserved);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxReadDigitalLines(IntPtr taskHandle, int numSampsPerChan, double timeout, bool fillMode, byte[] readArray, UInt32 arraySizeInBytes, out int sampsPerChanRead, out int numBytesPerSamp, IntPtr reserved);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxClearTask(IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxResetDevice(string deviceName);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxGetDeviceAttribute(string deviceName, int attribute, IntPtr value);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxGetErrorString(int errorCode, StringBuilder errorString, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxGetExtendedErrorInfo(StringBuilder errorString, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxGetTaskName(IntPtr taskHandle, StringBuilder data, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxGetTaskChannels(IntPtr taskHandle, StringBuilder data, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxGetTaskNumChans(IntPtr taskHandle, out uint data);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxLoadTask(string taskName, out IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxStartTask(IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DAQmxStopTask(IntPtr taskHandle);
}
