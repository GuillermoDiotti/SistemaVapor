using System.Text;
using Communication;
using Servidor.Collections;

namespace Servidor.Logics.UserLogic;

static class UserLogic
{
    public static void HandleRegister(NetworkDataHelper networkDataHelper)
    {
        byte[] lengthBytes = networkDataHelper.Receive(4);
        int dataLength = BitConverter.ToInt32(lengthBytes, 0);

        byte[] data = networkDataHelper.Receive(dataLength);
        string message = Encoding.UTF8.GetString(data);
        string[] parts = message.Split('#');

        string username = parts[0];
        string password = parts[1];

        if (UserCollection.Instance.FindUser(username) != null)
        {
            Program.SendResponse(networkDataHelper, "0#El nombre de usuario ya existe.");
        }
        else
        {
            User newUser = new User
            {
                Name = username,
                Password = password
            };
                
            UserCollection.Instance.AddUser(newUser);
            Program.SendResponse(networkDataHelper, "1#Registro exitoso.");
            Console.WriteLine($"Usuario registrado: {username}");
        }
        
    }
    
    public static User HandleLogin(NetworkDataHelper networkDataHelper)
    {
        byte[] lengthBytes = networkDataHelper.Receive(4);
        int dataLength = BitConverter.ToInt32(lengthBytes, 0);

        byte[] data = networkDataHelper.Receive(dataLength);
        string message = Encoding.UTF8.GetString(data);
        string[] parts = message.Split('#');

        string username = parts[0];
        string password = parts[1];

        bool isAuthenticated = UserCollection.Instance.ExistUser(username, password);

        if (isAuthenticated)
        {
            Program.SendResponse(networkDataHelper, "1#Inicio de sesion exitoso.");
            Console.WriteLine($"Usuario autenticado: {username}");
                
            return UserCollection.Instance.FindUser(username);
        }
        else
        {
            Program.SendResponse(networkDataHelper, "0#Credenciales incorrectas.");
            return null;
        }
        
    }
}