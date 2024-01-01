using Newtonsoft.Json;

public class Config
{
    public ConfigValues defaultConfig = new()
    {
        Doc0 = "Default VRChat Sending To port 9001",
        Server_port = 9001, // Default VRChat Sending To port 9001
        Doc1 = "Set IP of the Device running the OWO App If not using AutoConnect",
        Owo_ip = "0.0.0.0", // Set to the IP of the Device running the OWO App
        Doc2 = "Set False to use owo_ip ",
        UseAutoConnect = true, //Set False to use owo_ip 
        Doc3 = "Do Not Set False, Future Setting",
        ConnectAtStart = true, //Do Not Set False, Future Setting, This matters after we have a GUI
        Frequency = 100,
        Intensititys = new IntensitiesConfig
        {
            OwoSuitPectoralL = 100,
            OwoSuitPectoralR = 100,
            OwoSuitAbdominalL = 100,
            OwoSuitAbdominalR = 100,
            OwoSuitArmL = 100,
            OwoSuitArmR = 100,
            OwoSuitDorsalL = 100,
            OwoSuitDorsalR = 100,
            OwoSuitLumbarL = 100,
            OwoSuitLumbarR = 100,
        }
    };
    public ConfigValues currentConfig;
    public void Init()
    {
        currentConfig = ReadConfigFromDisk();
    }
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
            if (string.IsNullOrEmpty(deserializedConfig.Owo_ip))
            {
                deserializedConfig.Owo_ip = defaultConfig.Owo_ip;
            }

            // Ensure intensititys is not null
            deserializedConfig.Intensititys ??= defaultConfig.Intensititys;

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
}

public class ConfigValues
{
    public string Doc0 { get; set; }
    public int Server_port { get; set; }
    public string Doc1 { get; set; }
    public string Owo_ip { get; set; }
    public string Doc2 { get; set; }
    public bool UseAutoConnect { get; set; }
    public string Doc3 { get; set; }
    public bool ConnectAtStart { get; set; }
    public int Frequency { get; set; }
    public IntensitiesConfig Intensititys { get; set; }
    public int GetValueByKey(string key)
    {
        return key switch
        {
            "server_port" => Server_port,
            "frequency" => Frequency,
            // Add other cases for each property you want to retrieve
            _ => 0,// or throw an exception for an unknown key
        };
    }

    public void UpdateValueByKey(string key, object nextValue)
    {
        switch (key)
        {
            case "server_port": Server_port = (int)nextValue; break;
            case "frequency": Frequency = (int)nextValue; break;
                // Add other cases for each property you want to update
        }
    }
}

public class IntensitiesConfig
{
    public int OwoSuitPectoralL { get; set; }
    public int OwoSuitPectoralR { get; set; }
    public int OwoSuitAbdominalL { get; set; }
    public int OwoSuitAbdominalR { get; set; }
    public int OwoSuitArmL { get; set; }
    public int OwoSuitArmR { get; set; }
    public int OwoSuitDorsalL { get; set; }
    public int OwoSuitDorsalR { get; set; }
    public int OwoSuitLumbarL { get; set; }
    public int OwoSuitLumbarR { get; set; }
}
