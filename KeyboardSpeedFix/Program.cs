using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeyboardSpeedFix
{
    class Program
    {
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDWININICHANGE = 0x02;
        private const uint SPI_SETKEYBOARDSPEED = 0x000B;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint action, uint param, uint vparam, uint vinit);

        static void Main(string[] args)
        {
            var updateVal = SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE;
            var result = SystemParametersInfo(SPI_SETKEYBOARDSPEED, 28, 0, updateVal);
            Console.WriteLine(result ? "Keyboard speed updated!" : "Failed to update the keyboard speed.");

            Thread.Sleep(2000);
        }
    }
}
