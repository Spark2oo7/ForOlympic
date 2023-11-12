using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkLibrary
{
    internal class Backpropagation
    {
        private Network network;
        private float learningRate;

        public Backpropagation(Network _network, float _learningRate)
        {
            network = _network;
            learningRate = _learningRate;
        }

        public void Train(DataSet dataSet)
        {
            foreach (Example example in dataSet.GetRandomExamples())
            {
                Train(example);
            }
        }
        public void TrainRandom(DataSet dataSet, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Train(dataSet.GetRandomExample());
            }
        }

        public void Train(Example example)
        {
            float[] input = example.inputs;
            float[] targetOutput = example.outputs;

            float[][] results = network.FullCalculate(input);
            int layersCount = network.LayersCount;
            int outputSize = results[layersCount - 1].Length;

            float[] errors = new float[outputSize];
            for (int i = 0; i < outputSize; i++)
            {
                float target = targetOutput[i];
                float result = results[layersCount - 1][i];

                errors[i] = target - result;
            }

            for (int i = layersCount - 2; i >= 0; i--)
            {
                errors = ChangeWeights(i, errors, results[i], results[i + 1]);
            }
        }

        private float[] ChangeWeights(int layer, float[] errors, float[] inputResults, float[] outputResults)
        {
            float[,] weights = network.Weights.GetLayerWeights(layer);
            int prevLayerSize = errors.Length;
            int layerSize = inputResults.Length;

            float[] result = new float[layerSize];

            for (int i = 0; i < layerSize; i++)
            {
                float sum = 0;

                float derivative = inputResults[i] * (1 - inputResults[i]);

                for (int j = 0; j < prevLayerSize; j++)
                {
                    sum += errors[j] * weights[i, j];

                    float derivative2 = outputResults[j] * (1 - outputResults[j]);
                    weights[i, j] += learningRate * errors[j] * inputResults[i] * derivative2;
                }

                result[i] = sum * derivative;
            }

            int offsetNeuron = layerSize;
            for (int j = 0; j < prevLayerSize; j++)
            {
                float derivative2 = outputResults[j] * (1 - outputResults[j]);
                weights[offsetNeuron, j] += learningRate * errors[j] * derivative2;
            }

            return result;
        }

        public float[] GetFullError(DataSet dataSet)
        {
            int size = network.LayersSize[network.LayersCount - 1];
            float[] errors = new float[size + 1];

            foreach (Example example in  dataSet.GetRandomExamples())
            {
                float[] result = network.Calculate(example.inputs);
                float[] targetOutput = example.outputs;
                for (int j = 0; j < size; j++)
                {
                    float error = (targetOutput[j] - result[j]) * (targetOutput[j] - result[j]);
                    errors[size] += error;
                    errors[j] += error;
                }
            }

            for (int j = 0; j <= size; j++)
            {
                errors[j] /= dataSet.Length;
            }

            return errors;
        }
        public float GetError(DataSet dataSet)
        {
            float error = 0;
            foreach (Example example in dataSet.GetRandomExamples())
            {
                float[] result = network.Calculate(example.inputs);
                float[] targetOutput = example.outputs;
                for (int j = 0; j < result.Length; j++)
                {
                    error += Math.Abs((targetOutput[j] - result[j]) / targetOutput[j]); //(targetOutput[j] - result[j]) * (targetOutput[j] - result[j])
                }
            }

            return error / dataSet.Length * 100;
        }
        public float GetErrorRandom(DataSet dataSet, int count)
        {
            float error = 0;

            Example[] examples = dataSet.GetRandomExamples();
            for (int i = 0; i < count; i++)
            {
                float[] result = network.Calculate(examples[i].inputs);
                float[] targetOutput = examples[i].outputs;
                for (int j = 0; j < result.Length; j++)
                {
                    error += (targetOutput[j] - result[j]) * (targetOutput[j] - result[j]);
                }
            }

            return error / count;
        }

        public float GetErrorBy2Class(Example[] examples)
        {
            int errors = 0;

            foreach (Example example in examples)
            {
                float[] result = network.Calculate(example.inputs);

                int predict = 0;

                if (result[0] > 0.5f)
                {
                    predict = 1;
                }

                if (predict != example.classNumber)
                {
                    errors++;
                }
            }

            return (float)errors / examples.Length;
        }
    }
}
