namespace warehouseapp.ViewModels.Tag
{
    public class ProductTagRequestViewModel
    {
        public int ProductId { get; set; }
        public int TagId { get; set; }
        public string Value { get; set; } = string.Empty;
    }

}
