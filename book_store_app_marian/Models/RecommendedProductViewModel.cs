namespace book_store_app_marian.Models
{
    public class RecommendedProductViewModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductAuthor { get; set; }
        public double Price { get; set; }
        public string CategoryName { get; set; }
        public string MainImageUrl { get; set; }
    }
}
