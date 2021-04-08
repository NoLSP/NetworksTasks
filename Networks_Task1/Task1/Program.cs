using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Task1
{
    class Program
    {
        private static string Trace(string adress)
        {
            string command = "tracert ";
            string parametr = adress;

            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command + parametr);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            
            return result;
        }

        private static string[] ParseRawData(string rawData)
        {
            var strs = rawData.Split("\n")
                .Skip(4)
                .Where(s => s != "\r" && s != "")
                .ToArray();
            strs = strs.Take(strs.Length - 1)
                .Select(s => s.Substring(32))
                .Select(s => s.Substring(0, s.Length - 2))
                .ToArray();
            var strs2 = strs.Where(s => s.IndexOf("[") != -1)
                        .Select(s => s.Substring(s.IndexOf("[") + 1, s.IndexOf("]") - s.IndexOf("[") - 1))
                        .ToArray();
            strs = strs.Where(s => s.IndexOf("[") == -1).ToArray();
            strs = strs.Concat(strs2).ToArray();
            return strs;
        }

        private static string GetWhoisInformation(string whoisServer, string url)
        {
            StringBuilder stringBuilderResult = new StringBuilder();
            TcpClient tcpClinetWhois;
            try
            {
                tcpClinetWhois = new TcpClient(whoisServer, 43);
            }
            catch(Exception)
            {
                return "Not found";
            }
            NetworkStream networkStreamWhois = tcpClinetWhois.GetStream();
            BufferedStream bufferedStreamWhois = new BufferedStream(networkStreamWhois);
            StreamWriter streamWriter = new StreamWriter(bufferedStreamWhois);

            streamWriter.WriteLine(url);
            streamWriter.Flush();

            StreamReader streamReaderReceive = new StreamReader(bufferedStreamWhois);

            while (!streamReaderReceive.EndOfStream)
                stringBuilderResult.AppendLine(streamReaderReceive.ReadLine());

            return stringBuilderResult.ToString();
        }

        private static string GetIPInfo(string ip)
        {
            var curDir = Directory.GetCurrentDirectory();
            var servers = File.ReadAllLines(curDir.Substring(0, curDir.Length - 23) + "WhoIsServers.txt");
            string output = "";
            foreach(var server in servers)
            {
                output = GetWhoisInformation(server, ip);
                if (output.IndexOf("No Object Found") == -1 && output.IndexOf("is not supported") == -1 &&
                    output.IndexOf("you can consult the following sources") == -1 && 
                    output.IndexOf("No Data Found") == -1)
                    return output;
            }
            return "Not found";
        }

        private static string[] ParseIPInfo(string ipInfo)
        { 
            var result = new string[3] { "not found", "not found", "not found" }; // 0 - AS; 1 - Country; 2 - Org
            if (ipInfo.IndexOf("RIPE Database") != -1)
            {
                result[1] = ipInfo.Substring(ipInfo.IndexOf("country:"),
                                             ipInfo.Substring(ipInfo.IndexOf("country")).IndexOf("\r"));
                if (ipInfo.IndexOf("origin") != -1)
                {
                    result[0] = ipInfo.Substring(ipInfo.IndexOf("origin:"),
                                                ipInfo.Substring(ipInfo.IndexOf("origin:")).IndexOf("\r"));
                    ipInfo = ipInfo.Substring(ipInfo.IndexOf("origin"));
                }
                else result[0] = "not found";
                result[2] = ipInfo.Substring(ipInfo.IndexOf("mnt-by:"),
                                             ipInfo.Substring(ipInfo.IndexOf("mnt-by:")).IndexOf("\r"));
            }
            else if (ipInfo.IndexOf("ARIN WHOIS") != -1)
            {
                result[0] = ipInfo.Substring(ipInfo.IndexOf("OriginAS:"),
                                             ipInfo.Substring(ipInfo.IndexOf("OriginAS:")).IndexOf("\r"));
                result[1] = ipInfo.Substring(ipInfo.IndexOf("Country:"),
                                             ipInfo.Substring(ipInfo.IndexOf("Country")).IndexOf("\r"));
                result[2] = ipInfo.Substring(ipInfo.IndexOf("OrgName"),
                                             ipInfo.Substring(ipInfo.IndexOf("OrgName:")).IndexOf("\r"));
            }
            return result;
        }

        static void Main(string[] args)
        {
            Console.Write("Domen or IP-adress: ");
            string rawData = Trace(Console.ReadLine());
            var ips = ParseRawData(rawData);
            Console.WriteLine("№          IP         Info");
            for (var i = 0; i < ips.Length; i++)
            {
                Console.Write(i + "   " + ips[i] + "  ");
                var info = ParseIPInfo(GetIPInfo(ips[i]));
                foreach (var inf in info)
                    Console.Write(inf+", ");
                Console.WriteLine();
            }
            
        }
    }
}
