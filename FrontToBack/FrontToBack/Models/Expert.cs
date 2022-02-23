using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Models
{
    public class Expert
    {
        public int ID { get; set; }
        
        [Required(ErrorMessage ="Boş buraxıla bilməz!")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Boş buraxıla bilməz!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Boş buraxıla bilməz!")]
        public string Job { get; set; }

        [NotMapped]
        [Required]
        public IFormFile Photo { get; set; }
    }
}
