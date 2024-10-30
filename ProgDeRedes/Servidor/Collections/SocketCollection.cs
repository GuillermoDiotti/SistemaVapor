using System.Net.Sockets;

namespace Servidor.Collections;

public class SocketCollection
{
    private static SocketCollection _instance;
    private static readonly object _lock = new object();
    private List<Socket> sockets = new List<Socket>();

    private SocketCollection() { }

    public static SocketCollection Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SocketCollection();
                }
                return _instance;
            }
        }
    }

    public List<Socket> Sockets
    {
        get
        {
            lock (_lock)
            {
                return new List<Socket>(sockets);
            }
        }
    }
    
    public void AddSocket(Socket socket)
    {
        lock (_lock)
        {
            sockets.Add(socket);
        }
    }
}