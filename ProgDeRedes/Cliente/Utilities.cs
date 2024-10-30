namespace Cliente;

static class Utilities
{
    public static string ReadNonEmptyInput()
    {
        string input = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(input) || input.Contains("#"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("El valor no puede estar vacío ni contener '#'.");
            Console.ResetColor();
            Console.Write("Intente nuevamente: ");
            input = Console.ReadLine();
        }
        return input;
    }

    public static int ShowGameCategories()
    {
        Console.WriteLine("1- Acción");
        Console.WriteLine("2- Aventura");
        Console.WriteLine("3- Combate");
        Console.WriteLine("4- Puzzle");
        Console.WriteLine("5- Carreras");
        Console.WriteLine("6- Tiroteo");
        Console.WriteLine("7- Simulación");
        Console.WriteLine("8- Deportes");
        Console.WriteLine("9- Estrategia");
        
        return CheckIntegerValue(1, 9);
    }
    
    public static int ShowGamePlatforms()
    {
        Console.WriteLine("1- Windows");
        Console.WriteLine("2- Linux");
        Console.WriteLine("3- MacOs");
        Console.WriteLine("4- Android");
        
        return CheckIntegerValue(1, 4);
    }
    
    public static int CheckIntegerValue(int min, int max)
    { 
        int value;
        string input;
    
        do
        {
            input = Console.ReadLine();

            if (int.TryParse(input, out value) && value >= min && value <= max)
            {
                break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Entrada no válida.");
            Console.ResetColor();
            Console.Write("Introduzca un valor correcto: ");
        } while (true);

        return value;
    }
}