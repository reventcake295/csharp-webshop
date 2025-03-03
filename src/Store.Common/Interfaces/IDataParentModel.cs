namespace Store.Common.Interfaces;

public interface IDataParentModel<TModel, TEntity, TCModel, TCEntity> : IEntityModel<TModel, TEntity>
{
    public ICollection<TCEntity> Children { get; set; }
}