namespace warehouseapp.ViewModels.Tag
{
    public class TagResponseViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
    }
}
