using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Interfaces.Repository;

namespace Store.Data.Repository;

internal class DbRangeRepository<TModel, TEntity> : DbGenericRepository<TModel, TEntity> , IRangeGenericRepository<TModel>
                where TModel : CommonModel
                where TEntity : class, IEntityModel<TModel, TEntity>
{
    internal DbRangeRepository(IDataStorage context) : base(context) { }

    public void AddRange(IEnumerable<TModel> models)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        _context.Set<TEntity>().AddRange(models.Select(model =>
        {
            TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [model], null);
            if (entity == null) throw new MissingMethodException("Could not create entity");
            _context.UpdatedIds.AddModel(model, entity);
            return entity;
        }));
    }

    public void UpdateRange(IEnumerable<TModel> models)
    {        
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        _context.Set<TEntity>().UpdateRange(models.Select(model => {
            TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [model], null);
            if (entity == null) throw new MissingMethodException("Could not create entity");
            return entity;
        }));   
    }

    public void RemoveRange(IEnumerable<TModel> models)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        _context.Set<TEntity>().RemoveRange(models.Select(model => {
            TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [model], null);
            if (entity == null) throw new MissingMethodException("Could not create entity");
            return entity;
        }));
    }
}