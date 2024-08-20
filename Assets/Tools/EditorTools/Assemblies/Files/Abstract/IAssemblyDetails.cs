using System.Collections.Generic;

namespace Tools.EditorTools
{
    public interface IAssemblyDetails
    {
        IReadOnlyList<string> Namespaces { get; }
        IReadOnlyList<string> Usings { get; }
        IReadOnlyList<string> Interfaces { get; }
        bool IsOwned { get; }
    }
}