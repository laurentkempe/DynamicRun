using DynamicRun.Builder;
using System;
using System.IO;
using System.Reactive.Linq;

namespace DynamicRun
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourcesPath = Path.Combine(Environment.CurrentDirectory, "Sources");

            Console.WriteLine($"Running from: {Environment.CurrentDirectory}");
            Console.WriteLine($"Sources from: {sourcesPath}");
            Console.WriteLine("Modify the sources to compile and run it!");

            using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = @".\Sources"; }))
            {
                var changes = watcher.Changed.Throttle(TimeSpan.FromSeconds(.5)).Where(c => c.FullPath.EndsWith(@"Program.cs")).Select(c => c.FullPath);

                changes.Subscribe(file => Console.WriteLine($"Modidified file: {file}"));

                watcher.Start();

                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();
            }
        }
    }
}
