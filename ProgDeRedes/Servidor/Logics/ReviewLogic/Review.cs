using Servidor.Logics.GameLogic;
using Servidor.Logics.UserLogic;

namespace Servidor.Logics.ReviewLogic;

public class Review
{
    public Game Game { get; set; }
    public User User { get; set; }
    public string Text { get; set; }
    public int Score { get; set; }
    
    public override string ToString()
    {
        return $"\nJuego: {Game.Title} Usuario: {User.Name} Puntuacion: {Score} Texto: {Text}";
    }
}