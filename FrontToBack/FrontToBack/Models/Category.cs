using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Boş saxlanıla bilməz!"), MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(150, ErrorMessage = "150 simvoldan artıq ola bilməz!")]
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
