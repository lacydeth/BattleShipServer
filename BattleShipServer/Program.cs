using System.Net.Sockets;
using System.Net;
using System.Text;

class Program
{
    private const int Port = 5000;

    static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        StartServer();
        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();  // Prevents the server from stopping immediately
    }

    private static void StartServer()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        Console.WriteLine("Server started, waiting for clients...");

        // Accept client connections asynchronously
        Task.Run(async () =>
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");
                HandleClient(client);  // Handle each client in a new task
            }
        });
    }

    private static async void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;  // Client disconnected

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                // Respond to the client
                string response = "Message received";
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseData, 0, responseData.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnected.");
        }
    }
}