using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CMS_Revised
{
    public partial class LoadingAnimation : UserControl
    {
        private CancellationTokenSource? _cts;
        private float _angle = 0f;
        private int _spinSpeed = 5; // degrees per frame, lower is slower

        public int SpinSpeed
        {
            get => _spinSpeed;
            set => _spinSpeed = Math.Max(1, value);
        }

        public LoadingAnimation()
        {
            InitializeComponent();
            this.Visible = false; // Start hidden
            CMSLoading.Paint += CMSLoading_Paint;
        }

        public void StartSpin()
        {
            this.Visible = true;
            _cts = new CancellationTokenSource();
            _ = SpinLoop(_cts.Token);
        }

        public void StopSpin()
        {
            _cts?.Cancel();
            this.Visible = false;
        }

        private async Task SpinLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    _angle += _spinSpeed;
                    if (_angle >= 360f) _angle -= 360f;
                    CMSLoading.Invalidate();
                    await Task.Delay(16, token); // ~60 FPS
                }
            }
            catch (TaskCanceledException)
            {
                // Suppress: expected on cancellation
            }
        }

        private void CMSLoading_Paint(object sender, PaintEventArgs e)
        {
            if (CMSLoading.Image == null) return;
            var g = e.Graphics;
            g.TranslateTransform(CMSLoading.Width / 2, CMSLoading.Height / 2);
            g.RotateTransform(_angle);
            g.TranslateTransform(-CMSLoading.Image.Width / 2, -CMSLoading.Image.Height / 2);
            g.DrawImage(CMSLoading.Image, 0, 0);
        }
    }
}