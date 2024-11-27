using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipServer
{
    public class GameServer
    {
        private TcpListener listener;
        private Player player1;
        private Player player2;
        private bool gameStarted;

        public GameServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            gameStarted = false;
        }

        public async Task Start()
        {
            listener.Start();
            Console.WriteLine("Server started. Waiting for players...");

            player1 = new Player(await listener.AcceptTcpClientAsync());
            Console.WriteLine("Player 1 connected.");

            player2 = new Player(await listener.AcceptTcpClientAsync());
            Console.WriteLine("Player 2 connected.");

            gameStarted = true;
            StartGame();
        }

        private void StartGame()
        {
            // Inform players that the game has started
            player1.SendMessage("Game Start: You are Player 1.");
            player2.SendMessage("Game Start: You are Player 2.");

            // Set Player 1 to take the first turn
            player1.IsTurn = true;
            player1.SendMessage("Your turn.");
            player2.SendMessage("Waiting for opponent's move.");

            // Begin listening for player moves
            Task.Run(() => ProcessPlayerMoves(player1, player2));
            Task.Run(() => ProcessPlayerMoves(player2, player1));
        }

        private async void ProcessPlayerMoves(Player currentPlayer, Player opponent)
        {
            while (gameStarted)
            {
                string message = await currentPlayer.ReceiveMessage();
                if (message.StartsWith("SHOT"))
                {
                    var coordinates = message.Split(':')[1].Split(',');
                    int x = int.Parse(coordinates[0]);
                    int y = int.Parse(coordinates[1]);

                    // Process shot on opponent’s grid
                    bool hit = opponent.Grid.CheckHit(x, y);
                    if (hit)
                    {
                        currentPlayer.SendMessage("HIT");
                        opponent.SendMessage($"OPPONENT_HIT:{x},{y}");
                    }
                    else
                    {
                        currentPlayer.SendMessage("MISS");
                        opponent.SendMessage($"OPPONENT_MISS:{x},{y}");
                    }

                    // Check if game is over
                    if (opponent.Grid.AllShipsSunk())
                    {
                        currentPlayer.SendMessage("WIN");
                        opponent.SendMessage("LOSE");
                        gameStarted = false;
                        listener.Stop();
                    }
                    else
                    {
                        // Switch turns
                        currentPlayer.IsTurn = false;
                        opponent.IsTurn = true;
                        currentPlayer.SendMessage("Waiting for opponent's move.");
                        opponent.SendMessage("Your turn.");
                    }
                }
            }
        }
    }
}
