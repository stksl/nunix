using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Nunix.Core;
using Nunix.IO;
using static Nunix.Core.VMConsts;

namespace Nunix.Init;
// just a simple initializer
internal class Program
{
    public const string Info =
    "\t\tnunix VM \n Please configure the \"../data/options.json\" file and hit enter to start the initialization";
    public static async Task Main()
    {
        Console.WriteLine(Info);

        Console.Write("Do you want to boot up Nunix VM? (y/n): ");
        if (Console.ReadKey().KeyChar.ToString().ToLower() != "y")
        {
            Console.WriteLine("exiting...");
            return;
        }

        BootUp();

        Console.WriteLine("Starting the VM...");

    }
    private static void BootUp()
    {
        ConfigOptions opt =
            JsonConvert.DeserializeObject<ConfigOptions>(File.ReadAllText("../data/options.json"))!;

        if (!opt.empty)
        {

            return;
        }
        if (opt.vramsize < MIN_RAM || opt.vramsize > MAX_RAM)
        {
            throw new ArgumentOutOfRangeException($"Unable to set used RAM to {opt.vramsize}");
        }
        if (opt.vglobalsize < MIN_STORAGE)
        {
            throw new ArgumentOutOfRangeException($"Minimal storage size is {MIN_STORAGE}");
        }
    }
}

/* 
    vramsize - RAM size in bytes, single allocation for the entire VM
    vglobalsize - hard drive storage size in bytes (single file)
    empty - false for inizialized VM, true for VM to be initialized.
*/
public sealed record ConfigOptions(long vramsize, long vglobalsize, bool empty);