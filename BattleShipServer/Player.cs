using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipServer
{
    public class Player
    {
        public TcpClient Client { get; }
        public bool IsTurn { get; set; }
        public Grid Grid { get; private set; }
        private NetworkStream stream;

        public Player(TcpClient client)
        {
            Client = client;
            stream = client.GetStream();
            Grid = new Grid();
        }

        public async Task<string> ReceiveMessage()
        {
            byte[] buffer = new byte[256];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
