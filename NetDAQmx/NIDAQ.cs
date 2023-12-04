using NetDAQmx.Helpers;
using System.Text;
using static NetDAQmx.DllWrapper;

namespace NetDAQmx;

/// <summary>
/// This would not be much of a library if the user had to call the raw DLL functions.
/// This class provides more user-friendly functions for interacting with NI-DAQ and represents a NI-DAQ instance.
/// </summary>
public class NIDAQ
{
    /// <summary>
    /// Indicates the names of all devices installed in the system
    /// </summary>
    /// <param name="bufferSize">The buffer's length</param>
    /// <returns>The array of device's names</returns>
    public static string[] GetSystemDevices(int bufferSize = 100)
    {
        // Create a buffer to store the device names
        char[] buffer = new char[bufferSize];

        // Call the DAQmxGetSysDevNames function
        int status = DAQmxGetSysDevNames(buffer);

        // Check the result and handle it accordingly
        if (status == 0)
        {
            string deviceNames = new string(buffer).TrimEnd('\0'); ; // Convert char array to string
            var result = deviceNames.Split('\0');
            if (result.Length > 0 && string.IsNullOrEmpty(result[0])) // no devices attached
            {
                return Array.Empty<string>();
            }
            return result;
        }

        Console.WriteLine("Error occurred: " + status);
        return Array.Empty<string>();
    }

