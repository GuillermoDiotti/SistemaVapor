using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Communication;
using Servidor.Collections;
using Servidor.Exceptions;
using Servidor.Logics.UserLogic;

namespace Servidor.Logics.GameLogic;

static class GameLogic
{
    public static Game HandleGameCreation(NetworkDataHelper networkDataHelper, User currentUser)
    {
        try
        {
            var lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);
            string[] parts = message.Split('#');

            string title = parts[0];
            string genre = parts[1];
            string release = parts[2];
            string publisher = parts[3];
            string platform = parts[4];
            string units = parts[5];

            Game newGame = new Game
            { 
                Title = title,
                Release = release,
                Publisher = publisher,
                Units = int.Parse(units),
                Genre = Utilities.AssignGenre(genre),
                Platform = Utilities.AssignPlatform(platform),
                Creator = currentUser,
            };
                    
            GameCollection.Instance.AddGame(newGame);
            Program.SendResponse(networkDataHelper, "1#Registro exitoso.");
            Console.WriteLine($"Juego {title}" + $" creado por: {currentUser.Name}");

            return newGame;
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
            return null;
        }
    }
    
    public static void HandleGameCoverCreation(NetworkDataHelper networkDataHelper, ref Game currentGame, User currentUser, ref Socket socketClient)
    {
        try
        {
            var fileCommonHandler = new FileCommsHandler(socketClient);
            string imageName = fileCommonHandler.ReceiveFile();
            GameCollection.Instance.AssignCover(currentGame.Title, currentUser, imageName);
            Program.SendResponse(networkDataHelper, "1#Caratula asignada exitosamente");
            Console.WriteLine(currentGame.CoverImg);
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }
        
    }
    
    public static void HandleListGamesByName(NetworkDataHelper networkDataHelper)
    {
        StringBuilder sb = new StringBuilder();
        List<Game> games = GameCollection.Instance.Games;
        int index = 0;
        
        if(games.Count > 0)
            foreach (var game in games)
            {
                index++;
                sb.AppendLine(index+ " - " + game.Title);
            }
        else
        {
            sb.AppendLine("Actualmente no hay juegos publicados.");
        }
        Program.SendResponse(networkDataHelper, sb.ToString());
    }
    
    public static void HandleAcquireGame(NetworkDataHelper networkDataHelper, ref User currentUser)
    {
        try
        {
            byte[] lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);
            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);


            GameCollection.Instance.AcquireGame(message, currentUser);
            Program.SendResponse(networkDataHelper, "1#Juego adquirido.");
            Console.WriteLine($"Juego {message} adquirido por: {currentUser.Name}");
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }
    }
    
    public static Game HandleModifyGame(NetworkDataHelper networkDataHelper, ref User currentUser)
    {
        try
        {
            byte[] lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);
            
            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);
            string[] parts = message.Split('#');
            
            string title = parts[0];
            string newTitle = parts[1];
            string genre = parts[2];
            string release = parts[3];
            string publisher = parts[4];
            string platform = parts[5];
            string units = parts[6];

            Game game = new Game()
            {
                Title = title,
                Release = release,
                Publisher = publisher,
                Units = int.Parse(units),
                Genre = Utilities.AssignGenre(genre),
                Platform = Utilities.AssignPlatform(platform),
            };
            
            Game modifiedGame = GameCollection.Instance.ModifyGame(game, currentUser, newTitle);
            
            Program.SendResponse(networkDataHelper, "1#Juego modificado.");
            Console.WriteLine("Juego modificado.");

            return modifiedGame;
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }

        return null;
    }

    public static void HandleModifyCover(NetworkDataHelper networkDataHelper, ref Game currentGame, User currentUser, ref Socket socketClient)
    {
        try
        {
            var fileCommonHandler = new FileCommsHandler(socketClient);
            string imageName = fileCommonHandler.ReceiveFile();
            GameCollection.Instance.ModifyCover(currentGame.Title, currentUser, imageName, fileCommonHandler);
            Program.SendResponse(networkDataHelper, "1#Caratula modificada exitosamente");
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }
        
    }
    
    public static void HandleDeleteGame(NetworkDataHelper networkDataHelper, ref User currentUser, ref Socket socketClient)
    {
        try
        {

            byte[] lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);
            
            GameCollection.Instance.DeleteGame(message, currentUser, new FileCommsHandler(socketClient) );
            Program.SendResponse(networkDataHelper, "1#Juego eliminado.");
            Console.WriteLine($"Juego eliminado.");
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }
    }
    
    public static void HandleListAllGames(NetworkDataHelper networkDataHelper)
    {
        StringBuilder sb = new StringBuilder();
        List<Game> games = GameCollection.Instance.Games;
        
        if(games.Count > 0)
            foreach (var game in games)
            {
                sb.AppendLine(game.ToString());
            }
        else
        {
            sb.AppendLine("Actualmente no hay juegos publicados.");
        }
        
        Program.SendResponse(networkDataHelper, sb.ToString());
    }
    
    public static void HandleListGamesByGenre(NetworkDataHelper networkDataHelper)
    {
        byte[] lengthBytes = networkDataHelper.Receive(4);
        int dataLength = BitConverter.ToInt32(lengthBytes, 0);

        byte[] data = networkDataHelper.Receive(dataLength);
        string message = Encoding.UTF8.GetString(data);

        GameGenre genre = Utilities.AssignGenre(message);
        
        List<Game> games = GameCollection.Instance.Games;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Género: {genre}\n");

        if (games.Count > 0)
        {
            int count = 0;
            foreach (var game in games)
            {
                if (game.Genre == genre)
                {
                    sb.AppendLine(game.ToString());
                    count++;
                }
            }
            
            if (count == 0)
            {
                sb.AppendLine("Actualmente no hay juegos publicados con ese genero.");
            }
        }else
        {
            sb.AppendLine("Actualmente no hay juegos publicados.");
        }

        Program.SendResponse(networkDataHelper, sb.ToString());
    }
    
    public static void HandleListGamesByPlatform(NetworkDataHelper networkDataHelper)
    {
        byte[] lengthBytes = networkDataHelper.Receive(4);
        int dataLength = BitConverter.ToInt32(lengthBytes, 0);

        byte[] data = networkDataHelper.Receive(dataLength);
        string message = Encoding.UTF8.GetString(data);

        GamePlatform platform = Utilities.AssignPlatform(message);
        
        List<Game> games = GameCollection.Instance.Games;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Plataforma: {platform}\n");
        
        if (games.Count > 0)
        {
            int count = 0;
            foreach (var game in GameCollection.Instance.Games)
            {
                if (game.Platform == platform)
                {
                    sb.AppendLine(game.ToString());
                    count++;
                }
            }
            
            if (count == 0)
            {
                sb.AppendLine("Actualmente no hay juegos publicados con ese genero.");
            }
        }else
        {
            sb.AppendLine("Actualmente no hay juegos publicados.");
        }
        
        Program.SendResponse(networkDataHelper, sb.ToString());
    }

    public static void HandleShowGame(NetworkDataHelper networkDataHelper)
    {
        try
        {
            byte[] lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);

            Game game = GameCollection.Instance.FindGame(message);

            Program.SendResponse(networkDataHelper, game.ToString());
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }
    }
    
    public static void HandleAskForCover(NetworkDataHelper networkDataHelper)
    {
        try
        {
            byte[] lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);
            
            Game game = GameCollection.Instance.FindGame(message);

            bool hasCover = game.CoverImg != null;

            Program.SendResponse(networkDataHelper, hasCover.ToString());
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }
    }
    
    public static void HandleDownloadCover(NetworkDataHelper networkDataHelper, ref Socket socketClient)
    {
        try
        {
            byte[] lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);

            string path = GameCollection.Instance.GetCoverPath(message);
            
            var fileCommonHandler = new FileCommsHandler(socketClient);
            
            fileCommonHandler.SendFile(path);
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }
    }
}