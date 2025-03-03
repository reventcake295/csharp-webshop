using System.Linq.Expressions;
using System.Reflection;
using Store.Data.Enums;
using Store.Data.Attributes;
using Store.Common.Interfaces;

namespace Store.Data;

internal class ModelExpressionConverter<TModel, TEntity> : ExpressionVisitor where TModel : CommonModel
                                                                                   where TEntity : class, IEntityModel<TModel, TEntity>
{
    private readonly ParameterExpression _entityParameter;
    private readonly Dictionary<string, (string EntityProperty, Func<Expression, Expression>? Converter)> _propertyMappings = new();

    public ModelExpressionConverter()
    {
        _entityParameter = Expression.Parameter(typeof(TEntity), "entity");
        
        PropertyInfo[] entityProperties = typeof(TEntity).GetProperties();
        Dictionary<string, PropertyInfo> modelProperties = typeof(TModel).GetProperties().ToDictionary(p => p.Name);
        
        foreach (PropertyInfo entityProperty in entityProperties)
        {
            ModelMappingAttribute? attribute = entityProperty.GetCustomAttribute<ModelMappingAttribute>();
            if (attribute == null) return;
            // if the Property is disabled then we don't add 
            if (attribute.PropertyType == PropertyType.Disabled) continue;
            PropertyInfo? modelProperty = modelProperty = modelProperties.GetValueOrDefault(attribute.Name);
            if (modelProperty == null) continue;
            if (attribute.PropertyType == PropertyType.Model)
            {
                if (modelProperty.PropertyType.IsClass && entityProperty.PropertyType.IsValueType && attribute.Accessor != null)
                { // Automatically map object to its key property (e.g., Department -> DepartmentId)
                    _propertyMappings.Add(attribute.Accessor, (entityProperty.Name, GetConverter(attribute.PropertyType)));
                }
                continue;
            }
            
            _propertyMappings.Add(attribute.Name, (entityProperty.Name, GetConverter(attribute.PropertyType)));
        }
    }

    public Expression<Func<TEntity, bool>> Convert(Expression<Func<TModel, bool>> modelExpression) =>
        Expression.Lambda<Func<TEntity, bool>>(Visit(modelExpression.Body), _entityParameter);

    protected override Expression VisitParameter(ParameterExpression node) => _entityParameter; // Replace parameter with TEntity parameter

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is { NodeType: ExpressionType.Parameter })
        {
            // Check if there's a mapped property in TEntity
            if (_propertyMappings.TryGetValue(node.Member.Name, out (string EntityProperty, Func<Expression, Expression>? Converter) entityPropertyName))
            {
                PropertyInfo? entityProperty = typeof(TEntity).GetProperty(entityPropertyName.EntityProperty);

                if (entityProperty != null)
                {
                    MemberExpression entityMember = Expression.MakeMemberAccess(_entityParameter, entityProperty);

                    // Apply type conversion if necessary
                    return entityPropertyName.Converter != null ? entityPropertyName.Converter(entityMember) : entityMember;
                }
            }
        } else if (node.Expression is { NodeType: ExpressionType.MemberAccess })
        {
            if (_propertyMappings.TryGetValue($"{node.Expression.Type.Name}.Id",
                                              out (string EntityProperty, Func<Expression, Expression>? Converter) entityPropertyName))
            {
                PropertyInfo? entityProperty = typeof(TEntity).GetProperty(entityPropertyName.EntityProperty);
                
                if (entityProperty != null)
                    return Expression.MakeMemberAccess(_entityParameter, entityProperty);
            }
        }
        return base.VisitMember(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        // If the node is a conversion and we are converting an Enum to int, handle it
        if (node.NodeType == ExpressionType.Convert && node.Type == typeof(int) && node.Operand.Type.IsEnum)
            return Visit(node.Operand); // Strip the Convert expression
        
        return base.VisitUnary(node);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        // Handle cases like: model.Age > 30, where Age in model is string but int in entity
        Expression left = Visit(node.Left);
        Expression right = Visit(node.Right);

        // Ensure both sides of the expression are of the same type
        if (left.Type != right.Type)
        {
            right = Expression.Convert(right, left.Type);
        }

        return Expression.MakeBinary(node.NodeType, left, right);
    }
    
    private Func<Expression, Expression>? GetConverter(PropertyType type) => type switch
        {
            PropertyType.Simple => null,
            PropertyType.Enum   => expr => Expression.Convert(expr, typeof(int)),
            PropertyType.Model  => null, // model conversion is not needed because the accessor already solves the issue on its own
            PropertyType.Disabled => null,
            _                   => null
        };
}
