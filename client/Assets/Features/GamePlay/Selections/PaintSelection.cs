using Services;

namespace GamePlay.Selections
{
    public class PaintSelection : IPaintSelection
    {
        public PaintSelection(
            IObjectFactory<PaintDock> objectFactory,
            IMaskRenderOptions maskRenderOptions,
            IPaintSelectionScaler scaler,
            PaintSelectionOptions options)
        {
            _objectFactory = objectFactory;
            _maskRenderOptions = maskRenderOptions;
            _scaler = scaler;
            _options = options;
        }

        private readonly IObjectFactory<PaintDock> _objectFactory;
        private readonly IMaskRenderOptions _maskRenderOptions;
        private readonly IPaintSelectionScaler _scaler;
        private readonly PaintSelectionOptions _options;

        private int _index;

        public void Clear()
        {
            _index = 0;
            _objectFactory.DestroyAll();
        }

        public IPaintDock CreateDock()
        {
            var dock = _objectFactory.Create(_options.Prefab);
            dock.Construct(_scaler.DockSize, _maskRenderOptions.GetFromBack(_index));
            _index++;
            
            return dock;
        }
    }
}