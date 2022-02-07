namespace FrontToBack.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public int  CategoryID { get; set; }
        public Category Category { get; set; }
    }
}
