using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Peer
{
    private readonly int port; // The port number the peer will use for communication
    private UdpClient udpClient; // The UDP client for sending and receiving messages
    private IPEndPoint remoteEndPoint; // The remote endpoint for sending messages

    // Constructor to initialize the peer with a specific port
    public Peer(int port)
    {
        this.port = port;
        udpClient = new UdpClient(port);
    }

    // Sets the remote endpoint using the provided IP address and port
    public void SetRemoteEndPoint(string ipAddress, int port)
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }

    // Starts listening for incoming messages asynchronously
    public async Task StartListening()
    {
        try
        {
            while (true)
            {
                var receivedResult = await udpClient.ReceiveAsync(); // Wait for a message to be received
                string message = Encoding.UTF8.GetString(receivedResult.Buffer); // Decode the received message
                Console.WriteLine($"Received: {message}"); // Print the received message to the console
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}"); // Handle any exceptions that occur
        }
    }

    // Sends a message to the remote endpoint asynchronously
    public async Task SendMessage(string message)
    {
        if (remoteEndPoint == null)
        {
            Console.WriteLine("Remote endpoint not set."); // Ensure the remote endpoint is set before sending
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(message); // Convert the message to a byte array
        try
        {
            await udpClient.SendAsync(data, data.Length, remoteEndPoint); // Send the message to the remote endpoint
            Console.WriteLine($"Sent: {message}"); // Print the sent message to the console
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}"); // Handle any exceptions that occur
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Enter your listening port: ");
        int listeningPort = int.Parse(Console.ReadLine()); // Get the listening port from the user

        Peer peer = new Peer(listeningPort); // Create a new peer with the specified port
        Task listeningTask = peer.StartListening(); // Start the listening task

        Console.Write("Enter remote peer IP: ");
        string remoteIp = Console.ReadLine(); // Get the remote peer's IP address from the user

        Console.Write("Enter remote peer port: ");
        int remotePort = int.Parse(Console.ReadLine()); // Get the remote peer's port from the user

        peer.SetRemoteEndPoint(remoteIp, remotePort); // Set the remote endpoint

        Console.WriteLine("Type your message and press Enter to send. Type 'exit' to quit.");

        string message;
        while ((message = Console.ReadLine()) != "exit")
        {
            await peer.SendMessage(message); // Send the message to the remote peer
        }
    }
}
