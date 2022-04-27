namespace Blob;

using ProtoBuf;

[ProtoContract]
public class Point<T> : IComparable<Point<T>>, ICloneable
    where T : struct, IComparable<T>
{
    [ProtoMember(1)]
    public T X { get; set; }

    [ProtoMember(2)]
    public T Y { get; set; }

    public int CompareTo(Point<T>? other)
    {
        return X.CompareTo(other!.X);
    }

    public object Clone()
    {
        return new Point<T>
        {
            X = X,
            Y = Y
        };
    }
}