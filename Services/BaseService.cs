using AutoMapper;
using WebThoiTrang.Data.Interfaces;
using WebThoiTrang.Services.Interfaces;

namespace WebThoiTrang.Services
{
    public abstract class BaseService<TEntity, TViewModel> : IBaseService<TEntity, TViewModel>
    where TEntity : class
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IMapper _mapper;

        protected BaseService(IRepository<TEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<TViewModel>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TViewModel>>(entities);
        }

        public virtual async Task<TViewModel> GetByIdAsync(object id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<TViewModel>(entity);
        }

        public virtual async Task<TViewModel> CreateAsync(TViewModel viewModel)
        {
            var entity = _mapper.Map<TEntity>(viewModel);
            await _repository.AddAsync(entity);
            return _mapper.Map<TViewModel>(entity);
        }

        public virtual async Task UpdateAsync(TViewModel viewModel)
        {
            var entity = _mapper.Map<TEntity>(viewModel);
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task DeleteAsync(object id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
