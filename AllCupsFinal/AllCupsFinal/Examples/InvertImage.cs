using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AllCupsFinal
{
    class InvertImage
    {
        public void InvertFirstImage()
        {
            Image image = FileSystem.ReadImage("Resources/city.jpg");
            int width = image.Width;
            int height = image.Height;
            Console.WriteLine($"{width}, {height}");

            Image result = new Image(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color = image.GetColor(x, y);
                    int r = 255 - color.R;
                    int g = 255 - color.G;
                    int b = 255 - color.B;

                    result.SetColor(x, y, Color.FromArgb(r, g, b));
                }
            }

            FileSystem.WriteImage("Resources/invertCity.png", result);
        }
    }
}
