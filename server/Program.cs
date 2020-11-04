using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class Program
    {
       // Socket Listener acts as a server and listens to the incoming   
       // messages on the specified port and protocol.  
        public class SocketListener
        {

            public static int Main(String[] args)
            {
                Console.WriteLine("Write a server message to send a client!");
                //get message from user
                string mes = Console.ReadLine();
                StartServer(mes);
                //For use Encrypt system uncomment line 27.Note: you must comment line 25
                //StartServer(Encrypt(mes));

                return 0;
            }


            public static void StartServer( string  mes)
            {
                // Get Host IP Address that is used to establish a connection  
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1  
                // If a host has multiple addresses, you will get a list of addresses  
                IPAddress ipAddress = IPAddress.Parse("");//Enter your ip address middle of double quotation,like this: 192.168.xxx.xxx if you try locall
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000); //Enter a free port of your system anfer comma


                try
                {

                    // Create a Socket that will use Tcp protocol      
                    Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    // A Socket must be associated with an endpoint using the Bind method  
                    listener.Bind(localEndPoint);
                    // Specify how many requests a Socket can listen before it gives Server busy response.  
                    // We will listen 10 requests at a time  
                    listener.Listen(10);

                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();

                    // Incoming data from the client.    
                    string data = null;
                    byte[] bytes = null;

                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Text received : {0}", data);

                    byte[] msg = Encoding.ASCII.GetBytes(mes);
                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Console.WriteLine("\n Press any key to continue...");
                Console.ReadKey();
                //ip.addr == 192.168.1.50 and not udp
            }

            public static string Encrypt(string clearText)
            {
                //Write your key
                string EncryptionKey = "Instagram:@Saeed.Balkani";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
        }
    }
}
