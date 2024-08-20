using System.Collections.Generic;

namespace Tools.EditorTools
{
    public class AssemblyDomain : IAssemblyDomain
    {
        public AssemblyDomain(IReadOnlyList<IAssembly> assemblies)
        {
            Assemblies = assemblies;
        }
 
        public IReadOnlyList<IAssembly> Assemblies { get; }
    }
}