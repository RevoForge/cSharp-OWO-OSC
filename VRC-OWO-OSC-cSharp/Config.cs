using Newtonsoft.Json;

public class Config
{
    public ConfigValues defaultConfig = new()
    {
        doc0 = "Default VRChat Sending To port 9001",
        server_port = 9001, // Default VRChat Sending To port 9001
        doc1 = "Set IP of the Device running the OWO App If not using AutoConnect",
        owo_ip = "0.0.0.0", // Set to the IP of the Device running the OWO App
        doc2 = "Set False to use owo_ip ",
        useAutoConnect = true, //Set False to use owo_ip 
        doc3 = "Do Not Set False, Future Setting",
        connectAtStart = true, //Do Not Set False, Future Setting, This matters after we have a GUI
        frequency = 100,
        intensititys = new IntensitiesConfig
        {
            owo_suit_Pectoral_L = 100,
            owo_suit_Pectoral_R = 100,
            owo_suit_Abdominal_L = 100,
            owo_suit_Abdominal_R = 100,
            owo_suit_Arm_L = 100,
            owo_suit_Arm_R = 100,
            owo_suit_Dorsal_L = 100,
            owo_suit_Dorsal_R = 100,
            owo_suit_Lumbar_L = 100,
            owo_suit_Lumbar_R = 100,
        }
    };
    public ConfigValues currentConfig;

    public int GetByKey(string key)
    {
        return currentConfig.GetValueByKey(key);
    }

    public void Update(string key, object nextValue)
    {
        currentConfig.UpdateValueByKey(key, nextValue);
    }

    public ConfigValues ReadConfigFromDisk()
    {
        string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string configPath = Path.Combine(exeDirectory, "config.json");
        Console.WriteLine("Config File Can be Found At " + configPath); // Mostly For Debugging

        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            var deserializedConfig = JsonConvert.DeserializeObject<ConfigValues>(json);

            // Ensure owo_ip is not null
            if (string.IsNullOrEmpty(deserializedConfig.owo_ip))
            {
                deserializedConfig.owo_ip = defaultConfig.owo_ip;
            }

            // Ensure intensititys is not null
            if (deserializedConfig.intensititys == null)
            {
                deserializedConfig.intensititys = defaultConfig.intensititys;
            }

            return deserializedConfig;
        }
        else
        {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));

            return defaultConfig;
        }
    }
    // NEEDS GUI Save Button 
    public void WriteConfigToDisk()
    {
        if (currentConfig == null)
        {
            return;
        }

        string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string configPath = Path.Combine(exeDirectory, "config.json");

        File.WriteAllText(configPath, JsonConvert.SerializeObject(currentConfig, Formatting.Indented));
    }

    public void Init()
    {
        currentConfig = ReadConfigFromDisk();
    }
}

public class ConfigValues
{
    public string doc0 { get; set; }
    public int server_port { get; set; }
    public string doc1 { get; set; }
    public string owo_ip { get; set; }
    public string doc2 { get; set; }
    public bool useAutoConnect { get; set; }
    public string doc3 { get; set; }
    public bool connectAtStart { get; set; }
    public int frequency { get; set; }
    public IntensitiesConfig intensititys { get; set; }
    public int GetValueByKey(string key)
    {
        switch (key)
        {
            case "server_port": return server_port;
            case "frequency": return frequency;
            // Add other cases for each property you want to retrieve
            default: return 0; // or throw an exception for an unknown key
        }
    }

    public void UpdateValueByKey(string key, object nextValue)
    {
        switch (key)
        {
            case "server_port": server_port = (int)nextValue; break;
            case "frequency": frequency = (int)nextValue; break;
                // Add other cases for each property you want to update
        }
    }
}

public class IntensitiesConfig
{
    public int owo_suit_Pectoral_L { get; set; }
    public int owo_suit_Pectoral_R { get; set; }
    public int owo_suit_Abdominal_L { get; set; }
    public int owo_suit_Abdominal_R { get; set; }
    public int owo_suit_Arm_L { get; set; }
    public int owo_suit_Arm_R { get; set; }
    public int owo_suit_Dorsal_L { get; set; }
    public int owo_suit_Dorsal_R { get; set; }
    public int owo_suit_Lumbar_L { get; set; }
    public int owo_suit_Lumbar_R { get; set; }
}
