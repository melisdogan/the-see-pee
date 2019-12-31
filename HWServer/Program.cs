using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class StreamTcpSrvr
{
   public static void Main()
   {
      string data;
      IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);

      Socket newsock = new Socket(AddressFamily.InterNetwork,
                      SocketType.Stream, ProtocolType.Tcp);

      newsock.Bind(ipep);
      newsock.Listen(10);
      Console.WriteLine("Waiting for a client...");

      Socket client = newsock.Accept();
      IPEndPoint newclient = (IPEndPoint)client.RemoteEndPoint;
      Console.WriteLine("Connected with {0} at port {1}",
                      newclient.Address, newclient.Port);

      NetworkStream ns = new NetworkStream(client);
      //StreamReader sr = new StreamReader(ns);
      //StreamWriter sw = new StreamWriter(ns);

      string welcome = "Welcome to my test server";
      writeMessageBoundary(ns, welcome);
      //sw.Flush();

      while(true)
      {
         try
         {
            data = readMessageBoundary(ns);
         } catch (IOException)
         {
            break;
         }
       
         Console.WriteLine(data);
         writeMessageBoundary(ns, data);
         //sw.Flush();
      }
      Console.WriteLine("Disconnected from {0}", newclient.Address);
      //sw.Close();
      //sr.Close();
      ns.Close();
   }
   
private static String readMessageBoundary(NetworkStream ns){
      string next;
      string message = "";
      string temp = "";
      byte[] data = new byte[1];
      while(true){
         data[0] = (byte)ns.ReadByte();
         next = Encoding.ASCII.GetString(data);
         if (next.Equals("<")) {
            temp += next;
            for (int i = 0; i < 6; i++){
               data[0] = (byte)ns.ReadByte();
               next = Encoding.ASCII.GetString(data);
               temp += next;
            }
            if (temp.Equals("<##!##>")){
               return message;
            }
            else message += temp;
         }
         else message += next;
      }
   }
   private static void writeMessageBoundary(NetworkStream ns, String message){
      byte[] data = new byte[1024];
      message += "<##!##>";
      data = Encoding.ASCII.GetBytes(message);
      for (int i = 0; i < data.Length; i++ )
         ns.WriteByte(data[i]);
   }
}