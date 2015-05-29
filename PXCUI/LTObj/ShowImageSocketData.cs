using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlusObj
{
    public partial class ShowImageSocketDataForm : Form
    {
        public ShowImageSocketDataForm()
        {
            InitializeComponent();

            timer.Interval = 100;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Value_FPS.Text = "FPS:  " + VedioBuffer.FPS.ToString();
            Value_DataSize.Text = "DataSize:  " + (VedioBuffer.DataSize * 0.00097657).ToString("0.00") + " KB";
            Value_SendTime.Text = "Delay_Net:  " + VedioBuffer.Delay_Net.ToString() + " Ms";
        }

        Timer timer = new Timer();
        public Bitmap_VedioBuffer VedioBuffer = null;
    }
}
