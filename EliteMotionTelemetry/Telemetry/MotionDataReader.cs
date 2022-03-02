using LibElite.Logging;
using LibElite.PilotJournal;
using LibElite.PilotJournal.Events.Startup;
using Memory;
using System;
using System.Linq;
using System.Threading;

namespace EliteMotionTelemetry.Telemetry
{
    public class MotionDataReader
    {
        private int _referenceCount;
        private Mem _reader;

        private const string ProcessName = "EliteDangerous64";
        private const string ProcessMemoryName = "EliteDangerous64.exe";

        private Timer _timer;

        private bool _inSRV;
        private bool _inShip = true;
        private bool _horizons;
        private JournalReader _journalReader;

        private MemoryMappingSet _memoryMappingSet;
        private MemoryMappings _mappings;

        public event Action OnAttach;

        public event Action OnRelease;

        public event Action OnLostProcess;

        public event Action<MotionData> OnUpdate;

        public MotionDataReader(JournalReader reader, MemoryMappings mappings)
        {
            _reader = new Mem();
            _referenceCount = 0;
            _journalReader = reader;
            _mappings = mappings;
            _journalReader.OnLoadCompleted += LoadInitialState;
        }

        private void LoadInitialState()
        {
            _journalReader.OnLoadCompleted -= LoadInitialState;
            LoadGame lg = _journalReader.Events.OfType<LoadGame>().Last();
            _horizons = !lg.Odyssey;
        }

        public bool Acquire()
        {
            if (_referenceCount == 0)
            {
                UpdateMemoryMappings();
                _reader = new Mem();
                if (!_reader.OpenProcess(ProcessName))
                {
                    Logging.Error("Error attaching memory reader. Is Elite running?");
                    return false;
                }
                _timer = new Timer(ReadMotionData, null, 0, 50);
                _journalReader.Notifications.OnInSRVChanged += state =>
                {
                    _inSRV = state;
                    _inShip = !state;
                    UpdateMemoryMappings();
                };
                _journalReader.Notifications.OnFootChanged += state =>
                {
                    if (state)
                    {
                        _inSRV = false;
                        _inShip = false;
                        UpdateMemoryMappings();
                    }
                };

                _journalReader.OnNewEvent += e =>
                {
                    if (e is LoadGame lg)
                    {
                        _horizons = !lg.Odyssey;
                        UpdateMemoryMappings();
                    }
                };
                OnAttach?.Invoke();
            }
            _referenceCount++;
            return true;
        }

        private void UpdateMemoryMappings()
        {
            if (_horizons)
                _memoryMappingSet = _inSRV ? _mappings.HorizonsSRV : _mappings.HorizonsShip;
            else
                _memoryMappingSet = _inSRV ? _mappings.OdysseySRV : _mappings.OdysseyShip;
        }

        public void Release()
        {
            _referenceCount--;
            if (_referenceCount == 0 && _reader != null)
            {
                OnRelease?.Invoke();
                _reader.CloseProcess();
            }
        }

        private void ReadMotionData(object state)
        {
            if (_reader.mProc.Process.HasExited)
            {
                OnLostProcess?.Invoke();
                _timer.Dispose();
                return;
            }

            if (_memoryMappingSet == null)
                return;

            MotionData data = new MotionData();
            data.Speed = _reader.ReadFloat($"{ProcessMemoryName}+{_memoryMappingSet.Speed}");
            data.Roll = _reader.ReadFloat($"{ProcessMemoryName}+{_memoryMappingSet.Roll}");
            data.Pitch = _reader.ReadFloat($"{ProcessMemoryName}+{_memoryMappingSet.Pitch}");
            data.Yaw = _reader.ReadFloat($"{ProcessMemoryName}+{_memoryMappingSet.Yaw}");
            data.Heave = _reader.ReadFloat($"{ProcessMemoryName}+{_memoryMappingSet.Heave}");
            data.Surge = _reader.ReadFloat($"{ProcessMemoryName}+{_memoryMappingSet.Surge}");
            data.Sway = _reader.ReadFloat($"{ProcessMemoryName}+{_memoryMappingSet.Sway}");

            OnUpdate?.Invoke(data);
        }
    }
}