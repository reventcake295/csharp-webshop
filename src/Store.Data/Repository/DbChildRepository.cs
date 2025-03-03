using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Interfaces.Repository;

namespace Store.Data.Repository;

internal class DbChildRepository<TModel, TEntity, TPModel, TPEntity> : DbRangeRepository<TModel, TEntity>, IChildGenericRepository<TModel, TPModel> 
    where TModel : CommonModel
    where TEntity : class, IDataChildModel<TModel, TEntity, TPModel, TPEntity>
    where TPModel : CommonModel
    where TPEntity : class, IDataParentModel<TPModel, TPEntity, TModel, TEntity> // it is switched around here because this interface is viewed from the parent POV
{
    internal DbChildRepository(IDataStorage context) : base(context) { }
    
    public void AddRangeWithParent(TPModel parentModel, IEnumerable<TModel> childModels)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        TPEntity? parentEntity = (TPEntity?)Activator.CreateInstance(typeof(TPEntity), _BindingFlags, null, [parentModel], null);
        if (parentEntity == null) throw new NullReferenceException("Parent entity is null");
        _context.UpdatedIds.AddModel(parentModel, parentEntity);
        List<TEntity> childEntities = [];
        foreach (TModel childModel in childModels)
        {
            TEntity? childEntity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [childModel, parentEntity], null);
            if (childEntity == null) throw new NullReferenceException("Child entity is null");
            childEntities.Add(childEntity);
            parentEntity.Children.Add(childEntity);
            _context.UpdatedIds.AddModel(childModel, childEntity);
        }
        // add the parent first and then the child entities, and because we have the navigation properties set we should be able to have the id's automatically updated
        _context.Set<TPEntity>().Add(parentEntity);
        _context.Set<TEntity>().AddRange(childEntities);
    }
}