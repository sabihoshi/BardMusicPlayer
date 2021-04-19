using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayerInit
{
    public partial class BmpUpdate : Form
    {
        public BmpUpdate()
        {
            InitializeComponent();
        }

        private readonly Stopwatch _stopwatch = new();
        private string _currentFile;

        private int index = -1;

        private WebClient webClient;

        internal void DownloadUpdates(List<Lib.Dll> downloadList, string downloadPath, string localPath)
        {
            webClient = new WebClient();
            webClient.DownloadProgressChanged += (s, e) =>
            {
                progress_text.Text = _currentFile + " " + $"{(e.BytesReceived / 1024d / _stopwatch.Elapsed.TotalSeconds):0.00} Kb/s";
                progress_bar.Value = e.ProgressPercentage;
            };

            webClient.DownloadFileCompleted += (s, e) =>
            {
                _stopwatch.Stop();
                if (index == downloadList.Count-1)
                {
                    Hide();
                    BootLoader.Load();
                } else DownloadNext(downloadList, downloadPath, localPath);
            };
            
            DownloadNext(downloadList, downloadPath, localPath);
            ShowDialog();
        }

        private void DownloadNext(List<Lib.Dll> downloadList, string downloadPath, string localPath)
        {
            index++;
            _stopwatch.Reset();
            _stopwatch.Start();
            _currentFile = downloadList[index].name;
            webClient.DownloadFileTaskAsync(new Uri(downloadPath + downloadList[index].name), localPath + downloadList[index].name);
        }

    }
}
