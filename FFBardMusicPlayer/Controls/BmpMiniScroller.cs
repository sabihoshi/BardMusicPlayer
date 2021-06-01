using System;
using System.Windows.Forms;

namespace FFBardMusicPlayer.Controls
{
    public partial class BmpMiniScroller : UserControl
    {
        public EventHandler<int> OnScroll;
        public EventHandler OnStatusClick;

        private delegate void textDelegate(string text);

        private void setText(string text)
        {
            if (InvokeRequired)
            {
                var d = new textDelegate(setText);
                Invoke(d, new object[] { text });
            }
            else
            {
                Status.Text = text;
            }
        }

        public override string Text
        {
            get => Status.Text;
            set => setText(value);
        }

        public BmpMiniScroller()
        {
            InitializeComponent();

            LeftButton.Click  += LeftRightButton_Click;
            RightButton.Click += LeftRightButton_Click;
            Status.Click      += Status_Click;
        }

        private void LeftRightButton_Click(object sender, EventArgs e)
        {
            var scroll = 50;
            switch (ModifierKeys)
            {
                case Keys.Shift:   scroll = 100; break;
                case Keys.Control: scroll = 10; break;
            }

            scroll *= sender as Button == LeftButton ? -1 : 1;
            OnScroll?.Invoke(this, scroll);

            Status_Click(sender, e);
        }

        private void Status_Click(object sender, EventArgs e) { OnStatusClick?.Invoke(this, e); }
    }
}