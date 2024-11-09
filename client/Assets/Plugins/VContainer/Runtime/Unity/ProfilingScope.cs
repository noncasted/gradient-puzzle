using System.Diagnostics;

namespace VContainer.Unity
{
    public struct ProfilingScope
    {
        private const bool _isEnabled = false;

        private readonly string _name;
        private readonly Stopwatch _stopwatch;

        private long _lastStep;

        public ProfilingScope(string name)
        {
            _name = name;
            _lastStep = 0;

            if (_isEnabled == false)
            {
                _stopwatch = null;
                return;
            }

            _stopwatch = Stopwatch.StartNew();
        }

        public void Step(string name)
        {
            if (_isEnabled == false)
                return;

            var delta = _stopwatch.ElapsedMilliseconds - _lastStep;
            _lastStep = _stopwatch.ElapsedMilliseconds;
            UnityEngine.Debug.Log($"{_name} - {name} delta: {delta}");
        }

        public void Dispose()
        {
            if (_isEnabled == false)
                return;

            UnityEngine.Debug.Log($"{_name} completed: {_stopwatch.ElapsedMilliseconds}");
            _stopwatch.Stop();
        }
    }
}