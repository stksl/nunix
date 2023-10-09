// just a simple initializer
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Nunix.Core;
using Nunix.IO;
namespace Nunix.Init
{
    internal class Program
    {
        public const string Info =
        "\t\tnunix VM \n Please configure the \"../data/options.json\" file and hit enter to start the initialization";
        public static unsafe void Main()
        {
            Console.WriteLine(Info);

            Console.Write("Do you want to boot up Nunix VM? (y/n): ");
            if (Console.ReadKey().KeyChar != 'y')
            {
                Console.WriteLine("exiting...");
                return;
            }

            ConfigOptions opt = 
                JsonConvert.DeserializeObject<ConfigOptions>(File.ReadAllText("../data/options.json"))!;


            if (opt.vramsize < VMConsts.MIN_RAM || opt.vramsize > VMConsts.MAX_RAM) 
            {
                Console.WriteLine($"Unable to set used RAM to {opt.vramsize}");
                return;
            }
            if (opt.vglobalsize < VMConsts.MIN_STORAGE) 
            {
                Console.WriteLine($"Minimal storage size is {VMConsts.MIN_STORAGE}");
                return;
            }

            Console.WriteLine("Starting the VM...");
        }
    }
    /* 
        vramsize - RAM size in bytes, single allocation for the entire VM
        vglobalsize - hard drive storage size in bytes (single file)
    */
    internal record ConfigOptions(long vramsize, long vglobalsize);
}