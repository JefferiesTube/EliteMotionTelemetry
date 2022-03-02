namespace EliteMotionTelemetry.Telemetry
{
    public class MemoryMappingSet
    {
        public string Speed;
        public string Sway;
        public string Heave;
        public string Surge;
        public string Roll;
        public string Pitch;
        public string Yaw;
    }

    public class MemoryMappings
    {
        public MemoryMappingSet HorizonsShip;
        public MemoryMappingSet HorizonsSRV;
        public MemoryMappingSet OdysseyShip;
        public MemoryMappingSet OdysseySRV;
    }
}