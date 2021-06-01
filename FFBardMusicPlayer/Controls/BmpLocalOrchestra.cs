﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FFBardMusicPlayer.BmpProcessSelect;
using static FFBardMusicPlayer.Controls.BmpPlayer;
using static Sharlayan.Core.Enums.Performance;
using Sanford.Multimedia.Midi;
using Timer = System.Timers.Timer;
using System.Timers;
using Sharlayan.Models.ReadResults;
using Sharlayan.Core;
using System.Diagnostics;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpLocalOrchestra : UserControl
    {
        public EventHandler<bool> onMemoryCheck;

        public class SyncData
        {
            public Dictionary<uint, long> idTimestamp = new Dictionary<uint, long>();
        };

        private BmpSequencer parentSequencer;

        public BmpSequencer Sequencer
        {
            set
            {
                parentSequencer = value;

                UpdatePerformers(value);
                UpdateMemory();
            }
            get => parentSequencer;
        }

        private bool orchestraEnabled = false;

        public bool OrchestraEnabled
        {
            get => orchestraEnabled;
            set => orchestraEnabled = value;
        }

        public BmpLocalOrchestra() { InitializeComponent(); }

        private void StartSyncWorker()
        {
            var syncWorker = new BackgroundWorker();
            syncWorker.WorkerSupportsCancellation =  true;
            syncWorker.DoWork                     += SyncWorker_DoWork;
            syncWorker.RunWorkerCompleted         += SyncWorker_RunWorkerCompleted;

            UpdateMemory();

            var actorIds = new List<uint>();
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null && performer.PerformerEnabled && performer.PerformanceUp)
                {
                    actorIds.Add(performer.actorId);
                }
            }

            syncWorker.RunWorkerAsync(actorIds);

            onMemoryCheck.Invoke(this, true);
        }

        private void SyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var data = e.Result as SyncData;

            var debugDump = new StringBuilder();

            if (Sharlayan.MemoryHandler.Instance.IsAttached)
            {
                if (Sharlayan.Reader.CanGetActors())
                {
                    var actors = Sharlayan.Reader.GetActors();
                    foreach (var kvp in data.idTimestamp)
                    {
                        if (actors.CurrentPCs.ContainsKey(kvp.Key))
                        {
                            var item = actors.CurrentPCs[kvp.Key];
                            debugDump.AppendLine(string.Format("{0} MS {1}", item.Name, kvp.Value));
                        }
                    }
                }
            }

            onMemoryCheck.Invoke(this, false);

            //MessageBox.Show(this.Parent, debugDump.ToString());
            Console.WriteLine(debugDump.ToString());
        }

        // Start memory poll sync

        private void SyncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var actorIds = e.Argument as List<uint>;
            var data = new SyncData();
            var performanceToActor = new Dictionary<uint, uint>();

            if (Sharlayan.MemoryHandler.Instance.IsAttached)
            {
                if (Sharlayan.Reader.CanGetPartyMembers() && Sharlayan.Reader.CanGetActors())
                {
                    var actors = Sharlayan.Reader.GetActors();
                    foreach (var actor in actors.CurrentPCs.Values.ToList())
                    {
                        if (actorIds.Contains(actor.ID))
                        {
                            performanceToActor[actor.PerformanceID / 2] = actor.ID;
                        }
                    }
                }
            }

            if (e.Cancel)
            {
                return;
            }

            PerformanceResult performanceCache = null;
            var perfKeys = performanceToActor.Keys.ToList();

            var msCounter = Stopwatch.StartNew();
            var now = DateTime.Now;

            while (!worker.CancellationPending)
            {
                if (e.Cancel)
                {
                    return;
                }

                if (Sharlayan.MemoryHandler.Instance.IsAttached)
                {
                    if (Sharlayan.Reader.CanGetPerformance())
                    {
                        performanceCache = Sharlayan.Reader.GetPerformance();

                        foreach (var pid in perfKeys)
                        {
                            if (performanceToActor.ContainsKey(pid))
                            {
                                // Check it
                                if (performanceCache.Performances[pid].Animation > 0)
                                {
                                    var aid = performanceToActor[pid];
                                    //data.idTimestamp[aid] = msCounter.ElapsedMilliseconds;
                                    data.idTimestamp[aid] = (long) (DateTime.Now - now).TotalMilliseconds;
                                    performanceToActor.Remove(pid);
                                }
                            }
                        }

                        if (perfKeys.Count != performanceToActor.Keys.Count)
                        {
                            perfKeys = performanceToActor.Keys.ToList();
                        }

                        if (performanceToActor.Keys.Count == 0)
                        {
                            break;
                        }
                    }
                }
            }

            e.Result = data;
        }

        public void PopulateLocalProcesses(List<MultiboxProcess> processes)
        {
            PerformerPanel.Controls.Clear();

            var performers = new List<BmpLocalPerformer>();
            var track = 1;
            foreach (var mp in processes)
            {
                var perf = new BmpLocalPerformer(mp);
                perf.Dock = DockStyle.Top;

                if (mp.hostProcess == true)
                {
                    perf.hostProcess = true;
                    performers.Insert(0, perf);
                }
                else
                {
                    performers.Add(perf);
                }

                track++;
            }

            for (var i = 0; i < performers.Count; i++)
            {
                var perf = performers[i];
                perf.TrackNum = i + 1;
                PerformerPanel.Controls.Add(perf);
            }
        }

        public void UpdateMemory()
        {
            if (Sharlayan.MemoryHandler.Instance.IsAttached)
            {
                if (Sharlayan.Reader.CanGetActors() && Sharlayan.Reader.CanGetPerformance())
                {
                    var performerNames = GetPerformerNames();

                    var pid = -1;
                    var perfs = Sharlayan.Reader.GetPerformance();
                    var ares = Sharlayan.Reader.GetActors();
                    if (ares != null)
                    {
                        foreach (var actor in ares.CurrentPCs.Values.ToList())
                        {
                            if (performerNames.Contains(actor.Name))
                            {
                                var perfId = actor.PerformanceID / 2;
                                if (perfId >= 0 && perfId < 99 && perfs.Performances.ContainsKey(perfId))
                                {
                                    var item = perfs.Performances[perfId];

                                    var perf = FindPerformer(actor.Name, perfId, actor.ID);
                                    if (perf != null)
                                    {
                                        perf.PerformanceUp = item.IsReady();
                                        perf.performanceId = perfId;
                                        perf.actorId       = actor.ID;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void UpdatePerformers(BmpSequencer seq)
        {
            if (seq == null)
            {
                return;
            }

            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null)
                {
                    performer.Sequencer = seq;
                }
            }
        }

        public void PerformerProgress(int prog)
        {
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null)
                {
                    performer.SetProgress(prog);
                }
            }
        }

        public void PerformerPlay(bool play)
        {
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null)
                {
                    performer.Play(play);
                }
            }
        }

        public void PerformerStop()
        {
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null)
                {
                    performer.Stop();
                }
            }
        }

        public List<string> GetPerformerNames()
        {
            var performerNames = new List<string>();
            foreach (BmpLocalPerformer performer in PerformerPanel.Controls)
            {
                performerNames.Add(performer.PerformerName);
            }

            return performerNames;
        }

        public BmpLocalPerformer FindPerformer(string name, uint performerId, uint actorId)
        {
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null)
                {
                    // few things can happen here
                    if (performer.PerformerName == name)
                    {
                        if (performer.performanceId == 0 && performer.actorId == 0)
                        {
                            // here, we've found the performer with the name for the first time
                            // after returning here, the perfId and actorId should be set
                            // so, just return them, i guess
                            return performer;
                        }
                        else if (performer.performanceId == performerId && performer.actorId == actorId)
                        {
                            // we've seen this performer before, and can damn well make sure that
                            // this /is/ the character we want to return
                            return performer;
                        }
                    }
                }
            }

            // all else fails, blame the one person in BMP that decided to give
            // muiltiple characters the exact same name.
            return null;
        }

        private void openInstruments_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null && performer.PerformerEnabled)
                {
                    performer.OpenInstrument();
                }
            }
        }

        private void closeInstruments_Click(object sender, EventArgs e)
        {
            parentSequencer.Pause();
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null && performer.PerformerEnabled)
                {
                    performer.CloseInstrument();
                }
            }
        }

        private void muteAll_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null && performer.PerformerEnabled)
                {
                    performer.ToggleMute();
                }
            }
        }

        private void ensembleCheck_Click(object sender, EventArgs e)
        {
            var openTimer = new Timer
            {
                Interval = 500
            };
            openTimer.Elapsed += delegate(object o, ElapsedEventArgs ev)
            {
                openTimer.Stop();
                openTimer = null;

                foreach (Control ctl in PerformerPanel.Controls)
                {
                    var performer = ctl as BmpLocalPerformer;
                    if (performer != null && performer.PerformerEnabled && !performer.hostProcess)
                    {
                        performer.EnsembleAccept();
                    }
                }
            };
            openTimer.Start();
        }

        private void testC_Click(object sender, EventArgs e)
        {
            //StartSyncWorker();

            foreach (Control ctl in PerformerPanel.Controls)
            {
                var performer = ctl as BmpLocalPerformer;
                if (performer != null && performer.PerformerEnabled)
                {
                    performer.NoteKey("C");
                }
            }
        }
    }
}