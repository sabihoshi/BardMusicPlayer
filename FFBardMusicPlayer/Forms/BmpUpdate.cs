﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServiceStack;
using ServiceStack.Text;

namespace FFBardMusicPlayer
{
    public partial class BmpUpdate : Form
    {
        public UpdateVersion version = new UpdateVersion();
        private float currentVersion;
        private BackgroundWorker worker = new BackgroundWorker();

        public BmpUpdate()
        {
            InitializeComponent();

            worker.WorkerSupportsCancellation =  true;
            worker.DoWork                     += UpdateApp;
            worker.RunWorkerCompleted         += Worker_RunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { Close(); }

        private HttpWebResponse LoadFile(Uri url, out string result)
        {
            result = string.Empty;

            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                var response = (HttpWebResponse) request.GetResponse();

                request.Timeout = 2000;

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        private bool DownloadToProgramFile(string filename, string outFilename)
        {
            var signatureJson = new Uri(Program.urlBase + filename);
            var res2 = LoadFile(signatureJson, out var jsonText);
            if (res2.StatusCode == HttpStatusCode.OK)
            {
                var file = Path.Combine(Program.appBase, outFilename);
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(jsonText);
                    return true;
                }
            }

            return false;
        }

        public void UpdateApp(object sender, EventArgs args)
        {
            currentVersion = UpdateVersion.Version;

            var updateJson = Program.urlBase + $"update?v={UpdateVersion.Version}";
            Console.WriteLine("Updatejson: " + updateJson);

            version = JsonSerializer.DeserializeFromString<UpdateVersion>(updateJson.GetJsonFromUrl());

            if (version == null)
            {
                return;
            }

            DialogResult = version.updateVersion > UpdateVersion.Version ? DialogResult.Yes : DialogResult.No;

            if (UpdateVersion.Version > version.updateVersion)
            {
                DialogResult = DialogResult.Ignore;
            }

            // If not ignoring updates
            if (!Properties.Settings.Default.SigIgnore)
            {
                // If new version is above current sig version
                // Or if they don't exist

                var sigExist = File.Exists(Path.Combine(Program.appBase, "signatures.json"));
                var strExist = File.Exists(Path.Combine(Program.appBase, "structures.json"));
                if (version.sigVersion > Properties.Settings.Default.SigVersion || version.sigVersion == -1 ||
                    !(sigExist && strExist))
                {
                    Console.WriteLine("Downloading signatures");
                    var sigUrl = string.Format("update?f=signatures&v={0}", UpdateVersion.Version.ToString());
                    if (DownloadToProgramFile(sigUrl, "signatures.json"))
                    {
                        Console.WriteLine("Downloaded signatures");
                    }

                    Console.WriteLine("Downloading structures");
                    var strUrl = string.Format("update?f=structures&v={0}", UpdateVersion.Version.ToString());
                    if (DownloadToProgramFile(strUrl, "structures.json"))
                    {
                        Console.WriteLine("Downloaded structures");
                    }

                    Properties.Settings.Default.SigVersion = version.sigVersion;
                    Console.WriteLine(string.Format("ver1: {0} ver2: {1}", version.sigVersion,
                        Properties.Settings.Default.SigVersion));
                    Properties.Settings.Default.Save();

                    // New signature update
                    // Reset forced stuff so people don't get stuck on that junk
                    if (version.sigVersion > 0)
                    {
                        Properties.Settings.Default.ForcedOpen = false;
                    }
                }
            }
        }

        private void ButtonSkip_Click(object sender, EventArgs e) { worker.CancelAsync(); }
    }

    public class UpdateVersion
    {
        // If update, fill these in
        public float updateVersion { get; set; } = 0f;

        public string updateText { get; set; } = "";

        public string updateTitle { get; set; } = "";

        public string updateLog { get; set; } = "";

        public string appName { get; set; } = "Bard Music Player";
#if DEBUG
        public string appVersion { get; set; } = "Beta";
#else
		public string appVersion { get; set; } = string.Empty;
#endif
        public string creatorName { get; set; } = string.Empty;

        public string creatorWorld { get; set; } = string.Empty;

        public int sigVersion { get; set; } = -1;

        public static float Version
        {
            get
            {
                var v = typeof(Program).Assembly.GetName().Version;
                return (float) v.Major + (float) (v.Minor / 100f);
            }
        }

        public override string ToString()
        {
            var str = string.Format("{0} {1}", appName, Version);
            if (!string.IsNullOrEmpty(appVersion))
            {
                str = string.Format("{0} [{1}]", str, appVersion);
            }

            if (!string.IsNullOrEmpty(creatorName))
            {
                str = string.Format("{0} by {1}", str, creatorName);
            }

            if (!string.IsNullOrEmpty(creatorWorld))
            {
                str = string.Format("{0} ({1})", str, creatorWorld);
            }

            return str;
        }
    }
}