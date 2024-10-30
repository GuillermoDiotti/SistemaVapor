using System.Net.Sockets;

namespace Communication;

public class NetworkDataHelper
{
    private readonly TcpClient _tcpClient;

    public NetworkDataHelper(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
    }

    public async Task SendAsync(byte[] data)
    {
        try
        {
            int offset = 0;
            var networkStream = _tcpClient.GetStream();
            await networkStream.WriteAsync(
                data,
                offset,
                data.Length);
        }
        catch (Exception e)
        {
            throw new SocketException();
        }
    }

    public async Task<byte[]> ReceiveAsync(int length)
    {
        int offset = 0;
        var data = new byte[length];
        var networkStream = _tcpClient.GetStream();
        try
        {
            while (offset < length)
            {
                var received = await networkStream.ReadAsync(
                    data,
                    offset,
                    length - offset);
                
                offset += received;
            }

            return data;
        }
        catch (Exception)
        {
            throw new SocketException();
        }
    }
}