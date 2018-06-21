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
    public partial class HowToPlay : UserControl
    {
        public HowToPlay()
        {
            InitializeComponent();
        }

        private void HowToPlay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    Cursor.Show();
                    Form f = this.FindForm();
                    f.Controls.Remove(this);

                    MainScreen ms = new MainScreen();
                    f.Controls.Add(ms);
                    ms.Focus();
                    break;
            }
        }
    }
}
