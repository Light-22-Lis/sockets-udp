using System.Net;
using System.Net.Sockets;

string GetLine() => Console.ReadLine() ?? throw new OperationCanceledException("Cancelled");
System.Text.Encoding encoding = System.Text.Encoding.Unicode;

try {
    Console.Write("Enter the remote address to connect to:\t");
    string remoteAddress = GetLine();
    Console.Write("Enter the connection (remote) port:\t");
    int remotePort = int.Parse(GetLine());
    Console.Write("Enter the listening (local) port:\t");
    int localPort = int.Parse(GetLine());

    Thread receiveThread = new(new ParameterizedThreadStart(ReceiveMessage));
    receiveThread.Start(localPort);
    SendMessage(remoteAddress, remotePort);
}
catch (Exception ex) {
    Console.WriteLine(ex.Message);
}

void SendMessage(string remoteAddress, int remotePort)
{
    UdpClient sender = new();
    try {
        while(true) {
            string message = GetLine();
            byte[] data = encoding.GetBytes(message);
            sender.Send(data, data.Length, remoteAddress, remotePort);
        }
    }
    catch (Exception ex) {
        Console.WriteLine(ex.Message);
    }
    finally{
        sender.Close();
    }
}
        
void ReceiveMessage(object? arg)
{
    if (arg is not int localPort)
        throw new InvalidCastException();

    UdpClient receiver = new(localPort);
    IPEndPoint? remoteIp = null;
    try {
        while (true) {
            byte[] data = receiver.Receive(ref remoteIp);
            string message = encoding.GetString(data);
            Console.WriteLine("Other: " + message);
        }
    }
    catch(Exception ex) {
        Console.WriteLine(ex.Message);
    }
    finally{
        receiver.Close();
    }
} 