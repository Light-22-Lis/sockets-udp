using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UdpClientApp
{
    class Program
    {
        static string remoteAddress;
        static int remotePort, localPort;
        static void Main(string[] args)
        {
            try{
                Console.Write("Enter the listening port: ");
                localPort = Int32.Parse(Console.ReadLine());
                Console.Write("Enter the remote address to connect to: ");
                remoteAddress = Console.ReadLine();
                Console.Write("Enter the connection port: ");
                remotePort = Int32.Parse(Console.ReadLine());

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                SendMessage();
            }
            catch(Exception exception){
                Console.WriteLine(exception.Message);
            }
        }
        private static void SendMessage()
        {
            UdpClient sender = new UdpClient();
            try{
                while(true){
                    string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    sender.Send(data, data.Length, remoteAddress, remotePort);
                }
            }
            catch(Exception exception){
                Console.WriteLine(exception.Message);
            }
            finally{
                sender.Close();
            }
        }
        private static void ReceiveMessage()
        {
            UdpClient receiver = new UdpClient(localPort);
            IPEndPoint remoteIp = null;
            try{
                while(true){
                    byte[] data = receiver.Receive(ref remoteIp);
                    string message = Encoding.Unicode.GetString(data);
                    Console.Write("Enter your name: ");
                    string name = Console.ReadLine();
                    Console.WriteLine("{0}: {1}",name, message);
                }
            }
            catch(Exception exception){
                Console.WriteLine(exception.Message);
            }
            finally{
                receiver.Close();
            }
        }        
    }
}