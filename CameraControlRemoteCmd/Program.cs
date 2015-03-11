using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraControlRemoteCmd
{
    class Program
    {
        // set aperture 6.4
        // set camera 124578965
        // capture

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                PrintHelp();
                return;
            }
            try
            {
                CommandProcessor processor = new CommandProcessor();
                processor.Parse(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace );
            }
        }

        private static void PrintHelp()
        {
            
        }
    }
}
