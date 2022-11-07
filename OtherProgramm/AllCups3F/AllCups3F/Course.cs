using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCups3F
{
    class Course
    {
        public int courseId;
        public List<User> usersComplete = new List<User>();

        public Course(int id)
        {
            courseId = id;
        }

        public void AddUser(User user)
        {
            usersComplete.Add(user);
        }
    }
}
