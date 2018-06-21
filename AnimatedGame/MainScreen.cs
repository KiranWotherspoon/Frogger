using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnimatedGame
{
    public partial class MainScreen : UserControl
    {
        public MainScreen()
        {
            InitializeComponent();

        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Form f = this.FindForm();
            f.Controls.Remove(this);

            GameScreen gs = new GameScreen();
            f.Controls.Add(gs);
            gs.Focus();
            Cursor.Hide();
        }

        private void exitButton_Click_1(object sender, EventArgs e)
        {
            Form f = this.FindForm();
            f.Close();
        }

        private void howToButton_Click(object sender, EventArgs e)
        {
            Form f = this.FindForm();
            f.Controls.Remove(this);

            HowToPlay htp = new HowToPlay();
            f.Controls.Add(htp);
            htp.Focus();
            Cursor.Hide();
        }
    }
}
