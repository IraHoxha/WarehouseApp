using warehouseapp.ViewModels.Tag;

namespace warehouseapp.Interfaces
{
    public interface ITagService
    {
        Task<TagResponseViewModel> CreateAsync(TagRequestViewModel model);
        Task<List<TagResponseViewModel>> GetAllAsync();
        Task<List<TagValueResponseViewModel>> GetValuesAsync(int tagId);
        Task DeleteIfUnusedAsync(int id);
        Task DeleteTagValueIfUnusedAsync(int tagValueId);
        Task CleanupAsync();
    }
}