    /// <summary>
    /// The devices name in the system
    /// </summary>
    public string DeviceAlias { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="deviceAlias">The devices name in the system</param>
    public NIDAQ(string deviceAlias = "Dev0")
    {
        DeviceAlias = deviceAlias;
    }

    /// <summary>
    /// Indicates in bits the width of digital output port
    /// </summary>
    /// <param name="identifier">The path to the port. E.g. - Dev1/port0</param>
    /// <returns>The width of digital output port</returns>
    public static uint DAQmxGetPhysicalChanDOPortWidth(string identifier)
    {
        var status = DllWrapper.DAQmxGetPhysicalChanDOPortWidth(identifier, out uint portWidth);
        ThrowError(status);

        return portWidth;
    }

    /// <summary>
    /// Indicates in bits the width of digital output port
    /// </summary>
    /// <param name="port">The number of port</param>
    /// <returns>The width of digital output port</returns>
    public uint DAQmxGetPhysicalChanDOPortWidth(byte port)
    {
        return DAQmxGetPhysicalChanDOPortWidth($"{DeviceAlias}/port{port}");
    }

    /// <summary>
    /// Writes a single digital output to be on or off 
    /// </summary>
    /// <param name="port">The port number</param>
    /// <param name="channel">The channel number</param>
    /// <param name="close">True to close the line, o/w open</param>
    public void WriteDOChannel(byte port, uint channel, bool close)
    {
        WriteDOSingleLine(DeviceAlias, port, channel, close);
    }

    /// <summary>
    /// Creates a task and writes a single line of a DO port
    /// </summary>
    /// <param name="device_alias">The device name. E.g. - "Dev1"</param>
    /// <param name="port">The port number</param>
    /// <param name="channel">The channel number</param>
    /// <param name="close">True to close the line, o/w open</param>
    public static void WriteDOSingleLine(string device_alias, byte port, uint channel, bool close)
    {
        string identifier = $"{device_alias}/port{port}/line{channel}";

        using var task = new DaqTask();
        var status = DllWrapper.DAQmxCreateDOChan(task.handle, identifier, "", DAQmxLineGrouping.ChanPerLine);
        ThrowError(status);

        byte[] data = new byte[] { close ? (byte)0 : (byte)1 };
        status = DllWrapper.DAQmxWriteDigitalLines(task.handle, 1, true, 10.0, DAQmxDataLayout.GroupByChannel, data, out int written, IntPtr.Zero);
        ThrowError(status);
    }

    /// <summary>
    /// Creates channel(s) to generate digital signals and adds the channel(s) to the task you specify with taskHandle. You can group digital lines into one digital channel or separate them into multiple digital channels. If you specify one or more entire ports in lines by using port physical channel names, you cannot separate the ports into multiple channels. To separate ports into multiple channels, use this function multiple times with a different port each time.
    /// </summary>
    /// <param name="task">The task to which to add the channels that this function creates</param>
    /// <param name="lines">The names of the digital lines used to create a virtual channel. You can specify a list or range of lines. Specifying a port and no lines is the equivalent of specifying all the lines of that port in order. Therefore, if you specify Dev1/port0 and port 0 has eight lines, this is expanded to Dev1/port0/line0:7. </param>
    /// <param name="lineGrouping">Specifies whether to group digital lines into one or more virtual channels. If you specify one or more entire ports in lines, you must set lineGrouping to ChanForAllLines</param>
    /// <param name="nameToAssignToLines">The name of the created virtual channel(s). If you create multiple virtual channels with one call to this function, you can specify a list of names separated by commas. If you do not specify a name, NI-DAQmx uses the physical channel name as the virtual channel name. If you specify your own names for nameToAssignToLines, you must use the names when you refer to these channels in other NI-DAQmx functions.</param>
    public static void DAQmxCreateDOChan(DaqTask task, string lines, DAQmxLineGrouping lineGrouping, string nameToAssignToLines = "")
    {
        var status = DllWrapper.DAQmxCreateDOChan(task.handle, lines, nameToAssignToLines, lineGrouping);
        ThrowError(status);
    }

    /// <summary>
    /// Creates channel(s) to generate digital signals and adds the channel(s) to the task you specify with taskHandle. You can group digital lines into one digital channel or separate them into multiple digital channels. If you specify one or more entire ports in lines by using port physical channel names, you cannot separate the ports into multiple channels. To separate ports into multiple channels, use this function multiple times with a different port each time.
    /// </summary>
    /// <param name="task">The task to which to add the channels that this function creates</param>
    /// <param name="port">The port number</param>
    /// <param name="line">The line number</param>
    /// <param name="lineGrouping">Specifies whether to group digital lines into one or more virtual channels. If you specify one or more entire ports in lines, you must set lineGrouping to DAQmx_Val_ChanForAllLines</param>
    /// <param name="nameToAssignToLines">The name of the created virtual channel(s). If you create multiple virtual channels with one call to this function, you can specify a list of names separated by commas. If you do not specify a name, NI-DAQmx uses the physical channel name as the virtual channel name. If you specify your own names for nameToAssignToLines, you must use the names when you refer to these channels in other NI-DAQmx functions.</param>
    public void DAQmxCreateDOChan(DaqTask task, byte port, int line, DAQmxLineGrouping lineGrouping, string nameToAssignToLines = "")
    {
        DAQmxCreateDOChan(task, $"{DeviceAlias}/port{port}/line{line}", lineGrouping, nameToAssignToLines);
    }

    /// <summary>
    /// Creates channel(s) to generate digital signals and adds the channel(s) to the task you specify with taskHandle. You can group digital lines into one digital channel or separate them into multiple digital channels. If you specify one or more entire ports in lines by using port physical channel names, you cannot separate the ports into multiple channels. To separate ports into multiple channels, use this function multiple times with a different port each time.
    /// </summary>
    /// <param name="task">The task to which to add the channels that this function creates</param>
    /// <param name="port">The port number</param>
    /// <param name="lineStart">The first line number to include</param>
    /// <param name="lineEnd">The last line number to include</param>
    /// <param name="lineGrouping">Specifies whether to group digital lines into one or more virtual channels. If you specify one or more entire ports in lines, you must set lineGrouping to DAQmx_Val_ChanForAllLines</param>
    /// <param name="nameToAssignToLines">The name of the created virtual channel(s). If you create multiple virtual channels with one call to this function, you can specify a list of names separated by commas. If you do not specify a name, NI-DAQmx uses the physical channel name as the virtual channel name. If you specify your own names for nameToAssignToLines, you must use the names when you refer to these channels in other NI-DAQmx functions</param>
    public void DAQmxCreateDOChan(DaqTask task, byte port, byte lineStart, byte lineEnd, DAQmxLineGrouping lineGrouping, string nameToAssignToLines = "")
    {
        DAQmxCreateDOChan(task, $"{DeviceAlias}/port{port}/line{lineStart}:{lineEnd}", lineGrouping, nameToAssignToLines);
    }

    /// <summary>
    /// Writes multiple 8-bit unsigned integer samples to a task that contains one or more digital output channels. Use this format for devices with up to 8 lines per port.
    /// </summary>
    /// <param name="task">The task to write samples to</param>
    /// <param name="numSamplesPerChan">The number of samples, per channel, to write. You must pass in a value of 0 or more in order for the sample to write. If you pass a negative number, this function returns an error</param>
    /// <param name="autoStart">Specifies whether or not this function automatically starts the task if you do not start it</param>
    /// <param name="timeout">The amount of time, in seconds, to wait for this function to write all the samples. To specify an infinite wait, pass -1 (DAQmx_Val_WaitInfinitely). This function returns an error if the timeout elapses. 
    /// A value of 0 indicates to try once to write the submitted samples. If this function successfully writes all submitted samples, it does not return an error. Otherwise, the function returns a timeout error and returns the number of samples actually written</param>
    /// <param name="dataLayout">Specifies how the samples are arranged, either interleaved or noninterleaved</param>
    /// <param name="writeArray">The array of 8-bit integer samples to write to the task</param>
    /// <param name="sampsPerChanWritten">The actual number of samples per channel successfully written to the buffer</param>
    public static void DAQmxWriteDigitalU8(DaqTask task, int numSamplesPerChan, bool autoStart, double timeout, DAQmxDataLayout dataLayout, byte[] writeArray, out int sampsPerChanWritten)
    {
        var status = DllWrapper.DAQmxWriteDigitalU8(task.handle, numSamplesPerChan, autoStart, timeout, dataLayout, writeArray, out sampsPerChanWritten);
        ThrowError(status);
    }

    /// <summary>
    /// Creates a task and writes an entire port with a specific value
    /// </summary>
    /// <param name="port">The port number</param>
    /// <param name="data">The data to write to the port</param>
    public void WritePort(byte port, byte data)
    {
        var identifier = $"{DeviceAlias}/port{port}";
        var portWidth = DAQmxGetPhysicalChanDOPortWidth(identifier);

        using var task = new DaqTask();
        DAQmxCreateDOChan(task, identifier, DAQmxLineGrouping.ChanForAllLines);

        byte[] dataArray = new byte[] { data };
        DAQmxWriteDigitalU8(task, 1, true, 10.0, DAQmxDataLayout.GroupByChannel, dataArray, out int written);
    }

    /// <summary>
    /// Creates channel(s) to measure voltage and adds the channel(s) to the task you specify with taskHandle. If your measurement requires the use of internal excitation or you need the voltage to be scaled by excitation, call DAQmxCreateAIVoltageChanWithExcit.
    /// </summary>
    /// <param name="task">The task to which to add the channels that this function creates</param>
    /// <param name="physicalChannel">The names of the physical channels to use to create virtual channels. You can specify a list or range of physical channels</param>
    /// <param name="nameToAssignToChannel">The name(s) to assign to the created virtual channel(s). If you do not specify a name, NI-DAQmx uses the physical channel name as the virtual channel name. If you specify your own names for nameToAssignToChannel, you must use the names when you refer to these channels in other NI-DAQmx functions. If you create multiple virtual channels with one call to this function, you can specify a list of names separated by commas. If you provide fewer names than the number of virtual channels you create, NI-DAQmx automatically assigns names to the virtual channels.</param>
    /// <param name="terminalConfig">The input terminal configuration for the channel</param>
    /// <param name="minVal">The minimum value, in units, that you expect to measure</param>
    /// <param name="maxVal">The maximum value, in units, that you expect to measure</param>
    /// <param name="units">The units to use to return the voltage measurements</param>
    /// <param name="customScaleName">The name of a custom scale to apply to the channel. To use this parameter, you must set units to DAQmx_Val_FromCustomScale. If you do not set units to DAQmx_Val_FromCustomScale, you must set customScaleName to NULL</param>
    public static void DAQmxCreateAIVoltageChan(DaqTask task, string physicalChannel, string nameToAssignToChannel, DAQmxAITerminalConfiguration terminalConfig, double minVal, double maxVal, DAQmxAOVoltageUnits units, string? customScaleName = null)
    {
        var status = DllWrapper.DAQmxCreateAIVoltageChan(task.handle, physicalChannel, nameToAssignToChannel, terminalConfig, minVal, maxVal, units, customScaleName);
        ThrowError(status);
    }

    /// <summary>
    /// Creates channel(s) to measure voltage and adds the channel(s) to the task you specify with taskHandle. If your measurement requires the use of internal excitation or you need the voltage to be scaled by excitation, call DAQmxCreateAIVoltageChanWithExcit.
    /// </summary>
    /// <param name="task">The task to which to add the channels that this function creates</param>
    /// <param name="analogInput">The line to read from</param>
    /// <param name="nameToAssignToChannel">The name(s) to assign to the created virtual channel(s). If you do not specify a name, NI-DAQmx uses the physical channel name as the virtual channel name. If you specify your own names for nameToAssignToChannel, you must use the names when you refer to these channels in other NI-DAQmx functions. If you create multiple virtual channels with one call to this function, you can specify a list of names separated by commas. If you provide fewer names than the number of virtual channels you create, NI-DAQmx automatically assigns names to the virtual channels.</param>
    /// <param name="terminalConfig">The input terminal configuration for the channel</param>
    /// <param name="minVal">The minimum value, in units, that you expect to measure</param>
    /// <param name="maxVal">The maximum value, in units, that you expect to measure</param>
    /// <param name="units">The units to use to return the voltage measurements</param>
    /// <param name="customScaleName">The name of a custom scale to apply to the channel. To use this parameter, you must set units to DAQmx_Val_FromCustomScale. If you do not set units to DAQmx_Val_FromCustomScale, you must set customScaleName to NULL</param>
    public void DAQmxCreateAIVoltageChan(DaqTask task, byte analogInput, string nameToAssignToChannel, DAQmxAITerminalConfiguration terminalConfig, double minVal, double maxVal, DAQmxAOVoltageUnits units, string? customScaleName = null)
    {
        DAQmxCreateAIVoltageChan(task, $"{DeviceAlias}/ai{analogInput}", nameToAssignToChannel, terminalConfig, minVal, maxVal, units, customScaleName);
    }

    /// <summary>
    /// Creates channel(s) to measure voltage and adds the channel(s) to the task you specify with taskHandle. If your measurement requires the use of internal excitation or you need the voltage to be scaled by excitation, call DAQmxCreateAIVoltageChanWithExcit.
    /// </summary>
    /// <param name="task">The task to which to add the channels that this function creates</param>
    /// <param name="analogInputStart">The start of the range of the analog line</param>
    /// <param name="analogInputEnd">The end of the range of the analog line</param>
    /// <param name="nameToAssignToChannel">The name(s) to assign to the created virtual channel(s). If you do not specify a name, NI-DAQmx uses the physical channel name as the virtual channel name. If you specify your own names for nameToAssignToChannel, you must use the names when you refer to these channels in other NI-DAQmx functions. If you create multiple virtual channels with one call to this function, you can specify a list of names separated by commas. If you provide fewer names than the number of virtual channels you create, NI-DAQmx automatically assigns names to the virtual channels.</param>
    /// <param name="terminalConfig">The input terminal configuration for the channel</param>
    /// <param name="minVal">The minimum value, in units, that you expect to measure</param>
    /// <param name="maxVal">The maximum value, in units, that you expect to measure</param>
    /// <param name="units">The units to use to return the voltage measurements</param>
    /// <param name="customScaleName">The name of a custom scale to apply to the channel. To use this parameter, you must set units to DAQmx_Val_FromCustomScale. If you do not set units to DAQmx_Val_FromCustomScale, you must set customScaleName to NULL</param>
    public void DAQmxCreateAIVoltageChan(DaqTask task, byte analogInputStart, byte analogInputEnd, string nameToAssignToChannel, DAQmxAITerminalConfiguration terminalConfig, double minVal, double maxVal, DAQmxAOVoltageUnits units, string? customScaleName = null)
    {
        DAQmxCreateAIVoltageChan(task, $"{DeviceAlias}/ai{analogInputStart}:{analogInputEnd}", nameToAssignToChannel, terminalConfig, minVal, maxVal, units, customScaleName);
    }

    /// <summary>
    /// Reads a single floating-point sample from a task that contains a single analog input channel
    /// </summary>
    /// <param name="task">The task to read the sample from</param>
    /// <param name="timeout">The amount of time, in seconds, to wait for the function to read the sample(s). To specify an infinite wait, pass -1 (DAQmx_Val_WaitInfinitely). This function returns an error if the timeout elapses. A value of 0 indicates to try once to read the requested samples. If all the requested samples are read, the function is successful. Otherwise, the function returns a timeout error and returns the samples that were actually read</param>
    /// <param name="result">The sample read from the task</param>
    public static void DAQmxReadAnalogScalarF64(DaqTask task, double timeout, out double result)
    {
        var status = DllWrapper.DAQmxReadAnalogScalarF64(task.handle, timeout, out result);
        ThrowError(status);
    }

    /// <summary>
    /// Create a task and reads a single floating-point sample from a task that contains a single analog input channel
    /// </summary>
    /// <param name="channel">The line to read from</param>
    /// <param name="minValue">The minimum value, in units, that you expect to measure</param>
    /// <param name="maxValue">The maximum value, in units, that you expect to measure</param>
    /// <returns></returns>
    public double GetAnalogInputSingleLine(uint channel, double minValue, double maxValue)
    {
        using var task = new DaqTask();
        DAQmxCreateAIVoltageChan(task, $"{DeviceAlias}/ai{channel}", "", DAQmxAITerminalConfiguration.RSE, minValue, maxValue, DAQmxAOVoltageUnits.Volts);
        DAQmxReadAnalogScalarF64(task, 10, out double result);
        return result;
    }

    /// <summary>
    /// Handle errors here using DAQmxGetErrorString (or try DAQmxGetExtendedErrorInfo).
    /// </summary>
    public static void ThrowError(int code)
    {
        // No problems
        if (code == 0)
            return;

        // Give us a message for that error code.
        var error = new StringBuilder(2000);
        DAQmxGetErrorString(code, error, 2000);
        if (error.ToString().Trim().Length > 0)
            throw new Exception(error.ToString());

        // No message? Then just throw the error code.
        throw new Exception(code + "");
    }
}
