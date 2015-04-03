using System;

namespace AutoCleaner.Example.Domain
{
    class User
    {
        public User(Guid id, string name, string login)
        {
            Id = id;
            Name = name;
            Login = login;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Login { get; private set; }
    }
}