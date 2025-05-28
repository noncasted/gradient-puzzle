using System.Reflection;

namespace Metrics;

public static class MetricsExtensions
{
    public static string ReadMigrationScript(this IMigrationMetadata _, string name)
    {
        return Assembly.GetExecutingAssembly().ReadScript(name);
    }

    public static string ReadScript(this Assembly assembly, string name)
    {
        // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
        var manifestResourceNames = assembly.GetManifestResourceNames();
        
        var resourcePath = manifestResourceNames.Single(s => s.EndsWith(name));

        using var stream = assembly.GetManifestResourceStream(resourcePath)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}