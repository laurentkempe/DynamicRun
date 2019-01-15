using DynamicRun.Builder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
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

                changes.Subscribe(filepath => CompileAndRun(filepath));

                watcher.Start();

                Console.WriteLine("Press any key to exit!");
                Console.ReadLine();
            }
        }

        private static void CompileAndRun(string filepath)
        {
            Console.WriteLine($"Starting compilation of: '{filepath}'");

            var sourceCode = File.ReadAllText(filepath);

            using (var ms = new MemoryStream())
            {
                var result = GenerateCode(sourceCode).Emit(ms);

                Console.WriteLine("Compilation done.");
            }
        }

        static CSharpCompilation GenerateCode(string sourceCode)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options, "");

            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
            };

            return CSharpCompilation.Create("Hello.dll",
                                            new SyntaxTree[] { parsedSyntaxTree }, 
                                            references: references, 
                                            options: new CSharpCompilationOptions(OutputKind.ConsoleApplication, 
                                                                                  optimizationLevel: OptimizationLevel.Release,
                                                                                  assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}
