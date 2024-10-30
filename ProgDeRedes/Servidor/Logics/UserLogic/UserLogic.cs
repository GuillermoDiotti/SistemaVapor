using System.Text;
using Communication;
using Servidor.Collections;

namespace Servidor.Logics.UserLogic;

static class UserLogic
{
    public static async Task HandleRegister(NetworkDataHelper networkDataHelper)
    {
        byte[] lengthBytes = await networkDataHelper.ReceiveAsync(4);
        int dataLength = BitConverter.ToInt32(lengthBytes, 0);

        byte[] data = await networkDataHelper.ReceiveAsync(dataLength);
        string message = Encoding.UTF8.GetString(data);
        string[] parts = message.Split('#');

        string username = parts[0];
        string password = parts[1];

        if (UserCollection.Instance.FindUser(username) != null)
        {
            await Program.SendResponse(networkDataHelper, "0#El nombre de usuario ya existe.");
        }
        else
        {
            User newUser = new User
            {
                Name = username,
                Password = password
            };
                
            UserCollection.Instance.AddUser(newUser);
            await Program.SendResponse(networkDataHelper, "1#Registro exitoso.");
            Console.WriteLine($"Usuario registrado: {username}");
        }
        
    }
    
    public static async Task<User> HandleLogin(NetworkDataHelper networkDataHelper)
    {
        byte[] lengthBytes = await networkDataHelper.ReceiveAsync(4);
        int dataLength = BitConverter.ToInt32(lengthBytes, 0);

        byte[] data = await networkDataHelper.ReceiveAsync(dataLength);
        string message = Encoding.UTF8.GetString(data);
        string[] parts = message.Split('#');

        string username = parts[0];
        string password = parts[1];

        bool isAuthenticated = UserCollection.Instance.ExistUser(username, password);

        if (isAuthenticated)
        {
            await Program.SendResponse(networkDataHelper, "1#Inicio de sesion exitoso.");
            Console.WriteLine($"Usuario autenticado: {username}");
                
            return UserCollection.Instance.FindUser(username);
        }
        else
        {
            await Program.SendResponse(networkDataHelper, "0#Credenciales incorrectas.");
            return null;
        }
        
    }
}