using System.Collections.Generic;

namespace Tools.EditorTools
{
    public interface IAssemblyDomain
    {
        IReadOnlyList<IAssembly> Assemblies { get; }
    }
}