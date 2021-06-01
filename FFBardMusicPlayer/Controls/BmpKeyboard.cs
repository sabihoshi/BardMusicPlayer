﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using System.Drawing.Drawing2D;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpKeyboard : UserControl
    {
        private KeyboardData keyboardData = new KeyboardData();
        private int lowFreq = 0;
        private int highFreq = 0;
        private int maxFreq = 0;
        private SolidBrush fgContrastBrush = new SolidBrush(Color.FromArgb(255, 30, 30, 30));
        private SolidBrush bgContrastBrush = new SolidBrush(Color.FromArgb(100, 200, 200, 200));
        private Font textFont = new Font("Segoe UI", 8);
        private Font textLargeFont = new Font("Segoe UI", 12);

        private StringFormat centerTextFormat = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Alignment     = StringAlignment.Center
        };

        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Refresh();
            }
        }

        private string overrideText;

        public string OverrideText
        {
            get => overrideText;
            set
            {
                overrideText = value;
                Refresh();
            }
        }

        public BmpKeyboard()
        {
            InitializeComponent();

            DoubleBuffered = true;
            ResizeRedraw   = true;
        }

        public void UpdateFrequency(List<int> notes)
        {
            var list = keyboardData.letterList;
            var notesAvailable = 0;
            var notesInRange = 0;

            lowFreq  = 0;
            highFreq = 0;

            foreach (var key in list.Keys)
            {
                list[key].frequency = 0;
            }

            foreach (var note in notes)
            {
                notesAvailable++;
                var perfKey = FFXIVKeybindDat.NoteByteToPerformanceKey((byte) note);
                if (list.ContainsKey(perfKey))
                {
                    notesInRange++;
                    list[perfKey].frequency++;
                }
                else
                {
                    if (note <= 0)
                    {
                        lowFreq++;
                    }

                    if (note >= 38)
                    {
                        highFreq++;
                    }
                }
            }

            Text = string.Empty;
            if (notesAvailable == 0)
            {
                Text = "No notes playing on this track.";
            }
            else if (notesInRange == 0)
            {
                Text = "All notes out of range.";
            }

            Refresh();
        }

        public void UpdateNoteKeys(FFXIVKeybindDat hotkeys)
        {
            var list = keyboardData.letterList;
            foreach (var key in list.Keys)
            {
                list[key].key = string.Empty;

                var pk = FFXIVKeybindDat.NoteKeyToPerformanceKey(key);
                if (!string.IsNullOrEmpty(pk))
                {
                    list[key].key = hotkeys[pk].ToString();
                }
            }

            Refresh();
        }

        private void DrawKey(KeyboardUiLetter letter, PaintEventArgs e)
        {
            var noteKey = string.Empty;
            if (keyboardData.letterList.ContainsValue(letter))
            {
                foreach (var k in keyboardData.letterList.Keys)
                {
                    if (keyboardData.letterList[k].Equals(letter))
                    {
                        noteKey = k;
                        continue;
                    }
                }
            }

            if (string.IsNullOrEmpty(noteKey))
            {
                return;
            }

            var drawKey = letter.key; // k = letter.key

            // Determine colors
            var keyBrush = new SolidBrush(Color.Transparent);
            if (letter.sup)
            {
                keyBrush.Color = Enabled ? ForeColor : Color.FromArgb(50, 50, 50);
            }
            else
            {
                keyBrush.Color = Enabled ? BackColor : Color.FromArgb(180, 180, 180);
            }

            // Determine region
            var ww = Width / (float) ((letter.sup ? 26 : 22) - 1);
            var keySize = new SizeF(ww, letter.y * Height);
            var keyPosition = new PointF(letter.x * Width - keySize.Width / 2f, 0f);
            var keyRect = new RectangleF(keyPosition, keySize);

            // Draw color
            if (letter.sup)
            {
                e.Graphics.FillRectangle(keyBrush, keyRect);
            }

            // Set clipping and such
            var clip = e.Graphics.Clip;
            e.Graphics.SetClip(keyRect, CombineMode.Replace);

            // Draw key and frequency
            var keyX = keyRect.X + keyRect.Width / 2f;
            var keyY = keyRect.Y + keyRect.Height - 8f;
            if (letter.frequency > 0)
            {
                var freq = letter.frequency > 0 ? letter.frequency + 60 : 0;
                var size = freq / 260f;
                if (size > 1f)
                {
                    size = 1f;
                }

                var freqCol = letter.sup ? Color.Yellow : Color.Red;
                var freqBrush = new SolidBrush(Color.FromArgb(50 + (int) (size * 150), freqCol));
                e.Graphics.FillCircle(freqBrush, keyX, letter.sup ? keyRect.Y : keyRect.Y + keyRect.Height,
                    size * Height);
            }

            // Write note
            if (true)
            {
                var noteKeySize = e.Graphics.MeasureString(drawKey, textFont);
                var noteKeyPos = new PointF(keyX - noteKeySize.Width / 2, keyY - noteKeySize.Height / 2);
                e.Graphics.FillRectangle(bgContrastBrush, new RectangleF(noteKeyPos, noteKeySize));
                e.Graphics.DrawString(drawKey, textFont, fgContrastBrush, keyX, keyY, centerTextFormat);
            }

            // Draw line
            if (!letter.sup && !noteKey.Equals("C+2"))
            {
                var p1 = new PointF(keyRect.X + keyRect.Width - 2, keyRect.Y);
                var p2 = new PointF(keyRect.X + keyRect.Width - 2, keyRect.Y + keyRect.Height);
                e.Graphics.DrawLine(new Pen(fgContrastBrush), p1, p2);
            }

            e.Graphics.SetClip(clip, CombineMode.Replace);

            if (letter.sup)
            {
                e.Graphics.DrawRectangle(new Pen(fgContrastBrush), Rectangle.Round(keyRect));
            }
        }

        private void KeyboardControl_Paint(object sender, PaintEventArgs e)
        {
            var list = keyboardData.letterList;

            // Get max frequency
            foreach (var key in list.Keys)
            {
                if (list.ContainsKey(key))
                {
                    var letter = keyboardData.letterList[key];
                    if (letter.frequency > maxFreq)
                    {
                        maxFreq = letter.frequency;
                    }
                }
            }

            foreach (var key in list.Keys)
            {
                if (list.ContainsKey(key))
                {
                    var letter = keyboardData.letterList[key];
                    if (!letter.sup)
                    {
                        DrawKey(letter, e);
                    }
                }
            }

            foreach (var key in list.Keys)
            {
                if (list.ContainsKey(key))
                {
                    var letter = keyboardData.letterList[key];
                    if (letter.sup)
                    {
                        DrawKey(letter, e);
                    }
                }
            }

            //SolidBrush freqBrush = new SolidBrush(Color.FromArgb(120, Color.Red));
            if (lowFreq > 0)
            {
                var f = (lowFreq / 1024f).Clamp(0.01f, 1.0f);
                var freqSize = new SizeF(f * (Width / 4), Height);
                var freqRect = new RectangleF(new PointF(0, 0), freqSize);
                var lgrad = new LinearGradientBrush(freqRect, Color.Red, Color.Transparent,
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(lgrad, freqRect);
            }

            if (highFreq > 0)
            {
                var f = (highFreq / 1024f).Clamp(0.01f, 1.0f);
                var freqSize = new SizeF(f * (Width / 4), Height);
                var freqRect = new RectangleF(new PointF(Width - freqSize.Width, 0), freqSize);
                var lgrad = new LinearGradientBrush(freqRect, Color.Transparent, Color.Red,
                    LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(lgrad, freqRect);
            }

            var rect = new RectangleF(0, 0, Width, Height);
            if (Enabled)
            {
                var rectPen = new Pen(Color.Black);
                e.Graphics.DrawRectangle(rectPen, new Rectangle(0, 0, Width - 1, Height - 1));
            }

            if (true)
            {
                // Fade out if it's disabled or text is showing
                if (!string.IsNullOrEmpty(Text) || !Enabled)
                {
                    var disabledBrush = new SolidBrush(Color.FromArgb(120, Color.White));
                    e.Graphics.FillRectangle(disabledBrush, rect);
                }

                if (!string.IsNullOrEmpty(overrideText))
                {
                    var center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    var textSize = e.Graphics.MeasureString(overrideText, textLargeFont);
                    var textPos = new PointF(center.X - textSize.Width / 2, center.Y - textSize.Height / 2);

                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(80, 30, 30)),
                        new RectangleF(textPos, textSize));
                    e.Graphics.DrawString(overrideText, textLargeFont, new SolidBrush(Color.White), Width / 2,
                        Height / 2, centerTextFormat);
                }
                // Show custom message
                else if (!string.IsNullOrEmpty(Text))
                {
                    var center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    var textSize = e.Graphics.MeasureString(Text, textLargeFont);
                    var textPos = new PointF(center.X - textSize.Width / 2, center.Y - textSize.Height / 2);

                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), new RectangleF(textPos, textSize));
                    e.Graphics.DrawString(Text, textLargeFont, new SolidBrush(Color.White), Width / 2, Height / 2,
                        centerTextFormat);
                }
            }
        }
    }

    public class KeyboardUiLetter
    {
        public int frequency = 0;
        public string key = string.Empty;
        public float x;
        public float y;
        public bool sup;

        public KeyboardUiLetter(float xx, float yy, bool s = false)
        {
            x   = xx;
            y   = yy;
            sup = s;
        }
    }

    public class KeyboardLetterList : Dictionary<string, KeyboardUiLetter>
    {
    }

    public class KeyboardData
    {
        public KeyboardLetterList letterList = new KeyboardLetterList();

        public KeyboardData()
        {
            string[] mainKeys =
            {
                "C-1", "D-1", "E-1", "F-1", "G-1", "A-1", "B-1",
                "C", "D", "E", "F", "G", "A", "B",
                "C+1", "D+1", "E+1", "F+1", "G+1", "A+1", "B+1", "C+2"
            };
            string[] supKeys =
            {
                "C#-1", "Eb-1", "F#-1", "G#-1", "Bb-1",
                "C#", "Eb", "F#", "G#", "Bb",
                "C#+1", "Eb+1", "F#+1", "G#+1", "Bb+1"
            };
            var nk = 22;
            var ww = 1f / nk;
            for (var i = 0; i < nk; i++)
            {
                letterList.Add(mainKeys[i], new KeyboardUiLetter(i * ww + ww / 2f, 1.0f, false));
            }

            nk = 15;
            float add = 0;
            for (var i = 0; i < nk; i++)
            {
                if (i % 5 == 2 || i % 5 == 0)
                {
                    add += ww;
                }

                letterList.Add(supKeys[i], new KeyboardUiLetter(i * ww + add, 0.7f, true));
            }
        }
    }
}