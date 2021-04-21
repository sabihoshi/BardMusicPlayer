using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace FFBardMusicPlayer
{
    public class Program
    {
        internal const string downloadPath = @"https://dl.bardmusicplayer.com/bmp/legacy/lib/";
        internal static string localPath = Application.CommonAppDataPath + @"\";
        public void StartUp(int memoryVersion, string titleVersion, string messageText)
        {
            using (var webClient = new WebClient())
            {
                if (!(File.Exists(localPath + @"FFBardMusicPlayerInternal.exe") && "557D27116764D5D2F60C72DB77AE7AE0D374B6F7B06B0D2E08AAB92FFDBAF4B2".ToLower().Equals(Sha256.GetChecksum(localPath + @"FFBardMusicPlayerInternal.exe").ToLower())))
                    webClient.DownloadFile(new Uri(downloadPath + @"FFBardMusicPlayerInternal.exe"), localPath + @"FFBardMusicPlayerInternal.exe");
                
                ProcessStartInfo startInfo = new ProcessStartInfo(localPath + @"FFBardMusicPlayerInternal.exe")
                {
                    Arguments = memoryVersion + " \"" + titleVersion + "\" \"" + messageText + "\""
                };
                Process.Start(startInfo);
            }
        }
    }
}
