using System;
using System.Collections.Generic;
using System.IO;

namespace NeuralNetworkLibrary
{
    public class Table
    {
        public Line legend;
        public List<Line> lines = new List<Line>();

        public static Table ReadFromFile(string path, char splitter)
        {
            Table table = new Table();

            using (StreamReader stream = File.OpenText(path))
            {
                string line = stream.ReadLine();
                table.legend = new Line(line, splitter);

                int lineNumber = 2;
                int targetCount = table.legend.columns.Length;

                while ((line = stream.ReadLine()) != null)
                {
                    Line newLine = new Line(line, splitter);
                    table.lines.Add(newLine);
                    if (newLine.columns.Length != targetCount)
                    {
                        Console.WriteLine($"In {lineNumber} line not {targetCount} elements, there {newLine.columns.Length} elements");
                        return null;
                    }
                    lineNumber++;
                }
            }

            return table;
        }

        public static Table ReadTsv(string path)
        {
            return ReadFromFile(path, '\t');
        }

        public static Table FromColumns(string[] legend, params object[][] columns)
        {
            Table table = new Table();

            table.legend = new Line(legend);

            for (int i = 0; i < columns[0].Length; i++)
            {
                string[] line = new string[columns.Length];

                for (int j = 0; j < columns.Length; j++)
                {
                    line[j] = columns[j][i].ToString();
                }

                table.lines.Add(new Line(line));
            }

            return table;
        }

        public void SaveToFile(string path, char splitter)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                stream.WriteLine(legend.ToString(splitter));

                foreach (Line line in lines)
                {
                    stream.WriteLine(line.ToString(splitter));
                }
            }
        }

        public void SaveTsv(string path)
        {
            SaveToFile(path, '\t');
        }

        public void PreProcessing(string pathFrom, string pathTo, char symb, char substitute)
        {
            using (StreamWriter streamWrite = new StreamWriter(pathTo))
            {
                using (StreamReader streamRead = File.OpenText(pathFrom))
                {
                    bool quote = false;
                    string line;

                    while ((line = streamRead.ReadLine()) != null)
                    {
                        string result = "";
                        foreach (char c in line)
                        {
                            if (c == '"')
                            {
                                quote = !quote;
                            }
                            if (quote && c == symb)
                            {
                                result += substitute;
                            }
                            else
                            {
                                result += c;
                            }
                        }

                        streamWrite.WriteLine(result);
                    }
                }
            }
        }


        public static string[] ReadFolder(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            FileInfo[] files = directory.GetFiles();
            string[] pathes = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                pathes[i] = files[i].FullName;
            }

            return pathes;
        }
    }

    public class Line
    {
        public string[] columns;

        public Line(string[] _columns)
        {
            columns = _columns;
        }

        public Line(string line, char splitter)
        {
            columns = line.Split(splitter);
        }

        public string ToString(char splitter)
        {
            return string.Join(splitter.ToString(), columns);
        }
    }
}
