using Features.Services;

namespace Features.GamePlay
{
    public class PaintSelection : IPaintSelection
    {
        public PaintSelection(
            IObjectFactory<PaintDock> objectFactory,
            IPaintSelectionScaler scaler,
            PaintSelectionOptions options)
        {
            _objectFactory = objectFactory;
            _scaler = scaler;
            _options = options;
        }

        private readonly IObjectFactory<PaintDock> _objectFactory;
        private readonly IPaintSelectionScaler _scaler;
        private readonly PaintSelectionOptions _options;

        public IPaintDock CreateDock()
        {
            var dock = _objectFactory.Create(_options.Prefab);
            dock.Construct(_scaler.DockSize);
            
            return dock;
        }
    }
}