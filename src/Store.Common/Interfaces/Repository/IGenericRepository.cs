using System.Linq.Expressions;

namespace Store.Common.Interfaces.Repository;

public interface IGenericRepository<TModel> where TModel : CommonModel
{
    bool Any();
    
    TModel? GetByKey(params object[] keyValues);
    void Add(TModel model);
    void Remove(TModel model);
    void Update(TModel model);
    
    IEnumerable<TModel> GetAll();
    
    public IEnumerable<TModel> FindWhere(Expression<Func<TModel, bool>> modelExpression);
    
    
}