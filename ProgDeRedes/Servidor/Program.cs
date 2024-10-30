using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Communication;
using Servidor.Collections;
using Servidor.Logics;
using Servidor.Logics.GameLogic;
using Servidor.Logics.ReviewLogic;
using Servidor.Logics.UserLogic;

namespace Servidor;

class Program
{
    private static readonly object _tcpClientServerLock = new object();
    
    static readonly SettingsManager settingsManager = new SettingsManager();
    static bool salir = false;

    static async Task Main(string[] args)
    {
        Console.WriteLine("Inicio el servidor...");

        string serverip = settingsManager.ReadSettings(ServerConfig.ServerIpConfigkey);
        int serverport = int.Parse(settingsManager.ReadSettings(ServerConfig.ServerPortConfigKey));

        var localEndpoint = new IPEndPoint(IPAddress.Parse(serverip), serverport);

        
        
        try
        {
            TcpListener tcpListener = new TcpListener(localEndpoint);
            Task handleServer = Task.Run(async () => await HandleServer(tcpListener));
            tcpListener.Start();
            
            Console.WriteLine($"Servidor escuchando en {serverip}:{serverport}");
            Console.WriteLine("Esperando clientes...");
                
            while (!salir) 
            {
                try
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    TcpListenerCollection.Instance.AddTcpClient(tcpClient);
                    Console.WriteLine("Nuevo cliente conectado");
                    Task client = Task.Run(() => HandleClient(tcpClient));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Conexion cerrada");
                    await Task.Delay(5000);
                    salir = true;
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error del servidor: {ex.Message}");
        }
    }

    private static async Task HandleServer(TcpListener tcpListener)
    {
        while(!salir){
            Console.WriteLine("Escribe 'exit' para cerrar el servidor.");
            Console.WriteLine("Escribe 'files' para conocer el tamaño promedio, tamaño total y cantidad de fotos de los juegos que tiene stock.");
            var option = Console.ReadLine();
            if (option.Equals("exit"))
            {
                lock (_tcpClientServerLock)
                {
                    foreach (var sock in TcpListenerCollection.Instance.TcpClients)
                    {
                        sock.Close();
                    }
                    salir = true;
    
                }
                tcpListener.Stop();  
            }
            else
            {
                if (option.Equals("files"))
                {
                    FileHandler fileHandler = new FileHandler();
                    long[] fileInfo = GameCollection.Instance.GetFilesInfo(fileHandler);
                
                    Console.WriteLine($"Tamaño promedio de las fotos de los juegos que tienen stock: {fileInfo[1]}");
                    Console.WriteLine($"Tamaño total de las fotos de los juegos que tienen stock: {fileInfo[0]}");
                    Console.WriteLine($"Cantidad de fotos de los juegos que tienen stock: {fileInfo[2]}");
                }
            }
        }
    }

    private static async Task HandleClient(TcpClient tcpClient)
    {
        User? currentUser = null;
        Game? currentGame = null;
        
        NetworkDataHelper networkDataHelper = new NetworkDataHelper(tcpClient);
        bool isConnected = true;

        try
        {
            while (isConnected)
            {
                byte[] codeBytes = await networkDataHelper.ReceiveAsync(4);
                int actionCode = BitConverter.ToInt32(codeBytes, 0);

                if (actionCode == (int)Actions.Register)
                {
                    await UserLogic.HandleRegister(networkDataHelper);
                }
                else if (actionCode == (int)Actions.Login)
                {
                    currentUser = await UserLogic.HandleLogin(networkDataHelper);
                }
                else if (actionCode == (int)Actions.CreateGame)
                {
                    currentGame = await GameLogic.HandleGameCreation(networkDataHelper, currentUser);
                }
                else if(actionCode == (int)Actions.CreateGameCover)
                {
                    await GameLogic.HandleGameCoverCreation(networkDataHelper, currentGame, currentUser, tcpClient);
                }
                else if(actionCode == (int)Actions.ListByName)
                {
                    await GameLogic.HandleListGamesByName(networkDataHelper);
                }
                else if(actionCode == (int)Actions.AcquireGame)
                {
                    await GameLogic.HandleAcquireGame(networkDataHelper, currentUser);
                }
                else if(actionCode == (int)Actions.ModifyGame)
                {
                    currentGame = await GameLogic.HandleModifyGame(networkDataHelper, currentUser);
                }
                else if(actionCode == (int)Actions.ModifyCover)
                {
                    await GameLogic.HandleModifyCover(networkDataHelper, currentGame, currentUser, tcpClient);
                }
                else if(actionCode == (int)Actions.DeleteGame)
                {
                    await GameLogic.HandleDeleteGame(networkDataHelper, currentUser, tcpClient);
                }
                else if(actionCode == (int)Actions.ListAllGames)
                {
                    await GameLogic.HandleListAllGames(networkDataHelper);
                }
                else if(actionCode == (int)Actions.ListByGenre)
                {
                    await GameLogic.HandleListGamesByGenre(networkDataHelper);
                }
                else if(actionCode == (int)Actions.ListByPlatform)
                {
                    await GameLogic.HandleListGamesByPlatform(networkDataHelper);
                }
                else if(actionCode == (int)Actions.ShowGame)
                {
                    await GameLogic.HandleShowGame(networkDataHelper);
                }
                else if(actionCode == (int)Actions.AskForCover)
                {
                    await GameLogic.HandleAskForCover(networkDataHelper);
                }
                else if(actionCode == (int)Actions.DownloadCover)
                {
                    await GameLogic.HandleDownloadCover(networkDataHelper, tcpClient);
                }
                else if(actionCode == (int)Actions.GetAllReviews)
                {
                    await ReviewLogic.HandleGetAllReviews(networkDataHelper);
                }
                else if(actionCode == (int)Actions.QualifyGame)
                {
                    await ReviewLogic.HandleQualifyGame(networkDataHelper, currentUser);
                }
                else if(actionCode == (int)Actions.CloseConnection)
                {
                    TcpListenerCollection.Instance.RemoveTcpClient(tcpClient);
                    throw new SocketException();
                }
                else
                {
                    await SendResponse(networkDataHelper, "Acción desconocida.");
                }
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Cliente desconectado.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error manejando al cliente: {ex.Message}");
        }
        finally
        {
            tcpClient.Close();
        }
    }

    public static async Task SendResponse(NetworkDataHelper networkDataHelper, string response)
    {
        byte[] responseData = Encoding.UTF8.GetBytes(response);
        byte[] responseLength = BitConverter.GetBytes(responseData.Length);

        await networkDataHelper.SendAsync(responseLength);
        await networkDataHelper.SendAsync(responseData);
    }
    
}