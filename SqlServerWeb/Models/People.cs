using System;

namespace SqlServerWeb.Models
{
    public class People
    {
        public People()
        {
        }

        public People(string name, DateTime? created, bool active)
        {            
            Name = name;
            Created = created;
            Active = active;
        }

        public People(int id, string name, DateTime? created, bool active)
        {
            Id = id;
            Name = name;
            Created = created;
            Active = active;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Created { get; set; }
        public bool Active { get; set; }
    }
}
