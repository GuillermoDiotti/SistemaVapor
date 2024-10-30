using Communication;

namespace Cliente.Menu;

static class MenuManager
{
    public static async Task ShowMenu(NetworkDataHelper networkDataHelper)
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
                await GameManager.CreateGame(networkDataHelper);
                break;
            case "2":
                await GameManager.AcquireGame(networkDataHelper);
                break;
            case "3":
                await GameManager.ModifyGame(networkDataHelper);
                break;
            case "4":
                await GameManager.DeleteGame(networkDataHelper);
                break;
            case "5":
                await GameManager.SearchGames(networkDataHelper);
                break;
            case "6":
                await GameManager.SpecificGameSearch(networkDataHelper);
                break;
            case "7":
                await GameManager.AddGameReview(networkDataHelper);
                break;
            case "8":
                Program._logged = false;
                break;
            default:
                Console.WriteLine("\nOpcion no valida, ingrese una opcion entre 1 y 8");
                await ShowMenu(networkDataHelper);
                break;
        }
    }
}