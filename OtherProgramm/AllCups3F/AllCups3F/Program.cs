using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCups3F
{
    class Program
    {

        static private DataBase LoadData()
        {
            DataBase dataBase = new DataBase();

            string path = @"E:\Files\Programming\Resource\AllCups3Test.txt";

            using (StreamReader sr = File.OpenText(path))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new char[] { ',' }, 2);

                    dataBase.AddTrainUser(int.Parse(data[0]));
                }
            }


            path = @"E:\Files\Programming\Resource\AllCups3Train.txt";

            using (StreamReader sr = File.OpenText(path))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(new char[] { ',' }, 3);

                    int userId = int.Parse(data[0]);
                    int courseId = int.Parse(data[1]);
                    dataBase.AddComplete(userId, courseId);
                }
            }

            return dataBase;
        }

        static Course[] GetCourses(User user, List<Course> courses)
        {
            Course[] result = new Course[3];
            int next = 0;

            foreach(Course course in courses)
            {
                if (!user.completeCourses.Contains(course))
                {
                    result[next] = course;
                    next++;
                    if (next > 2)
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        static void GetResult(DataBase dataBase)
        {
            List<Course> popularCourses = dataBase.courses;
            popularCourses = popularCourses.OrderByDescending(x => x.usersComplete.Count).ToList();

            foreach (User user in dataBase.trainUsers)
            {
                user.recomendation = GetCourses(user, popularCourses);
            }
        }

        static void SaveResult(DataBase database)
        {
            string[] lines = new string[database.trainUsers.Count + 1];
            lines[0] = "user_id,course_id_1,course_id_2,course_id_3";

            for (int i = 0; i < database.trainUsers.Count; i++)
            {
                User user = database.trainUsers[i];
                string line = user.userId.ToString();
                line += "," + user.recomendation[0].courseId.ToString();
                line += "," + user.recomendation[1].courseId.ToString();
                line += "," + user.recomendation[2].courseId.ToString();
                lines[i + 1] = line;
            }

            File.WriteAllLines(@"E:\Files\Programming\Resource\AllCups3Result.csv", lines);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Load data...");
            DataBase database = LoadData();
            Console.WriteLine("Get result...");
            GetResult(database);
            Console.WriteLine("Save result...");
            SaveResult(database);
            Console.WriteLine("Programm complete");
            Console.ReadKey();
        }
    }
}
