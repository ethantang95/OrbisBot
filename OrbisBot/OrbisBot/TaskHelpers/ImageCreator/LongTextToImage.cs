using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.ImageCreator
{
    public static class LongTextToImage
    {
        private const int CHAR_PER_LINE = 140;
        private const int FONT_SIZE = 20;
        private const int FONT_WIDTH_PX = 17;
        private const int FONT_HEIGHT_PX = 30;
        private const int LINE_MARGIN_PX = 4;
        private const int SIDE_MARGIN_PX = 10;

        private const int PAGE_WIDTH = (CHAR_PER_LINE * FONT_WIDTH_PX) + (2 * SIDE_MARGIN_PX);

        public static Image TextToImage(string longText)
        {
            var imageText = SplitLongText(longText);

            var imageHeight = (imageText.Count() * (FONT_HEIGHT_PX + LINE_MARGIN_PX)) + 20;

            var currentHeight = 10;

            var image = new Bitmap(PAGE_WIDTH, imageHeight, PixelFormat.Format24bppRgb);

            using (var graphicsDrawer = Graphics.FromImage(image))
            {
                graphicsDrawer.FillRectangle(Brushes.White, 0, 0, PAGE_WIDTH, imageHeight);
                foreach (var line in imageText)
                {
                    graphicsDrawer.DrawString(line, GenerateFont(), new SolidBrush(Color.Black), GenerateTextBox(currentHeight));

                    currentHeight += FONT_HEIGHT_PX + LINE_MARGIN_PX;
                }

                graphicsDrawer.Flush();
            }

            return image;
        }

        private static IEnumerable<string> SplitLongText(string longText)
        {
            var splitText = longText.Split(new string[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.None);

            var textList = new List<string>();

            foreach (var line in splitText)
            {
                if (line.Length <= CHAR_PER_LINE)
                {
                    textList.Add(line);
                }
                else
                {
                    var lineCopy = line;
                    while (lineCopy.Length > CHAR_PER_LINE)
                    {
                        var partialLine = lineCopy.Substring(0, CHAR_PER_LINE);
                        lineCopy = lineCopy.Substring(CHAR_PER_LINE);
                        textList.Add(partialLine);
                    }
                    textList.Add(lineCopy);
                }
            }

            return textList;
        }

        private static RectangleF GenerateTextBox(int heightLocation)
        {
            var textArea = new RectangleF(SIDE_MARGIN_PX, heightLocation, CHAR_PER_LINE * FONT_WIDTH_PX, FONT_HEIGHT_PX);
            return textArea;
        }

        private static Font GenerateFont()
        {
            var font = new Font("Lucida Console", FONT_SIZE);
            return font;
        }
    }
}
