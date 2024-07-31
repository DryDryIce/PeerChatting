using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Peer
{
    private readonly int port;
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    public Peer(int port)
    {
        this.port = port;
        udpClient = new UdpClient(port);
    }

    public void SetRemoteEndPoint(string ipAddress, int port)
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }

    public async Task StartListening()
    {
        try
        {
            while (true)
            {
                var receivedResult = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(receivedResult.Buffer);
                Console.WriteLine($"Received: {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task SendMessage(string message)
    {
        if (remoteEndPoint == null)
        {
            Console.WriteLine("Remote endpoint not set.");
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(message);
        try
        {
            await udpClient.SendAsync(data, data.Length, remoteEndPoint);
            Console.WriteLine($"Sent: {message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
