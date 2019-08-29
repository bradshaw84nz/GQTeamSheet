using System;
using System.IO;

namespace GQTSS
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateFromFile("<Filename>", "Round 4 - 31st August 2019");
        }
        private static void GenerateFromFile(string fileName,
                                             string dateField)
        {
            var ts = new TeamsheetGenerator();
            string[] fileContent;
            try
            {
                fileContent = File.ReadAllLines("/Users/chris/Downloads/revolutionise-GridironQueenslandInc-Signon-Sheet (48).csv");
                
                ts.GenerateTeamsheet(fileContent,
                                    dateField);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
