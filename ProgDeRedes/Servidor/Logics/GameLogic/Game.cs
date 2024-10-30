using Servidor.Logics.ReviewLogic;
using Servidor.Logics.UserLogic;

namespace Servidor.Logics.GameLogic;

public record Game
{
    public string Title { get; set; }
    public GameGenre Genre { get; set; }
    public string Release { get; set; }
    public GamePlatform Platform { get; set; }
    public string Publisher { get; set; }
    public User Creator { get; set; }
    public int Units { get; set; }
    public List<User> Buyers { get; set; }
    public List<Review> Reviews { get; set; }
    
    
    public string? CoverImg { get; set; }

    public Game()
    {
        Buyers = new List<User>();
        Reviews = new List<Review>();
    }
    
    public override string ToString()
    {
        return $"Titulo: {Title} " +
               $"Genero: {Genre} " +
               $"Año de Lanzamiento: {Release} " +
               $"Plataforma: {Platform} " +
               $"Publicador: {Publisher} " +
               $"Unidades: {Units} ";
    }
}

