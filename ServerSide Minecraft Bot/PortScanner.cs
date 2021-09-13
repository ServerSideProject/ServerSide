using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerSide_Minecraft_Bot
{
    class PortScanner
    {
        public Process process = new Process();
        public string data;
        public string ip;
        public string status;
        public string hosts;
        public string output = null;


        public void LaunchProcess(string host, string min, string max)
        {
            try
            {
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(process_Exited);
                process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
                process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);

                process.StartInfo.FileName = "core.exe";
                process.StartInfo.Arguments = host + $" -p {min}-{max} --rate 100000 --open --echo> last_scan.txt";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();


                process.Kill();

                process.Dispose();


                hosts = host;
            }
            catch { }
        }

        public void process_Exited(object sender, EventArgs e)
        {
            try
            {
                ServerFunc ip_tools = new ServerFunc();
                status = "finished";
                Console.WriteLine(data);
                data = data.Replace("Discovered open port ", "");
                data = data.Replace("/tcp on ", ":");


                string value = (@"[0-9]{4,5}");


                string filtered = "Open ports:\n";
                MatchCollection openports = Regex.Matches(data, value);

                var result = new List<string>();

                foreach (Match m in openports)
                {
                    result.Add(m.Value);
                    filtered += ip + ":" + m.Value + "\n";
                }

                output = filtered;
                try
                {
                    process.Kill();
                    process.CancelOutputRead();
                    process.CancelErrorRead();
                    process.Dispose();

                    data = null;
                    ip = null;
                    status = null;
                    hosts = null;
                    output = null;
                }
                catch { }
            }
            catch { }
        }

        void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine(e.Data + "\n");
            }
            catch { }

        }

        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                data += e.Data + "\n";
                status = "wait";

                if (data.Contains("100.00% done"))
                {
                    process.Kill();
                    process.CancelOutputRead();
                    process.CancelErrorRead();
                    process.Dispose();
                }
            }
            catch { }

        }

        public void end_process()
        {
            try
            {
                process.CancelOutputRead();
                process.CancelErrorRead();
                process.Close();
                process.Kill();
                process.Dispose();
            }
            catch { }
        }
    }
}
