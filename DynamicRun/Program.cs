using System;
using System.IO;
using System.Reactive.Linq;
using DynamicRun.Builder;

var sourcesPath = Path.Combine(Environment.CurrentDirectory, "Sources");

Console.WriteLine($"Running from: {Environment.CurrentDirectory}");
Console.WriteLine($"Sources from: {sourcesPath}");
Console.WriteLine("Modify the sources to compile and run it!");

using var watcher = new ObservableFileSystemWatcher(c => { c.Path = $".{Path.DirectorySeparatorChar}Sources"; });
var changes = watcher.Changed.Throttle(TimeSpan.FromSeconds(.5)).Where(c => c.FullPath.EndsWith("DynamicProgram.cs")).Select(c => c.FullPath);

changes.Subscribe(filepath => Runner.Execute(Compiler.Compile(filepath), new[] { "France" }));

watcher.Start();

Console.WriteLine("Press any key to exit!");
Console.ReadLine();

watcher.Stop();