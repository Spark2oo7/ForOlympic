using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AllCupsF
{
    class Program
    {
        static (bool, string)[] ReadTrain()
        {
            string path = @"E:\Files\Programming\Resource\AllCupsTrain.txt";

            List<(bool, string)> reviews = new List<(bool, string)>();

            using (StreamReader sr = File.OpenText(path))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new char[] { ' ' }, 3);

                    reviews.Add((data[1] == "Positive", data[2]));
                }
            }

            return reviews.ToArray();
        }

        static Dictionary<int, string> ReadTest()
        {
            string path = @"E:\Files\Programming\Resource\AllCupsTest.txt";

            Dictionary<int, string> reviews = new Dictionary<int, string>();

            using (StreamReader sr = File.OpenText(path))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new char[] { ' ' }, 2);

                    reviews.Add(int.Parse(data[0]), data[1]);
                }
            }

            return reviews;
        }

        static Dictionary<string, (int, int)> GetWts((bool, string)[] reviews)
        {
            Dictionary<string, (int, int)> result = new Dictionary<string, (int, int)>();

            foreach ((bool, string) revie in reviews)
            {
                foreach (string word in revie.Item2.Split(' ', ',', '.', '!', '?', '-'))
                {
                    if (word == "")
                    {
                        continue;
                    }

                    string lowerWord = word.ToLower();

                    if (!result.ContainsKey(lowerWord))
                    {
                        result.Add(lowerWord, (0, 0));
                    }

                    if (revie.Item1)
                    {
                        result[lowerWord] = (result[lowerWord].Item1 + 1, result[lowerWord].Item2);
                    }
                    else
                    {
                        result[lowerWord] = (result[lowerWord].Item1, result[lowerWord].Item2 + 1);
                    }
                }
            }

            return result;
        }

        static Dictionary<int, double> GetResults(Dictionary<int, string> reviews, Dictionary<string, (int, int)> wts)
        {
            double offset = 0;
            Dictionary<int, double> result = new Dictionary<int, double>();

            foreach (int id in reviews.Keys)
            {
                string revie = reviews[id];

                double score = 0;

                string[] words = revie.Split(' ', ',', '.', '!', '?', '-');

                double wordsCount = 0;
                foreach (string word in words)
                {
                    if (word == "")
                    {
                        continue;
                    }

                    wordsCount++;
                    if (wts.ContainsKey(word))
                    {
                        if (wts[word].Item1 > 0 && wts[word].Item2 > 0)
                        {
                            if (wts[word].Item1 > wts[word].Item2)
                            {
                                score += (double)(wts[word].Item1 + offset) / (double)(wts[word].Item2 + offset);
                            }
                            else
                            {
                                score -= (double)(wts[word].Item2 + offset) / (double)(wts[word].Item1 + offset);
                            }
                        }
                    }
                }

                result.Add(id, ((score + revie.Count(c => c == '!') / 5) / wordsCount) * Math.Pow(wordsCount, 0.02));
            }

            return result;
        }

        static void SaveResults(Dictionary<int, double> result)
        {
            int[] ids = result.Keys.ToList().OrderBy(x => result[x]).ToArray();

            double middle = result[ids[ids.Length / 2 + 16]];

            string[] lines = new string[ids.Length + 1];
            lines[0] = "idx	Score";

            foreach (int id in result.Keys)
            {
                string line = id.ToString() + "	";
                if (result[id] > middle)
                {
                    line += "Positive";
                }
                else
                {
                    line += "Negative";
                }

                lines[id - 13998] = line;
            }

            File.WriteAllLines(@"E:\Files\Programming\Resource\AllCupsResult.csv", lines);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("open train file...");
            var reviews = ReadTrain();
            Console.WriteLine("get wts...");
            var wts = GetWts(reviews);

            Console.WriteLine("open test file...");
            var testReviews = ReadTest();
            Console.WriteLine("get results...");
            var result = GetResults(testReviews, wts);
            Console.WriteLine("save results...");
            SaveResults(result);
            Console.WriteLine("complete");
            Console.ReadKey();
        }
    }
}
