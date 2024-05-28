using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;

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
        static async Task Main(string[] args)
        {
            Console.WriteLine($"CLosest IP: {await GetClosestIPToSite("vk.com")}");
        }
    }
}
