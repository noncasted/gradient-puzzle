using UnityEditor;

namespace Tools.EditorTools
{
    public static class RewriteAssemblies
    {
        [MenuItem("Tools/Assemblies/Rewrite")]
        private static void RemoveInvalidAssemblyReferences()
        {
            var domainConstructor = new DomainConstructor();
            var assemblies = domainConstructor.Construct();

            foreach (var assembly in assemblies)
            {
                if (assembly.Details.IsOwned == false)
                    continue;

                assembly.Toggles.AutoReference = false;

                assembly.Write();
            }
        }
    }
}