using System;
namespace SqlServerWeb.Models
{
    public class Part
    {
        public Part() { }
        public Part(string description)
        {
            Description = description;
        }
        public Part(Guid id, string description)
        {
            Id = id;
            Description = description;
        }
        public Guid Id { get; set; }
        public string Description { get; set; }
    }
}
