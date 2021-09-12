using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace ServerSide_Minecraft_Bot
{
    public partial class ServerFunc : Form
    {
        PortScanner portscanner = new PortScanner();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;


        //Winm WindowsSound
        [DllImport("winmm.dll")]
        internal static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);
        [DllImport("winmm.dll")]
        internal static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]

        


        public static extern bool ReleaseCapture();

        public ServerFunc()
        {
            InitializeComponent();

        }


        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }



        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            bunifuButton1.Enabled = false;
            bunifuLabel2.Text = "Status: scanning...";
            string host = bunifuTextBox3.Text;
            string min = bunifuTextBox1.Text;
            string max = bunifuTextBox2.Text;
            
            portscanner.ip = host;
            Thread t = new Thread(() => portscanner.LaunchProcess(host, min, max));
            t.Start();

            Update();

        }

        async private new void Update()
        {
            await Task.Run(()=>{

                while (portscanner.output == null)
                {
                    Thread.Sleep(200);
                    if (portscanner.output != null)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            open_ports.Text = portscanner.output;
                            bunifuLabel2.Text = "Status: done!";
                            bunifuButton1.Enabled = true;
                        }));

                        break;
                    }
                }
            });
            
        }





        private void button1_Click(object sender, EventArgs e)
        {
            
        }

       

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            portscanner_panel.Visible = true;
            portscanner_panel.Location = new Point(291, 129);
            dnsresolver_panel.Visible = false;
            serverchecker_panel.Visible = false;


        }

        private void bunifuButton6_Click(object sender, EventArgs e)
        {
            dnsresolver_panel.Visible = true;
            dnsresolver_panel.Location = new Point(291, 129);
            portscanner_panel.Visible = false;
            serverchecker_panel.Visible = false;
        }

        private void bunifuButton11_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                string dns = bunifuTextBox4.Text;
                Getip(dns, 2);
            }

            if (checkBox1.Checked == false)
            {
                string dns = bunifuTextBox4.Text;
                Getip(dns, 1);
            }
        }

        private void Getip(string dns, int a)
        {
            if (a == 1)
            {
                WebClient wc = new WebClient();
                string resolved = wc.DownloadString("https://pingmeminecraftbot.000webhostapp.com/ServerSide/dns_resolve.php?dns=" + dns);
                richTextBox2.Text += dns + ":" + resolved + "\n";
            }

            if (a == 2)
            {
                WebClient wc = new WebClient();
                string resolved = wc.DownloadString("https://pingmeminecraftbot.000webhostapp.com/ServerSide/dns_resolve.php?dns=" + dns);
                richTextBox2.Text += resolved + "\n";
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void bunifuButton9_Click(object sender, EventArgs e)
        {
            serverchecker_panel.Visible = true;
            serverchecker_panel.Location = new Point(291, 129);
            portscanner_panel.Visible = false;
            dnsresolver_panel.Visible = false;

        }

        private void bunifuButton12_Click(object sender, EventArgs e)
        {
            string lservers = "";
            this.servers.Text.Replace(" ", "");
            int count = this.servers.Lines.Length;
            this.servers.Enabled = false;
            this.check.Enabled = false;
            this.progress.Value = 0;
            this.progress.Maximum = count;
            new Thread(checked(delegate ()
            {
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        lservers = lservers + this.serveradd(this.servers.Lines[i]) + "\r\n";
                    }
                    catch
                    {
                    }
                    this.BeginInvoke(new Action(delegate ()
                    {
                        this.progress.Value++;
                    }));
                }
                this.BeginInvoke(new Action(delegate ()
                {
                    this.servers.Text = lservers;
                    this.servers.Enabled = true;
                    this.check.Enabled = true;
                    MessageBox.Show("Сервера успешно просканированы!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.progress.Value = 0;
                }));
            })).Start();

        }

        private string serveradd(string line)
        {
            string value = ServerFunc.GET("https://api.mcsrvstat.us/2/" + line);
            ServerFunc.statics statics = JsonConvert.DeserializeObject<ServerFunc.statics>(value);
            string result;
            result = string.Concat(new object[]
                            {
                    statics.ip,
                    ':',
                    statics.port.ToString(),
                    " [+] [",
                    statics.players.online,
                    '/',
                    statics.players.max.ToString(),
                    "] [",
                    statics.version,
                    ']'
                            });
            return result;
        }










        private static string GET(string Url)
        {
            WebRequest webRequest = WebRequest.Create(Url);
            WebResponse response = webRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            return result;
        }

















        private class statics
        {
            // Token: 0x17000013 RID: 19
            // (get) Token: 0x06000031 RID: 49 RVA: 0x000028D0 File Offset: 0x000018D0
            // (set) Token: 0x06000032 RID: 50 RVA: 0x000028E7 File Offset: 0x000018E7
            public bool online { get; set; }

            // Token: 0x17000014 RID: 20
            // (get) Token: 0x06000033 RID: 51 RVA: 0x000028F0 File Offset: 0x000018F0
            // (set) Token: 0x06000034 RID: 52 RVA: 0x00002907 File Offset: 0x00001907
            public string ip { get; set; }

            // Token: 0x17000015 RID: 21
            // (get) Token: 0x06000035 RID: 53 RVA: 0x00002910 File Offset: 0x00001910
            // (set) Token: 0x06000036 RID: 54 RVA: 0x00002927 File Offset: 0x00001927
            public int port { get; set; }

            // Token: 0x17000016 RID: 22
            // (get) Token: 0x06000037 RID: 55 RVA: 0x00002930 File Offset: 0x00001930
            // (set) Token: 0x06000038 RID: 56 RVA: 0x00002947 File Offset: 0x00001947
            public ServerFunc._debug debug { get; set; }

            // Token: 0x17000017 RID: 23
            // (get) Token: 0x06000039 RID: 57 RVA: 0x00002950 File Offset: 0x00001950
            // (set) Token: 0x0600003A RID: 58 RVA: 0x00002967 File Offset: 0x00001967
            public ServerFunc._motd motd { get; set; }

            // Token: 0x17000018 RID: 24
            // (get) Token: 0x0600003B RID: 59 RVA: 0x00002970 File Offset: 0x00001970
            // (set) Token: 0x0600003C RID: 60 RVA: 0x00002987 File Offset: 0x00001987
            public ServerFunc._players players { get; set; }

            // Token: 0x17000019 RID: 25
            // (get) Token: 0x0600003D RID: 61 RVA: 0x00002990 File Offset: 0x00001990
            // (set) Token: 0x0600003E RID: 62 RVA: 0x000029A7 File Offset: 0x000019A7
            public string version { get; set; }

            // Token: 0x1700001A RID: 26
            // (get) Token: 0x0600003F RID: 63 RVA: 0x000029B0 File Offset: 0x000019B0
            // (set) Token: 0x06000040 RID: 64 RVA: 0x000029C7 File Offset: 0x000019C7
            public int protocol { get; set; }

            // Token: 0x1700001B RID: 27
            // (get) Token: 0x06000041 RID: 65 RVA: 0x000029D0 File Offset: 0x000019D0
            // (set) Token: 0x06000042 RID: 66 RVA: 0x000029E7 File Offset: 0x000019E7
            public string hostname { get; set; }

            // Token: 0x1700001C RID: 28
            // (get) Token: 0x06000043 RID: 67 RVA: 0x000029F0 File Offset: 0x000019F0
            // (set) Token: 0x06000044 RID: 68 RVA: 0x00002A07 File Offset: 0x00001A07
            public string icon { get; set; }

            // Token: 0x1700001D RID: 29
            // (get) Token: 0x06000045 RID: 69 RVA: 0x00002A10 File Offset: 0x00001A10
            // (set) Token: 0x06000046 RID: 70 RVA: 0x00002A27 File Offset: 0x00001A27
            public string software { get; set; }

            // Token: 0x1700001E RID: 30
            // (get) Token: 0x06000047 RID: 71 RVA: 0x00002A30 File Offset: 0x00001A30
            // (set) Token: 0x06000048 RID: 72 RVA: 0x00002A47 File Offset: 0x00001A47
            public string map { get; set; }

            // Token: 0x1700001F RID: 31
            // (get) Token: 0x06000049 RID: 73 RVA: 0x00002A50 File Offset: 0x00001A50
            // (set) Token: 0x0600004A RID: 74 RVA: 0x00002A67 File Offset: 0x00001A67
            public ServerFunc._plugins plugins { get; set; }

            // Token: 0x17000020 RID: 32
            // (get) Token: 0x0600004B RID: 75 RVA: 0x00002A70 File Offset: 0x00001A70
            // (set) Token: 0x0600004C RID: 76 RVA: 0x00002A87 File Offset: 0x00001A87
            public ServerFunc._mods mods { get; set; }

            // Token: 0x17000021 RID: 33
            // (get) Token: 0x0600004D RID: 77 RVA: 0x00002A90 File Offset: 0x00001A90
            // (set) Token: 0x0600004E RID: 78 RVA: 0x00002AA7 File Offset: 0x00001AA7
            public ServerFunc._motd info { get; set; }
        }

        private class _debug
        {
            // Token: 0x17000001 RID: 1
            // (get) Token: 0x06000008 RID: 8 RVA: 0x00002668 File Offset: 0x00001668
            // (set) Token: 0x06000009 RID: 9 RVA: 0x0000267F File Offset: 0x0000167F
            public bool ping { get; set; }

            // Token: 0x17000002 RID: 2
            // (get) Token: 0x0600000A RID: 10 RVA: 0x00002688 File Offset: 0x00001688
            // (set) Token: 0x0600000B RID: 11 RVA: 0x0000269F File Offset: 0x0000169F
            public bool query { get; set; }

            // Token: 0x17000003 RID: 3
            // (get) Token: 0x0600000C RID: 12 RVA: 0x000026A8 File Offset: 0x000016A8
            // (set) Token: 0x0600000D RID: 13 RVA: 0x000026BF File Offset: 0x000016BF
            public bool srv { get; set; }

            // Token: 0x17000004 RID: 4
            // (get) Token: 0x0600000E RID: 14 RVA: 0x000026C8 File Offset: 0x000016C8
            // (set) Token: 0x0600000F RID: 15 RVA: 0x000026DF File Offset: 0x000016DF
            public bool querymismatch { get; set; }

            // Token: 0x17000005 RID: 5
            // (get) Token: 0x06000010 RID: 16 RVA: 0x000026E8 File Offset: 0x000016E8
            // (set) Token: 0x06000011 RID: 17 RVA: 0x000026FF File Offset: 0x000016FF
            public bool ipinsrv { get; set; }

            // Token: 0x17000006 RID: 6
            // (get) Token: 0x06000012 RID: 18 RVA: 0x00002708 File Offset: 0x00001708
            // (set) Token: 0x06000013 RID: 19 RVA: 0x0000271F File Offset: 0x0000171F
            public bool animatedmotd { get; set; }

            // Token: 0x17000007 RID: 7
            // (get) Token: 0x06000014 RID: 20 RVA: 0x00002728 File Offset: 0x00001728
            // (set) Token: 0x06000015 RID: 21 RVA: 0x0000273F File Offset: 0x0000173F
            public bool proxypipe { get; set; }

            // Token: 0x17000008 RID: 8
            // (get) Token: 0x06000016 RID: 22 RVA: 0x00002748 File Offset: 0x00001748
            // (set) Token: 0x06000017 RID: 23 RVA: 0x0000275F File Offset: 0x0000175F
            public int cachetime { get; set; }
        }

        private class _motd
        {
            // Token: 0x17000009 RID: 9
            // (get) Token: 0x06000019 RID: 25 RVA: 0x00002770 File Offset: 0x00001770
            // (set) Token: 0x0600001A RID: 26 RVA: 0x00002787 File Offset: 0x00001787
            public string[] raw { get; set; }

            // Token: 0x1700000A RID: 10
            // (get) Token: 0x0600001B RID: 27 RVA: 0x00002790 File Offset: 0x00001790
            // (set) Token: 0x0600001C RID: 28 RVA: 0x000027A7 File Offset: 0x000017A7
            public string[] clean { get; set; }

            // Token: 0x1700000B RID: 11
            // (get) Token: 0x0600001D RID: 29 RVA: 0x000027B0 File Offset: 0x000017B0
            // (set) Token: 0x0600001E RID: 30 RVA: 0x000027C7 File Offset: 0x000017C7
            public string[] html { get; set; }
        }

        private class _mods
        {
            // Token: 0x1700000C RID: 12
            // (get) Token: 0x06000020 RID: 32 RVA: 0x000027D8 File Offset: 0x000017D8
            // (set) Token: 0x06000021 RID: 33 RVA: 0x000027EF File Offset: 0x000017EF
            public string[] names { get; set; }

            // Token: 0x1700000D RID: 13
            // (get) Token: 0x06000022 RID: 34 RVA: 0x000027F8 File Offset: 0x000017F8
            // (set) Token: 0x06000023 RID: 35 RVA: 0x0000280F File Offset: 0x0000180F
            public string[] raw { get; set; }
        }

        private class _players
        {
            // Token: 0x1700000E RID: 14
            // (get) Token: 0x06000025 RID: 37 RVA: 0x00002820 File Offset: 0x00001820
            // (set) Token: 0x06000026 RID: 38 RVA: 0x00002837 File Offset: 0x00001837
            public int online { get; set; }

            // Token: 0x1700000F RID: 15
            // (get) Token: 0x06000027 RID: 39 RVA: 0x00002840 File Offset: 0x00001840
            // (set) Token: 0x06000028 RID: 40 RVA: 0x00002857 File Offset: 0x00001857
            public int max { get; set; }

            // Token: 0x17000010 RID: 16
            // (get) Token: 0x06000029 RID: 41 RVA: 0x00002860 File Offset: 0x00001860
            // (set) Token: 0x0600002A RID: 42 RVA: 0x00002877 File Offset: 0x00001877
            public string[] list { get; set; }
        }

        private class _plugins
        {
            // Token: 0x17000011 RID: 17
            // (get) Token: 0x0600002C RID: 44 RVA: 0x00002888 File Offset: 0x00001888
            // (set) Token: 0x0600002D RID: 45 RVA: 0x0000289F File Offset: 0x0000189F
            public string[] names { get; set; }

            // Token: 0x17000012 RID: 18
            // (get) Token: 0x0600002E RID: 46 RVA: 0x000028A8 File Offset: 0x000018A8
            // (set) Token: 0x0600002F RID: 47 RVA: 0x000028BF File Offset: 0x000018BF
            public string[] raw { get; set; }
        }





        private void ServerFunc_Load(object sender, EventArgs e)
        {
            ChangeVolume();
        }

        private void bunifuButton12_Click_1(object sender, EventArgs e)
        {
        }

        private void open_ports_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void bunifuButton12_Click_2(object sender, EventArgs e)
        {

        }

        private void bunifuHSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {

        }

        private void ChangeVolume()
        {
            uint CurrVol = ushort.MaxValue / 2;
            NativeMethods.waveOutGetVolume(IntPtr.Zero, out CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            int NewVolume = ((ushort.MaxValue / 100) * bunifuHSlider1.Value);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            NativeMethods.waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }

        public static class NativeMethods
        {
            //Winm WindowsSound
            [DllImport("winmm.dll")]
            internal static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);
            [DllImport("winmm.dll")]
            internal static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        }
    }
}
