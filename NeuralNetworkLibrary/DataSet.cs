using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuralNetworkLibrary
{
    internal class DataSet
    {
        public int Length { get; private set; }
        private List<Example> examples = new List<Example>();
        private Dictionary<int, List<Example>> classExamples;
        private HashSet<int> classNumbers;
        [ThreadStatic]
        private static Random random = new Random();
        private readonly bool classification;

        public DataSet(bool _classification)
        {
            classification = _classification;
            if (classification)
            {
                classExamples = new Dictionary<int, List<Example>>();
                classNumbers = new HashSet<int>();
            }
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Length: {Length}");
            if (classification)
            {
                foreach (int clas in classNumbers)
                {
                    Console.WriteLine($"Class {clas}: {classExamples[clas].Count}");
                }
            }
            else
            {
                Console.WriteLine("No classification");
            }
        }

        public void Add(Example example)
        {
            examples.Add(example);
            Length++;

            if (example.classNumber != -1)
            {
                if (classExamples.ContainsKey(example.classNumber))
                {
                    classExamples[example.classNumber].Add(example);
                    classNumbers.Add(example.classNumber);
                }
                else
                {
                    classExamples.Add(example.classNumber, new List<Example>() { example });
                }
            }
        }

        public Example[] GetRandomExamples()
        {
            random = new Random(Thread.CurrentThread.ManagedThreadId + (int)DateTime.Now.Ticks);

            Example[] result = new Example[Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = examples[i];
            }

            for (int i = result.Length - 1; i >= 0; i--)
            {
                int j = random.Next(i + 1);

                Example temp = result[j];
                result[j] = result[i];
                result[i] = temp;
            }

            return result;
        }

        public Example GetRandomExample()
        {
            if (classification)
            {
                int classNumber = classNumbers.ElementAt(random.Next(classNumbers.Count));
                List<Example> classesxamples = classExamples[classNumber];
                return classesxamples[random.Next(classesxamples.Count)];
            }
            return examples[random.Next(examples.Count)];
        }

        public (DataSet, DataSet) Split(float chance)
        {
            DataSet dataSet1 = new DataSet(classification);
            DataSet dataSet2 = new DataSet(classification);

            Example[] randomExamples = GetRandomExamples();

            for (int i = 0; i < Length; i++)
            {
                if (i < Length * chance)
                {
                    dataSet1.Add(randomExamples[i]);
                }
                else
                {
                    dataSet2.Add(randomExamples[i]);
                }
            }

            return (dataSet1, dataSet2);
        }
    }

    public class Example
    {
        public float[] inputs;
        public float[] outputs;
        public int classNumber;

        public Example(float[] _inputs, float[] _outputs, int _classNumber)
        {
            inputs = _inputs;
            outputs = _outputs;
            classNumber = _classNumber;
        }

        public Example(float[] _inputs, float[] _outputs) : this(_inputs, _outputs, -1) { }
    }
}
