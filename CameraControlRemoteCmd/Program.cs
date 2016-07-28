using System;

namespace CameraControlRemoteCmd
{
    class Program
    {
        // set aperture 6.4
        // set camera 124578965
        // capture

        static void Main(string[] args)
        {

            Console.WriteLine(String.Format("digiCamControl remote command line utility ({0}, {1}) running\n", ApplicationInformation.ExecutingAssemblyVersion, ApplicationInformation.CompileDate));

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
    }
}
