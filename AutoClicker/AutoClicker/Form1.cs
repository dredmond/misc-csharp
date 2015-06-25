using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoClicker
{
    public partial class Form1 : Form
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTON = 0x1;
        private bool _shown;
        private Thread _thread;
        private bool _running;
        private int _x;
        private int _y;
        private int _clicksPerSecond;

        public Form1()
        {
            InitializeComponent();

            Shown += Form1_Shown;
        }

        private void ThreadProc()
        {
            WriteToLog("Click Thread Started!");

            while (_running)
            {
                var p = Process.GetProcessesByName("Clicker Heroes").FirstOrDefault();

                if (p == null)
                {
                    WriteToLog("Process not found!");
                    return;
                }

                var lParam = (_y << 16) | (_x & 0xFFFF);

                PostMessage(p.MainWindowHandle, WM_LBUTTONDOWN, WM_LBUTTON, lParam);
                PostMessage(p.MainWindowHandle, WM_LBUTTONUP, 0, lParam);

                WriteToLog(string.Format("Sent Click To: {0} At x = {1} y = {2}", p.ProcessName, _x, _y));

                Thread.Sleep(1000 / _clicksPerSecond);
            }

            WriteToLog("Click Thread Stopped!");
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            _shown = true;
        }

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private void button1_Click(object sender, EventArgs e)
        {
            if (_running && _thread != null)
            {
                _running = false;
                _thread = null;
                txtClicksPerSecond.ReadOnly = false;
                txtX.ReadOnly = false;
                txtY.ReadOnly = false;
                button1.Text = @"Start";
                return;
            }

            var p = Process.GetProcessesByName("Clicker Heroes").FirstOrDefault();

            if (p == null)
            {
                WriteToLog("Process not found!");
                return;
            }

            txtClicksPerSecond.ReadOnly = true;
            txtX.ReadOnly = true;
            txtY.ReadOnly = true;

            _clicksPerSecond = Convert.ToInt32(txtClicksPerSecond.Text);
            _x = Convert.ToInt32(txtX.Text);
            _y = Convert.ToInt32(txtY.Text);

            txtLog.Clear();
            _running = true;
            _thread = new Thread(ThreadProc);
            _thread.Start();
            button1.Text = @"Stop";
        }

        private delegate void WriteLogDelegate(string msg);
        private void WriteToLog(string msg)
        {
            if (InvokeRequired)
            {
                var d = new WriteLogDelegate(WriteToLog);
                Invoke(d, msg);
                return;
            }

            if (!_shown)
                return;

            txtLog.AppendText(string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("hh:mm:ss.ffff tt"), msg));
        }
    }
}
