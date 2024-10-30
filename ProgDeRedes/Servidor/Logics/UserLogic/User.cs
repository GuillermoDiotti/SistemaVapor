using Servidor.Logics.GameLogic;

namespace Servidor.Logics.UserLogic;

public record class User
{
    public string Name { get; set; }
    public string Password { get; set; }

    public User()
    {
    }
}