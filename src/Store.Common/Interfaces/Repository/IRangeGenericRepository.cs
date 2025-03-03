namespace Store.Common.Interfaces.Repository;

public interface IRangeGenericRepository<TModel> : IGenericRepository<TModel> where TModel : CommonModel
{
    void AddRange(IEnumerable<TModel> entities);
    
    void UpdateRange(IEnumerable<TModel> entities);
    
    void RemoveRange(IEnumerable<TModel> entities);
}