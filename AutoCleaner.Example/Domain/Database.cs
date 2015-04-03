using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCleaner.Example.Domain
{
    internal class Database
    {
        private readonly List<User> _users = new List<User>();

        public Guid AddUser(string name, string login)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Please provide name");
            if (string.IsNullOrEmpty(login)) throw new ArgumentException("Please provide login");

            var user = new User(Guid.NewGuid(), name, login);
            _users.Add(user);
            return user.Id;
        }

        public User GetUser(Guid id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User FindUserByName(string name)
        {
            return _users.FirstOrDefault(u => u.Name == name);
        }
    }
}