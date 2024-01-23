namespace ProductManagementAPI.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public List<string> Image { get; set; }
    }
}
