using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PostgresWeb.Models
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

        
        public static implicit operator Dictionary<string, object>(Credit cr)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (PropertyInfo info in cr.GetType().GetProperties())
            {
                values.Add(info.Name.ToLower(), info.GetValue(cr, null));
            }
            return values;
        }

    }
}
