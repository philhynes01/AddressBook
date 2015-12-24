using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AddressBook
{
    public partial class splashScreen : Form
    {
        public splashScreen()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Top = (Screen.PrimaryScreen.Bounds.Height - this.Height) / 2;
            this.Left = (Screen.PrimaryScreen.Bounds.Width - this.Width) / 2;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            label3.Text = "Version: " + version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision;
        }

        Timer t;
        private void splashScreen_Shown(object sender, EventArgs e)
        {
            t = new Timer();
            t.Interval = 2000;
            t.Start();
            t.Tick += new EventHandler(t_Tick);
        }

        void t_Tick(object sender, EventArgs e)
        {
            t.Stop();
            Form f = new Form1();
            f.Show();
            this.Hide();
        }
    }
}
