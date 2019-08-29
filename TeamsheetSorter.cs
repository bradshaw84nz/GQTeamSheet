using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GQTSS.Entities;

namespace GQTSS
{
    public class TeamsheetGenerator
    {
        public void GenerateTeamsheet(string[] file, string dateHeading)
        {
            var startOfGame = true;
            var firstTime = true;

            var homeTeam = new Team();
            var awayTeam = new Team();
            homeTeam.PlayerList = new List<Player>();
            awayTeam.PlayerList = new List<Player>();

            foreach (var line in file)
            {
                var splitLine = line.Split(',');

                if (splitLine.First() == "Members")
                {
                    startOfGame = false;

                    if (!firstTime)
                    {
                        CreatePdf(homeTeam);
                        CreatePdf(awayTeam);
                    }
                    firstTime = false;

                    homeTeam = new Team();
                    awayTeam = new Team();
                    homeTeam.PlayerList = new List<Player>();
                    awayTeam.PlayerList = new List<Player>();
                    continue;
                }

                if (splitLine.First().ToString().Contains("SEQ") || splitLine.First().ToString().Contains("NQ"))
                {
                    startOfGame = true;
                    homeTeam.TeamName = splitLine.First().ToString();
                    awayTeam.TeamName = splitLine[2].ToString();
                    homeTeam.GameInfo = dateHeading;
                    awayTeam.GameInfo = dateHeading;
                    continue;
                }

                if (startOfGame == true)
                {
                    string[] player;
                    if (splitLine[0].ToString().Contains("."))
                    {
                        player = splitLine[0].Split('.');
                        homeTeam.PlayerList.Add(AddPlayerEntry(player[0], player[1]));
                    }

                    if (splitLine.Count() >= 2 && splitLine[2].ToString().Contains("."))
                    {
                        player = splitLine[2].Split('.');
                        awayTeam.PlayerList.Add(AddPlayerEntry(player[0], player[1]));
                    }
                }
            }

            CreatePdf(homeTeam);
            CreatePdf(awayTeam);
        }

        private static void CreatePdf(Team homeTeam)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);

                document.SetFont(font);
                document.SetTextAlignment(TextAlignment.CENTER);
                

                ImageData imageData = ImageDataFactory.Create("Images/gq.png");
                Image image = new Image(imageData);
                image.SetMarginLeft(170);
                document.Add(image);

                PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);

                var para = new Paragraph(homeTeam.TeamName)
                                    .SetFont(titleFont).SetFontSize(20).SetUnderline(0.5f, -1.5f)
                                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                                    .SetVerticalAlignment(VerticalAlignment.TOP);

                document.Add(para);

                para = new Paragraph(homeTeam.GameInfo)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER);

                document.Add(para);

                document.Add(new Paragraph(" "));

                Table table = new Table(new float[] { 10, 25, 9, 170 });

                table.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                table.AddCell("Number");
                table.AddCell("Name");
                table.AddCell("Import?");
                table.AddCell("Signature");

                foreach (var player in homeTeam.PlayerList)
                {
                    table.AddCell(player.PlayerNumber);
                    table.AddCell(player.PlayerName);
                    table.AddCell(player.Import);
                    table.AddCell(player.SignatureBox);
                }

                table.AddCell(".");
                table.AddCell("");
                table.AddCell("");
                table.AddCell("");

                table.AddCell(". ");
                table.AddCell(" ");
                table.AddCell(" ");
                table.AddCell(" ");

                table.AddCell(". ");
                table.AddCell(" ");
                table.AddCell(" ");
                table.AddCell(" ");

                table.AddCell(". ");
                table.AddCell(" ");
                table.AddCell(" ");
                table.AddCell(" ");

                document.Add(table);

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));

                document = CreateMvpTable(document);
                document.Close();
                byte[] bytes = memoryStream.ToArray();

                File.WriteAllBytes($@"../{homeTeam.TeamName}.pdf", bytes);

            }
        }

        private static Document CreateMvpTable(Document document)
        {
            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(@"HC must assign one vote for an OPPOSING player in each position group (enter name or number)."));

            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(" "));

            Table table = new Table(new float[] { 15, 20, 5, 15, 20 });

            table.AddCell("MVP Voting Offence");
            table.AddCell("Player Number/Name");
            table.AddCell("");
            table.AddCell("MVP Voting Defense");
            table.AddCell("Player Number/Name");

            table.AddCell("OB (RB/QB)");
            table.AddCell("");
            table.AddCell("");
            table.AddCell("DB");
            table.AddCell("");

            table.AddCell("WR/TE");
            table.AddCell("");
            table.AddCell("");
            table.AddCell("LB");
            table.AddCell("");

            table.AddCell("OL");
            table.AddCell("");
            table.AddCell("");
            table.AddCell("DL");
            table.AddCell("");

            table.AddCell("Special Teams");
            table.AddCell("");
            table.AddCell("");
            table.AddCell("");
            table.AddCell("");

            document.Add(table);

            return document;
        }

        private static Player AddPlayerEntry(string playerNumber, string playerName)
        {
            return new Player
            {
                PlayerName = playerName.TrimStart(' '),
                PlayerNumber = playerNumber,
                Import = "",
                SignatureBox = ""
            };
        }
    }
}
