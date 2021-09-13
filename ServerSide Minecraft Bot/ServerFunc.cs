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
            status.Text = "Status: scanning...";
            string host = bunifuTextBox3.Text;
            string min = bunifuTextBox1.Text;
            string max = bunifuTextBox2.Text;
            
            portscanner.ip = host;
            new Thread(() => portscanner.LaunchProcess(host, min, max)).Start();
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
                            status.Text = "Status: done!";
                            bunifuButton1.Enabled = true;
                            portscanner.end_process();
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
            string[] hosts = richTextBox2.Lines;
            int lenght = richTextBox2.Lines.Length;
            bunifuCircleProgress1.Maximum = lenght;
            richTextBox2.Text = "";
            bunifuButton11.Enabled = false;
            richTextBox2.Enabled = false;


            for (int i = 0; i < lenght; i++)
            {
                richTextBox2.Text += Getip(hosts[i]);
                bunifuCircleProgress1.Value = i;
            }
            bunifuCircleProgress1.Value = bunifuCircleProgress1.Maximum;
            bunifuButton11.Enabled = true;
            richTextBox2.Enabled = true;

        }

        private string Getip(string dns)
        {
            try
            {
                WebClient wc = new WebClient();
                string resolved = wc.DownloadString("https://pingmeminecraftbot.000webhostapp.com/ServerSide/dns_resolve.php?dns=" + dns);
                wc.Dispose();
                return resolved + "\n";
            }
            catch {
                return "IDK";
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
            check_Servers();
        }




        private void check_Servers()
        {
            BeginInvoke(new Action(delegate ()
            {
                string lservers = "";
                this.servers.Text.Replace(" ", "");
                int count = this.servers.Lines.Length;
                this.servers.Enabled = false;
                this.check.Enabled = false;
                this.progress.Value = 0;
                this.progress.Maximum = count;

                for (int i = 0; i < count; i++)
                {

                    try
                    {
                        
                        lservers = lservers + serveradd(servers.Lines[i]) + "\r\n";
                     
                    }
                    catch
                    {
                    }
                    BeginInvoke(new Action(delegate ()
                    {
                        this.progress.Value++;
                    }));
                }
                this.servers.Text = lservers;
                this.servers.Enabled = true;
                this.check.Enabled = true;
                MessageBox.Show("Сервера успешно просканированы!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.progress.Value = 0;
            }));
        }






        public void edit_portscanner_textbot(string data)
        {
            BeginInvoke(new Action(() =>
            {
                open_ports.Text = data;
            }));
        }



        private string serveradd(string line)
        {

            string value = ServerFunc.GET("https://api.mcsrvstat.us/2/" + line);
            ServerFunc.statics statics = JsonConvert.DeserializeObject<ServerFunc.statics>(value);
            string result;
            if (!statics.online)
            {
                result = string.Concat(new object[]
                {
                    statics.ip,
                    ':',
                    statics.port.ToString(),
                    " [-]"
                });
            }
            else
            {
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
            }
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

            
            public string map { get; set; }

            public ServerFunc._plugins plugins { get; set; }

            public ServerFunc._mods mods { get; set; }

            // Token: 0x17000021 RID: 33
            public ServerFunc._motd info { get; set; }
        }

        private class _debug
        {
            public bool ping { get; set; }

            public bool query { get; set; }

            public bool srv { get; set; }

            public bool querymismatch { get; set; }

            public bool ipinsrv { get; set; }

            public bool animatedmotd { get; set; }

            public bool proxypipe { get; set; }

            public int cachetime { get; set; }
        }

        private class _motd
        {
            public string[] raw { get; set; }

            public string[] clean { get; set; }

            public string[] html { get; set; }
        }

        private class _mods
        {
            public string[] names { get; set; }

            public string[] raw { get; set; }
        }

        private class _players
        {
            public int online { get; set; }

            public int max { get; set; }

            public string[] list { get; set; }
        }

        private class _plugins
        {
            public string[] names { get; set; }

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
