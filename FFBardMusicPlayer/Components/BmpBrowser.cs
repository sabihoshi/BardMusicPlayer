using FFBardMusicCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Components
{
    public partial class BmpBrowser : ListBox
    {
        #region Midi classes

        public class MidiFileName
        {
            private string fileName = string.Empty;

            public string FileName => fileName;

            public string ShortFileName => Path.GetFileName(fileName);

            public string TinyFileName => Path.GetFileNameWithoutExtension(fileName);

            public string RelativeFileName
            {
                get
                {
                    var sp = Properties.Settings.Default.SongDirectory;
                    var file1 = Path.GetFullPath(fileName);
                    var path2 = Path.GetFullPath(sp);
                    if (file1.IndexOf(path2) != 0)
                    {
                        return file1;
                    }

                    return file1.Remove(0, path2.Length - sp.Length);
                }
            }

            public MidiFileName() { }

            public MidiFileName(string f) { fileName = f; }

            // Return the full relative path
            public string RelativePath => Path.GetDirectoryName(RelativeFileName);

            // Return a compressed version of the path
            public string CompressedPath
            {
                get
                {
                    var path = RelativePath;
                    var paths = path.Split(Path.DirectorySeparatorChar).ToList();
                    if (paths.Count == 1)
                    {
                        return paths.First();
                    }

                    path =  "../".Repeat(paths.Count - 1);
                    path += paths.Last();
                    return path;
                }
            }

            public int PathDepth
            {
                get
                {
                    var path = Path.GetDirectoryName(RelativeFileName);
                    var paths = path.Split(Path.DirectorySeparatorChar).ToList();
                    return paths.Count - 1;
                }
            }
        }

        public class MidiFile
        {
            private MidiFileName fileName = new MidiFileName();

            public MidiFileName FileName => fileName;

            private bool enabled = true;

            public bool Enabled
            {
                get => enabled;
                set => enabled = value;
            }

            public MidiFile(string f, bool e = true)
            {
                fileName = new MidiFileName(f);
                enabled  = e;
            }
        }

        public class MidiList : List<MidiFile>
        {
        }

        private MidiList midis = new MidiList();

        public MidiList List => midis;

        private string filenameFilter = string.Empty;

        public string FilenameFilter
        {
            get => filenameFilter;
            set => filenameFilter = value;
        }

        #endregion

        public EventHandler<BmpMidiEntry> OnMidiFileSelect;
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();

        public BmpBrowser()
        {
            InitializeComponent();
            Setup();
        }

        public BmpBrowser(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Setup();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            EnterFile();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            // nothing needs to happen here, since we've already force loaded the
            // song list in Setup, and the FileSystemWatcher will refresh when
            // the directory of midis has been updated
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (Focused)
            {
                return;
            }

            if (SelectedItem is MidiFile file)
            {
                if (!file.Enabled)
                {
                    return;
                }
            }

            var ft = (int) Math.Round((double) (Height / ItemHeight)) / 2;
            var i = SelectedIndex - ft;
            TopIndex = i.Clamp(0, Items.Count);
        }

        private void Setup()
        {
            var songDir = Properties.Settings.Default.SongDirectory;
            if (Directory.Exists(songDir))
            {
                fileWatcher.Changed += (object sender, FileSystemEventArgs e) => { this.Invoke(t => t.RefreshList()); };
                fileWatcher.Path = songDir;
                fileWatcher.Filter = "*.*";
                fileWatcher.EnableRaisingEvents = true;
            }

            RefreshList();
        }

        private void DrawItemEvent(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }

            if (Items[e.Index] is MidiFile file)
            {
                var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                var myBrush = file.Enabled ? Brushes.Black : Brushes.Gray;
                var myBrush2 = Brushes.Gray;
                Brush sBrush = new SolidBrush(BackColor);
                if (file.Enabled)
                {
                    if (selected)
                    {
                        e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(sBrush, e.Bounds);
                        myBrush2 = sBrush;
                    }
                }

                var sfmt = StringFormat.GenericDefault;
                sfmt.LineAlignment = StringAlignment.Far;

                if (file.Enabled)
                {
                    var path = file.FileName.CompressedPath;
                    var filename = file.FileName.ShortFileName;
                    var fmt = string.Format("└ {0}{1}{2}", path, Path.DirectorySeparatorChar, filename);

                    var lastSlash = fmt.LastIndexOf(Path.DirectorySeparatorChar);
                    if (lastSlash > 0)
                    {
                        var bound = e.Bounds;
                        e.Graphics.DrawString(path, e.Font, myBrush2, bound, sfmt);
                        bound.X += (int) e.Graphics.MeasureString(path, e.Font).Width;
                        e.Graphics.DrawString(filename, e.Font, myBrush, bound, sfmt);
                    }
                }
                else
                {
                    e.Graphics.DrawString(file.FileName.FileName, e.Font, myBrush, e.Bounds, sfmt);
                }
            }
        }

        public bool IsFilenameValid(string path)
        {
            try
            {
                Path.GetFileName(path);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public List<string> GetSongFiles()
        {
            var fileNames = new List<string>();
            var dir = Properties.Settings.Default.SongDirectory;
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.EnumerateFiles(dir, "*.m*", SearchOption.AllDirectories)
                    .Where(s => s.ToLower().EndsWith(".mid") || s.ToLower().EndsWith(".mmsong")))
                {
                    fileNames.Add(file);
                }
            }

            return fileNames;
        }

        public void RefreshList()
        {
            var pathDepth = string.Empty;
            MidiFile singleEntry = null;
            MidiFile focusEntry = null;

            // this function is called so many times. since we're clearing the list each time,
            // the UI element is resetting it's selected index and forcing the selected item to always be 0
            // storing the previously selected index should be sufficient, for now
            var previouslySelectedIndex = SelectedIndex;

            if (!string.IsNullOrEmpty(filenameFilter))
            {
                if (!IsFilenameValid(FilenameFilter))
                {
                    midis.Clear();
                    Items.Clear();
                    return;
                }
                else
                {
                    if (filenameFilter.ToLower().EndsWith(".mid") || filenameFilter.ToLower().EndsWith(".mmsong"))
                    {
                        foreach (MidiFile file in Items)
                        {
                            if (file.Enabled)
                            {
                                if (file.FileName.RelativeFileName == filenameFilter)
                                {
                                    singleEntry = file;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            midis.Clear();
            Items.Clear();

            foreach (var path in GetSongFiles())
            {
                var file = new MidiFile(path);

                if (!string.IsNullOrEmpty(filenameFilter))
                {
                    var f1 = file.FileName.TinyFileName.ToUpper();
                    var f2 = file.FileName.ShortFileName.ToUpper();
                    var f3 = filenameFilter.ToUpper();
                    if (singleEntry != null)
                    {
                        if (file.FileName.RelativeFileName == singleEntry.FileName.RelativeFileName)
                        {
                            focusEntry = file;
                        }
                    }
                    else if (singleEntry == null)
                    {
                        if (!f1.Contains(f3))
                        {
                            continue;
                        }
                    }
                }

                midis.Add(file);

                var pd = file.FileName.PathDepth;
                if (pd > 0)
                {
                    var newPath = file.FileName.CompressedPath;
                    if (newPath != pathDepth)
                    {
                        Items.Add(new MidiFile(newPath, false));
                    }

                    pathDepth = newPath;
                }

                Items.Add(file);
            }

            var c = Items.Count;
            if (c == 0)
            {
                Items.Add(new MidiFile("No Midi files found. Make sure your song files are", false));
                Items.Add(new MidiFile("in a \"songs\" sub-folder next to the executable.", false));
            }

            if (focusEntry != null)
            {
                SelectedItem = focusEntry;
            }
            else
            {
                if (SelectedIndex == -1)
                {
                    if (Items.Count > 0)
                    {
                        // don't allow this to pass the current index limits. this could change, say, when the user is using the search feature
                        SelectedIndex = previouslySelectedIndex < Items.Count
                            ? previouslySelectedIndex
                            : Items.Count - 1;
                    }
                }
            }
        }

        // Own

        public void EnterFile()
        {
            if (SelectedItem != null)
            {
                if (SelectedItem is MidiFile file)
                {
                    if (file.Enabled)
                    {
                        OnMidiFileSelect?.Invoke(this, new BmpMidiEntry(file.FileName.RelativeFileName));
                    }
                }
            }
        }

        public bool SelectFile(string filename)
        {
            MidiFile halfMatch = null;
            MidiFile fullMatch = null;

            var f1 = filename.ToUpper();
            foreach (MidiFile file in Items)
            {
                if (file.Enabled)
                {
                    var f2 = file.FileName.ShortFileName.ToUpper();
                    if (f2.StartsWith(f1) && halfMatch == null)
                    {
                        halfMatch = file;
                    }

                    var f3 = file.FileName.RelativeFileName.ToUpper();
                    if (f3.Equals(f1) && fullMatch == null)
                    {
                        fullMatch = file;
                    }
                }
            }

            if (fullMatch != null)
            {
                this.Invoke(t => t.SelectedItem = fullMatch);
                return true;
            }

            if (halfMatch != null)
            {
                this.Invoke(t => t.SelectedItem = halfMatch);
                return true;
            }

            return false;
        }

        public void NextFile(int step = 1)
        {
            if (Items.Count <= 0)
            {
                return;
            }

            var i = SelectedIndex + step;
            if (i >= Items.Count)
            {
                i = 0;
            }

            if (Items[i] is MidiFile file)
            {
                if (!file.Enabled)
                {
                    if (i < Items.Count - 1)
                    {
                        i++;
                    }
                }
            }

            SelectedIndex = i;
        }

        public void PreviousFile(int step = 1)
        {
            if (Items.Count <= 0)
            {
                return;
            }

            var i = SelectedIndex - step;
            if (i < 0)
            {
                i = Items.Count - 1;
            }

            if (Items[i] is MidiFile file)
            {
                if (!file.Enabled)
                {
                    if (i > 0)
                    {
                        i--;
                    }
                }
            }

            SelectedIndex = i;
        }
    }
}