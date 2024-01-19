namespace Mango.Web.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; } = "https://www.w3schools.com/html/pic_trulli.jpg";
        public int Count { get; set; } = 1;
    }
}
