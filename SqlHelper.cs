using System.Reflection;
using MySqlConnector;

namespace Store;

internal static class SqlHelper
{
    /// <summary>
    /// map the output of a MysqlDataReader to a single class
    /// </summary>
    /// <param name="input">The MySqlDataReader with the input</param>
    /// <typeparam name="T">The class to map the result to</typeparam>
    /// <returns>Returns an instance with the results mapped</returns>
    internal static T MapToClass<T>(MySqlDataReader input) where T : SqlBuilder
    {
        T returnedObject = Activator.CreateInstance<T>();
        PropertyInfo[] modelProperties = returnedObject.GetType().GetProperties();
        foreach (PropertyInfo t in modelProperties)
        {
            MappingAttribute[] attributes = t.GetCustomAttributes<MappingAttribute>(true).ToArray();

            if (attributes.Length > 0)
                t.SetValue(returnedObject, Convert.ChangeType(input[attributes[0].ColumnName], t.PropertyType), null);
        }
        return returnedObject;
    }
    
    /// <summary>
    /// map the output of a MysqlDataReader to a single class type and return it in an array
    /// </summary>
    /// <param name="input">The MySqlDataReader with the input</param>
    /// <typeparam name="T">The class to map the result to</typeparam>
    /// <returns>Returns an array of instances with the results mapped</returns>
    internal static T[] MapToClassArray<T>(MySqlDataReader input) where T : SqlBuilder
    {
        // create the returning object
        List<T> returnedObject = [];
        
        // get the properties of the class, this is done this way for the array processing because it reduces the amount of overhead
        // to either create a separate object or get the properties again each loop
        PropertyInfo[] modelProperties = typeof(T).GetProperties();
        
        // walk through the result set given and match it to the class Properties
        
        while (input.Read())
        {
            T currentObject = Activator.CreateInstance<T>();
            foreach (PropertyInfo t in modelProperties)
            {
                MappingAttribute[] attributes = t.GetCustomAttributes<MappingAttribute>(true).ToArray();

                if (attributes.Length > 0)
                    t.SetValue(currentObject, Convert.ChangeType(input[attributes[0].ColumnName], t.PropertyType), null);
            
            }
            returnedObject.Add(currentObject);
        }
        return returnedObject.ToArray();
    }
}

[AttributeUsage(AttributeTargets.Property)]
[Serializable]
public class MappingAttribute : Attribute
{
    public required string ColumnName;
}