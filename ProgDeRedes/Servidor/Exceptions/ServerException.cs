namespace Servidor.Exceptions;

public class ServerException : Exception
{
    public ServerException(string? message) : base(message){}
}