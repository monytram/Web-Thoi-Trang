namespace WebThoiTrang.Services.Interfaces
{
    public interface IBaseService<TEntity, TViewModel> where TEntity : class
    {
        Task<IEnumerable<TViewModel>> GetAllAsync();
        Task<TViewModel> GetByIdAsync(object id);
        Task<TViewModel> CreateAsync(TViewModel viewModel);
        Task UpdateAsync(TViewModel viewModel);
        Task DeleteAsync(object id);
    }
}
