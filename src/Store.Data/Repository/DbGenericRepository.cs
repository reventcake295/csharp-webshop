using System.Linq.Expressions;
using System.Reflection;
using Store.Common;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Interfaces.Repository;

namespace Store.Data.Repository;

internal class DbGenericRepository<TModel, TEntity> : IGenericRepository<TModel> where TModel : CommonModel 
                                                                               where TEntity : class, IEntityModel<TModel, TEntity>
{
    internal readonly StoreContext _context;
    
    private static readonly ModelExpressionConverter<TModel, TEntity> _ModelExpressionConverter = new();
    protected static readonly BindingFlags _BindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
    
    internal DbGenericRepository(IDataStorage context)
    {
        _context = (StoreContext)context;
    }

    public virtual void Add(TModel model)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [model], null);
        if (entity == null) throw new MissingMethodException("Could not create entity"); 
        _context.Set<TEntity>().Add(entity);
        _context.UpdatedIds.AddModel(model, entity);
    }
    
    public TModel? GetByKey(params object[] keyValues) => _context.Set<TEntity>().Find(keyValues)?.ToCommonModel();
    
    public virtual void Update(TModel model)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [model], null);
        if (entity == null) throw new MissingMethodException("Could not create entity"); 
        _context.Set<TEntity>().Update(entity);
    }
    
    public void Remove(TModel model)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [model], null);
        if (entity == null) throw new MissingMethodException("Could not create entity"); 
        _context.Set<TEntity>().Remove(entity);
    }

    public bool Any() => _context.Set<TEntity>().Any();
    
    public IEnumerable<TModel> FindWhere(Expression<Func<TModel, bool>> modelExpression)
    {
        Expression<Func<TEntity, bool>> entityExpression = _ModelExpressionConverter.Convert(modelExpression);
        
        return _context.Set<TEntity>().Where(entityExpression).Select(entity => entity.ToCommonModel());
    }

    public IEnumerable<TModel> GetAll() => _context.Set<TEntity>().Select(entity => entity.ToCommonModel()).ToList();
    
}