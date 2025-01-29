namespace Store;

internal abstract class Collectible<T> : SqlBuilder
{
    protected readonly Dictionary<int, T> Collectibles = [];


    /// <summary>
    /// Get the Collectibles stored in the class as a List of type T, this is done so because it prevents edits of the Collectible Dictionary stored in the class.    
    /// </summary>
    /// <returns>The values of Collectibles as a List</returns>
    internal List<T> GetValues() => Collectibles.Values.ToList();
    
    internal List<int> GetIds() => Collectibles.Keys.ToList();
    
    internal Dictionary<int, T> GetDictionary() => Collectibles;
    
    /// <summary>
    /// Get the specified Collectible from the dictionary
    /// </summary>
    /// <param name="id">The id to retrieve the Collectible with</param>
    /// <returns>The specified value</returns>
    internal T Get(int id) => Collectibles[id];
}