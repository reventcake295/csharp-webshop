namespace Store.Common.Interfaces;

public interface IEntityModel<TModel, TEntity> : IId
{
    public TModel ToCommonModel();
    
//    internal abstract TEntity FromCommonModel(TModel model);

}