using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoryRazorParser.Core;
using InMemoryRazorParsing.Tools;

namespace InMemoryRazorParsing
{
    class Program
    {
        static void Main(string[] args)
        {


            var parser = new RazorParser();

            parser.Model = GetSampleModel();


            Console.Write(parser.Parse(ResourceFilesReader.GetResourceFileContent(typeof(Program), "InMemoryRazorParser.html")));
            Console.WriteLine();

            Console.ReadLine();
        }


        // the model can be anything, its dynamic !!! 
        static CustomerContainer GetSampleModel()
        {
            var retVal = new CustomerContainer();

            retVal.Name = "Mr Smith";
            retVal.IsEnabled = true;

            retVal.Customers.Add(new Customer() { ID = 1, Name = "Bruce Wayne", Telephone = "0800 BATMAN" });
            retVal.Customers.Add(new Customer() { ID = 2, Name = "Bruce Banner", Telephone = "0800 HULK" });
            retVal.Customers.Add(new Customer() { ID = 3, Name = "Clark Kent", Telephone = "0800 SUPERMAN" });
            retVal.Customers.Add(new Customer() { ID = 4, Name = "Tony Stark", Telephone = "0800 IRONMAN" });
            retVal.Customers.Add(new Customer() { ID = 5, Name = "Eric Wimp", Telephone = "0800 BANNA MAN" });


            return retVal;
        }
    }
}

