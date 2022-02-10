using FrontToBack.Models;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.DataAccessLayer
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<AboutImage> AboutImages { get; set; }
        public DbSet<AboutList> AboutLists { get; set; }
        public DbSet<ExpertsTitle> ExpertsTitles { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<BlogTitle> BlogTitles { get; set; }
        public DbSet<BlogContent> BlogContents { get; set; }
        public DbSet<Say> Says { get; set; }
        public DbSet<Instagrams> Instagramss { get; set; }
        public DbSet<Bio> Bios { get; set; }
   
    }
}
