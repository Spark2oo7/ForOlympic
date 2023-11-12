using System;
using System.Drawing;

namespace NeuralNetworkLibrary
{
    public class Pooling
    {
        public int step;

        public static Bitmap Resize(Bitmap bitmap, int width, int height)
        {
            return new Bitmap(bitmap, new Size(width, height));
        }

        public Pooling(int _step)
        {
            step = _step;
        }

        public FloatCuboid MaxPool(FloatCuboid floatCuboid)
        {
            int xs = floatCuboid.sizeX;
            int ys = floatCuboid.sizeY;
            int zs = floatCuboid.layers;

            FloatCuboid result = new FloatCuboid(xs / step, ys / step, zs);

            for (int x = 0; x < xs; x += step)
            {
                for (int y = 0; y < ys; y += step)
                {
                    for (int z = 0; z < zs; z++)
                    {
                        float max = float.MinValue;

                        for (int dx = 0; dx < step; dx++)
                        {
                            for (int dy = 0; dy < step; dy++)
                            {
                                max = Math.Max(max, floatCuboid.values[x + dx, y + dy, z]);
                            }
                        }

                        result.values[x / step, y / step, z] = max;
                    }
                }
            }

            return result;
        }

        public FloatCuboid AveragePool(FloatCuboid floatCuboid)
        {
            int xs = floatCuboid.sizeX;
            int ys = floatCuboid.sizeY;
            int zs = floatCuboid.layers;

            FloatCuboid result = new FloatCuboid(xs / step, ys / step, zs);

            for (int x = 0; x < xs; x += step)
            {
                for (int y = 0; y < ys; y += step)
                {
                    for (int z = 0; z < zs; z++)
                    {
                        float sum = 0;

                        for (int dx = 0; dx < step; dx++)
                        {
                            for (int dy = 0; dy < step; dy++)
                            {
                                sum += floatCuboid.values[x + dx, y + dy, z];
                            }
                        }

                        result.values[x / step, y / step, z] = sum / step / step;
                    }
                }
            }

            return result;
        }
    }
}
