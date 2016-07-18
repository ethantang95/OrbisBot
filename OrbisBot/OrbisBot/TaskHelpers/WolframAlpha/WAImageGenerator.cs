using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolframAlphaNET.Objects;

namespace OrbisBot.TaskHelpers.WolframAlpha
{

    sealed class WAImageGenerator
    {
        private const int TITLE_SIZE = 35;
        private const int TITLE_TOP_MARGIN = 10;
        private List<Pod> _pods { get; set; }
        private int _height { get; set; }
        private int _width { get; set; }

        public WAImageGenerator(IEnumerable<Pod> pods)
        {
            _pods = new List<Pod>();
            _height = 0;
            _width = 0;

            foreach (var pod in pods)
            {
                AddPod(pod);
            }
        }

        private void AddPod(Pod pod)
        {
            _height += TITLE_SIZE;
            _pods.Add(pod);
            foreach (var subPod in pod.SubPods)
            {
                if (subPod.Image != null)
                {
                    _height += subPod.Image.Height;
                    _width = subPod.Image.Width > _width ? subPod.Image.Width : _width;
                }
            }
        }

        public Image GenerateImage()
        {
            if (_pods.Count == 0) //no pods, we just return a null
            {
                return null;
            }
            var fullImageBitMap = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
            using (var graphicsDrawer = Graphics.FromImage(fullImageBitMap))
            {
                graphicsDrawer.FillRectangle(Brushes.White, 0, 0, _width, _height);

                int currentHeight = 0;

                foreach (var pod in _pods)
                {
                    graphicsDrawer.DrawString(pod.Title, GenerateFont(), new SolidBrush(Color.Black), GenerateTextBox(currentHeight));
                    currentHeight += TITLE_SIZE;
                    foreach (var subPod in pod.SubPods)
                    {
                        if (subPod.Image != null)
                        {
                            var podImage = GetImageFromUrl(subPod.Image.Src);
                            if (podImage != null)
                            {
                                graphicsDrawer.DrawImage(podImage, 0, currentHeight);
                            }
                            else
                            {
                                graphicsDrawer.DrawString("Image not found", new Font("Arial", 16), new SolidBrush(Color.Black), GenerateTextBox(currentHeight));
                            }
                            currentHeight += subPod.Image.Height;
                        }
                    }
                }

                graphicsDrawer.Flush();
            }

            return fullImageBitMap;
        }

        private RectangleF GenerateTextBox(int heightLocation)
        {
            var textArea = new RectangleF(0, heightLocation + TITLE_TOP_MARGIN, _width, TITLE_SIZE - TITLE_TOP_MARGIN);
            return textArea;
        }

        private Font GenerateFont()
        {
            var font = new Font("Arial", 16, FontStyle.Underline);
            return font;
        }

        private Image GetImageFromUrl(string url)
        {
            try
            {
                var request = System.Net.WebRequest.Create(url);
                var response = request.GetResponse();
                var responseStream = response.GetResponseStream();
                var bitmapResult = new Bitmap(responseStream);
                return bitmapResult;

            }
            catch (System.Net.WebException)
            {
                return null;
            }
        }
    }
}
