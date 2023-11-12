using System;
using System.Threading;

namespace NeuralNetworkLibrary
{
    [Serializable]
    public class Weights
    {
        private float[][,] weights; //[layersNumber][inputNeuron, outputNeuron]
        [ThreadStatic]
        private static Random random = new Random();

        public Weights() { }

        public Weights(int[] layersSize)
        {
            int layersCount = layersSize.Length;

            weights = new float[layersCount - 1][,];

            for (int i = 0; i < layersCount - 1; i++)
            {
                weights[i] = new float[layersSize[i] + 1, layersSize[i + 1]];
            }
        }

        public Weights(int[] layersSize, float min, float max)
        {
            if (random == null)
            {
                random = new Random(Thread.CurrentThread.ManagedThreadId);
            }

            int layersCount = layersSize.Length;

            weights = new float[layersCount - 1][,];

            for (int i = 0; i < layersCount - 1; i++)
            {
                weights[i] = new float[layersSize[i] + 1, layersSize[i + 1]];

                for (int x = 0; x < layersSize[i] + 1; x++)
                {
                    for (int y = 0; y < layersSize[i + 1]; y++)
                    {
                        weights[i][x, y] = (float)random.NextDouble() * (max - min) + min;
                    }
                }
            }
        }

        public float[,] GetLayerWeights(int inputLayerNumber)
        {
            return weights[inputLayerNumber];
        }

        public void SetLayerWeights(int inputLayerNumber, float[,] newWeights)
        {
            weights[inputLayerNumber] = newWeights;
        }

        public void Save(string path)
        {
            Serializer.Save(path, this);
        }

        public static Weights Load(string path)
        {
            return Serializer.Load<Weights>(path);
        }
    }
}
