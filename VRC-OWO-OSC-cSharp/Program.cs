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
        pectoral_L_Int = config.currentConfig.intensititys.owo_suit_Pectoral_L;
        pectoral_R_Int = config.currentConfig.intensititys.owo_suit_Pectoral_R;
        abdominal_L_Int = config.currentConfig.intensititys.owo_suit_Abdominal_L;
        abdominal_R_Int = config.currentConfig.intensititys.owo_suit_Abdominal_R;
        arm_L_Int = config.currentConfig.intensititys.owo_suit_Arm_L;
        arm_R_Int = config.currentConfig.intensititys.owo_suit_Arm_R;
        dorsal_L_Int = config.currentConfig.intensititys.owo_suit_Dorsal_L;
        dorsal_R_Int = config.currentConfig.intensititys.owo_suit_Dorsal_R;
        lumbar_L_Int = config.currentConfig.intensititys.owo_suit_Lumbar_L;
        lumbar_R_Int = config.currentConfig.intensititys.owo_suit_Lumbar_R;
        int port = config.GetByKey("server_port");
        string suitIP = config.currentConfig.owo_ip;
        //-------------

        Timer sendTimer;
        bool autoConnect = config.currentConfig.useAutoConnect;
        bool startUpConnect = config.currentConfig.connectAtStart;

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
                IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedBytes = udpClient.Receive(ref senderEndPoint);

                // Parse the received OSC message using OscCore
                var oscPacket = OscPacket.Read(receivedBytes, 0, receivedBytes.Length);

                if (oscPacket is OscMessage oscMessage)
                {
                    // Handle the received OSC message
                    //Console.WriteLine($"Received OSC Message: {oscMessage.Address}"); //for debug
                    if (oscMessage.Address.Contains("owo_suit"))
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
                                else if (convertible.GetTypeCode() == TypeCode.Int32)
                                {
                                    // Update the Intensity with a Parameter "owo_suit_Intensity"
                                    intensity = (int)oscMessage[i];
                                    microSensation = SensationsFactory.Create(frequency, duration, intensity, rampUp, rampDown, 0);
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
            List<Muscle> updatedMuscles = new List<Muscle>();

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

            // Print the muscle information
            Console.WriteLine($"Muscles: {musclesInfo}");
        }
    }
    // just for making the triggered muscles pretty in the console
    private static string MuslceName(int muscleID)
    {
        switch (muscleID)
        {
            case 0: // Pectoral_R
                return "Pectoral_R";
            case 1: // Pectoral_L
                return "Pectoral_L";
            case 2: // Abdominal_R
                return "Abdominal_R";
            case 3: // Abdominal_L
                return "Abdominal_L";
            case 4: // Arm_R
                return "Arm_R";
            case 5: // Arm_L
                return "Arm_L";
            case 6: // Dorsal_R
                return "Dorsal_R";
            case 7: // Dorsal_L
                return "Dorsal_L";
            case 8: // Lumbar_R
                return "Lumbar_R";
            case 9: // Lumbar_L
                return "Lumbar_L";
        }
        return "Muslce ID Error";
    }
}
