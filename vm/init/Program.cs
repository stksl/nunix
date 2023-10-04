// just a simple initializer
using Newtonsoft.Json;
using Nunix.Core;
namespace Nunix.Init
{
    internal class Program
    {
        public const string Info =
        "\t\tnunix VM \n Please configure the \"options.json\" file and enter to start the initialization";
        public static void Main()
        {
            Console.WriteLine(Info);

            Console.Write("Begin the installation? (y/n): ");
            if (Console.ReadKey().KeyChar != 'y')
            {
                Console.WriteLine("exiting...");
                return;
            }
            string config = File.ReadAllText("./options.json");

            ConfigOptions options = JsonConvert.DeserializeObject<ConfigOptions>(config)!;

            if (options.vramsize > VMConsts.VramMax || options.vglobalsize > VMConsts.VglobalsizeMax)
            {
                Console.WriteLine("vram or vglobalsize is out of the range.");
                Console.WriteLine(VMConsts.VramMax + " VramMax");
                Console.WriteLine(VMConsts.VglobalsizeMax + " VglobalsizeMax");
                return;
            }
            File.Create("../data/storage.bin", (int)options.vglobalsize, FileOptions.None);
            Console.WriteLine("Successfully initialized");
        }
    }
    /* 
        vramsize - RAM size in bytes, single allocation for the entire VM
        vglobalsize - hard drive storage size in bytes (single file)
    */
    internal record ConfigOptions(long vramsize, long vglobalsize);
}