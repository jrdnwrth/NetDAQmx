namespace NetDAQmx.Helpers;
/// <summary>
/// The `Daq` functions usually operate on a "Task" handle.  This handle should be closed when finished.
/// To ensure that it will be closed, we created this IDisposable object.
/// </summary>
public class DaqTask : IDisposable
{
    internal IntPtr handle;
    /// <summary>
    /// Construct a daq task with a name (default is empty string)
    /// </summary>
    /// <param name="taskName">The task name</param>
    public DaqTask(string taskName = "")
    {
        var status = DllWrapper.DAQmxCreateTask(taskName, out handle);
        NIDAQ.ThrowError(status);
    }

    /// <summary>
    /// Fun Fact!  Dispose is not called in debug mode.  However, it does work when we 'Start Without Debugging'
    /// https://stackoverflow.com/questions/518352/does-dispose-still-get-called-when-exception-is-thrown-inside-of-a-using-stateme
    /// </summary>
    public void Dispose()
    {
        var status = DllWrapper.DAQmxClearTask(handle);
        NIDAQ.ThrowError(status);
    }
}
