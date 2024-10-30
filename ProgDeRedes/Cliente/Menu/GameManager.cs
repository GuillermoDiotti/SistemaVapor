using System.Net.Sockets;
using Communication;

namespace Cliente.Menu;

static class GameManager
{
    public static void CreateGame(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Publicar Juego ---");

        Console.Write("Nombre del juego: ");
        string title = Utilities.ReadNonEmptyInput();

        Console.WriteLine("Género del juego: ");
        string genre = Utilities.ShowGameCategories().ToString();
        
        Console.Write("Año de lanzamiento del juego: ");
        int date = Utilities.CheckIntegerValue(1952, DateTime.Today.Year + 2);
        
        Console.Write("Publicador del juego: ");
        string publisher = Utilities.ReadNonEmptyInput();
        
        Console.WriteLine("Plataforma del juego: ");
        string platform = Utilities.ShowGamePlatforms().ToString();
        
        Console.Write("Unidades del juego: ");
        int units = Utilities.CheckIntegerValue(1, int.MaxValue);

        string message = $"{title}#{genre}#{date}#{publisher}#{platform}#{units}";;
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.CreateGame);
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, message);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
            if(response.Split("#")[0] == "1") CreateGameCover(networkDataHelper, ref exit, title);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
        
    }
    
    static void CreateGameCover(NetworkDataHelper networkDataHelper, ref bool exit, string title)
    {
        Console.WriteLine("¿Desea subir una portada para dicho juego? (S/N)");
        string res = Utilities.ReadNonEmptyInput();
        
        if (res.ToUpper() == "S")
        {
            Console.Write("Ruta completa de la imagen: ");
            string path = Utilities.ReadNonEmptyInput();

            try
            {
                var fileCommonHandler = new FileCommsHandler(Program.socketClient);

                if (fileCommonHandler._fileHandler.FileExists(path))
                {
                    byte[] actionCode = BitConverter.GetBytes((int)Actions.CreateGameCover);
                    networkDataHelper.Send(actionCode);

                    fileCommonHandler.SendFile(path);

                    string response = Program.ReceiveResponse(networkDataHelper);
                    Console.WriteLine($"Servidor:");
                    Console.WriteLine();
                    Program.ShowResponse(response);
                }
                else
                {
                    Program.ShowResponse("0#Imagen no encontrada.");
                    CreateGameCover(networkDataHelper, ref exit, title);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("La conexión ha sido interrumpida.");
                exit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrio un problema en el envio de la imagen.");
            }
        }
    }
    
    public static void AcquireGame(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Adquirir Juego ---");

        ListGamesByName(networkDataHelper, ref exit);
        
        Console.Write("Nombre del juego a adquirir: ");
        string name = Utilities.ReadNonEmptyInput();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.AcquireGame);
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, name);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    public static void ModifyGame(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Modificar Juego ---");

        Console.Write("Nombre del juego: ");
        string title = Utilities.ReadNonEmptyInput();
        
        Console.Write("Nuevo nombre del juego: ");
        string newTitle = Utilities.ReadNonEmptyInput();

        Console.WriteLine("Nuevo género del juego: ");
        string genre = Utilities.ShowGameCategories().ToString();
        
        Console.Write("Nuevo año de lanzamiento del juego: ");
        int date = Utilities.CheckIntegerValue(1952, DateTime.Today.Year + 2);
        
        Console.Write("Nuevo publicador del juego: ");
        string publisher = Utilities.ReadNonEmptyInput();
        
        Console.WriteLine("Nueva plataforma del juego: ");
        string platform = Utilities.ShowGamePlatforms().ToString();
        
        Console.Write("Nueva cantidad de Unidades del juego: ");
        int units = Utilities.CheckIntegerValue(1, int.MaxValue);

        string message = $"{title}#{newTitle}#{genre}#{date}#{publisher}#{platform}#{units}";
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ModifyGame);
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, message);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
            
            if(response.Split("#")[0] == "1") ModifyGameCover(networkDataHelper, ref exit, title);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    static void ModifyGameCover(NetworkDataHelper networkDataHelper, ref bool exit, string title)
    {
        Console.WriteLine("¿Desea modificar la portada para dicho juego? (S/N)");
        string res = Utilities.ReadNonEmptyInput();
        
        if (res.ToUpper() == "S")
        {
            Console.Write("Ruta completa de la imagen: ");
            string path = Utilities.ReadNonEmptyInput();

            try
            {
                var fileCommonHandler = new FileCommsHandler(Program.socketClient);

                if (fileCommonHandler._fileHandler.FileExists(path))
                {
                    byte[] actionCode = BitConverter.GetBytes((int)Actions.ModifyCover);
                    networkDataHelper.Send(actionCode);

                    fileCommonHandler.SendFile(path);

                    string response = Program.ReceiveResponse(networkDataHelper);
                    Console.WriteLine($"Servidor:");
                    Console.WriteLine();
                    Program.ShowResponse(response);
                }
                else
                {
                    Program.ShowResponse("0#Imagen no encontrada.");
                    ModifyGameCover(networkDataHelper, ref exit, title);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("La conexión ha sido interrumpida.");
                exit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrio un problema en el envio de la imagen.");
            }
        }
    }
    
    public static void DeleteGame(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Eliminar Juego ---");

        ListGamesByName(networkDataHelper, ref exit);
        
        Console.Write("Nombre del juego a eliminar: ");
        string name = Utilities.ReadNonEmptyInput();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.DeleteGame);
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, name);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    public static void SearchGames(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Listar Juegos ---");
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListAllGames);
            networkDataHelper.Send(actionCode);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Console.WriteLine(response);
            
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
        
        Console.WriteLine();
        Console.WriteLine("1- Filtrar juegos por género");
        Console.WriteLine("2- Filtrar juegos por plataforma");
        Console.WriteLine("3- Volver");
        
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                FilterByGenre(networkDataHelper, ref exit);
                break;
            case "2":
                FilterByPlatform(networkDataHelper, ref exit);
                break;
            case "3":
                break;
            default:
                Console.WriteLine("Opción no válida");
                SearchGames(networkDataHelper, ref exit);
                break;
        }
    }
    
    public static void SpecificGameSearch(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Listar Información de un juego ---");

        ListGamesByName(networkDataHelper, ref exit);
        
        Console.Write("Nombre del juego a buscar: ");
        string title = Utilities.ReadNonEmptyInput();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ShowGame);
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, title);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            if (response.Split("#")[0] == "0")
            {
                Program.ShowResponse(response);
            }
            else
            {
                Console.WriteLine(response);
                Console.WriteLine();
                Console.WriteLine("1- Descargar imagen");
                Console.WriteLine("2- Ver reseñas");
                Console.WriteLine("3- Volver");
        
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DownloadCover(networkDataHelper, ref exit, title);
                        break;
                    case "2":
                        ViewReviews(networkDataHelper, ref exit, title);
                        break;
                    case "3":
                        break;
                    default:
                        Console.WriteLine("Opción no válida");
                        SearchGames(networkDataHelper, ref exit);
                        break;
                }
            }
            
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }

    public static void DownloadCover(NetworkDataHelper networkDataHelper, ref bool exit,string title)
    {
        try
        {
            Console.WriteLine("\n--- Descargando imagen ---");
            
            
            byte[] actionCodeAsk = BitConverter.GetBytes((int)Actions.AskForCover);
            networkDataHelper.Send(actionCodeAsk);
            
            Program.SendData(networkDataHelper, title);
            
            string response = Program.ReceiveResponse(networkDataHelper);

            if (response == "True")
            {
                byte[] actionCode = BitConverter.GetBytes((int)Actions.DownloadCover);
                networkDataHelper.Send(actionCode);
                
                Program.SendData(networkDataHelper, title);
                
                var fileCommonHandler = new FileCommsHandler(Program.socketClient);
                fileCommonHandler.ReceiveFile();
                Program.ShowResponse("1#Imagen descargada");
            }
            else
            {
                if (response == "False"){
                    Console.WriteLine();
                    Program.ShowResponse("0#El juego no cuenta con imagen");
                }
                else
                {
                    Console.WriteLine();
                    Program.ShowResponse(response);
                    
                }
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    public static void ViewReviews(NetworkDataHelper networkDataHelper, ref bool exit, string name)
    {
        try
        {
            Console.WriteLine("\n--- Mostrando reseñas ---");
            byte[] actionCode = BitConverter.GetBytes((int)Actions.GetAllReviews);
            networkDataHelper.Send(actionCode);
            
            Program.SendData(networkDataHelper, name);
            
            
            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            if (response.Split("#")[0] == "0")
            {
                Program.ShowResponse(response);
            }
            else
            {
                Console.WriteLine(response);
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    public static void AddGameReview(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Calificar Juego ---");

        ListGamesByName(networkDataHelper, ref exit);
        
        Console.Write("Nombre del juego a calificar: ");
        string name = Utilities.ReadNonEmptyInput();
        
        Console.Write("Calificación del juego: ");
        int score = Utilities.CheckIntegerValue(1, 10);
        
        Console.Write("Reseña del juego: ");
        string review = Utilities.ReadNonEmptyInput();
        
        string message = $"{name}#{score}#{review}";
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.QualifyGame);
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, message);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    public static void FilterByGenre(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Listar Juegos por género ---");

        int genre = Utilities.ShowGameCategories();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListByGenre);
            networkDataHelper.Send(actionCode);
            
            Program.SendData(networkDataHelper, genre.ToString());

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Console.WriteLine(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    public static void FilterByPlatform(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Listar Juegos por plataforma ---");
        
        int platform = Utilities.ShowGamePlatforms();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListByPlatform);
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, platform.ToString());
            
            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Console.WriteLine(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
    
    public static void ListGamesByName(NetworkDataHelper networkDataHelper, ref bool exit)
    {
        Console.WriteLine("\n--- Listar Juegos ---");
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListByName);
            networkDataHelper.Send(actionCode);

            string response = Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Console.WriteLine(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
        }
    }
}