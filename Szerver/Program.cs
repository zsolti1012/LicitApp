using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Threading;

namespace Szerver
{
    class Program
    {
        static TcpListener listener = null;
        static void Main(string[] args)
        {
            IPHostEntry cimek = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in cimek.AddressList)
            {
                Console.WriteLine("{0}",ip);
            }

            string ipcim = ConfigurationManager.AppSettings["IP"];
            int port = int.Parse(ConfigurationManager.AppSettings["PORT"]);

            IPAddress address = IPAddress.Parse(ipcim);
            listener = new TcpListener(address, port);
            listener.Start();
            Thread t1 = new Thread(Kapcsolat_letrehozasa);
            t1.Start();

            Console.ReadLine();
        }

        static void Kapcsolat_letrehozasa()
        {
            while (true)
            {
                TcpClient kliens = listener.AcceptTcpClient();
                Komm szolgaltatas = new Komm(kliens);
                Thread t1 = new Thread(szolgaltatas.Komm_Indit);
                t1.Start();
            }
        }
    }
}
