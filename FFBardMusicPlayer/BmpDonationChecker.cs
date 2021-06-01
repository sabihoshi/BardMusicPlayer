﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace FFBardMusicPlayer
{
    public class BmpDonationChecker
    {
        private BackgroundWorker worker = new BackgroundWorker();
        public EventHandler<DonatorResponse> OnDonatorResponse;

        public class DonatorResponse
        {
            public bool donator { get; set; }

            public string donationMessage { get; set; }
        };

        public BmpDonationChecker(string characterName, string characterWorld)
        {
            worker.WorkerSupportsCancellation =  true;
            worker.DoWork                     += UpdateApp;
            worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs a)
            {
                var don = a.Result as DonatorResponse;
                if (don != null)
                {
                    OnDonatorResponse?.Invoke(this, don);
                }
            };
            var donatorJson =
                new Uri(Program.urlBase + string.Format("donator?n={0}&w={1}", characterName, characterWorld));
            worker.RunWorkerAsync(donatorJson);
        }

        public void UpdateApp(object sender, DoWorkEventArgs args)
        {
            var url = args.Argument as Uri;
            var request = (HttpWebRequest) WebRequest.Create(url);

            var res = request.BeginGetResponse(null, null);
            while (!res.IsCompleted)
            {
                if (worker.CancellationPending)
                {
                    return;
                }
            }

            var response = request.EndGetResponse(res) as HttpWebResponse;
            args.Result = new DonatorResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    args.Result = JsonSerializer.DeserializeFromReader<DonatorResponse>(reader);
                }
            }
        }
    }
}