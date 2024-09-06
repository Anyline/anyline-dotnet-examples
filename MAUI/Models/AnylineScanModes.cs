
namespace Anyline.Examples.MAUI.Models
{
    /// <summary>
    /// This class is only used to display the list of  Anyline's Technical Capabilities on the Main Screen.
    /// You can ignore this file for your project.
    /// </summary>
    public class AnylineScanModes
    {
        public static List<AnylineScanModeGroup> GetAnylineScanModesGroupedList()
        {
            return new List<AnylineScanModeGroup>()
            {
                new AnylineScanModeGroup("Energy", new List<AnylineScanMode>{
                    new AnylineScanMode("Analog/Digital Meter","energy_analog_digital_config.json"),
                    new AnylineScanMode("Dial Meter","energy_dial_config.json"),
                }),
                new AnylineScanModeGroup("Identity Documents", new List<AnylineScanMode>{
                    new AnylineScanMode("Universal ID","id_config_universal.json"),
                    new AnylineScanMode("Arabic ID","id_config_arabic.json"),
                    new AnylineScanMode("Cyrillic ID","id_config_cyrillic.json"),
                    new AnylineScanMode("MRZ / Passport","id_config_mrz.json"),
                    new AnylineScanMode("Japanese Landing Permission","id_config_jlp.json"),
                }),
                new AnylineScanModeGroup("Vehicle", new List<AnylineScanMode>{
                    new AnylineScanMode("License Plates - EU", "vehicle_config_license_plate_eu.json"),
                    new AnylineScanMode("License Plates - USA", "vehicle_config_license_plate_us.json"),
                    new AnylineScanMode("License Plates - Africa", "vehicle_config_license_plate_africa.json"),
                    new AnylineScanMode("Vehicle Identification Number (VIN)", "vehicle_vin_config.json"),
                    new AnylineScanMode("Vehicle Registration Certificate", "vehicle_registration_certificate_config.json"),
                }),
                new AnylineScanModeGroup("Tire", new List<AnylineScanMode>{
                    new AnylineScanMode("TIN - Universal","tire_tin_universal_config.json"),
                    new AnylineScanMode("TIN - DOT (North America Only)","tire_tin_dot_config.json"),
                    new AnylineScanMode("Tire Size Specifications","tire_size_config.json"),
                    new AnylineScanMode("Commercial Tire Identification Numbers","tire_commercial_tire_id_config.json"),
                    new AnylineScanMode("Tire Make","tire_make_config.json"),
                }),
                new AnylineScanModeGroup("Maintenance, Repair & Operations", new List<AnylineScanMode>{
                    new AnylineScanMode("Universal Serial Number", "mro_usnr_config.json"),
                    new AnylineScanMode("Shipping Container - Horizontal", "mro_shipping_container_horizontal_config.json"),
                    new AnylineScanMode("Shipping Container - Vertical", "mro_shipping_container_vertical_config.json"),
                }),
                new AnylineScanModeGroup("Barcode", new List<AnylineScanMode>{
                    new AnylineScanMode("Barcode", "others_config_barcode.json"),
                }),
                new AnylineScanModeGroup("Composite", new List<AnylineScanMode>{
                    new AnylineScanMode("Serial Scanning (LPT - EU > DVL > VIN)","workflows_config_serial_scanning.json"),
                    new AnylineScanMode("Parallel Scanning (Meter / Serial Number)","workflows_config_parallel_scanning.json"),
                    new AnylineScanMode("Parallel - First Scan (VIN or Barcode)","workflows_config_parallel_first_scan.json")
                })
            };
        }
    }

    public class AnylineScanModeGroup : List<AnylineScanMode>
    {
        public string Name { get; private set; }

        public AnylineScanModeGroup(string name, List<AnylineScanMode> scanModes) : base(scanModes)
        {
            Name = name;
        }
    }

    public class AnylineScanMode
    {
        public string Name { get; set; }
        public string JSONConfigPath { get; set; }

        public AnylineScanMode(string name, string jsonConfigPath, string configsPath = "Configs/")
        {
            Name = name;
            JSONConfigPath = configsPath + jsonConfigPath;
        }
    }

}
