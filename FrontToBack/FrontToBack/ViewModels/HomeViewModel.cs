using FrontToBack.Models;
using System.Collections.Generic;

namespace FrontToBack.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderImage> SliderImages { get; set; }
        public Slider Slider { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categorys { get; set; }
        public About Abouts { get; set; }
        public AboutImage AboutImages { get; set; }
        public List<AboutList> AboutLists { get; set; }

    }
}
