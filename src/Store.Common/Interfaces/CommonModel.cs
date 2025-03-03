namespace Store.Common.Interfaces;

public abstract class CommonModel(int id) : IEquatable<CommonModel>
{
    public int Id { get; set; } = id;

    public override bool Equals(object? obj) => obj is CommonModel other && Equals(other);

    public override int GetHashCode()
    {
        return Id;
    }

    public static bool operator ==(CommonModel? left, CommonModel? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CommonModel? left, CommonModel? right)
    {
        return !Equals(left, right);
    }

    //    public override int GetHashCode()
//    {
//        return Id.GetHashCode();
//    }


    // This is the method that must be implemented to conform to the 
    // IEquatable contract

    public bool Equals(CommonModel? other)
    {
        if( other == null ) return false;
        if(ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

}