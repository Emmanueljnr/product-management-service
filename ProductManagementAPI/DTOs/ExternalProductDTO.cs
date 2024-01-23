namespace ProductManagementAPI.DTOs
{
    // This model matches the structure of the product data from the Amazon API.
    public class ExternalProductDTO
    {
        public string Asin { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public ImageDTO Image { get; set; }
    }

    public class ImageDTO
    {
        public string Primary { get; set; }
        public List<string> Variants { get; set; } // The rest of the images that arent the 'primary'/'main' image
        // If you want to store additional image URLs, you can include them here as well.
        // public List<string> Variants { get; set; }
    }

}











// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//                                                         CODE FOR THE 'undercutters' API
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

// This model matches the structure of the product data from the external API.
// This model acts as a DTOfor the external API response
/*
 * public class ExternalProductDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
*/