using System.Net.Sockets;
using Communication;

namespace Cliente.Menu;

static class SessionManager
{
    public static async Task<bool> ShowSession(NetworkDataHelper networkDataHelper)
    {
        Console.WriteLine("\n--- SISTEMA VAPOR ---");
        
        Console.WriteLine("\nSeleccione una acción:");
        Console.WriteLine("1- Registrar");
        Console.WriteLine("2- Iniciar Sesión");
        Console.WriteLine("3- Salir");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                return await Register(networkDataHelper);
            case "2":
                return await Login(networkDataHelper);
            case "3":
                Program._exit = true;
                return false;
            default:
                Console.WriteLine("Opción no válida. Intente nuevamente.");
                return await ShowSession(networkDataHelper);
        }
    }

    public static async Task<bool> Register(NetworkDataHelper networkDataHelper)
    {
        Console.Clear();
        Console.WriteLine("\n--- Registro ---");

        Console.Write("Nombre de usuario: ");
        string username = Utilities.ReadNonEmptyInput();

        Console.Write("Contraseña: ");
        string password = Utilities.ReadNonEmptyInput();
        
        string message = $"{username}#{password}";
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.Register);
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
        return false;
    }
    
    static async Task<bool> Login(NetworkDataHelper networkDataHelper)
    {
        Console.Clear();
        Console.WriteLine("\n--- Iniciar Sesión ---");

        Console.Write("Nombre de usuario: ");
        string username = Utilities.ReadNonEmptyInput();

        Console.Write("Contraseña: ");
        string password = Utilities.ReadNonEmptyInput();

        string message = $"{username}#{password}";
        
        try
        {
            byte[] actionCode = BitConverter.GetBytes((int)Actions.Login);
            await networkDataHelper.SendAsync(actionCode);

            await Program.SendData(networkDataHelper, message);

            string response = await Program.ReceiveResponse(networkDataHelper);
            string code = response.Split('#')[0];
            Console.WriteLine($"Servidor: ");
            Console.WriteLine();
            Program.ShowResponse(response);
            return code.Equals("1");
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            Program._exit = true;
            return false;
        }
    }
}