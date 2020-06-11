using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Integral
{
    public partial class Form1 : Form
    {
        private int a, b;
        private int m;
        private Client cl;
        public Form1()
        {
            InitializeComponent();
            cl = new Client("taerd");//host name
            cl.EventInfo += OnChange;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            a = Int32.Parse(textBox1.Text);
            b = Int32.Parse(textBox2.Text);
            m = Int32.Parse(textBox3.Text);
            if (b <= a) LabelResult.Text = "Результат : Ошибка при вводе пределов интегрирования";
            else if (m <= 0) LabelResult.Text = "Результат : Ошибка при вводе количества разбиений";
            else
            {
                LabelResult.Text = "Результат : Вычисляется";
                string data = a + " " + b + " " + m;
                cl.Send(data);
            }
        }
        private void OnChange(string data)
        {
            if (!LabelResult.InvokeRequired)
            {
                LabelResult.Text = "Результат : " + data;
            }
            else Invoke(new Client.Log(OnChange), data);
        }
    }
}
