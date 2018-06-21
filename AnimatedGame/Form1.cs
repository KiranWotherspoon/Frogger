using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using GameSystemServices;

namespace AnimatedGame
{
    public partial class Form1 : Form
    {
        const string gameToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJnYW1lSWQiOiI1YjJiZGJkMTUwZTRiODIxZDYxNzg1MmQiLCJjYXJkSWQiOiIxIiwiaWF0IjoxNTI5NjAwOTkyfQ.ZPeGSek1aUve01laeAtV8xm_UmzplzTTQQ5qSpsBefg";
        public static Service service = new Service(Environment.GetCommandLineArgs(), gameToken);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;

            MainScreen ms = new MainScreen();
            this.Controls.Add(ms);
        }

    }
}
