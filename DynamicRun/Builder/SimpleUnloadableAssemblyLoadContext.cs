using System.Reflection;
using System.Runtime.Loader;

namespace DynamicRun.Builder;

internal sealed class SimpleUnloadableAssemblyLoadContext() : AssemblyLoadContext(true)
{
    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }
}