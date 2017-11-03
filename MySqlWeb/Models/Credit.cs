using System;
using System.ComponentModel.DataAnnotations;

namespace MySqlWeb.Models
{
    public class Credit
    {
        public Credit() { }

        public Credit(string description, DateTime? created = null)
        {         
            Description = description;
            Created = created;
        }

        public Credit(int id, string description, DateTime? created = null)
        {
            Id = id;
            Description = description;
            Created = created;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Digite a descrição")]
        [MinLength(3, ErrorMessage = "Digite com no minimo 3 letras")]
        public string Description { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Data inválida")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Created { get; set; }
    }
}
