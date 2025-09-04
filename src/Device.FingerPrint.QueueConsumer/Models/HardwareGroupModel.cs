namespace Device.FingerPrint.QueueConsumer.Models
{
    public class HardwareGroupModel
    {
        public string? Platform { get; set; }
        public string? Architecture { get; set; }
        public string? Oscpu { get; set; }
        public string? CpuClass { get; set; }
        public object? HardwareConcurrency { get; set; } // number | string
        public object? DeviceMemory { get; set; } // number | string
    }

    public class DeviceMemoryModel
    {
        public object? DeviceMemory { get; set; } // number | string
    }

    public class HardwareConcurrencyModel
    {
        public object? HardwareConcurrency { get; set; } // number | string
    }

    public class CpuClassModel
    {
        public string? CpuClass { get; set; }
    }

    public class PlatformModel
    {
        public string? Platform { get; set; }
    }

    public class ArchitectureModel
    {
        public string? Architecture { get; set; }
    }

    public class OSCpuModel
    {
        public string? Oscpu { get; set; }
    }
}
