using System;
using System.IO;
using System.Reactive.Linq;

namespace DynamicRun.Builder;

/// <summary>
///     This is a wrapper around a file system watcher to use the Rx framework instead of event handlers to handle
///     notifications of file system changes.
/// </summary>
public sealed class ObservableFileSystemWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;

    public IObservable<FileSystemEventArgs> Changed { get; private set; }

    /// <summary>
    ///     Pass an existing FileSystemWatcher instance, this is just for the case where it's not possible to only pass the
    ///     configuration, be aware that disposing this wrapper will dispose the FileSystemWatcher instance too.
    /// </summary>
    /// <param name="watcher">An existing instance of the FileSystemWatcher class</param>
    private ObservableFileSystemWatcher(FileSystemWatcher watcher)
    {
        _watcher = watcher;

        Changed = Observable
            .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => _watcher.Changed += h, h => _watcher.Changed -= h)
            .Select(x => x.EventArgs);
    }

    /// <summary>
    ///     Pass a function to configure the FileSystemWatcher as desired, this constructor will manage creating and applying
    ///     the configuration.
    /// </summary>
    public ObservableFileSystemWatcher(Action<FileSystemWatcher> configure)
        : this(new FileSystemWatcher())
    {
        configure(_watcher);
    }

    public void Start() => _watcher.EnableRaisingEvents = true;

    public void Stop() => _watcher.EnableRaisingEvents = false;

    public void Dispose() => _watcher.Dispose();
}