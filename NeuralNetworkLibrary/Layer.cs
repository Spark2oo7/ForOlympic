using System;

namespace NeuralNetworkLibrary
{
    public interface ILayer
    {
        float[] GetOutput(float[] input);
        int GetSize();
    }

    public class InputLayer : ILayer
    {
        private float[] input;
        private int size;

        public InputLayer(int _size)
        {
            size = _size;
        }

        public void SetInput(float[] newInput)
        {
            input = newInput;
        }

        public float[] GetOutput(float[] input)
        {
            return input;
        }

        public int GetSize()
        {
            return size;
        }
    }

    public class Layer : ILayer
    {
        private int size;
        private ILayer prevLayer;
        private Weights weights;
        private int number;

        public Layer(int _number, int _size, ILayer _prevLayer, Weights _weights)
        {
            number = _number;
            size = _size;
            prevLayer = _prevLayer;
            weights = _weights;
        }

        public float[] GetOutput(float[] input)
        {
            float[,] layerWeights = weights.GetLayerWeights(number - 1);

            float[] result = new float[size];

            int prevLayerSize = prevLayer.GetSize();
            for (int neuron = 0; neuron < size; neuron++)
            {
                float sum = 0;

                for (int j = 0; j < prevLayerSize; j++)
                {
                    sum += input[j] * layerWeights[j, neuron];
                }
                sum += layerWeights[prevLayerSize, neuron];

                result[neuron] = Activation(sum);
            }

            return result;
        }

        private float Activation(float x)
        {
            return 1 / (1 + (float)Math.Pow(Math.E, -x));
        }

        public int GetSize()
        {
            return size;
        }
    }
}
