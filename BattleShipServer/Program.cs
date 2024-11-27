using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
class Program
{
    static void Main(string[] args)
    {
        BattleshipServer server = new BattleshipServer();
        server.StartServer();

        // Keep the server running
        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();
    }
}
class BattleshipServer
{
    private TcpListener listener;
    private Player player1;
    private Player player2;
    private bool gameStarted = false;

    public void StartServer()
    {
        try
        {
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Server started. Waiting for players...");

            Task.Run(async () => await AcceptPlayers());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting server: {ex.Message}");
        }
    }

    private async Task AcceptPlayers()
    {
        try
        {
            // Accept Player 1
            var client1 = await listener.AcceptTcpClientAsync();
            player1 = new Player(client1, "Player 1");
            Console.WriteLine("Player 1 connected.");

            // Notify Player 1 to wait
            await player1.SendMessage("Waiting for Player 2...");

            // Accept Player 2
            var client2 = await listener.AcceptTcpClientAsync();
            player2 = new Player(client2, "Player 2");
            Console.WriteLine("Player 2 connected.");

            // Notify players that the game is starting
            await player1.SendMessage("Game starting. Your turn.");
            await player2.SendMessage("Game starting. Waiting for opponent's move.");

            player1.IsTurn = true; // Player 1 starts the game
            player2.IsTurn = false;

            gameStarted = true;

            // Start the game loop
            await Task.WhenAll(ProcessPlayerMoves(player1, player2), ProcessPlayerMoves(player2, player1));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accepting players: {ex.Message}");
        }
    }

    private async Task ProcessPlayerMoves(Player currentPlayer, Player opponent)
    {
        try
        {
            while (gameStarted)
            {
                if (currentPlayer.IsTurn)
                {
                    string message = await currentPlayer.ReceiveMessage();
                    if (message.StartsWith("SHOT"))
                    {
                        var coordinates = message.Split(':')[1].Split(',');
                        int x = int.Parse(coordinates[0]);
                        int y = int.Parse(coordinates[1]);

                        bool hit = opponent.Grid.CheckHit(x, y);
                        if (hit)
                        {
                            await currentPlayer.SendMessage("HIT");
                            await opponent.SendMessage($"OPPONENT_HIT:{x},{y}");
                        }
                        else
                        {
                            await currentPlayer.SendMessage("MISS");
                            await opponent.SendMessage($"OPPONENT_MISS:{x},{y}");
                        }

                        if (opponent.Grid.AllShipsSunk())
                        {
                            await currentPlayer.SendMessage("WIN");
                            await opponent.SendMessage("LOSE");
                            gameStarted = false; // Stop the game loop
                            break;
                        }

                        // Switch turns
                        currentPlayer.IsTurn = false;
                        opponent.IsTurn = true;

                        await currentPlayer.SendMessage("Waiting for opponent's move.");
                        await opponent.SendMessage("Your turn.");
                    }
                }
                else
                {
                    await Task.Delay(100); // Wait for the opponent to take their turn
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ProcessPlayerMoves: {ex.Message}");
            gameStarted = false;

            // Notify players about the error
            await currentPlayer.SendMessage("ERROR");
            await opponent.SendMessage("OPPONENT_DISCONNECTED");
        }
    }
}

public class Player
    {
        public string Name { get; private set; }
        public bool IsTurn { get; set; }
        public BattleshipGrid Grid { get; private set; }
        private TcpClient client;
        private NetworkStream stream;

        public Player(TcpClient client, string name)
        {
            this.client = client;
            stream = client.GetStream();
            Name = name;
            Grid = new BattleshipGrid(); // Initialize the player's grid
        }

        public async Task SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                await stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to {Name}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ReceiveMessage()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message from {Name}: {ex.Message}");
                throw;
            }
        }
}
public class BattleshipGrid
{
    private const int GridSize = 10;
    private readonly int[,] grid; // 0 = empty, 1 = ship, -1 = hit

    public BattleshipGrid()
    {
        grid = new int[GridSize, GridSize];
    }

    public bool PlaceShip(int x, int y, int length, bool isHorizontal)
    {
        if (isHorizontal)
        {
            if (x + length > GridSize) return false; // Out of bounds

            for (int i = 0; i < length; i++)
                if (grid[x + i, y] != 0) return false; // Overlap

            for (int i = 0; i < length; i++)
                grid[x + i, y] = 1; // Place ship
        }
        else
        {
            if (y + length > GridSize) return false; // Out of bounds

            for (int i = 0; i < length; i++)
                if (grid[x, y + i] != 0) return false; // Overlap

            for (int i = 0; i < length; i++)
                grid[x, y + i] = 1; // Place ship
        }

        return true;
    }
    public bool CheckHit(int x, int y)
    {
        if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");

        if (grid[x, y] == 1) // Ship is present
        {
            grid[x, y] = -1; // Mark as hit
            return true;
        }

        return false; // Miss
    }
    public bool AllShipsSunk()
    {
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                if (grid[i, j] == 1) return false; // At least one ship part is still intact
            }
        }
        return true;
    }

    public void PrintGrid()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                Console.Write(grid[x, y] + " ");
            }
            Console.WriteLine();
        }
    }
}

