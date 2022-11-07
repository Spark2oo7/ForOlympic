using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCups3F
{
    class DataBase
    {
        public List<User> trainUsers = new List<User>();
        public List<User> users = new List<User>();
        public List<Course> courses = new List<Course>();
        public Dictionary<int, User> usersIds = new Dictionary<int, User>();
        public Dictionary<int, Course> coursesIds = new Dictionary<int, Course>();

        public void AddTrainUser(int id)
        {
            User user = AddUser(id);
            trainUsers.Add(user);
        }

        public User AddUser(int id)
        {
            User user = new User(id);
            users.Add(user);
            usersIds.Add(id, user);

            return user;
        }

        public Course AddCourse(int id)
        {
            Course course = new Course(id);
            courses.Add(course);
            coursesIds.Add(id, course);

            return course;
        }

        public void AddComplete(int userId, int courseId)
        {
            Course course;

            if (coursesIds.ContainsKey(courseId))
            {
                course = coursesIds[courseId];
            }
            else
            {
                course = AddCourse(courseId);
            }

            User user;

            if (usersIds.ContainsKey(userId))
            {
                user = usersIds[userId];
            }
            else
            {
                user = AddUser(userId);
            }

            course.AddUser(user);
            user.AddCourse(course);
        }
    }
}
