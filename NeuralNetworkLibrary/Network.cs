using System;

namespace NeuralNetworkLibrary
{
    public class Network
    {
        public int LayersCount { get; private set; }
        public int[] LayersSize { get; private set; }
        public Weights Weights { get; private set; }

        private InputLayer inputLayer;
        private ILayer outputLayer;
        private ILayer[] layers;

        public Network(int[] layersSize, Weights weights)
        {
            LayersSize = layersSize;
            Weights = weights;
            LayersCount = LayersSize.Length;
            layers = new ILayer[LayersCount];

            inputLayer = new InputLayer(LayersSize[0]);
            layers[0] = inputLayer;

            for (int i = 1; i < LayersCount; i++)
            {
                ILayer layer = new Layer(i, LayersSize[i], layers[i - 1], Weights);
                layers[i] = layer;
            }

            outputLayer = layers[LayersCount - 1];
        }

        public float[][] FullCalculate(float[] input)
        {
            float[][] fullResult = new float[LayersCount][];
            float[] result = input;

            for (int i = 0; i < LayersCount; i++)
            {
                result = layers[i].GetOutput(result);
                fullResult[i] = result;
            }

            return fullResult;
        }

        public float[] Calculate(float[] input)
        {
            float[] result = input;

            for (int i = 0; i < LayersCount; i++)
            {
                result = layers[i].GetOutput(result);
            }

            return result;
        }
    }
}
