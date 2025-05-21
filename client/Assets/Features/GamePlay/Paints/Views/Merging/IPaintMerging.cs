using System.Collections.Generic;
using GamePlay.Common;
using Internal;

namespace GamePlay.Paints
{
    public interface IPaintMerging
    {
        void Show(PaintMergingHandleOptions options);
    }

    public class PaintMergingHandleOptions
    {
        public IReadOnlyLifetime Lifetime { get; set; }
        public IReadOnlyList<IPaintTarget> Targets { get; set; }
        public bool ShowBody { get; set; }
        public bool ShowFill { get; set; } = true;
    }
}