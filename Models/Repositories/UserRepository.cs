namespace MiniTodo.Models.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin",
                    Role = "manager"
                },
                new User
                {
                    Id = 2,
                    Username = "employee",
                    Password = "employee",
                    Role = "employee"
                }
            };

            return users.Where(u => u.Username.ToLower() == username && u.Password.ToLower() == password).FirstOrDefault();
        }
    }
}