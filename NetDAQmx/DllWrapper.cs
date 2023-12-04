using System.Runtime.InteropServices;
using System.Text;

namespace NetDAQmx;

/// <summary>
/// The main DLL wrapper of nicaiu.dll
/// </summary>
public static class DllWrapper
{
    /// <summary>
    /// Values for the Line Grouping parameter of DAQmxCreateDIChan and DAQmxCreateDOChan
    /// </summary>
    public enum DAQmxLineGrouping
    {
        /// <summary>
        /// One Channel For Each Line
        /// </summary>
        ChanPerLine = 0,
        /// <summary>
        /// One Channel For All Lines
        /// </summary>
        ChanForAllLines = 1
    }

    /// <summary>
    /// Values for the Data Layout parameter of DAQmxWriteAnalogF64, DAQmxWriteBinaryI16, DAQmxWriteDigitalU8, DAQmxWriteDigitalU32, DAQmxWriteDigitalLines
    /// </summary>
    public enum DAQmxDataLayout
    {
        /// <summary>
        /// Group by Channel
        /// </summary>
        GroupByChannel = 0,
        /// <summary>
        /// Group by Scan Number
        /// </summary>
        GroupByScanNumber = 1
    }

    /// <summary>
    /// Values for DAQmx_AI_TermCfg
    /// </summary>
    public enum DAQmxAITerminalConfiguration
    {
        /// <summary>
        /// At run time, NI-DAQmx chooses the default terminal configuration for the channel
        /// </summary>
        Default = -1,
        /// <summary>
        /// Referenced single-ended mode
        /// </summary>
        RSE = 10083,
        /// <summary>
        /// Non-referenced single-ended mode
        /// </summary>
        NRSE = 10078,
        /// <summary>
        /// Differential mode
        /// </summary>
        Differential = 10106,
        /// <summary>
        /// Pseudodifferential mode
        /// </summary>
        PseudoDiff = 12529
    }

    /// <summary>
    /// Values for DAQmx_AO_Voltage_Units
    /// </summary>
    public enum DAQmxAOVoltageUnits
    {
        /// <summary>
        /// Volts
        /// </summary>
        Volts = 10348,
        /// <summary>
        /// From Custom Scale
        /// </summary>
        FromCustomScale = 10065
    }

    private const string DllPath = @"nicaiu.dll";

    /// <summary>
    /// Get the connected to the system devices
    /// </summary>
    /// <param name="data"> Buffer where device names will be stored</param>
    /// <param name="bufferSize">Size of the buffer</param>
    /// <returns></returns>
    [DllImport(DllPath, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    private static extern int DAQmxGetSysDevNames([Out] char[] data, uint bufferSize);

    /// <summary>
    /// Indicates the names of all devices installed in the system
    /// </summary>
    /// <param name="data">Buffer to store device names</param>
    /// <returns>The error code returned by the function in the event of an error or warning. A value of 0 indicates success. A positive value indicates a warning. A negative value indicates an error.</returns>
    internal static int DAQmxGetSysDevNames(char[] data)
    {
        return DAQmxGetSysDevNames(data, (uint)data.Length);
    }

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxGetPhysicalChanDOPortWidth(string physicalChannel, out uint data);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxCreateTask(string taskName, out IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxCreateDOChan(IntPtr taskHandle, string lines, string nameToAssignToLines, DAQmxLineGrouping lineGrouping);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxCreateAIVoltageChan(
    IntPtr taskHandle,
    string physicalChannel,
    string nameToAssignToChannel,
    DAQmxAITerminalConfiguration terminalConfig,
    double minVal,
    double maxVal,
    DAQmxAOVoltageUnits units,
    string? customScaleName = null
    );

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxWriteDigitalLines(IntPtr taskHandle, int numSampsPerChan, bool autoStart, double timeout, DAQmxDataLayout dataLayout, byte[] writeArray, out int sampsPerChanWritten, IntPtr reserved);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    private static extern int DAQmxWriteDigitalU8(
    IntPtr taskHandle,
    int numSampsPerChan,
    bool autoStart,
    double timeout,
    DAQmxDataLayout dataLayout,
    byte[] writeArray,
    out int sampsPerChanWritten,
    IntPtr reserved
    );

    internal static int DAQmxWriteDigitalU8(
    IntPtr taskHandle,
    int numSampsPerChan,
    bool autoStart,
    double timeout,
    DAQmxDataLayout dataLayout,
    byte[] writeArray,
    out int sampsPerChanWritten
    )
    {
        return DAQmxWriteDigitalU8(taskHandle, numSampsPerChan, autoStart, timeout, dataLayout, writeArray, out sampsPerChanWritten, IntPtr.Zero);
    }

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    private static extern int DAQmxReadDigitalLines(IntPtr taskHandle, int numSampsPerChan, double timeout, DAQmxDataLayout fillMode, byte[] readArray, uint arraySizeInBytes, out int sampsPerChanRead, out int numBytesPerSamp, IntPtr reserved);
    internal static int DAQmxReadDigitalLines(IntPtr taskHandle, int numSampsPerChan, double timeout, DAQmxDataLayout fillMode, byte[] readArray, uint arraySizeInBytes, out int sampsPerChanRead, out int numBytesPerSamp)
    {
        return DAQmxReadDigitalLines(taskHandle, numSampsPerChan, timeout, fillMode, readArray, arraySizeInBytes, out sampsPerChanRead, out numBytesPerSamp, IntPtr.Zero);
    }

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    private static extern int DAQmxReadAnalogScalarF64(
    IntPtr taskHandle,
    double timeout,
    out double value,
    IntPtr reserved
);
    internal static int DAQmxReadAnalogScalarF64(IntPtr taskHandle, double timeout, out double value)
    {
        return DAQmxReadAnalogScalarF64(taskHandle, timeout, out value, IntPtr.Zero);
    }
    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxClearTask(IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxResetDevice(string deviceName);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxGetDeviceAttribute(string deviceName, int attribute, IntPtr value);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxGetErrorString(int errorCode, StringBuilder errorString, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxGetExtendedErrorInfo(StringBuilder errorString, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxGetTaskName(IntPtr taskHandle, StringBuilder data, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxGetTaskChannels(IntPtr taskHandle, StringBuilder data, uint bufferSize);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxGetTaskNumChans(IntPtr taskHandle, out uint data);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxLoadTask(string taskName, out IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxStartTask(IntPtr taskHandle);

    [DllImport(DllPath, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    internal static extern int DAQmxStopTask(IntPtr taskHandle);
}
