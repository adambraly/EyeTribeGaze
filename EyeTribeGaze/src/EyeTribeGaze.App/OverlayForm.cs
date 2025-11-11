using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;

namespace EyeTribeGaze.App
{
    public partial class OverlayForm : Form, IGazeListener
    {

        // Make the window click-through
        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x20;
        const int WS_EX_LAYERED = 0x80000;

        [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private readonly Timer _repaintTimer;
        private PointF _gazePx = new PointF(-1000, -1000); // off-screen initially
        private readonly int _dotRadius = 16;



        public OverlayForm()
        {
            // Create a fullscreen borderless overlay.
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Bounds = Screen.PrimaryScreen.Bounds;

            // Make the background transparent and the window layered.
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            Load += OverlayForm_Load;

            DoubleBuffered = true;

            // Repaint frequency at ~60Hz.
            _repaintTimer = new Timer { Interval = 16 };
            _repaintTimer.Tick += (s, e) => Invalidate();
        }

        private void OverlayForm_Load(object sender, EventArgs e)
        {
            // Make the window click-through so that we can interact with games underneath it.
            int exStyle = GetWindowLong(Handle, GWL_EXSTYLE);
            SetWindowLong(Handle, GWL_EXSTYLE, exStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

            // Start the EyeTribe gaze listener.
            var gazeManager = GazeManager.Instance;
            bool running = gazeManager.Activate(GazeManager.ApiVersion.VERSION_1_0);

            if (!running)
            {
                MessageBox.Show("Failed to connect to EyeTribe Server. Is it running?", "EyeTribe", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            // Register this form as a gaze listener.
            gazeManager.AddGazeListener(this);

            // Start the repaint timer.
            _repaintTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_gazePx.X >= 0 && _gazePx.Y >= 0)
            {
                // Draw a circle at the gaze point.
                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                var rect = new RectangleF(_gazePx.X - _dotRadius, _gazePx.Y - _dotRadius, _dotRadius * 2, _dotRadius * 2);

                using (var pen = new Pen(Color.Red, 2))
                    g.DrawEllipse(pen, rect);

                // Optional fill, but can obscure what is under the gase point.
                //using (var brush = new SolidBrush(Color.FromArgb(128, Color.Yellow)))
                //    g.FillEllipse(brush, rect);
            }
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            if (gazeData == null) return;

            // Try using the smoothed "average" gaze point in pixels.
            // Coordinates are in screen pixels when the server was calibrated.
            var gazePoint = gazeData.SmoothedCoordinates;

            // Otherwise use the raw coordinates.
            if (double.IsNaN(gazePoint.X) || double.IsNaN(gazePoint.Y))
                gazePoint = gazeData.RawCoordinates;

            // Update the gaze point.
            _gazePx = new PointF((float)gazePoint.X, (float)gazePoint.Y);

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Clean up the EyeTribe connection.
            try
            {
                var gazeManager = GazeManager.Instance;
                gazeManager.RemoveGazeListener(this);
                gazeManager.Deactivate();
            }
            catch (Exception)
            {
                
            }            
            base.OnFormClosed(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Close the overlay when Escape is pressed.
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
