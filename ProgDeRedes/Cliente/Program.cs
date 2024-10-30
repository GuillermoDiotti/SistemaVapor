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
    static readonly SettingsManager SettingsManager = new SettingsManager();
    
    public static bool _exit = false;
    public static bool _logged = false;

    public static TcpClient tcpClient;
    
    static async Task Main(string[] args)
    {
        string clientip = SettingsManager.ReadSettings(ClientConfig.ClientIpConfigkey);
        int clientport = int.Parse(SettingsManager.ReadSettings(ClientConfig.ClientPortConfigKey));
        string serverip = SettingsManager.ReadSettings(ClientConfig.ServerIpConfigkey);
        int serverport = int.Parse(SettingsManager.ReadSettings(ClientConfig.ServerPortConfigKey));


        var localEndpoint = new IPEndPoint(IPAddress.Parse(clientip), clientport);
        var remoteEndpoint = new IPEndPoint(IPAddress.Parse(serverip), serverport);

        
        var connected = await TryConnectToServer(localEndpoint, remoteEndpoint, serverip, serverport);
        
        if (connected)
        {
            await RunMainLoop();
        }
    }
    
    public static async Task<bool> TryConnectToServer(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, string serverip, int serverport)
    {
        bool connected = false;

        while (!connected && !_exit)
        {
            try
            {
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }

                tcpClient = new TcpClient(localEndpoint);
                Console.WriteLine("Intentando conectar al servidor...");
                await tcpClient.ConnectAsync(remoteEndpoint);
                connected = true;
                Console.WriteLine($"Conectado al servidor en {serverip}:{serverport}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"No se pudo conectar al servidor: {ex.Message}");
                Console.WriteLine("Reintentando en 2 segundos...");
                await Task.Delay(2000);
                
            }
        }

        return connected;
    }
    
    public static async Task RunMainLoop()
    {
        try
        {
            NetworkDataHelper networkDataHelper = new NetworkDataHelper(tcpClient);

            while (!_exit)
            {
                while (!_logged && !_exit)
                {
                    _logged = await SessionManager.ShowSession(networkDataHelper);
                }

                while (_logged && !_exit)
                {
                    await MenuManager.ShowMenu(networkDataHelper);
                }
            }
            
            byte[] actionCode = BitConverter.GetBytes((int)Actions.CloseConnection);
            await networkDataHelper.SendAsync(actionCode);
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"Error de conexión: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Desconectado del servidor...");
            
            tcpClient.Close();
        }
    }
    
    public static async Task SendData(NetworkDataHelper networkDataHelper, string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        byte[] dataLength = BitConverter.GetBytes(data.Length);

        await networkDataHelper.SendAsync(dataLength);
        await networkDataHelper.SendAsync(data);
    }

    public static async Task<string> ReceiveResponse(NetworkDataHelper networkDataHelper)
    {
        byte[] responseLengthBytes = await networkDataHelper.ReceiveAsync(4);
        int responseLength = BitConverter.ToInt32(responseLengthBytes, 0);
        byte[] responseData = await networkDataHelper.ReceiveAsync(responseLength);
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