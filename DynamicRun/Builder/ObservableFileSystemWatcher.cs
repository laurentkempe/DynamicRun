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
    public IObservable<RenamedEventArgs> Renamed { get; private set; }
    public IObservable<FileSystemEventArgs> Deleted { get; private set; }
    public IObservable<ErrorEventArgs> Errors { get; private set; }
    public IObservable<FileSystemEventArgs> Created { get; private set; }

    /// <summary>
    ///     Pass an existing FileSystemWatcher instance, this is just for the case where it's not possible to only pass the
    ///     configuration, be aware that disposing this wrapper will dispose the FileSystemWatcher instance too.
    /// </summary>
    /// <param name="watcher"></param>
    private ObservableFileSystemWatcher(FileSystemWatcher watcher)
    {
        _watcher = watcher;

        Changed = Observable
            .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => _watcher.Changed += h, h => _watcher.Changed -= h)
            .Select(x => x.EventArgs);

        Renamed = Observable
            .FromEventPattern<RenamedEventHandler, RenamedEventArgs>(h => _watcher.Renamed += h, h => _watcher.Renamed -= h)
            .Select(x => x.EventArgs);

        Deleted = Observable
            .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => _watcher.Deleted += h, h => _watcher.Deleted -= h)
            .Select(x => x.EventArgs);

        Errors = Observable
            .FromEventPattern<ErrorEventHandler, ErrorEventArgs>(h => _watcher.Error += h, h => _watcher.Error -= h)
            .Select(x => x.EventArgs);

        Created = Observable
            .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => _watcher.Created += h, h => _watcher.Created -= h)
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