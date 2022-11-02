using System.Net;
using System.Net.Sockets;

string GetLine() => Console.ReadLine() ?? throw new OperationCanceledException("Cancelled");
System.Text.Encoding encoding = System.Text.Encoding.Unicode;

IPAddress remoteAddress = IPAddress.Parse("224.0.0.1");

try {
    //Console.Write("Enter the remote address to connect to:\t");
    //Console.Write("Enter the connection (remote) port:\t");
    int remotePort = 8001;// int.Parse(GetLine());
    //Console.Write("Enter the listening (local) port:\t");
    int localPort = 8001;// int.Parse(GetLine());

    Console.Write("Your name: ");
    string userName = GetLine();

    Thread receiveThread = new(new ParameterizedThreadStart(ReceiveMessage));
    receiveThread.Start(localPort);
    SendMessage(remotePort, userName);
}
catch (Exception ex) {
    Console.WriteLine(ex.Message);
}

void SendMessage(int remotePort, string userName)
{
    UdpClient sender = new();
    IPEndPoint remoteEp = new(remoteAddress, remotePort);
    try {
        while(true) {
            string message = $"{userName}: {GetLine()}";
            byte[] data = encoding.GetBytes(message);
            sender.Send(data, data.Length, remoteEp);
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
    receiver.JoinMulticastGroup(remoteAddress, 20);
    IPEndPoint? remoteIp = null;
    IPAddress localAddress = LocalIPAddress();
    try {
        while (true) {
            byte[] data = receiver.Receive(ref remoteIp);
            if (remoteIp.Address == localAddress)
                continue;
            string message = encoding.GetString(data);
            Console.WriteLine(message);
        }
    }
    catch(Exception ex) {
        Console.WriteLine(ex.Message);
    }
    finally{
        receiver.Close();
    }
} 

IPAddress? LocalIPAddress()
{
    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (IPAddress ip in host.AddressList)
        if (ip.AddressFamily == AddressFamily.InterNetwork)
            return ip;
    return null;
}