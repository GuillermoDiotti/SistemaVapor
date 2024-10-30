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
    private static readonly object _socketServerLock = new object();
    
    static readonly SettingsManager settingsManager = new SettingsManager();
    static bool salir = false;

    static void Main(string[] args)
    {
        Console.WriteLine("Inicio el servidor...");
        
        var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        string serverip = settingsManager.ReadSettings(ServerConfig.ServerIpConfigkey);
        int serverport = int.Parse(settingsManager.ReadSettings(ServerConfig.ServerPortConfigKey));

        var localEndpoint = new IPEndPoint(IPAddress.Parse(serverip), serverport);

        try
        {
            socketServer.Bind(localEndpoint);
            socketServer.Listen(10); 
            new Thread(() => HandleServer(socketServer)).Start();
            Console.WriteLine($"Servidor escuchando en {serverip}:{serverport}");
            Console.WriteLine("Esperando clientes...");
                
            while (!salir) 
            {
                try
                {
                    var socketClient = socketServer.Accept();
                    SocketCollection.Instance.AddSocket(socketClient);
                    Console.WriteLine("Nuevo cliente conectado");
                    new Thread(() => HandleClient(socketClient)).Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Conexion cerrada");
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error del servidor: {ex.Message}");
        }
    }

    private static void HandleServer(Socket socket)
    {
        while(!salir){
            Console.WriteLine("Escribe 'exit' para cerrar el servidor.");
            var exit = Console.ReadLine();
            if (exit.Equals("exit"))
            {
                lock (_socketServerLock)
                {
                    foreach (var sock in SocketCollection.Instance.Sockets)
                    {
                        sock.Close();
                    }
                    salir = true;
    
                }
                socket.Close();  
            }
        }
    }

    private static void HandleClient(Socket clientSocket)
    {
        User? currentUser = null;
        Game? currentGame = null;
        
        NetworkDataHelper networkDataHelper = new NetworkDataHelper(clientSocket);
        bool isConnected = true;

        try
        {
            while (isConnected)
            {
                byte[] codeBytes = networkDataHelper.Receive(4);
                int actionCode = BitConverter.ToInt32(codeBytes, 0);

                if (actionCode == (int)Actions.Register)
                {
                    UserLogic.HandleRegister(networkDataHelper);
                }
                else if (actionCode == (int)Actions.Login)
                {
                    currentUser = UserLogic.HandleLogin(networkDataHelper);
                }
                else if (actionCode == (int)Actions.CreateGame)
                {
                    currentGame = GameLogic.HandleGameCreation(networkDataHelper, currentUser);
                }
                else if(actionCode == (int)Actions.CreateGameCover)
                {
                    GameLogic.HandleGameCoverCreation(networkDataHelper, ref currentGame, currentUser, ref clientSocket);
                }
                else if(actionCode == (int)Actions.ListByName)
                {
                    GameLogic.HandleListGamesByName(networkDataHelper);
                }
                else if(actionCode == (int)Actions.AcquireGame)
                {
                    GameLogic.HandleAcquireGame(networkDataHelper, ref currentUser);
                }
                else if(actionCode == (int)Actions.ModifyGame)
                {
                    currentGame = GameLogic.HandleModifyGame(networkDataHelper, ref currentUser);
                }
                else if(actionCode == (int)Actions.ModifyCover)
                {
                    GameLogic.HandleModifyCover(networkDataHelper, ref currentGame, currentUser, ref clientSocket);
                }
                else if(actionCode == (int)Actions.DeleteGame)
                {
                    GameLogic.HandleDeleteGame(networkDataHelper, ref currentUser, ref clientSocket);
                }
                else if(actionCode == (int)Actions.ListAllGames)
                {
                    GameLogic.HandleListAllGames(networkDataHelper);
                }
                else if(actionCode == (int)Actions.ListByGenre)
                {
                    GameLogic.HandleListGamesByGenre(networkDataHelper);
                }
                else if(actionCode == (int)Actions.ListByPlatform)
                {
                    GameLogic.HandleListGamesByPlatform(networkDataHelper);
                }
                else if(actionCode == (int)Actions.ShowGame)
                {
                    GameLogic.HandleShowGame(networkDataHelper);
                }
                else if(actionCode == (int)Actions.AskForCover)
                {
                    GameLogic.HandleAskForCover(networkDataHelper);
                }
                else if(actionCode == (int)Actions.DownloadCover)
                {
                    GameLogic.HandleDownloadCover(networkDataHelper, ref clientSocket);
                }
                else if(actionCode == (int)Actions.GetAllReviews)
                {
                    ReviewLogic.HandleGetAllReviews(networkDataHelper);
                }
                else if(actionCode == (int)Actions.QualifyGame)
                {
                    ReviewLogic.HandleQualifyGame(networkDataHelper, ref currentUser);
                }
                else
                {
                    SendResponse(networkDataHelper, "Acción desconocida.");
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
            clientSocket.Close();
        }
    }

    public static void SendResponse(NetworkDataHelper networkDataHelper, string response)
    {
        byte[] responseData = Encoding.UTF8.GetBytes(response);
        byte[] responseLength = BitConverter.GetBytes(responseData.Length);

        networkDataHelper.Send(responseLength);
        networkDataHelper.Send(responseData);
    }
    
}