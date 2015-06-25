using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{
    public partial class Form1 : Form
    {
        private const int WM_SETCURSOR = 0x0020;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTON = 0x1;
        private bool _shown;
        private Thread _thread;
        private bool _running;

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

                var x = 850;
                var y = 425;
                var lParam = (y << 16) | (x & 0xFFFF);

                PostMessage(p.MainWindowHandle, WM_LBUTTONDOWN, WM_LBUTTON, lParam);
                PostMessage(p.MainWindowHandle, WM_LBUTTONUP, 0, lParam);

                WriteToLog(string.Format("Sent Click To: {0} At x = {1} y = {2}", p.ProcessName, x, y));

                Thread.Sleep(100);
            }

            WriteToLog("Click Thread Stopped!");
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            _shown = true;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr SetCapture(IntPtr hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        private void button1_Click(object sender, EventArgs e)
        {
            if (_running && _thread != null)
            {
                _running = false;
                _thread = null;
                button1.Text = @"Start";
                return;
            }

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

            txtLog.AppendText(string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("G"), msg));
        }
    }
}
