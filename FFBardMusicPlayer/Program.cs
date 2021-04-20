using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayerInit {

	class BootLoader {

        internal const string downloadPath = @"https://dl.bardmusicplayer.com/bmp/legacy/lib/";
        internal static string localPath = Application.CommonAppDataPath + @"\";
        internal static Lib lib;

        [STAThread]
        static void Main(string[] args)
        {
#if LOCAL
            var dlls = Directory.GetFiles(@"Lib\", "*.dll");
            Type bmpProgram = null;
            foreach (var dll in dlls)
            {
                if (Path.GetFileName(dll).ToLower().Equals("ffbardmusicplayerinternal.dll")) bmpProgram = Assembly.LoadFrom(dll).GetType("FFBardMusicPlayer.Program");
                else Assembly.LoadFrom(dll);
            }
            dynamic main = Activator.CreateInstance(bmpProgram ?? throw new InvalidOperationException("FFBardMusicPlayerInternal.dll"));
            main.StartUp(55, "Bard Music Player 1.3 - Global Beta", "Bard Music Player 1.3 - Global Beta");

#else

            try
            {

                Directory.CreateDirectory(localPath);

                using WebClient webClient = new();

                var json = webClient.DownloadString(downloadPath + "lib.json");

                lib = Lib.Deserialize(json);

                if (lib.expired)
                {
                    MessageBox.Show(
                    lib.expiredtext,
                    "BMP",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                }

                List<Lib.Dll> downloadList = new();

                foreach(var dll in lib.dlls)
                {
                    if (File.Exists(localPath + dll.name) && dll.sha256.ToLower().Equals(Sha256.GetChecksum(localPath + dll.name).ToLower())) continue;
                    downloadList.Add(dll);
                }

                if (downloadList.Count > 0) new BmpUpdate().DownloadUpdates(downloadList, downloadPath, localPath);
                else Load();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Uh oh, something went wrong and BMP is shutting down.\nPlease ask for support in the Discord Server and provide a picture of this error message:\n\n" + exception.Message,
                    "BMP Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            #endif
		}

        internal static void Load()
        {
            try
            {
                Type bmpProgram = null;
                foreach (var dll in lib.dlls)
                {
                    if (dll.name.ToLower().Equals("ffbardmusicplayerinternal.dll"))
                        bmpProgram = Assembly
                            .LoadFrom(localPath + dll.name, Sha256.StringToBytes(dll.sha256), AssemblyHashAlgorithm.SHA256)
                            .GetType("FFBardMusicPlayer.Program");
                    else
                        Assembly.LoadFrom(localPath + dll.name, Sha256.StringToBytes(dll.sha256),
                            AssemblyHashAlgorithm.SHA256);
                }

                var bmpRegion = new BmpRegion
                {
                    lib = lib,
                    bmpProgram = bmpProgram
                };
                bmpRegion.ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Uh oh, something went wrong and BMP is shutting down.\nPlease ask for support in the Discord Server and provide a picture of this error message:\n\n" + exception.Message,
                    "BMP Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
	}
}
