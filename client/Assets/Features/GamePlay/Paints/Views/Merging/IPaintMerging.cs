using System.Collections.Generic;
using GamePlay.Common;
using GamePlay.Levels;
using Internal;

namespace GamePlay.Paints
{
    public interface IPaintMerging
    {
        void Show(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets);
    }

    public static class PaintMergingExtensions
    {
        public static void Show(this IPaintMerging merging, IReadOnlyLifetime lifetime, IPaintTarget target)
        {
            merging.Show(lifetime, new[] { target });
        }
    }
}