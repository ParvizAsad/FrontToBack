using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Models
{
    public class SliderImage
    {
        public int ID { get; set; }
        public string Name { get; set; }

        [NotMapped]
        //[Required]
        public IFormFile Photo { get; set; }

        [NotMapped]
        [Required]
        public IFormFile[] Photos { get; set; }
    }
}
