using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Text;

namespace Asyncronousness
{
    internal class Program
    {
        public static async Task<IPAddress> GetClosestIPToSite(string htmlAddress)
        {
            //get available site addreses to ping them
            var IPAddresses = Dns.GetHostAddresses(htmlAddress, AddressFamily.InterNetwork);
            foreach (var item in IPAddresses)
                Console.WriteLine(item);
            Console.WriteLine("Pinging IPs:");
            var pings = new ConcurrentDictionary<IPAddress, long>();
            //adding available addresses with reply time in dictionary
            foreach (var address in IPAddresses)
            {
                await Task.Run(() =>
                {
                    var ping = new Ping();
                    var reply = ping.Send(address).RoundtripTime;
                    pings.TryAdd(address, reply);
                    Console.WriteLine($"{address} : {reply}");
                });
            }
            var minPing = pings.MinBy(x => x.Value);
            return minPing.Key;
        }
        static async Task PrintStringFromMemory(MemoryStream memoryStream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            StringBuilder sb = new StringBuilder();
            while ((bytesRead = await memoryStream.ReadAsync(buffer)) > 0)
            {
                await Console.Out.WriteAsync(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            }
        }
        static async Task Main(string[] args)
        {
            //Console.WriteLine($"CLosest IP: {await GetClosestIPToSite("vk.com")}");
            var data = Encoding.UTF8.GetBytes("Data form memory stream, written by strings.");
            var memoryStream = new MemoryStream(data);
            await PrintStringFromMemory(memoryStream);
        }
    }
}
