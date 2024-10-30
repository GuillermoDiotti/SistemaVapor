using Servidor.Logics.GameLogic;

namespace Servidor;

static class Utilities
{
    public static GameGenre AssignGenre(string gameGenre)
    {
        switch (gameGenre)
        {
            case "1":
                return GameGenre.Action;
            case "2":
                return GameGenre.Adventure;
            case "3":
                return GameGenre.Combat;
            case "4":
                return GameGenre.Puzzle;
            case "5":
                return GameGenre.Racing;
            case "6":
                return GameGenre.Shooting;
            case "7":
                return GameGenre.Simulation;
            case "8":
                return GameGenre.Sports;
            case "9":
                return GameGenre.Strategy;
            default:
                return GameGenre.Action;
        }
    }
    
    public static GamePlatform AssignPlatform(string gamePlatform)
    {
        switch (gamePlatform)
        {
            case "1":
                return GamePlatform.Windows;
            case "2":
                return GamePlatform.Linux;
            case "3":
                return GamePlatform.MacOs;
            case "4":
                return GamePlatform.Android;
            default:
                return GamePlatform.Windows;
        }
    }
}