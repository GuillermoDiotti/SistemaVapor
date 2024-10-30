using System.Net.Sockets;

namespace Servidor.Collections;

public class TcpListenerCollection
{
    private static TcpListenerCollection _instance;
    private static readonly object _lock = new object();
    private List<TcpClient> tcpClients = new List<TcpClient>();

    private TcpListenerCollection() { }

    public static TcpListenerCollection Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new TcpListenerCollection();
                }
                return _instance;
            }
        }
    }

    public List<TcpClient> TcpClients
    {
        get
        {
            lock (_lock)
            {
                return new List<TcpClient>(tcpClients);
            }
        }
    }
    
    public void AddTcpClient(TcpClient TcpClient)
    {
        lock (_lock)
        {
            tcpClients.Add(TcpClient);
        }
    }
    
    public void RemoveTcpClient(TcpClient TcpClient)
    {
        lock (_lock)
        {
            tcpClients.Remove(TcpClient);
        }
    }
}