namespace ProductManagementAPI.Models
{
    public class Product
    {
        public string? Type { get; set; } = "product";
        public string Name { get; set; }
        public string? Details { get; set; }
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Image { get; set; }
    }
}
