using System;
using System.IO;
using System.Reflection;
using FFBardMusicPlayer;

namespace FFBardMusicPlayerInit {

	class BootLoader {

        [STAThread]
        static void Main(string[] args)
        {
            
            #if LOCAL
            var dlls = Directory.GetFiles("Lib", "*.dll");
            Type bmpProgram = null;
            foreach (var dll in dlls)
            {
                if (Path.GetFileName(dll).ToLower().Equals("ffbardmusicplayerinternal.dll")) bmpProgram = Assembly.LoadFrom(dll).GetType("FFBardMusicPlayer.Program");
                else Assembly.LoadFrom(dll);
            }
            dynamic main = Activator.CreateInstance(bmpProgram ?? throw new InvalidOperationException("FFBardMusicPlayerInternal.dll"));
            main.StartUp(55);

            #else 

            #endif
		}
	}
}
