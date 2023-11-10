using DynamicRun.Builder;
using System;
using System.IO;
using System.Reactive.Linq;

var sourcesPath = Path.Combine(Environment.CurrentDirectory, "Sources");

Console.WriteLine($"Running from: {Environment.CurrentDirectory}");
Console.WriteLine($"Sources from: {sourcesPath}");
Console.WriteLine("Modify the sources to compile and run it!");

var compiler = new Compiler();
var runner = new Runner();

using var watcher = new ObservableFileSystemWatcher(c => { c.Path = @$".{Path.DirectorySeparatorChar}Sources"; });
var changes = watcher.Changed.Throttle(TimeSpan.FromSeconds(.5)).Where(c => c.FullPath.EndsWith(@"DynamicProgram.cs")).Select(c => c.FullPath);

changes.Subscribe(filepath => runner.Execute(compiler.Compile(filepath), new[] { "France" }));

watcher.Start();

Console.WriteLine("Press any key to exit!");
Console.ReadLine();