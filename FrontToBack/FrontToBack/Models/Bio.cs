using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class Bio
    {
        public int ID { get; set; }
        [Required]
        public string Logo { get; set; }
        [StringLength(100)]
        public string FacebookUrl { get; set; }
        [StringLength(100)]
        public string LinkedinUrl { get; set; }






    }
}
