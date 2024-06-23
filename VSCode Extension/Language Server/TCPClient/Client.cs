using System.Net;
using System.Net.Sockets;
using System.Text;

string hostName = Dns.GetHostName();
IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
// This is the IP address of the local machine
IPAddress localIpAddress = localhost.AddressList[0];
IPEndPoint ipEndPoint = new(localIpAddress, 6500);

Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

await client.ConnectAsync(ipEndPoint);
while (true)
{
    // Send message.
    var message = "Hi friends 👋!\x04";
    var messageBytes = Encoding.UTF8.GetBytes(message);
    _ = await client.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message}\"");

    // Receive ack.
    var buffer = new byte[1_024];
    var received = await client.ReceiveAsync(buffer, SocketFlags.None);
    var response = Encoding.UTF8.GetString(buffer, 0, received);
    Console.WriteLine($"Socket client received acknowledgment: \"{response}\"");
    if (response.Contains("10"))
    {
        break;
    }

    Thread.Sleep(500);
}

client.Shutdown(SocketShutdown.Both);