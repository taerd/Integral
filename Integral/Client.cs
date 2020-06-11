using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonNet;

namespace Integral
{
    class Client
    {
        public delegate void Log(string data);
        public event Log EventInfo;

        private string sourceData;
        private String serverHost;
        private Socket cSocket;
        private int port = 8034;
        private NetMessaging net;
        public Client(String serverHost)
        {
            try
            {
                this.serverHost = serverHost;
                Console.WriteLine("Подключение к {0}", this.serverHost);
                cSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                cSocket.Connect(this.serverHost, port);
                net = new NetMessaging(cSocket);
                net.DataReceived += OnResult;
                //подписки
                new Thread(() =>
                {
                    try
                    {
                        net.Communicate();
                    }
                    catch (Exception ex)
                    {
                        //exception
                    }
                }).Start();
            }
            catch (Exception e)
            {
                //exception
            }
        }
        public void Send(string data)
        {
            if(data.Trim().Length > 0)
            {
                sourceData = data;
                net.SendData("REQUEST",data);
            }
        }
        private void OnResult(string command,string data)
        {
            if (data.Trim().Equals("NOTHAVE"))
            {
                Integrate(sourceData);
            }
            else
            {
                EventInfo?.Invoke(data);
            }
        }
        int numProcs = Environment.ProcessorCount;  // количество доступных ядер
        private  Func<double, double> function;  // Интегрируемая функция
        private int B, A, N;
        private void Integrate(string data)
        {
            //метод интегрирования в пределех data(сплитить)
            char[] sep = { ' ' };
            var cd = data.Split(sep, 3);
            A = Int32.Parse(cd[0]);
            B = Int32.Parse(cd[1]);
            N = Int32.Parse(cd[2]);
            MediumRectangle();
        }
        public void Method(Func<double, double> f)
        {
            function = f;
        }

        public void MediumRectangle()
        {
            double dx = (B - A) / N;  // шаг разбиения отрезка [A;B] 
            double res = 0;
            double point_offset = A + 0.5 * dx;  // задаёт долю смещения точки, в которой вычисляется функция, от левого края отрезка dx
            int partsSize = N / numProcs;
            int balance = N - partsSize * numProcs;
            object block = new object();

            void Calculations(int part)
            {
                int start = part * partsSize + ((part < balance) ? part : balance);
                int finish = (part + 1) * partsSize + ((part + 1 < balance) ? part : (balance - 1));
                double partial_result = 0;
                for (int i = start; i < finish; i++)
                {
                    partial_result += function(point_offset + i * dx);
                }
                Monitor.Enter(block);
                try
                {
                    res += partial_result;
                }
                finally
                {
                    Monitor.Exit(block);
                }
            }

            Parallel.For(0, numProcs, Calculations);

            EventInfo?.Invoke(res.ToString());
        }

    }
}
