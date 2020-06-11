using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonNet
{
    public delegate void Receiving(string command,string data);

    public class NetMessaging
    {
        private Socket cSocket;
        public event Receiving DataReceived;
        public NetMessaging(Socket s)
        {
            cSocket = s;
        }

        public void SendData(string command,string data)
        {
            if (cSocket != null)
            {
                try
                {
                    if (data.Trim().Equals("") ) return;
                    var b = Encoding.UTF8.GetBytes(command+"="+data+"\n");
                    cSocket.Send(b);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Не удалось отправить сообщение :(");
                    //exception
                }
            }
        }
        public String ReceiveData()
        {
            String res = "";
            if (cSocket != null)
            {
                var b = new byte[65536];
                do
                {
                    var cnt = cSocket.Receive(b);
                    //Console.WriteLine("Получена порция данных №{0}", ++i);
                    var r = Encoding.UTF8.GetString(b, 0, cnt);
                    res += r;
                } while (res[res.Length - 1] != '\n');
                //Console.WriteLine("Данные успешно получены");
            }
            return res;
        }
        public void Communicate()
        {
            if (cSocket != null)
            {
                while (true)
                {
                    String d = ReceiveData();
                    Parse(d);
                }
            }
        }
        private void Parse(string data)
        {
            char[] sep = { '=' };
            var cd = data.Split(sep, 2);
            switch (cd[0])
            {
                case "REQUEST":
                    {
                        DataReceived?.Invoke(cd[0], cd[1]);
                        break;
                    }
                case "ANSWER":
                    {
                        DataReceived?.Invoke(cd[0], cd[1]);
                        break;
                    }
            }
        }
    }
}
