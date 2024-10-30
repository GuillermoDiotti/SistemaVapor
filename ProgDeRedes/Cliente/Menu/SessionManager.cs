using System.Net.Sockets;
using Communication;

namespace Cliente.Menu;

static class SessionManager
{
    public static bool ShowSession(NetworkDataHelper networkDataHelper, ref bool exit)
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
                return Register(networkDataHelper, ref exit);
            case "2":
                return Login(networkDataHelper, ref exit);
            case "3":
                exit = true;
                return false;
            default:
                Console.WriteLine("Opción no válida. Intente nuevamente.");
                return ShowSession(networkDataHelper, ref exit);
        }
    }

    public static bool Register(NetworkDataHelper networkDataHelper, ref bool exit)
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
        return false;
    }
    
    static bool Login(NetworkDataHelper networkDataHelper, ref bool exit)
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
            networkDataHelper.Send(actionCode);

            Program.SendData(networkDataHelper, message);

            string response = Program.ReceiveResponse(networkDataHelper);
            string code = response.Split('#')[0];
            Console.WriteLine($"Servidor: ");
            Console.WriteLine();
            Program.ShowResponse(response);
            return code.Equals("1");
        }
        catch (SocketException)
        {
            Console.WriteLine("La conexión ha sido interrumpida.");
            exit = true;
            return false;
        }
    }
}