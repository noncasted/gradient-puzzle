using System.Collections.Generic;

namespace Features.GamePlay
{
    public interface ILevel
    {
        IReadOnlyList<IArea> Areas { get; }
    }
}