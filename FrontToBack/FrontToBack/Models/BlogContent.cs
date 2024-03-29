﻿using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Models
{
    public class BlogContent
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Boş saxlanıla bilməz!")]

        public string Image { get; set; }

        [Required(ErrorMessage = "Boş saxlanıla bilməz!"), MaxLength(50)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Boş saxlanıla bilməz!"), MaxLength(200)]
        public string Content { get; set; }

        [Required(ErrorMessage = "Boş saxlanıla bilməz!")]
        public DateTime date { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Boş saxlanıla bilməz!")]
        public IFormFile Photo { get; set; }
    }
}
