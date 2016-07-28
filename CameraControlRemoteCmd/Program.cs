using System;

namespace CameraControlRemoteCmd
{
    class Program
    {
        /* all of the input parameters are passed to the GUI - help is in CommandProcessor.cs */

        static void Main(string[] args)
        {
            int rc = 0;

            Console.WriteLine(String.Format("digiCamControl remote command line utility ({0}, {1}) running\n", ApplicationInformation.ExecutingAssemblyVersion, ApplicationInformation.CompileDate));

            try
            {
                CommandProcessor processor = new CommandProcessor();
                rc = processor.Parse(args);
            }
            catch (Exception ex)
            {
                /* Should never happen as the likely errors are caught in Parse() and return here via rc */
                Console.WriteLine(String.Format("Unusual Error: {0}\n{1}", ex.Message, ex.StackTrace));
                rc = -2;
            }
            System.Environment.Exit(rc);
        }
    }
}
