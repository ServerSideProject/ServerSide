using System;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace ServerSide_Minecraft_Bot
{
    public partial class Bots : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Bots()
        {
            InitializeComponent();
        }

        public static class NativeMethods
        {
            //Winm WindowsSound
            [DllImport("winmm.dll")]
            internal static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);
            [DllImport("winmm.dll")]
            internal static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        }

        private void bunifuHSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {
            uint CurrVol = ushort.MaxValue / 2;
            NativeMethods.waveOutGetVolume(IntPtr.Zero, out CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            int NewVolume = ((ushort.MaxValue / 100) * bunifuHSlider1.Value);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            NativeMethods.waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);

        }

        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuPictureBox1_MouseEnter(object sender, EventArgs e)
        {
            bunifuPictureBox1.Image = Image.FromFile("pictures/close1.png");
        }

        private void bunifuPictureBox1_MouseLeave(object sender, EventArgs e)
        {
            bunifuPictureBox1.Image = Image.FromFile("pictures/close2.png");
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
