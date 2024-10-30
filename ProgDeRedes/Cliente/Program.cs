using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Cliente.Menu;
using Communication;

namespace Cliente;

class Program
{
    static readonly SettingsManager settingsManager = new SettingsManager();
    
    private static bool exit = false;
    private static bool logged = false;
    
    public static Socket socketClient;
    
    static void Main(string[] args)
    {
        socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        string clientip = settingsManager.ReadSettings(ClientConfig.ClientIpConfigkey);
        int clientport = int.Parse(settingsManager.ReadSettings(ClientConfig.ClientPortConfigKey));
        string serverip = settingsManager.ReadSettings(ClientConfig.ServerIpConfigkey);
        int serverport = int.Parse(settingsManager.ReadSettings(ClientConfig.ServerPortConfigKey));


        var localEndpoint = new IPEndPoint(IPAddress.Parse(clientip), clientport);
        var remoteEndpoint = new IPEndPoint(IPAddress.Parse(serverip), serverport);

        bool connected = TryConnectToServer(localEndpoint, remoteEndpoint, serverip, serverport);
        
        if (connected)
        {
            RunMainLoop();
        }
    }
    
    public static bool TryConnectToServer(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, string serverip, int serverport)
    {
        bool connected = false;

        while (!connected && !exit)
        {
            try
            {
                if (socketClient != null)
                {
                    socketClient.Close();
                    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                
                socketClient.Bind(localEndpoint);
                Console.WriteLine("Intentando conectar al servidor...");
                socketClient.Connect(remoteEndpoint);
                connected = true;
                Console.WriteLine($"Conectado al servidor en {serverip}:{serverport}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"No se pudo conectar al servidor: {ex.Message}");
                Console.WriteLine("Reintentando en 2 segundos...");
                System.Threading.Thread.Sleep(2000);
            }
        }

        return connected;
    }
    
    public static void RunMainLoop()
    {
        try
        {
            NetworkDataHelper networkDataHelper = new NetworkDataHelper(socketClient);

            while (!exit)
            {
                while (!logged && !exit)
                {
                    logged = SessionManager.ShowSession(networkDataHelper, ref exit);
                }

                while (logged && !exit)
                {
                    MenuManager.ShowMenu(networkDataHelper, ref logged, ref exit);
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error de conexión: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Desconectado del servidor...");
            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();
        }
    }
    
    public static void SendData(NetworkDataHelper networkDataHelper, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        byte[] dataLength = BitConverter.GetBytes(data.Length);

        networkDataHelper.Send(dataLength);
        networkDataHelper.Send(data);
    }

    public static string ReceiveResponse(NetworkDataHelper networkDataHelper)
    {
        byte[] responseLengthBytes = networkDataHelper.Receive(4);
        int responseLength = BitConverter.ToInt32(responseLengthBytes, 0);
        byte[] responseData = networkDataHelper.Receive(responseLength);
        return Encoding.UTF8.GetString(responseData);
    }

    public static void ShowResponse(string responseMessage)
    {
        string code = responseMessage.Split("#")[0];
        string message = responseMessage.Split("#")[1];

        if (code == "0")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{message}");
            Console.ResetColor();
        }
        else
        {
            if (code == "1")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{message}");
                Console.ResetColor();
            }
        }
    }
    
}