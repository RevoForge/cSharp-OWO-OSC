using OscCore;
using OWOGame;
using System.Net;
using System.Net.Sockets;

class Program
{
    private static List<Muscle> activatedMuscles = new();
    private static OscMessageHandler oscMessageHandler = new(activatedMuscles);
    private static int pectoral_L_Int;
    private static int pectoral_R_Int;
    private static int abdominal_L_Int;
    private static int abdominal_R_Int;
    private static int arm_L_Int;
    private static int arm_R_Int;
    private static int dorsal_L_Int;
    private static int dorsal_R_Int;
    private static int lumbar_L_Int;
    private static int lumbar_R_Int;
    private static string oldMuscleInfo = "";
    private static int muscleCounter = 0;
    public static Config config = new();

    static void Main()
    {

        config.Init();
        MicroSensation microSensation;

        // GUI NEEDED Sliders mostly prolly
        int frequency = config.GetByKey("frequency"); ;
        float duration = 0.2f;
        int intensity = 100;
        float rampUp = 0;
        float rampDown = 0;
        pectoral_L_Int = config.currentConfig.Intensititys.OwoSuitPectoralL;
        pectoral_R_Int = config.currentConfig.Intensititys.OwoSuitPectoralR;
        abdominal_L_Int = config.currentConfig.Intensititys.OwoSuitAbdominalL;
        abdominal_R_Int = config.currentConfig.Intensititys.OwoSuitAbdominalR;
        arm_L_Int = config.currentConfig.Intensititys.OwoSuitArmL;
        arm_R_Int = config.currentConfig.Intensititys.OwoSuitArmR;
        dorsal_L_Int = config.currentConfig.Intensititys.OwoSuitDorsalL;
        dorsal_R_Int = config.currentConfig.Intensititys.OwoSuitDorsalR;
        lumbar_L_Int = config.currentConfig.Intensititys.OwoSuitLumbarL;
        lumbar_R_Int = config.currentConfig.Intensititys.OwoSuitLumbarR;
        int port = config.GetByKey("server_port");
        string suitIP = config.currentConfig.Owo_ip;
        //-------------

        Timer sendTimer;
        bool autoConnect = config.currentConfig.UseAutoConnect;
        bool startUpConnect = config.currentConfig.ConnectAtStart;

        if (startUpConnect)
        {
            OWOConnect(autoConnect, suitIP);
            while (OWO.ConnectionState != ConnectionState.Connected)
            {
                Thread.Sleep(250);
            }
        }

        // Create a UdpClient to listen for UDP messages on the loopback address
        UdpClient udpClient = new(new IPEndPoint(IPAddress.Loopback, port));

        Console.WriteLine($"Listening for OSC messages on port {port}");

        // Initialize microSensation outside the loop
        microSensation = SensationsFactory.Create(frequency, duration, intensity, rampUp, rampDown, 0);

        // Set up the timer to trigger the send function every 0.2 seconds
        sendTimer = new Timer(state => SendActivatedMuscles(microSensation), null, TimeSpan.Zero, TimeSpan.FromSeconds(0.2));

        while (true)
        {
            try
            {
                // Receive bytes
                IPEndPoint senderEndPoint = new(IPAddress.Any, 0);
                byte[] receivedBytes = udpClient.Receive(ref senderEndPoint);

                // Parse the received OSC message using OscCore
                var oscPacket = OscPacket.Read(receivedBytes, 0, receivedBytes.Length);

                if (oscPacket is OscMessage oscMessage)
                {
                    // Handle the received OSC message
                    //Console.WriteLine($"Received OSC Message: {oscMessage.Address}"); //for debug
                    if (oscMessage.Address.Contains("owo_"))
                    {
                        for (int i = 0; i < oscMessage.Count; i++)
                        {
                            //Console.WriteLine($"Argument {i}: {oscMessage[i]}"); //for debug

                            // Handle the OSC message using the handler
                            if (oscMessage[i] is IConvertible convertible)
                            {
                                if (convertible.GetTypeCode() == TypeCode.Boolean)
                                {
                                    // do the stuff with owo suit bool parameters
                                    oscMessageHandler.HandleOscMessage(oscMessage.Address, (bool)oscMessage[i]);
                                }
                                else if (convertible.GetTypeCode() == TypeCode.Single)
                                {
                                    string address = oscMessage.Address.ToLower();

                                    switch (address)
                                    {
                                        case var _ when address.Contains("intensity"):
                                            UpdateParameter("intensity", (float)oscMessage[i] * 100);
                                            Console.WriteLine($"\nIntensity Changed to: {intensity}");
                                            break;

                                        case var _ when address.Contains("frequency"):
                                            UpdateParameter("frequency", (float)oscMessage[i] * 100);
                                            Console.WriteLine($"\nFrequency Changed to: {frequency}");
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        void UpdateParameter(string paramName, float value)
        {
            switch (paramName)
            {
                case "intensity":
                    intensity = (int)value;
                    break;

                case "frequency":
                    frequency = (int)value;
                    break;
            }

            microSensation = SensationsFactory.Create(frequency, duration, intensity, rampUp, rampDown, 0);
        }
    }

    // NEEDS GUI BUTTON for use of !startUpConnect
    static void OWOConnect(bool autoConnect, string suitIP)
    {
        Console.WriteLine("Waiting for Suit Connection");
        if (autoConnect)
        {
            OWO.AutoConnect();
        }
        else
        {
            OWO.Connect(suitIP);
        }
        while (OWO.ConnectionState != ConnectionState.Connected)
        {
            Thread.Sleep(250);
        }
        Console.WriteLine("Suit Connected");
    }

    // Function to be called by the timer for owo send event
    private static void SendActivatedMuscles(MicroSensation microSensation)
    {
        if (activatedMuscles.Count > 0)
        {
            List<Muscle> updatedMuscles = new();

            foreach (var muscle in activatedMuscles)
            {
                int muscleInt = 100;

                // Update intensity based on muscle type
                switch (muscle.id)
                {
                    case 0: // Pectoral_R
                        muscleInt = pectoral_R_Int;
                        break;
                    case 1: // Pectoral_L
                        muscleInt = pectoral_L_Int;
                        break;
                    case 2: // Abdominal_R
                        muscleInt = abdominal_R_Int;
                        break;
                    case 3: // Abdominal_L
                        muscleInt = abdominal_L_Int;
                        break;
                    case 4: // Arm_R
                        muscleInt = arm_R_Int;
                        break;
                    case 5: // Arm_L
                        muscleInt = arm_L_Int;
                        break;
                    case 6: // Dorsal_R
                        muscleInt = dorsal_R_Int;
                        break;
                    case 7: // Dorsal_L
                        muscleInt = dorsal_L_Int;
                        break;
                    case 8: // Lumbar_R
                        muscleInt = lumbar_R_Int;
                        break;
                    case 9: // Lumbar_L
                        muscleInt = lumbar_L_Int;
                        break;
                }

                // Capture the new instance with updated intensity
                var updatedMuscle = muscle.WithIntensity(muscleInt);

                updatedMuscles.Add(updatedMuscle);
            }

            Muscle[] musclesArray = updatedMuscles.ToArray();
            OWO.Send(microSensation, musclesArray);

            // Concatenate muscle information into a single string
            string musclesInfo = string.Join(",", musclesArray.Select(m => $"{MuslceName(m.id)}:{m.intensity}"));
            if (oldMuscleInfo != musclesInfo)
            {
                muscleCounter = 1;
                oldMuscleInfo = musclesInfo;
                // Print the muscle information
                Console.WriteLine($"Muscles: {musclesInfo}");
            }
            else
            {
                Console.Write($"\rEvent Counter: {muscleCounter}");
                muscleCounter++;
            }
        }
        else
        {
            if (oldMuscleInfo != "")
            {
                oldMuscleInfo = "";
                muscleCounter = 0;
            }
        }
    }
    // just for making the triggered muscles pretty in the console
    private static string MuslceName(int muscleID)
    {
        return muscleID switch
        {
            // Pectoral_R
            0 => "Pectoral_R",
            // Pectoral_L
            1 => "Pectoral_L",
            // Abdominal_R
            2 => "Abdominal_R",
            // Abdominal_L
            3 => "Abdominal_L",
            // Arm_R
            4 => "Arm_R",
            // Arm_L
            5 => "Arm_L",
            // Dorsal_R
            6 => "Dorsal_R",
            // Dorsal_L
            7 => "Dorsal_L",
            // Lumbar_R
            8 => "Lumbar_R",
            // Lumbar_L
            9 => "Lumbar_L",
            _ => "Muslce ID Error",
        };
    }
}
