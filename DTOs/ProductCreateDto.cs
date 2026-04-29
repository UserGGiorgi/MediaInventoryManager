namespace MediaInventoryManager.DTOs
{
    public class ProductCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageFileName { get; set; }
        public decimal Price { get; set; }
    }
}
