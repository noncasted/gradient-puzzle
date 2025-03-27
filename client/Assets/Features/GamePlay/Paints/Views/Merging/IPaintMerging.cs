using System.Collections.Generic;
using GamePlay.Common;
using GamePlay.Levels;
using Internal;

namespace GamePlay.Paints
{
    public interface IPaintMerging
    {
        void Show(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets, bool showBody = true);
    }

    public static class PaintMergingExtensions
    {
        public static void Show(
            this IPaintMerging merging,
            IReadOnlyLifetime lifetime,
            IPaintTarget target,
            bool showBody = true)
        {
            merging.Show(lifetime, new[] { target }, showBody);
        }
    }
}