using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FFBardMusicPlayer
{
    public class Program
    {
        internal static string localPath = Application.CommonAppDataPath + @"\";
        public void StartUp(int memoryVersion, string titleVersion, string messageText)
        {
            localPath = localPath + @"FFBardMusicPlayerInternal.exe";

            File.WriteAllBytes(localPath, FFBardMusicPlayerInit.Properties.Resources.FFBardMusicPlayerInternal);
            ProcessStartInfo startInfo = new ProcessStartInfo(localPath)
            {
                Arguments = memoryVersion + " \"" + titleVersion + "\" \"" + messageText + "\""
            };
            Process.Start(startInfo);
        }
    }
}
