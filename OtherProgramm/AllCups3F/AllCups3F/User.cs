using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCups3F
{
    class User
    {
        public int userId;
        public List<Course> completeCourses = new List<Course>();
        public Course[] recomendation = new Course[3];

        public User(int id)
        {
            userId = id;
        }

        public void AddCourse(Course course)
        {
            completeCourses.Add(course);
        }
    }
}
