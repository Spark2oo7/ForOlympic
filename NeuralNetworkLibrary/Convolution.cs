using System;

namespace NeuralNetworkLibrary
{
    public class Convolution
    {
        public readonly int sizeX;
        public readonly int sizeY;
        public readonly int layers;

        public readonly Network network;

        public Convolution(int _sizeX, int _sizeY, int _layers, Weights weights)
        {
            sizeX = _sizeX;
            sizeY = _sizeY;
            layers = _layers;

            int[] layersSizes = new int[] { sizeX * sizeY * layers, 1 };

            network = new Network(layersSizes, weights);
        }

        public void Use(FloatCuboid input, FloatCuboid output, int layer)
        {
            for (int x = 0; x < output.sizeX; x++)
            {
                for (int y = 0; y < output.sizeY; y++)
                {
                    float[] inputData = input.GetSection(x, y, 0, x + sizeX, y + sizeY, layers);
                    output.values[x, y, layer] = network.Calculate(inputData)[0];
                }
            }
        }

        public static Weights ByOneD(float[] values)
        {
            int size = values.Length - 1;
            Weights weights = new Weights(new int[] { size, 1 });

            float[,] layer = new float[size + 1, 1];

            for (int i = 0; i < size + 1; i++)
            {
                layer[i, 0] = values[i];
            }

            weights.SetLayerWeights(0, layer);

            return weights;
        }

        public static  float[] WhiteBlack = { 2f, 2f, 2f, -3f };
        //public static readonly float[] Verticals = { -1, 2, -1, -1, 2, -1, -1, 2, -1, 0 };
        public static readonly float[] Verticals = { -1.5f, 3, -1.5f, -1.5f, 3, -1.5f, -1.5f, 3, -1.5f, 0 };
        public static readonly float[] Horizontals = { -1.5f, -1.5f, -1.5f, 3, 3, 3, -1.5f, -1.5f, -1.5f, 0 };
    }
}
