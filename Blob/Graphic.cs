namespace Blob;

using System.Data;
using System.Data.Common;
using FluentNHibernate.Mapping;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NpgsqlTypes;
using ProtoBuf;

[ProtoContract]
public class Graphic<T>
    where T : struct, IComparable<T>
{
    [ProtoMember(1)]
    public virtual int Id { get; set; }
    
    [ProtoMember(2)]
    public virtual string Name { get; set; }
    
    [ProtoMember(3)]
    public virtual SortedSet<Point<T>> Points { get; set; }

    public virtual object Clone()
    {
        return new Graphic<T>
        {
            Id = Id,
            Name = Name,
            Points = new SortedSet<Point<T>>(Points.Select(p => (Point<T>)p.Clone()))
        };
    }
}

public class GraphicMap : ClassMap<Graphic<double>>
{
    public GraphicMap()
    {
        Table("graphics");
        
        Id(item => item.Id, "id").GeneratedBy.Increment();
        Map(item => item.Name, "name");
        Map(item => item.Points, "graphic").CustomType<ByteaValue<SortedSet<Point<double>>>>();
    }
}

public class ByteaValue<T> : IUserType 
    where T : class
{
    public bool Equals(object? x, object? y)
    {
        return object.Equals(x, y);
    }

    public int GetHashCode(object? x)
    {
        return x?.GetHashCode() ?? 0;
    }

    public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
        if (names.Length != 1)
        {
            throw new InvalidOperationException("Only expecting one column...");
        }

        if (rs == null)
        {
            throw new ArgumentNullException(nameof(rs));
        }

        if (rs[names[0]] is byte[] val)
        {
            return ProtoBufSerializerWithCompress.Deserialize<T>(val);
        }

        return null;
    }

    public void NullSafeSet(DbCommand cmd, object? value, int index, ISessionImplementor session)
    {
        var parameter = cmd.Parameters[index];

        if (value is not T convertedValue)
        {
            parameter.Value = DBNull.Value;
            return;
        }

        parameter.Value = ProtoBufSerializerWithCompress.Serialize(convertedValue);
    }

    public object DeepCopy(object value)
    {
        return value switch
        {
            ICloneable cloneable => cloneable.Clone(),
            //T t => ProtoBufSerializerWithCompress.DeepCopy(t),
            _ => value
        };
    }

    public object Replace(object original, object target, object owner)
    {
        return original;
    }

    public object? Assemble(object cached, object owner)
    {
        return cached is not byte[] bytes 
            ? null 
            : ProtoBufSerializerWithCompress.Deserialize<T>(bytes);
    }

    public object Disassemble(object? value)
    {
        return ProtoBufSerializerWithCompress.Serialize(value as T);
    }

    public SqlType[] SqlTypes => new SqlType[]
    {
        new NpgsqlExtendedSqlType(DbType.Object, NpgsqlDbType.Bytea)
    };

    public Type ReturnedType => typeof(T);

    public bool IsMutable => true;
}

public class NpgsqlExtendedSqlType : SqlType
{
    public NpgsqlExtendedSqlType(DbType dbType, NpgsqlDbType npgDbType)
        : base(dbType)
    {
        NpgDbType = npgDbType;
    }

    public NpgsqlDbType NpgDbType { get; }
}

