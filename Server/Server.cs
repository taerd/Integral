using CommonNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private StreamReader f;
        class ConnectedClient
        {
            public Socket cSocket;
            private NetMessaging net;
            public static List<ConnectedClient> clients = new List<ConnectedClient>();
            //public string Name { get; private set; }
            public ConnectedClient(Socket s)
            {
                cSocket = s;
                net = new NetMessaging(cSocket);
                net.DataReceived += OnData;
                new Thread(() =>
                {
                    try
                    {
                        net.Communicate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Не удалось получить данные от клиента :(");
                        clients.Remove(this);
                    }
                }).Start();
            }

            private void OnData(string command, string data)
            {
                //проверка наличия ответа или отправка отрицательного ответа
                this.net.SendData("NOTHAVE", data);
                
            }
        }
        public Server(string filename)
        {
            f = new StreamReader(filename);
            //ход работы
        }

    }
}
