using System;
using System.IO;

namespace GQTSS
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateFromFile("/Users/cbradshaw/Downloads/revolutionise-GridironQueenslandInc-Signon-Sheet.csv", "Round 4 - 31st August 2019");
            GenerateFromFile("/Users/cbradshaw/Downloads/revolutionise-GridironQueenslandInc-Signon-Sheet-2.csv", "Round 4 - 31st August 2019");
            GenerateFromFile("/Users/cbradshaw/Downloads/revolutionise-GridironQueenslandInc-Signon-Sheet-3.csv", "Round 4 - 31st August 2019");
            GenerateFromFile("/Users/cbradshaw/Downloads/revolutionise-GridironQueenslandInc-Signon-Sheet-4.csv", "Round 4 - 31st August 2019");
        }
        private static void GenerateFromFile(string fileName,
                                             string dateField)
        {
            var ts = new TeamsheetGenerator();
            string[] fileContent;
            try
            {
                fileContent = File.ReadAllLines(fileName);
                
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
