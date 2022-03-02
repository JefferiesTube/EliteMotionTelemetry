using System;
using System.IO;
using System.ServiceModel.Channels;
using DevExpress.Mvvm.DataAnnotations;
using LibElite.PilotJournal;
using System.Threading.Tasks;
using System.Windows;
using EliteMotionTelemetry.Telemetry;
using Newtonsoft.Json;

namespace EliteMotionTelemetry.ViewModels
{
    public enum ViewState { Loading, Idle, Attached }

    [POCOViewModel]
    public class MainWindowVM
    {
        private JournalReader _journalReader;

        public virtual bool IsLoaded { get; set; }

        public virtual ViewState State { get; set; } = ViewState.Idle;

        public virtual string LoadingMessage { get; set; }
        public virtual string AttachErrorMessage { get; set; }

        public virtual float Speed { get; set; }
        public virtual float Roll { get; set; }
        public virtual float Pitch { get; set; }
        public virtual float Yaw { get; set; }
        public virtual float Heave { get; set; }
        public virtual float Sway { get; set; }
        public virtual float Surge { get; set; }

        private TelemetryProvider _telemetryProvider;

        private MotionDataReader _motionDataReader;

        private MemoryMappings _memoryMappings;

        public MainWindowVM()
        {
            State = ViewState.Loading;
            _journalReader = new JournalReader();
            _telemetryProvider = new TelemetryProvider();
            LoadMappings();            
            SetupMotionReader();
            LoadingMessage = "LOADING JOURNAL";
        }

        private void SetupMotionReader()
        {
            _motionDataReader = new MotionDataReader(_journalReader, _memoryMappings);
            _motionDataReader.OnLostProcess += HandleLostProcess;
            _motionDataReader.OnUpdate += data => _telemetryProvider.SendDatagram(data);
            _motionDataReader.OnUpdate += UpdateLiveData;
        }

        private void UpdateLiveData(MotionData data)
        {
            Speed = data.Speed;
            Roll = data.Roll;
            Pitch = data.Pitch;
            Yaw = data.Yaw;
            Heave = data.Heave;
            Sway = data.Sway;
            Surge = data.Surge;
        }

        private void HandleLostProcess()
        {
            _telemetryProvider.Close();
            _motionDataReader.Release();
            State = ViewState.Idle;
        }

        private void LoadMappings()
        {
            LoadingMessage = "LOADING MAPPINGS";
            string fileName = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "MotionOffsets.json");
            if (!File.Exists(fileName))
            {
                MessageBox.Show("MotionOffsets.json not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                return;
            }

            _memoryMappings = JsonConvert.DeserializeObject<MemoryMappings>(File.ReadAllText(fileName));
        }

        public async void LoadJournal()
        {
            await Task.Yield(); // shows the UI
            _journalReader.ReadJournalFast();
            State = ViewState.Idle;
        }

        public void AttachProcess()
        {
            if (_motionDataReader.Acquire())
            {
                AttachErrorMessage = string.Empty;
                State = ViewState.Attached;
                _telemetryProvider.Open(4444);
            }
            else
            {
                AttachErrorMessage = "Could not attach to process.\nIs the game running?";
            }
        }
    }
}