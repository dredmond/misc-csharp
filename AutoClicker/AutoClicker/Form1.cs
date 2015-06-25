using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

        public Form1()
        {
            InitializeComponent();

            Shown += Form1_Shown;
        }

        void Form1_Shown(object sender, EventArgs e)
        {
            _shown = true;
        }

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        private void button1_Click(object sender, EventArgs e)
        {
            var p = Process.GetProcessesByName("Clicker Heroes").FirstOrDefault();

            if (p == null)
            {
                WriteToLog("Process not found!");
                return;
            }

            var x = 500;
            var y = 500;
            var lParam = (y << 16) | (x & 0xFFFF);
            SendMessage(p.MainWindowHandle, WM_SETCURSOR, p.MainWindowHandle.ToInt32(), lParam);
            var success = SendMessage(p.MainWindowHandle, WM_LBUTTONDOWN, WM_LBUTTON, lParam);
            SendMessage(p.MainWindowHandle, WM_SETCURSOR, p.MainWindowHandle.ToInt32(), lParam);
            success = SendMessage(p.MainWindowHandle, WM_LBUTTONUP, 0, lParam);
            SendMessage(p.MainWindowHandle, WM_SETCURSOR, p.MainWindowHandle.ToInt32(), lParam);

            WriteToLog(string.Format("Process Found: {0}\r\nExecutable: {1}\r\nSuccess: {2}", p.ProcessName, p.MainModule.FileName, success));
        }

        private void WriteToLog(string msg)
        {
            if (!_shown)
                return;

            txtLog.AppendText(string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("G"), msg));
        }

        protected override void WndProc(ref Message m)
        {


            if (m.Msg == WM_LBUTTONDOWN)
            {
                var x = m.LParam.ToInt32() & 0xFFFF;
                var y = (m.LParam.ToInt32() >> 16) & 0xFFFF;

                Console.WriteLine(m.Msg.ToString("X4") + " " + x + " " + y);
            }

            base.WndProc(ref m);
        }
    }
}
