using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class StreamTcpClient
{
   public static void Main()
   {
      string input;
      IPEndPoint ipep = new IPEndPoint(
                      IPAddress.Parse("127.0.0.1"), 9050);

      Socket server = new Socket(AddressFamily.InterNetwork,
                     SocketType.Stream, ProtocolType.Tcp);

      try
      {
         server.Connect(ipep);
      } catch (SocketException e)
      {
         Console.WriteLine("Unable to connect to server.");
         Console.WriteLine(e.ToString());
         return;
      }


      NetworkStream ns = new NetworkStream(server);
      //StreamReader sr = new StreamReader(ns);
      //StreamWriter sw = new StreamWriter(ns);

   
      Console.WriteLine(readMessageBoundary(ns));

      while(true)
      {
         input = Console.ReadLine();
         if (input == "exit")
            break;
         writeMessageBoundary(ns, input);
         //sw.Flush();

         
         Console.WriteLine(readMessageBoundary(ns));
      }
      Console.WriteLine("Disconnecting from server...");
      ns.Close();
      server.Shutdown(SocketShutdown.Both);
      server.Close();
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