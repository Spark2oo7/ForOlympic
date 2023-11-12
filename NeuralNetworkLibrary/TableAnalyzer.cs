using System;
using System.Collections.Generic;
using System.IO;

namespace NeuralNetworkLibrary
{
    public class TableAnalyzer
    {
        private int Select()
        {
            string value = Console.ReadLine();

            if (int.TryParse(value, out _))
            {
                return int.Parse(value);
            }

            Console.WriteLine("Error, no number");
            return Select();
        }

        private string ColumnProcessing(Column column)
        {
            Console.WriteLine($"{column.number} column {column.name}?\n0-skip 1-embedding 2-number 3-manually");

            string method = $"private float[] Column{column.number}(string value) //{column.name}" + "\n{\n";

            switch (Select())
            {
                case 0:
                    method += "return new float[0]\n";
                    break;
                case 1:
                    method += "return embedding.Predict(value);\n";
                    break;
                case 2:
                    float minValue = float.MaxValue;
                    float maxValue = float.MinValue;

                    foreach(string value in column.values)
                    {
                        float f = float.Parse(value.Replace(',', '.'));

                        maxValue = Math.Max(maxValue, f);
                        minValue = Math.Min(minValue, f);
                    }

                    Console.WriteLine($"min: {minValue}, max: {maxValue}");

                    method += $@"float dif = {maxValue} - {minValue}
                    float val = float.Parse(value.Replace(',', '.'))
                    return new float[] {{ (val - {minValue}) / dif }}\n";

                    break;
                case 3:
                    method += "const Dictionary<string, float[]> values = new Dictionary<string, float[]>() {\n";

                    foreach (string value in column.values)
                    {
                        method += "{ " + '"' + value.ToString() + '"' + ", new float[] {0f} },\n";
                    }

                    method += "}\n";
                    method += "return values[value];\n";

                    break;
            }

            method += "}\n";

            return method;
        }

        public TableAnalyzer(Table table)
        {
            int count = table.legend.columns.Length;
            Column[] columns = new Column[count];

            for (int i = 0; i < count; i++)
            {
                columns[i] = new Column(table.legend.columns[i], i);
            }

            foreach (Line line in table.lines)
            {
                for (int i = 0; i < count; i++)
                {
                    columns[i].Add(line.columns[i]);
                }
            }

            Console.WriteLine("Create DataOpener?\n0-no 1-yes");
            bool createDataOpener = Select() == 1;

            string result = @"using System;
using System.Collections.Generic;

namespace NeuralNetworkLibrary
{
    public class DataOpener
    {
        private Random random = new Random();
        //private Embedding embedding = new Embedding();
        
        public DataOpener() { }
";

            for (int i = 0; i < count; i++)
            {
                Column column = columns[i];

                string method = ColumnProcessing(column);

                result += method;
                result += '\n';
            }

            if (createDataOpener)
            {
                result += @"
        public DataSet Open(Table table)
        {
            DataSet dataSet = new DataSet(false);
            foreach (Line line in table.lines)
            {
                float[][] inputs = new float[" + count.ToString() + @"][];
            ";
                for (int i = 0; i < count; i++)
                {
                    result += $"inputs[{i}] = Column{i}(line.columns[{i}])\n";
                }

                result += @"var input = new float[inputs.Sum(a => a.Length)];
                int offset = 0;
                foreach (float[] array in inputs)
                {
                    inputs.CopyTo(input, offset);
                    offset += array.Length;
                }

                dataSet.Add(new Example(input, null));
            }

            return dataSet;
        }
        }
        }";

                Console.WriteLine("File path?");
                string path = Console.ReadLine();

                File.WriteAllText(path, result);
            }
        }

        public class Column
        {
            public string name;
            public int number;
            public int valuesCount = 0;
            public HashSet<string> values = new HashSet<string>();
            public bool wasEmpty = false;

            public Column(string _name, int _number)
            {
                name = _name;
                number = _number;
            }

            public void Add(string value)
            {
                if (value == "")
                {
                    wasEmpty = true;
                }

                if (!values.Contains(value))
                {
                    valuesCount++;
                    values.Add(value);
                }
            }
        }
    }
}
