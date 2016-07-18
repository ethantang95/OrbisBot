using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using System.Drawing;
using System.Net;
using System.IO;
using OrbisBot.TaskHelpers.AWS;
using OrbisBot.TaskPermissions;
using System.Drawing.Imaging;

namespace OrbisBot.Tasks
{
    class EdgeDrawerTask : MultiInputTaskAbstract
    {
        public EdgeDrawerTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Creates an edge image";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string CommandText()
        {
            return "edge";
        }

        public override bool StateCheckArgs(int state, string[] args)
        {
            return true; //we will check if there's an image in the task component
        }

        public override int StateTaskComponent(int state, string[] args, MessageEventArgs messageSource)
        {
            if (state == 0)
            {
                string imageUrl;
                if (messageSource.Message.Embeds.Length != 0)
                {
                    imageUrl = messageSource.Message.Embeds[0].Url;
                }
                else if (messageSource.Message.Attachments.Length != 0)
                {
                    imageUrl = messageSource.Message.Attachments[0].Url;
                }
                else
                {
                    var result = PublishMessage("There are no images detected from your message, please post a link with an image or upload an image", messageSource);
                    return -1;
                }

                if (!CheckImageExtension(imageUrl))
                {
                    var result = PublishMessage("The item you have uploaded or linked is not an image", messageSource);
                    return -1;
                }

                var webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(imageUrl);

                Image image;

                using (var ms = new MemoryStream(imageBytes))
                {
                    image = Image.FromStream(ms);
                }

                var processedImage = ProcessImage(image);

                var url = AWSS3UploadUtil.UploadImage(processedImage);

                var taskResult = PublishMessage(url, messageSource);

                processedImage.Dispose();

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, blocking: true);
            }
            else
            {
                throw new InvalidOperationException($"Unchecked state of {state} detected");
            }

            return -1;
        }

        public override string StateUsageText(int state)
        {
            if (state == 0)
            {
                return "Please upload an image";
            }
            throw new InvalidOperationException($"Unchecked state of {state} detected");
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            return "Please upload your image";
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }

        private bool CheckImageExtension(string url)
        {
            var isImage = false;

            isImage |= url.Contains(".jpg");
            isImage |= url.Contains(".JPG");
            isImage |= url.Contains(".jpeg");
            isImage |= url.Contains(".JPEG");
            isImage |= url.Contains(".png");
            isImage |= url.Contains(".PNG");

            return isImage;
        }

        private Image ProcessImage(Image image)
        {
            var width = image.Width;
            var height = image.Height;
            var bitmap = new Bitmap(image);

            image.Dispose();

            //we are done using those now

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, blocking: true);//free up some memory

            var greyScaleMini = new double[9];
            var edge = new Edge();
            var edgeMap = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);

            for (var i = 1; i < height - 1; i++)
            {
                //obtain a new set

                for (var k = 0; k < 3; k++)
                {
                    for (var l = 0; l < 3; l++)
                    {
                        greyScaleMini[(k + 1) * l] = CalculateGreyScale(bitmap.GetPixel(l, i + k - 1));
                    }
                }

                for (var j = 1; j < width - 1; j++)
                {
                    edge.Reset();
                    foreach (Direction enumValue in Enum.GetValues(typeof(Direction)))
                    {
                        edge.AddEdge(CalculateEdge(greyScaleMini, enumValue), enumValue);
                    }

                    if (edge.IsEdge)
                    {
                        edgeMap.SetPixel(j, i, System.Drawing.Color.White);
                    }
                    else
                    {
                        edgeMap.SetPixel(j, i, System.Drawing.Color.Black);
                    }
                    //shift it to the left
                    greyScaleMini[0] = greyScaleMini[1];
                    greyScaleMini[3] = greyScaleMini[4];
                    greyScaleMini[6] = greyScaleMini[7];

                    greyScaleMini[1] = greyScaleMini[2];
                    greyScaleMini[4] = greyScaleMini[5];
                    greyScaleMini[7] = greyScaleMini[8];

                    greyScaleMini[2] = CalculateGreyScale(bitmap.GetPixel(j + 1, i - 1));
                    greyScaleMini[5] = CalculateGreyScale(bitmap.GetPixel(j + 1, i));
                    greyScaleMini[8] = CalculateGreyScale(bitmap.GetPixel(j + 1, i + 1));
                }
            }

            return edgeMap;
        }

        private double CalculateGreyScale(System.Drawing.Color pixel)
        {
            return 0.2126 * pixel.R + 0.7152 * pixel.G + 0.0722 * pixel.B;
        }

        private double CalculateEdge(double[] arr, Direction dir)
        {

            //we are using bitmaps... because I am not doing a whole lot of if statements
            int bit = 0x80;
            int dirValue = (int)dir;

            double result = 0;
            for (int i = 0; i < 9; i++)
            {
                if (i == 4) //4 is the middle value
                {
                    continue;
                }
                if ((bit & dirValue) > 0)
                {
                    result += arr[i] * 5;
                }
                else
                {
                    result += arr[i] * -3;
                }
                //left shift 1 bit
                bit = bit >> 1;
            }

            return result;
        }
    }

    internal class Edge
    {
        public double MaxValue { get; set; }
        public Direction MaxDirection { get; set; }
        public bool IsEdge { get; private set; }

        public Edge()
        {
            Reset();
        }

        //set the new edge if it is higher than the current
        public void AddEdge(double value, Direction direction)
        {
            if (value > MaxValue)
            {
                MaxValue = value;
                MaxDirection = direction;
                if (MaxValue > 175)
                {
                    IsEdge = true;
                }
            }
        }

        public void Reset()
        {
            MaxValue = 0;
            MaxDirection = Direction.N;
            IsEdge = false;
        }
    }

    internal enum Direction
    {
        N = 0xE0, //green
        NE = 0x70, //yellow
        E = 0x38, //blue
        SE = 0x1C, //pink
        S = 0x0E, //green
        SW = 0x07, //yellow
        W = 0x83, //blue
        NW = 0xC1 //pink
    };
}
