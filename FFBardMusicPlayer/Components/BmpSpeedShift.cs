using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Controls
{
    public partial class SpeedShiftComponent : NumericUpDown
    {
        public SpeedShiftComponent()
        {
            InitializeComponent();

            BackColor = Color.FromArgb(120, 120, 120);
            ForeColor = Color.FromArgb(250, 250, 250);

            Minimum   = 10;
            Maximum   = 200;
            Increment = 5;
            Value     = 100;

            TextAlign = HorizontalAlignment.Center;
        }

        protected override void UpdateEditText()
        {
            ChangingText = true;
            Text         = Value.ToString() + "%";
            ChangingText = false;
        }
    }
}