using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCupsFinal
{
    static class FileSystem
    {
        static public bool debug = true;

        static public bool FileExits(string path)
        {
            return FileExits(path, false);
        }
        static public bool FileExits(string path, bool write)
        {
            if (File.Exists(path))
            {
                if (debug && write)
                {
                    Console.WriteLine($@"File ""{path}"" exit.");
                }

                return true;
            }
            else
            {
                if (debug && (!write))
                {
                    Console.WriteLine($@"File ""{path}"" dont exit.");
                }

                return false;
            }
        }

        static public void DeleteFile(string path)
        {
            if (!FileExits(path))
            {
                return;
            }

            File.SetAttributes(path, FileAttributes.Normal);
            File.Delete(path);
        }

        static public string GetName(string path)
        {
            return Path.GetFileName(path);
        }


        static public Image ReadImage(string path)
        {
            if (!FileExits(path))
            {
                return null;
            }

            return new Image(new Bitmap(path));
        }

        static public void WriteImage(string path, Image image)
        {
            DeleteFile(path);
            SaveImage(path, image);
        }
        static public void SaveImage(string path, Image image)
        {
            if (FileExits(path, true))
            {
                return;
            }

            image.GetBitmap().Save(path);
        }


        static public string[] ReadTextLines(string path)
        {
            if (!FileExits(path))
            {
                return null;
            }

            return File.ReadAllLines(path);
        }
        static public string ReadText(string path)
        {
            if (!FileExits(path))
            {
                return null;
            }

            return File.ReadAllText(path);
        }

        static public void WriteTextLines(string path, string[] lines)
        {
            DeleteFile(path);
            SaveTextLines(path, lines);
        }
        static public void SaveTextLines(string path, string[] lines)
        {
            if (FileExits(path, true))
            {
                return;
            }

            File.WriteAllLines(path, lines);
        }

        static public void WriteText(string path, string text)
        {
            DeleteFile(path);
            SaveText(path, text);
        }
        static public void SaveText(string path, string text)
        {
            if (FileExits(path, true))
            {
                return;
            }

            File.WriteAllText(path, text);
        }


        static public Table ReadCSV(string path, char splitter)
        {
            if (!FileExits(path))
            {
                return null;
            }

            return new Table(ReadTextLines(path), splitter);
        }

        static public void WriteCSV(string path, Table table, char splitter)
        {
            DeleteFile(path);
            SaveCSV(path, table, splitter);
        }
        static public void SaveCSV(string path, Table table, char splitter)
        {
            if (FileExits(path, true))
            {
                return;
            }

            File.WriteAllLines(path, table.ToLines(splitter));
        }

        
        static public string[] ReadFolder(string path)
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

    class Image
    {
        private Bitmap bitmap;
        private Color[,] colorMap;

        public int Height { get { return bitmap.Height; } }
        public int Width { get { return bitmap.Width; } }

        public Image(Bitmap _bitmap)
        {
            bitmap = _bitmap;
        }

        public Image(int width, int height)
        {
            bitmap = new Bitmap(width, height);
        }

        public Color GetColor(int x, int y)
        {
            return bitmap.GetPixel(x, y);
        }
        public void SetColor(int x, int y, Color color)
        {
            bitmap.SetPixel(x, y, color);
        }

        public Color[,] GetColotMap()
        {
            if (colorMap != null)
            {
                return colorMap;
            }

            colorMap = new Color[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    colorMap[x, y] = GetColor(x, y);
                }
            }

            return colorMap;
        }

        public Bitmap GetBitmap()
        {
            return bitmap;
        }
        public Bitmap CloneBitmap()
        {
            return (Bitmap)bitmap.Clone();
        }
    }

    class Table
    {
        public delegate T Parse<T>(string str);

        public string Legend { get; set; }

        public int Length { get; } = 0;
        public int Columns { get; } = 0;

        private string[,] data; //[line, column]

        public Table(string[] lines, char splitter)
        {
            Length = lines.Length - 1;
            Legend = lines[0];

            Columns = lines[1].Split(splitter).Length;

            data = new string[Length, Columns];

            for (int i = 1; i < Length; i++)
            {
                string[] line = lines[i].Split(splitter);

                if (line.Length > Columns)
                {
                    Console.WriteLine($"Line {i + 1} in table too big");
                }
                else if (line.Length < Columns)
                {
                    Console.WriteLine($"Line {i + 1} in table too small");
                }

                for (int j = 0; j < Columns; j++)
                {
                    if (line.Length > i)
                    {
                        data[i, j] = line[j];
                    }
                    else
                    {
                        data[i, j] = string.Empty;
                    }
                }
            }
        }
        public Table(string legend, params object[][] columns)
        {
            Legend = legend;
            Columns = columns.Length;
            Length = columns[0].Length;

            data = new string[Length, Columns];

            for (int i = 0; i < Columns; i++)
            {
                object[] column = columns[i];
                for (int j = 0; j < Length; j++)
                {
                    if (column.Length > j)
                    {
                        data[j, i] = column[j].ToString();
                    }
                    else
                    {
                        data[j, i] = string.Empty;
                    }
                }
            }
        }

        public T[] GetColumn<T>(int n, Parse<T> parse)
        {
            T[] result = new T[Length];

            for (int i = 0; i < Length; i++)
            {
                result[i] = parse(data[n, i]);
            }

            return result;
        }

        public string[] ToLines(char splitter)
        {
            string[] result = new string[Length + 1];
            result[0] = Legend;

            for (int i = 0; i < Length; i++)
            {
                string line = data[i, 0];

                for (int j = 1; j < Columns; j++)
                {
                    line += splitter + data[i, j];
                }

                result[i + 1] = line;
            }

            return result;
        }
    }
}
