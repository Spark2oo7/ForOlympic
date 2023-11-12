using System;
using System.Drawing;

namespace NeuralNetworkLibrary
{
    public class FloatCuboid
    {
        public readonly float[,,] values; //[x, y, layer]

        public readonly int sizeX;
        public readonly int sizeY;
        public readonly int layers;


        public FloatCuboid(int _sizeX, int _sizeY, int _layers)
        {
            sizeX = _sizeX;
            sizeY = _sizeY;
            layers = _layers;

            values = new float[sizeX, sizeY, layers];
        }

        public FloatCuboid(Bitmap bitmap)
        {
            sizeX = bitmap.Width;
            sizeY = bitmap.Height;
            layers = 3;
            values = new float[bitmap.Width, bitmap.Height, 3];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);

                    values[x, y, 0] = color.R / 255f;
                    values[x, y, 1] = color.G / 255f;
                    values[x, y, 2] = color.B / 255f;
                }
            }
        }

        public Bitmap ToBitmap()
        {
            Bitmap bitmap = new Bitmap(sizeX, sizeY);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Color color;

                    byte r = (byte)Math.Round(values[x, y, 0] * 255);

                    if (layers > 1)
                    {
                        byte g = (byte)Math.Round(values[x, y, 1] * 255);

                        if (layers > 2)
                        {
                            byte b = (byte)Math.Round(values[x, y, 2] * 255);
                            color = Color.FromArgb(r, g, b);
                        }
                        else
                        {
                            color = Color.FromArgb(r, g, 0);
                        }
                    }
                    else
                    {
                        color = Color.FromArgb(r, r, r);
                    }

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public float[] GetSection(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            int xs = x2 - x1;
            int ys = y2 - y1;
            int zs = z2 - z1;
            float[] result = new float[xs * ys * zs];

            for (int dx = 0; dx < xs; dx++)
            {
                for (int dy = 0; dy < ys; dy++)
                {
                    for (int dz = 0; dz < zs; dz++)
                    {
                        result[dx + dy * xs + dz * xs * ys] = values[x1 + dx, y1 + dy, z1 + dz];
                    }
                }
            }

            return result;
        }
    }
}
