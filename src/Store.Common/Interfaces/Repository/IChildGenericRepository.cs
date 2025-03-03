namespace Store.Common.Interfaces.Repository;

public interface IChildGenericRepository<TModel, TPModel> : IRangeGenericRepository<TModel> where TModel : CommonModel
                                                                                            where TPModel : CommonModel
{
    public void AddRangeWithParent(TPModel parentModel, IEnumerable<TModel> children);
    
}