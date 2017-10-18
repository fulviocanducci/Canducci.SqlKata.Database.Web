namespace SqlServerWeb.Models
{
    public class Credit
    {
        public Credit() { }
        public Credit(string description)
        {
            Description = description;
        }
        public Credit(int id, string description)
        {
            Id = id;
            Description = description;
        }
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
