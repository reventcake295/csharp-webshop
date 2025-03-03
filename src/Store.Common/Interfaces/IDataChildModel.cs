namespace Store.Common.Interfaces;

public interface IDataChildModel<TModel, TEntity, TPModel, TPEntity> : IEntityModel<TModel, TEntity>
{
    public TPEntity Parent { get; set; }
}