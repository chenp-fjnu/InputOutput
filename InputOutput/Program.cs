using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using InputOutput.Processer;

namespace InputOutput
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            IProcesser ActiveProcesser = new UserInfoProcesser();
            Console.WriteLine("Enter " + ActiveProcesser.ToString());
            while (true)
            {
                var input = Console.ReadLine();
                var processer = ProcesserFactory.GetProcesser(input);
                if (processer != null)
                {
                    ActiveProcesser = processer;
                    Console.WriteLine("Enter " + processer.ToString());
                }
                else
                {
                    Console.WriteLine(ActiveProcesser.Process("pc45025", input));
                }

            }
        }
    }
}
