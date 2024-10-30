using Communication;

namespace Cliente.Menu;

static class MenuManager
{
    public static void ShowMenu(NetworkDataHelper networkDataHelper, ref bool logged, ref bool exit)
    {
        Console.WriteLine("\n--- MENU DE ACCIONES ---");
        Console.WriteLine("1- Publicar un juego");
        Console.WriteLine("2- Adquirir un juego");
        Console.WriteLine("3- Modificar un juego");
        Console.WriteLine("4- Eliminar un juego");
        Console.WriteLine("5- Explorar juegos");
        Console.WriteLine("6- Buscar un juego");
        Console.WriteLine("7- Calificar un juego");
        Console.WriteLine("8- Cerrar Sesión");

        string message = Console.ReadLine();

        Console.Clear();
        
        switch (message)
        {
            case "1":
                GameManager.CreateGame(networkDataHelper, ref exit);
                break;
            case "2":
                GameManager.AcquireGame(networkDataHelper, ref exit);
                break;
            case "3":
                GameManager.ModifyGame(networkDataHelper, ref exit);
                break;
            case "4":
                GameManager.DeleteGame(networkDataHelper, ref exit);
                break;
            case "5":
                GameManager.SearchGames(networkDataHelper, ref exit);
                break;
            case "6":
                GameManager.SpecificGameSearch(networkDataHelper, ref exit);
                break;
            case "7":
                GameManager.AddGameReview(networkDataHelper, ref exit);
                break;
            case "8":
                logged = false;
                break;
            default:
                Console.WriteLine("\nOpcion no valida, ingrese una opcion entre 1 y 8");
                ShowMenu(networkDataHelper, ref logged, ref exit);
                break;
        }
    }
}