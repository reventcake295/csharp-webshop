using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Interfaces.Repository;
using Store.Common.Model;
using Store.Data.Models;

namespace Store.Data.Repository;

internal class DbUserRepository<TEntity> : DbRangeRepository<UserModel, TEntity>, IUserRepository where TEntity : class, IEntityModel<UserModel, TEntity>
{
    internal DbUserRepository(IDataStorage context) : base(context) { }
    
    public UserModel? GetByUsername(string username) => _context.User.FirstOrDefault(data => data.username == username)?.ToCommonModel();

    public bool UserExists(string username) => _context.User.Any(data => data.username == username);
    
    public UserModel? Login(string username, string password) => _context.User.FirstOrDefault(data => data.username == username && data.password == password)?.ToCommonModel();

    public void Register(UserModel userModel, string password)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        Console.WriteLine(password);
        UserData entity = new(userModel, password);
        _context.User.Add(entity);
        _context.UpdatedIds.AddModel(userModel, entity);
    }
    
    public override void Update(UserModel model)
    {
        if (_context.SessionState != DataSessionState.Started) throw new InvalidOperationException("Session is not started");
        UserData entity = new(model);
        // we normally would use this version but because this class is already so specific as to work only with one table,
        // we can just create the data model directly
//        TEntity? entity = (TEntity?)Activator.CreateInstance(typeof(TEntity), _BindingFlags, null, [model], null);
        _context.Set<UserData>().Update(entity);
        // due to the way the password is handled, we really cannot have the password be updated in this;
        // it will cause an exception on the password column
        _context.Entry(entity).Property(x => x.password).IsModified = false;
    }
}