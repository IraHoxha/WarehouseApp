namespace warehouseapp.ViewModels.Tag
{
    public class ProductTagDisplayViewModel
    {
        public int Id { get; set; }

        public int TagId { get; set; }
        public string TagKey { get; set; } = string.Empty;

        public int TagValueId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
