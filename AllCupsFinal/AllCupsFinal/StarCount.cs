using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace AllCupsFinal
{
    class StarCount
    {
        static int sensitivity = 150;
        static int radius = 4;

        static bool IsSpace(Pixel pixel)
        {
            Color color = pixel.color;
            if (color.R + color.G + color.B > 500)
            {
                return false;
            }
            if (Math.Abs(color.R - color.G) > 40)
            {
                return false;
            }
            return true;
        }

        static bool More(Color color1, Color color2)
        {
            if (color1.R + color1.G + color1.B > color2.R + color2.G + color2.B + sensitivity)
            {
                return true;
            }
            return false;
        }

        static void Process(Pixel[,] pixels, Pixel pixel)
        {
            int x = pixel.x;
            int y = pixel.y;
            Pixel[] otherPixels = new Pixel[4] { pixels[x + 1, y],
            pixels[x - 1, y],
            pixels[x, y + 1],
            pixels[x, y - 1] };

            foreach (Pixel otherPixel in otherPixels)
            {
                if (!otherPixel.isProcessing && otherPixel.isStar)
                {
                    otherPixel.isProcessing = true;
                    Process(pixels, otherPixel);
                }
            }
            pixel.isProcessed = true;
        }

        static int GetStarsCount(Pixel[,] pixels, int width, int height)
        {
            int stars = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Pixel pixel = pixels[x, y];
                    if (pixel.isStar && (!pixel.isProcessed))
                    {
                        stars++;
                        pixel.isStart = true;
                        Process(pixels, pixel);
                    }
                }
            }

            return stars;
        }

        static bool IsStar(Pixel[,] pixels, Pixel pixel)
        {
            Color thisColor = pixel.color;
            int x = pixel.x;
            int y = pixel.y;
            Pixel[] otherPixels = new Pixel[4] { pixels[x + radius, y],
            pixels[x - radius, y],
            pixels[x, y + radius],
            pixels[x, y - radius] };

            foreach (Pixel otherPixel in otherPixels)
            {
                if (!otherPixel.isSpace)
                {
                    return false;
                }
                if (!More(thisColor, otherPixel.color))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetResult(string path, bool createImage)
        {
            Image image = FileSystem.ReadImage(path);
            if (image == null)
            {
                Console.WriteLine("File dont exits");
                return 0;
            }

            int height = image.Height;
            int width = image.Width;
            Pixel[,] pixels = new Pixel[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixels[x, y] = new Pixel(image, x, y);
                    pixels[x, y].isSpace = IsSpace(pixels[x, y]);
                }
            }

            for (int x = radius; x < width - radius; x++)
            {
                for (int y = radius; y < height - radius; y++)
                {

                    if (IsStar(pixels, pixels[x, y]))
                    {
                        pixels[x, y].isStar = true;
                    }
                }
            }

            int stars = GetStarsCount(pixels, width, height);

            if (createImage)
            {
                Image result = new Image(width, height);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (pixels[x, y].isStart)
                        {
                            result.SetColor(x, y, Color.Yellow);
                        }
                        else if (pixels[x, y].isStar)
                        {
                            result.SetColor(x, y, Color.White);
                        }
                        else if (!pixels[x, y].isSpace)
                        {
                            result.SetColor(x, y, Color.Green);
                        }
                        else
                        {
                            result.SetColor(x, y, Color.Black);
                        }
                    }
                }

                FileSystem.WriteImage("Resources/debug.png", result);
            }

            return stars;
        }

        public void Main()
        {
            //int star2 = GetResult("Resources/stars/star2.jpg", true);
            //Console.WriteLine(star2);
            //return;

            string[] files = FileSystem.ReadFolder("Resources/stars");
            int count = files.Length;

            string[] filesName = new string[count];
            object[] stars = new object[count];

            for (int i = 0; i < count; i++)
            {
                int star = GetResult(files[i], false);
                stars[i] = star;
                string file = FileSystem.GetName(files[i]);
                filesName[i] = file;
                Console.WriteLine($"{file}: {star}");
            }

            Table table = new Table("file,star_count", filesName, stars);
            FileSystem.WriteCSV("Resources/starsCount.csv", table, ',');
        }
    }

    class Pixel
    {
        public int x;
        public int y;
        public bool isSpace = false;
        public bool isStar = false;
        public bool isStart = false;
        public bool isProcessed = false;
        public bool isProcessing = false;
        public Color color;

        public Pixel(Image image, int _x, int _y)
        {
            color = image.GetColor(_x, _y);
            x = _x;
            y = _y;
        }
    }
}
