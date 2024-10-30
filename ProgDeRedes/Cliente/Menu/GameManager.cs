using System.Net.Sockets;
using Communication;

namespace Cliente.Menu;

static class GameManager
{
    public static async Task CreateGame (NetworkDataHelper networkDataHelper)
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
        int units = Utilities.CheckIntegerValue(0, int.MaxValue);

        string message = $"{title}#{genre}#{date}#{publisher}#{platform}#{units}";
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.CreateGame);
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, message);

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
            if(response.Split("#")[0] == "1") await CreateGameCover(networkDataHelper, title);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
        
    }
    
    static async Task CreateGameCover(NetworkDataHelper networkDataHelper, string title)
    {
        Console.WriteLine("¿Desea subir una portada para dicho juego? (S/N)");
        string res = Utilities.ReadNonEmptyInput();
        
        if (res.ToUpper() == "S")
        {
            Console.Write("Ruta completa de la imagen: ");
            string path = Utilities.ReadNonEmptyInput();

            try
            {
                var fileCommonHandler = new FileCommsHandler(Program.tcpClient);

                if (await fileCommonHandler._fileHandler.FileExists(path))
                {
                    byte[] actionCode = BitConverter.GetBytes((int)Actions.CreateGameCover);
                    await networkDataHelper.SendAsync(actionCode);

                    await fileCommonHandler.SendFile(path);

                    string response = await Program.ReceiveResponse(networkDataHelper);
                    Console.WriteLine($"Servidor:");
                    Console.WriteLine();
                    Program.ShowResponse(response);
                }
                else
                {
                    Program.ShowResponse("0#Imagen no encontrada.");
                    await CreateGameCover(networkDataHelper, title);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("La conexión ha sido interrumpida.");
                Program._exit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrio un problema en el envio de la imagen.");
            }
        }
    }
    
    public static async Task AcquireGame(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Adquirir Juego ---");

        await ListGamesByName(networkDataHelper);
        
        Console.Write("Nombre del juego a adquirir: ");
        string name = Utilities.ReadNonEmptyInput();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.AcquireGame);
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, name);

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }
    
    public static async Task ModifyGame(NetworkDataHelper networkDataHelper)
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
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, message);

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
            
            if(response.Split("#")[0] == "1") await ModifyGameCover(networkDataHelper, title);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }
    
    static async Task ModifyGameCover(NetworkDataHelper networkDataHelper, string title)
    {
        Console.WriteLine("¿Desea modificar la portada para dicho juego? (S/N)");
        string res = Utilities.ReadNonEmptyInput();
        
        if (res.ToUpper() == "S")
        {
            Console.Write("Ruta completa de la imagen: ");
            string path = Utilities.ReadNonEmptyInput();

            try
            {
                var fileCommonHandler = new FileCommsHandler(Program.tcpClient);

                if (await fileCommonHandler._fileHandler.FileExists(path))
                {
                    byte[] actionCode = BitConverter.GetBytes((int)Actions.ModifyCover);
                    await networkDataHelper.SendAsync(actionCode);

                    await fileCommonHandler.SendFile(path);

                    string response = await Program.ReceiveResponse(networkDataHelper);
                    Console.WriteLine($"Servidor:");
                    Console.WriteLine();
                    Program.ShowResponse(response);
                }
                else
                {
                    Program.ShowResponse("0#Imagen no encontrada.");
                    await ModifyGameCover(networkDataHelper, title);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("La conexión ha sido interrumpida.");
                Program._exit = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrio un problema en el envio de la imagen.");
            }
        }
    }
    
    public static async Task DeleteGame(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Eliminar Juego ---");

        await ListGamesByName(networkDataHelper);
        
        Console.Write("Nombre del juego a eliminar: ");
        string name = Utilities.ReadNonEmptyInput();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.DeleteGame);
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, name);

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }
    
    public static async Task SearchGames(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Listar Juegos ---");
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListAllGames);
            await networkDataHelper.SendAsync(actionCode);

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Console.WriteLine(response);
            
        }
        catch
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
        
        Console.WriteLine();
        Console.WriteLine("1- Filtrar juegos por género");
        Console.WriteLine("2- Filtrar juegos por plataforma");
        Console.WriteLine("3- Volver");
        
        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await FilterByGenre(networkDataHelper);
                break;
            case "2":
                await FilterByPlatform(networkDataHelper);
                break;
            case "3":
                break;
            default:
                Console.WriteLine("Opción no válida");
                await SearchGames(networkDataHelper);
                break;
        }
    }
    
    public static async Task SpecificGameSearch(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Listar Información de un juego ---");

        await ListGamesByName(networkDataHelper);
        
        Console.Write("Nombre del juego a buscar: ");
        string title = Utilities.ReadNonEmptyInput();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ShowGame);
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, title);

            string response = await Program.ReceiveResponse(networkDataHelper);
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
                        await DownloadCover(networkDataHelper, title);
                        break;
                    case "2":
                        await ViewReviews(networkDataHelper, title);
                        break;
                    case "3":
                        break;
                    default:
                        Console.WriteLine("Opción no válida");
                        await SearchGames(networkDataHelper);
                        break;
                }
            }
            
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }

    public static async Task DownloadCover(NetworkDataHelper networkDataHelper, string title)
    {
        try
        {
            Console.WriteLine("\n--- Descargando imagen ---");
            
            
            byte[] actionCodeAsk = BitConverter.GetBytes((int)Actions.AskForCover);
            await networkDataHelper.SendAsync(actionCodeAsk);
            
            await Program.SendData(networkDataHelper, title);
            
            string response = await Program.ReceiveResponse(networkDataHelper);

            if (response == "True")
            {
                byte[] actionCode = BitConverter.GetBytes((int)Actions.DownloadCover);
                await networkDataHelper.SendAsync(actionCode);
                
                await Program.SendData(networkDataHelper, title);
                
                var fileCommonHandler = new FileCommsHandler(Program.tcpClient);
                await fileCommonHandler.ReceiveFile();
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
            Program._exit = true;
        }
    }
    
    public static async Task ViewReviews(NetworkDataHelper networkDataHelper, string name)
    {
        try
        {
            Console.WriteLine("\n--- Mostrando reseñas ---");
            byte[] actionCode = BitConverter.GetBytes((int)Actions.GetAllReviews);
            await networkDataHelper.SendAsync(actionCode);
            
            await Program.SendData(networkDataHelper, name);
            
            
            string response = await Program.ReceiveResponse(networkDataHelper);
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
            Program._exit = true;
        }
    }
    
    public static async Task AddGameReview(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Calificar Juego ---");

        await ListGamesByName(networkDataHelper);
        
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
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, message);

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Program.ShowResponse(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }
    
    public static async Task FilterByGenre(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Listar Juegos por género ---");

        int genre = Utilities.ShowGameCategories();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListByGenre);
            await networkDataHelper.SendAsync(actionCode);
            
            await Program.SendData(networkDataHelper, genre.ToString());

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Console.WriteLine(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }
    
    public static async Task FilterByPlatform(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Listar Juegos por plataforma ---");
        
        int platform = Utilities.ShowGamePlatforms();
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListByPlatform);
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, platform.ToString());
            
            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            Console.WriteLine(response);
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }
    
    public static async Task ListGamesByName(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- Listar Juegos ---");
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.ListByName);
            await networkDataHelper.SendAsync(actionCode);

            string response = await Program.ReceiveResponse(networkDataHelper);
            Console.WriteLine($"Servidor:");
            Console.WriteLine();
            
            if (response == "Actualmente no hay juegos publicados.\r\n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{response}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"{response}");
            }

        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
        }
    }
}