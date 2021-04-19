using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayerInit
{
    public partial class BmpRegion : Form
    {
        public BmpRegion()
        {
            InitializeComponent();
        }
        internal Lib lib;
        internal Type bmpProgram;
        private void BmpRegion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void region_global_Click(object sender, EventArgs e)
        {
            dynamic main = Activator.CreateInstance(bmpProgram ?? throw new InvalidOperationException("FFBardMusicPlayerInternal.dll"));
            Hide();
            main.StartUp(lib.global, lib.title + " - Global", lib.message);
            
        }

        private void region_cn_Click(object sender, EventArgs e)
        {
            dynamic main = Activator.CreateInstance(bmpProgram ?? throw new InvalidOperationException("FFBardMusicPlayerInternal.dll"));
            Hide();
            main.StartUp(lib.cn, lib.title + " - CN", lib.message);
            
        }

        private void region_kr_Click(object sender, EventArgs e)
        {
            dynamic main = Activator.CreateInstance(bmpProgram ?? throw new InvalidOperationException("FFBardMusicPlayerInternal.dll"));
            Hide();
            main.StartUp(lib.kr, lib.title + " - KR", lib.message);
            
        }
    }
}
