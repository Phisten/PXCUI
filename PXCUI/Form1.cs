using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PXCUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PlusObj.ImageSocket NetImageSource = new PlusObj.ImageSocket();
        private void Form1_Load(object sender, EventArgs e)
        {
            NetImageSource.Run();
            timer1.Interval = 33;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!NetImageSource.IsRun)
            {
                Console.WriteLine("AutoReStart");
                NetImageSource.Run();
            }
            picView.Image = NetImageSource.VedioBuffer.ReadBitmap();
            this.Text = "FPS = " + NetImageSource.VedioBuffer.FPS + ", delay = " + NetImageSource.VedioBuffer.Delay_Net + ", data = " + NetImageSource.VedioBuffer.DataSize;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (NetImageSource.IsRun)
            {
                NetImageSource.Close();
            }
            else
            {
                NetImageSource.Run();
            }
        }
    }
}
