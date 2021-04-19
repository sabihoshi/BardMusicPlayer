using FFBardMusicPlayer.Forms;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FFBardMusicPlayer {

	 public class Program {

		public static string urlBase = "https://bardmusicplayer.com/";

#if DEBUG
        public static string appBase = Application.StartupPath;
#else
        public static string appBase = Application.CommonAppDataPath;
#endif

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[STAThread]
		public void StartUp(int memoryVersion, string titleVersion, string messageText)
        {
			Sharlayan.Reader.memoryVersion = memoryVersion;

			Application.EnableVisualStyles();

			CultureInfo nonInvariantCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = nonInvariantCulture;
			Application.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("en"));

			if(GetConsoleWindow() != IntPtr.Zero) {
				Console.OutputEncoding = System.Text.Encoding.UTF8;
			}

			BmpMain app = new BmpMain(titleVersion, messageText);
			app.ShowDialog();
		}
	}
}
