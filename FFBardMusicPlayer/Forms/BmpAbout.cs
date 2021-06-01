﻿using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace FFBardMusicPlayer
{
    public partial class BmpAbout : Form
    {
        public BmpAbout() { InitializeComponent(); }

        private void DonateLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start($"{Program.urlBase}donate/");
        }

        private void CloseButton_Click(object sender, EventArgs e) { Close(); }

        private void DonationButton_Click(object sender, EventArgs e) { Process.Start($"{Program.urlBase}#donate"); }
    }
}