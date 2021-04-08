using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace DNSCache
{
    class Cache
    {
        private Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();
        private Dictionary<string, int> ttls = new Dictionary<string, int>();
        private Timer cleaner;

        public Cache()
        {
            ReadCache();
            cleaner = new Timer(2000);
            cleaner.Elapsed += Cleaner_Elapsed;
            cleaner.Start();
        }

        public void ReadCache()
        {
            if (!Directory.Exists("Cache"))
                return;
            var time = long.Parse(File.ReadAllLines("Cache/time.txt")[0]);
            var oldDateTime = DateTime.FromFileTime(time);
            var now = DateTime.Now;

            foreach(var file in Directory.GetFiles("Cache/Info"))
            {
                cache.Add((new FileInfo(file).Name).Split(".")[0], File.ReadAllBytes(file));
            }

            foreach (var file in Directory.GetFiles("Cache/ttls"))
            {
                ttls.Add((new FileInfo(file).Name).Split(".")[0], int.Parse(File.ReadAllLines(file)[0]));
            }
            clean(now, oldDateTime);

        }

        public void clean(DateTime now, DateTime old)
        {
            foreach (var key in cache.Keys)
            {
                ttls[key] -= (int) ((now - old).TotalSeconds);
                if (ttls[key] <= 0)
                {
                    cache.Remove(key);
                    ttls.Remove(key);
                }
            }
        }

        public void StopWork()
        {
            cleaner.Stop();
            if (Directory.Exists("Cache"))
            {
                DirectoryInfo dirInfo = new DirectoryInfo("Cache");
                dirInfo.Delete(true);
            }
            DirectoryInfo dir = new DirectoryInfo("Cache");
            dir.Create();
            dir = new DirectoryInfo("Cache/Info");
            dir.Create();
            dir = new DirectoryInfo("Cache/ttls");
            dir.Create();

            foreach(var key in cache.Keys)
            {
                File.WriteAllBytes("Cache/Info/" + key + ".txt", cache[key]);
                File.WriteAllText("Cache/ttls/" + key + ".txt", ttls[key].ToString());
            }
            File.WriteAllText("Cache/time.txt", DateTime.Now.ToFileTime() + "");
        }

        private void Cleaner_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach(var key in ttls.Keys)
            {
                ttls[key] -= 2;
                if (ttls[key] <= 0)
                {
                    cache.Remove(key);
                    ttls.Remove(key);
                }
            }
        }

        private string getKey(byte[] key)
        {
            var strKeys = key.Select(x => x + "").ToArray();
            return String.Join("", strKeys);
        }

        public bool Contains(byte[] hostAndType)
        {
            var result = cache.ContainsKey(getKey(hostAndType));
            return result;
        }

        public byte[] getAnswer(byte[] hostAndType)
        {
            return cache[getKey(hostAndType)];
        }

        public void add(byte[] hostAndType, byte[] data, int ttl)
        {
            cache.Add(getKey(hostAndType), data);
            ttls.Add(getKey(hostAndType), ttl);
        }
    }
}
