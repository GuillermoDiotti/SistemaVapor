using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Communication;
using Servidor.Exceptions;
using Servidor.Logics.GameLogic;
using Servidor.Logics.ReviewLogic;
using Servidor.Logics.UserLogic;

namespace Servidor.Collections;

public class GameCollection
{
    private static GameCollection _instance;
    private static readonly object _lock = new object();
    private List<Game> games = new List<Game>();

    private GameCollection() { }

    public static GameCollection Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GameCollection();
                }
                return _instance;
            }
        }
    }

    public List<Game> Games
    {
        get
        {
            lock (_lock)
            {
                return new List<Game>(games);
            }
        }
    }
    
    public void AddGame(Game game)
    {
        lock (_lock)
        {
            Game duplicatedGame = games.Find(u => u.Title.Equals(game.Title, StringComparison.OrdinalIgnoreCase))!;

            if (duplicatedGame != null)
            {
                throw new ServerException("0#El juego ingresado ya existe.");
            }
            
            games.Add(game);
        }
    }

    public void AcquireGame(string title, User user)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(title, StringComparison.OrdinalIgnoreCase))!;
            
            if (game == null) throw new ServerException("0#Juego no encontrado.");
            
            if (game.Buyers.Contains(user)) throw new ServerException("0#El usuario ya posee el juego.");
            
            if (game.Units <= 0) throw new ServerException("0#No hay unidades disponibles.");
            
            game.Units--;
            game.Buyers.Add(user);
        }
    }
    
    public Game FindGame(string title)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(title, StringComparison.OrdinalIgnoreCase))!;

            if (game == null)
            {
                throw new ServerException("0#Juego no encontrado.");
            }
            
            return game;
        }
    }
    
    public bool DeleteGame(string title, User user, FileCommsHandler fileHandler)
    {
        lock (_lock)
        {
            Game gameToDelete = games.Find(u => u.Title.Equals(title, StringComparison.OrdinalIgnoreCase))!;
            
            if (gameToDelete == null)
            {
                throw new ServerException("0#Juego no encontrado.");
            }
            
            if (gameToDelete.Creator.Name != user.Name)
            {
                throw new ServerException("0#No tienes permisos para eliminar este juego.");
            }

            if (gameToDelete.CoverImg != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), gameToDelete.CoverImg);
                fileHandler._fileHandler.DeleteFile(path);
            }

            return games.Remove(gameToDelete);
        }
    }
    
    public Game ModifyGame(Game game, User user, string newTitle)
    {
        lock (_lock)
        {
            Game oldGame = games.Find(u => u.Title.Equals(game.Title, StringComparison.OrdinalIgnoreCase))!;
            
            if (oldGame == null) throw new ServerException("0#Juego no encontrado.");

            if (game.Title != newTitle)
            {
                Game newTitleExists = games.Find(u => u.Title == newTitle);
                if (newTitleExists != null) throw new ServerException("0#Ya existe un juego con ese nombre.");
            }

            if (oldGame.Creator.Name != user.Name)
            {
                throw new ServerException("0#No tienes permisos para modificar este juego.");
            }
            
            oldGame.Title = newTitle;
            oldGame.Release = game.Release;
            oldGame.Units = game.Units;
            oldGame.Platform = game.Platform;
            oldGame.Genre = game.Genre;
            
            return oldGame;
        }
    }

    public void AssignCover(string gameTitle, User user, string cover)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(gameTitle, StringComparison.OrdinalIgnoreCase))!;
            
            if (game == null) throw new ServerException("0#Juego no encontrado.");
            
            if (game.Creator.Name != user.Name)
            {
                throw new ServerException("0#No tienes permisos para modificar este juego.");
            }

            game.CoverImg = cover;
        }
    }
    
    public void ModifyCover(string gameTitle, User user, string cover, FileCommsHandler fileHandler)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(gameTitle, StringComparison.OrdinalIgnoreCase))!;
            
            if (game == null) throw new ServerException("0#Juego no encontrado.");
            
            if (game.Creator.Name != user.Name)
            {
                throw new ServerException("0#No tienes permisos para modificar este juego.");
            }
            
            game.CoverImg = cover;
        }
    }

    public void AddReview(string title, User user, Review review)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(title, StringComparison.OrdinalIgnoreCase))!;
            
            if (game == null) throw new ServerException("0#Juego no encontrado.");
            
            if (!game.Buyers.Contains(user)) throw new ServerException("0#No puedes calificar un juego que no has adquirido.");
            
            if (game.Reviews.Exists(c => c.User == user))  throw new ServerException("0#El usuario ya cuenta con una reseña para este juego.");
            
            review.Game = game;
            game.Reviews.Add(review);
        }
    }

    public List<Review> GetReviews(string title)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(title, StringComparison.OrdinalIgnoreCase))!;
            
            if (game == null) throw new ServerException("0#Juego no encontrado.");
            
            return game.Reviews;
        }
    }
    
    public bool ExistCover(string title)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(title, StringComparison.OrdinalIgnoreCase))!;
            
            if (game == null)
            {
                throw new ServerException("0#Juego no encontrado.");
            }

            return game.CoverImg != null;
        }
    }
    
    public string GetCoverPath(string title)
    {
        lock (_lock)
        {
            Game game = games.Find(u => u.Title.Equals(title, StringComparison.OrdinalIgnoreCase))!;
            
            if (game == null)
            {
                throw new ServerException("0#Juego no encontrado.");
            }

            return Path.Combine(Directory.GetCurrentDirectory(), game.CoverImg);
        }
    }

    public long[] GetFilesInfo(FileHandler fileHandler)
    {
        lock (_lock)
        {
            long[] infoFiles = new long[3] {0, 0, 0};

            Parallel.ForEach(Games, game =>
            {
                if (game.CoverImg != null && game.Units > 0)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), game.CoverImg);
                    var fileSize = fileHandler.GetFileSize(path);
                    Interlocked.Add(ref infoFiles[0], fileSize.Result);
                    Console.WriteLine(infoFiles[0]);

                    Interlocked.Increment(ref infoFiles[2]);
                    
                }
            });
            
            if(infoFiles[2] > 0){
                infoFiles[1] = infoFiles[0] / infoFiles[2];
            }   
            
            return infoFiles;
        }
    }
}