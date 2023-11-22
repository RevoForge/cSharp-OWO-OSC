using OWOGame;

public class OscMessageHandler
{
    // Dictionary to map OSC addresses to boolean properties
    private Dictionary<string, bool> oscAddressMap = new Dictionary<string, bool>();
    public OscMessageHandler(List<Muscle> activatedMuscles)
    {
        // Initialize the mapping
        InitializeOscAddressMap();

        // Initialize the list of activated muscles
        ActivatedMuscles = activatedMuscles;
    }
    // List to keep track of activated muscles
    public List<Muscle> ActivatedMuscles { get; }
    private void InitializeOscAddressMap()
    {
        // Add your OSC addresses and corresponding boolean properties to the map
        oscAddressMap.Add("/avatar/parameters/owo_suit_Pectoral_R", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Pectoral_L", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Abdominal_R", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Abdominal_L", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Arm_R", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Arm_L", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Dorsal_R", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Dorsal_L", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Lumbar_R", false);
        oscAddressMap.Add("/avatar/parameters/owo_suit_Lumbar_L", false);
    }
    // Method to handle received OSC messages
    public void HandleOscMessage(string oscAddress, bool value)
    {
        // Check if the OSC address is in the map
        if (oscAddressMap.ContainsKey(oscAddress))
        {
            // Update the corresponding boolean property
            oscAddressMap[oscAddress] = value;
            // If the value is true, add the corresponding muscle to the list
            if (value)
            {
                Muscle muscle = GetMuscleFromOscAddress(oscAddress);
                if (!EqualityComparer<Muscle>.Default.Equals(muscle, default))
                {
                    ActivatedMuscles.Add(muscle);
                }
            }
            // If the value is false, remove the corresponding muscle from the list
            else
            {
                Muscle muscle = GetMuscleFromOscAddress(oscAddress);
                if (!EqualityComparer<Muscle>.Default.Equals(muscle, default))
                {
                    ActivatedMuscles.Remove(muscle);
                }
            }
            // Do something with the updated values, e.g., trigger events, update UI, etc.
            Console.WriteLine($"Received: {OSCAddressCleanup(oscAddress)} {value}");
        }
    }
    private string OSCAddressCleanup(string oscAddress)
    {
        string[] parts = oscAddress.Split('/');
        string result = parts[^1];
        return result;
    }

    // Method to get the current state of the boolean properties
    public Dictionary<string, bool> GetOscAddressMap()
    {
        return oscAddressMap;
    }

    // Helper method to get the Muscle instance from the OSC address
    private Muscle GetMuscleFromOscAddress(string oscAddress)
    {
        switch (oscAddress)
        {
            case "/avatar/parameters/owo_suit_Pectoral_R":
                return Muscle.Pectoral_R;
            case "/avatar/parameters/owo_suit_Pectoral_L":
                return Muscle.Pectoral_L;
            case "/avatar/parameters/owo_suit_Abdominal_R":
                return Muscle.Abdominal_R;
            case "/avatar/parameters/owo_suit_Abdominal_L":
                return Muscle.Abdominal_L;
            case "/avatar/parameters/owo_suit_Arm_R":
                return Muscle.Arm_R;
            case "/avatar/parameters/owo_suit_Arm_L":
                return Muscle.Arm_L;
            case "/avatar/parameters/owo_suit_Dorsal_R":
                return Muscle.Dorsal_R;
            case "/avatar/parameters/owo_suit_Dorsal_L":
                return Muscle.Dorsal_L;
            case "/avatar/parameters/owo_suit_Lumbar_R":
                return Muscle.Lumbar_R;
            case "/avatar/parameters/owo_suit_Lumbar_L":
                return Muscle.Lumbar_L;
            default:
                return default;
        }
    }
}
