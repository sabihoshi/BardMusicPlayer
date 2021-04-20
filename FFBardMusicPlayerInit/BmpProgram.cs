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
                if (!(File.Exists(localPath + @"FFBardMusicPlayerInternal.exe") && "FA66E7F67CD2E04439D6F46B99504EE7E8EDC096FBBBD485541BB2B26CB6A1DF".ToLower().Equals(Sha256.GetChecksum(localPath + @"FFBardMusicPlayerInternal.exe").ToLower())))
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
