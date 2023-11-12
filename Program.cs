using System;
using System.Drawing;
using NeuralNetworkLibrary;

namespace TestNeuralNetwork3F
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string dataPath = "/media/spark/data/Files/Programming/Resources/ProfileEnface/Data/"; // /home/innodatahub/Data/
            string profilePath = dataPath + "profile/";
            string enfacePath = dataPath + "enface/";
            string testPath = dataPath + "test/";

            string weightsPath = dataPath + "Weights/";

            DataSet trainData = new DataSet(true);

            Console.WriteLine("Loading profile...");

            foreach (string path in Table.ReadFolder(profilePath))
            {
                Example example = OpenImage(path, path.Replace("Data", "Results"));
                example.classNumber = 0;
                example.outputs = new float[] { 0f };

                trainData.Add(example);

                if (trainData.Length % 1000 == 0)
                {
                    Console.WriteLine($"Load: {trainData.Length}");
                }
            }

            Console.WriteLine("Loading enface...");

            foreach (string path in Table.ReadFolder(enfacePath))
            {
                Example example = OpenImage(path, path.Replace("Data", "Results"));
                example.classNumber = 1;
                example.outputs = new float[] { 1f };

                trainData.Add(example);

                if (trainData.Length % 1000 == 0)
                {
                    Console.WriteLine($"Load: {trainData.Length}");
                }
            }

            DataSet testData = new DataSet(true);
            (trainData, testData) = trainData.Split(0.9f);

            Console.WriteLine("Train data:");
            trainData.PrintInfo();
            Console.WriteLine("Test data:");
            testData.PrintInfo();

            int[] layers = { 450, 70, 10, 1 };
            Weights weights = new Weights(layers);
            Network network = new Network(layers, weights);

            Backpropagation backpropagation = new Backpropagation(network, 0.1f);

            int epoche = 0;
            while (epoche < 50)
            {
                float error = backpropagation.GetErrorBy2Class(testData.GetRandomExamples());

                if (epoche % 1 == 0)
                {
                    Console.WriteLine($"Epoche {epoche}: {error}");
                    weights.Save(weightsPath + $"W{epoche}-{(int)(error * 10000)}");
                }

                backpropagation.Train(trainData);

                epoche++;
            }


            Console.WriteLine("Loading test...");

            Table results = new Table();
            results.legend = new Line("filename,label", ',');

            int count = 0;
            foreach (string path in Table.ReadFolder(testPath))
            {
                Example example = OpenImage(path, path.Replace("Data", "Results"));

                float predict = network.Calculate(example.inputs)[0];

                string clas;
                if (predict < 0.5f)
                {
                    clas = "1";
                }
                else
                {
                    clas = "0";
                }

                string[] folders = path.Split('/');
                string file = folders[folders.Length - 1];
                results.lines.Add(new Line(new string[] { file, clas }));

                count++;
                if (count % 1000 == 0)
                {
                    Console.WriteLine($"Predict: {count}");
                }
            }

            results.SaveToFile(weightsPath + "Result.csv", ',');
        }

        private static Example OpenImage(string path, string savePath)
        {
            //Bitmap bitmap = new Bitmap(savePath);
            //FloatCuboid image = new FloatCuboid(bitmap);

            //float[] input = image.GetSection(0, 0, 0, 10, 10, 2);
            //return new Example(input, null);

            Bitmap bitmap = new Bitmap(path);
            bitmap = Pooling.Resize(bitmap, 64, 64);
            FloatCuboid image = new FloatCuboid(bitmap);
            image = new Pooling(2).AveragePool(image);

            FloatCuboid blackWhite = new FloatCuboid(32, 32, 1);
            FloatCuboid output = new FloatCuboid(30, 30, 2);

            Weights weights = Convolution.ByOneD(Convolution.WhiteBlack);

            Convolution convolution = new Convolution(1, 1, 3, weights);
            convolution.Use(image, blackWhite, 0);

            Weights weights2 = Convolution.ByOneD(Convolution.Horizontals);
            Convolution convolution2 = new Convolution(3, 3, 1, weights2);
            convolution2.Use(blackWhite, output, 0);

            Weights weights3 = Convolution.ByOneD(Convolution.Verticals);
            Convolution convolution3 = new Convolution(3, 3, 1, weights3);
            convolution3.Use(blackWhite, output, 1);

            Pooling pooling = new Pooling(2);
            output = pooling.MaxPool(output);

            bitmap = output.ToBitmap();
            bitmap.Save(savePath);


            float[] input = output.GetSection(0, 0, 0, 15, 15, 2);
            return new Example(input, null);
        }
    }
}
