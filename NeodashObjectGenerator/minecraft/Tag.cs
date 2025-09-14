using System.Text;

namespace NeodashObjectGenerator.Minecraft;
//http://web.archive.org/web/20110723210920/http://www.minecraft.net/docs/NBT.txt

public enum Type : byte
{
    TagEnd = 0,
    TagByte = 1,
    TagShort = 2,
    TagInt = 3,
    TagLong = 4,
    TagFloat = 5,
    TagDouble = 6,
    TagByteArray = 7,
    TagString = 8,
    TagList = 9,
    TagCompound = 10,
    TagIntArray = 11,
    TagLongArray = 12,
}

public abstract class Tag
{
    public Type Type;
    public TagString Name = null;
    public bool IsNamed = false;
    public abstract void Read(Stream data);
    public abstract void Write(Stream data);

    public virtual Tag this[string key]
    {
        get
        {
            throw new NotImplementedException("Indexing only possible with TAG_Compound");
        }
        set
        {
            throw new NotImplementedException("Indexing only possible with TAG_Compound");
        }
    }

    public virtual bool TryGetValue(string key, out Tag value)
    {
        throw new NotImplementedException("Indexing only possible with TAG_Compound");
    }

    public static explicit operator byte(Tag tag)
    {
        if (tag is TagByte tagByte)
            return tagByte.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to byte.", tag.GetType()));
    }
    
    public static explicit operator sbyte(Tag tag)
    {
        if (tag is TagByte tagByte)
            return (sbyte) tagByte.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to sbyte.", tag.GetType()));
    }

    public static explicit operator short(Tag tag)
    {
        if (tag is TagShort tagShort)
            return tagShort.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to short.", tag.GetType()));
    }

    public static explicit operator int(Tag tag)
    {
        if (tag is TagInt tagInt)
            return tagInt.Payload;
        else if (tag is TagString tagString && int.TryParse(tagString.PayloadString, out var value))
        {
            return value;
        }
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to int.", tag.GetType()));
    }

    public static explicit operator long(Tag tag)
    {
        if (tag is TagLong tagLong)
            return tagLong.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to long.", tag.GetType()));
    }

    public static explicit operator float(Tag tag)
    {
        if (tag is TagFloat tagFloat)
            return tagFloat.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to float.", tag.GetType()));
    }

    public static explicit operator double(Tag tag)
    {
        if (tag is TagDouble tagDouble)
            return tagDouble.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to double.", tag.GetType()));
    }

    public static explicit operator byte[](Tag tag)
    {
        if (tag is TagByteArray array)
            return array.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to byte[].", tag.GetType()));
    }

    public static explicit operator int[](Tag tag)
    {
        if (tag is TagIntArray array)
            return array.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to int[].", tag.GetType()));
    }
    
    public static explicit operator long[](Tag tag)
    {
        if (tag is TagLongArray array)
            return array.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to int[].", tag.GetType()));
    }

    public static explicit operator string(Tag tag)
    {
        if (tag is TagString tagString)
            return tagString.PayloadString;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to String.", tag.GetType()));
    }

    public static explicit operator Tag[](Tag tag)
    {
        if (tag is TagList list)
            return list.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to TAG[].", tag.GetType()));
    }

    public static explicit operator Dictionary<string, Tag>(Tag tag)
    {
        if (tag is TagCompound compound)
            return compound.Payload;
        else
            throw new InvalidCastException(string.Format("Unable to cast {0} to Dictionary<String, TAG>.", tag.GetType()));
    }
}

public class TagByte : Tag
{
    public byte Payload;

    public TagByte()
    {
        Payload = 0;
        Type = Type.TagByte;
    }

    public TagByte(byte payload)
        : this()
    {
        Payload = payload;
    }

    public TagByte(byte payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagByte(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        Payload = (byte)data.ReadByte();
    }

    public override void Write(Stream data)
    {
        data.WriteByte(Payload);
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Byte(\"{0}\"): {1}{2}", Name.PayloadString, Payload, Environment.NewLine);
        else
            return string.Format("TAG_Byte: {0}{1}", Payload, Environment.NewLine);
    }
}

public class TagShort : Tag
{
    public short Payload;

    public TagShort()
    {
        Payload = 0;
        Type = Type.TagShort;
    }

    public TagShort(short payload)
        : this()
    {
        Payload = payload;
    }

    public TagShort(short payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagShort(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        var temp = new byte[2];
        data.Read(temp, 0, 2);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        Payload = BitConverter.ToInt16(temp, 0);
    }

    public override void Write(Stream data)
    {
        var temp = BitConverter.GetBytes(Payload);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        data.Write(temp, 0, 2);
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Short(\"{0}\"): {1}{2}", Name.PayloadString, Payload, Environment.NewLine);
        else
            return string.Format("TAG_Short: {0}{1}", Payload, Environment.NewLine);
    }
}

public class TagInt : Tag
{
    public int Payload;

    public TagInt()
    {
        Payload = 0;
        Type = Type.TagInt;
    }

    public TagInt(int payload)
        : this()
    {
        Payload = payload;
    }

    public TagInt(int payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagInt(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        var temp = new byte[4];
        data.Read(temp, 0, 4);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        Payload = BitConverter.ToInt32(temp, 0);
    }

    public override void Write(Stream data)
    {
        var temp = BitConverter.GetBytes(Payload);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        data.Write(temp, 0, 4);
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Int(\"{0}\"): {1}{2}", Name.PayloadString, Payload, Environment.NewLine);
        else
            return string.Format("TAG_Int: {0}{1}", Payload, Environment.NewLine);
    }
}

public class TagLong : Tag
{
    public long Payload;

    public TagLong()
    {
        Payload = 0;
        Type = Type.TagLong;
    }

    public TagLong(long payload)
        : this()
    {
        Payload = payload;
    }

    public TagLong(long payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagLong(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        var temp = new byte[8];
        data.Read(temp, 0, 8);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        Payload = BitConverter.ToInt64(temp, 0);
    }

    public override void Write(Stream data)
    {
        var temp = BitConverter.GetBytes(Payload);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        data.Write(temp, 0, 8);
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Long(\"{0}\"): {1}{2}", Name.PayloadString, Payload, Environment.NewLine);
        else
            return string.Format("TAG_Long: {0}{1}", Payload, Environment.NewLine);
    }
}

public class TagFloat : Tag
{
    public float Payload;

    public TagFloat()
    {
        Payload = 0.0f;
        Type = Type.TagFloat;
    }

    public TagFloat(float payload)
        : this()
    {
        Payload = payload;
    }

    public TagFloat(float payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagFloat(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        var temp = new byte[4];
        data.Read(temp, 0, 4);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        Payload = BitConverter.ToSingle(temp, 0);
    }

    public override void Write(Stream data)
    {
        var temp = BitConverter.GetBytes(Payload);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        data.Write(temp, 0, 4);
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Float(\"{0}\"): {1}{2}", Name.PayloadString, Payload, Environment.NewLine);
        else
            return string.Format("TAG_Float: {0}{1}", Payload, Environment.NewLine);
    }
}

public class TagDouble : Tag
{
    public double Payload;

    public TagDouble()
    {
        Payload = 0.0d;
        Type = Type.TagDouble;
    }

    public TagDouble(double payload)
        : this()
    {
        Payload = payload;
    }

    public TagDouble(double payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagDouble(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        var temp = new byte[8];
        data.Read(temp, 0, 8);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        Payload = BitConverter.ToDouble(temp, 0);
    }

    public override void Write(Stream data)
    {
        var temp = BitConverter.GetBytes(Payload);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        data.Write(temp, 0, 8);
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Double(\"{0}\"): {1}{2}", Name.PayloadString, Payload, Environment.NewLine);
        else
            return string.Format("TAG_Double: {0}{1}", Payload, Environment.NewLine);
    }
}

public class TagByteArray : Tag
{
    public TagInt Length;
    public byte[] Payload;

    public TagByteArray()
    {
        Length = null;
        Payload = null;
        Type = Type.TagByteArray;
    }

    public TagByteArray(byte[] payload)
        : this()
    {
        Length = new TagInt(payload.Length);
        Payload = payload;
    }

    public TagByteArray(byte[] payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagByteArray(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        Length = new TagInt(data);

        Payload = new byte[Length.Payload];
        for (var i = 0; i < Length.Payload; i++)
            Payload[i] = (byte)data.ReadByte();
    }

    public override void Write(Stream data)
    {
        Length.Write(data);
        data.Write(Payload, 0, Payload.Length);
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Byte_Array(\"{0}\"): [{1} bytes]{2}", Name.PayloadString, Length.Payload, Environment.NewLine);
        else
            return string.Format("TAG_Byte_Array: [{0} bytes]{1}", Length.Payload, Environment.NewLine);
    }
}

public class TagIntArray : Tag
{
    public TagInt Length;
    public int[] Payload;

    public TagIntArray()
    {
        Length = null;
        Payload = null;
        Type = Type.TagIntArray;
    }

    public TagIntArray(int[] payload)
        : this()
    {
        Length = new TagInt(payload.Length);
        Payload = payload;
    }

    public TagIntArray(int[] payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagIntArray(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        Length = new TagInt(data);

        Payload = new int[Length.Payload];
        var temp = new byte[4];
        for (var i = 0; i < Length.Payload; i++)
        {
            data.Read(temp, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(temp);
            Payload[i] = BitConverter.ToInt32(temp, 0);
        }
    }

    public override void Write(Stream data)
    {
        Length.Write(data);
        for (var i = 0; i < Length.Payload; i++)
        {
            var temp = BitConverter.GetBytes(Payload[i]);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(temp);
            data.Write(temp, 0, 4);
        }
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Int_Array(\"{0}\"): [{1} integers]{2}", Name.PayloadString, Length.Payload, Environment.NewLine);
        else
            return string.Format("TAG_Int_Array: [{0} integers]{1}", Length.Payload, Environment.NewLine);
    }
}

public class TagLongArray : Tag
{
    public TagInt Length;
    public long[] Payload;

    public TagLongArray()
    {
        Length = null;
        Payload = null;
        Type = Type.TagIntArray;
    }

    public TagLongArray(long[] payload)
        : this()
    {
        Length = new TagInt(payload.Length);
        Payload = payload;
    }

    public TagLongArray(long[] payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagLongArray(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        Length = new TagInt(data);

        Payload = new long[Length.Payload];
        var temp = new byte[8];
        for (var i = 0; i < Length.Payload; i++)
        {
            data.Read(temp, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(temp);
            Payload[i] = BitConverter.ToInt64(temp, 0);
        }
    }

    public override void Write(Stream data)
    {
        Length.Write(data);
        for (var i = 0; i < Length.Payload; i++)
        {
            var temp = BitConverter.GetBytes(Payload[i]);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(temp);
            data.Write(temp, 0, 8);
        }
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_Long_Array(\"{0}\"): [{1} integers]{2}", Name.PayloadString, Length.Payload, Environment.NewLine);
        else
            return string.Format("TAG_Long_Array: [{0} integers]{1}", Length.Payload, Environment.NewLine);
    }
}

public class TagString : Tag
{
    public ushort Length;
    public byte[] Payload;
    public string PayloadString;

    public TagString()
    {
        Length = 0;
        Payload = null;
        PayloadString = null;
        Type = Type.TagString;
    }

    public TagString(string payloadString)
        : this()
    {
        Payload = Encoding.UTF8.GetBytes(payloadString);
        Length = (ushort) Payload.Length;
        PayloadString = payloadString;
    }

    public TagString(string payloadString, string name)
        : this(payloadString)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagString(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        Length = (ushort) new TagShort(data).Payload;

        Payload = new byte[Length];
        for (short i = 0; i < Length; i++)
            Payload[i] = (byte)data.ReadByte();
        PayloadString = Encoding.UTF8.GetString(Payload);
    }

    public override void Write(Stream data)
    {
        var temp = BitConverter.GetBytes(Length);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(temp);
        data.Write(temp, 0, 2);
        data.Write(Payload, 0, Payload.Length);
    }

    public void ChangePayload(string s)
    {
        Length = (ushort) s.Length;
        Payload = Encoding.UTF8.GetBytes(s);
        PayloadString = s;
    }

    public override string ToString()
    {
        if (IsNamed)
            return string.Format("TAG_String(\"{0}\"): {1}{2}", Name.PayloadString, PayloadString, Environment.NewLine);
        else
            return string.Format("TAG_String: {0}{1}", PayloadString, Environment.NewLine);
    }
}

public class TagList : Tag
{
    public TagByte TagId;
    public TagInt Length;
    public Tag[] Payload;

    public TagList()
    {
        TagId = null;
        Length = null;
        Payload = null;
        Type = Type.TagList;
    }

    public TagList(Tag[] payload, byte tagId)
        : this()
    {
        TagId = new TagByte(tagId);
        Length = new TagInt(payload.Length);
        Payload = payload;
    }

    public TagList(Tag[] payload, byte tagId, string name)
        : this(payload, tagId)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagList(Stream data)
        : this()
    {
        Read(data);
    }

    public override void Read(Stream data)
    {
        TagId = new TagByte();
        TagId.Read(data);

        Length = new TagInt();
        Length.Read(data);

        Payload = new Tag[Length.Payload];
        for (var i = 0; i < Length.Payload; i++)
        {
            switch ((Type)TagId.Payload)
            {
                case Type.TagByte:
                    Payload[i] = new TagByte(data);
                    break;
                case Type.TagShort:
                    Payload[i] = new TagShort(data);
                    break;
                case Type.TagInt:
                    Payload[i] = new TagInt(data);
                    break;
                case Type.TagLong:
                    Payload[i] = new TagLong(data);
                    break;
                case Type.TagFloat:
                    Payload[i] = new TagFloat(data);
                    break;
                case Type.TagDouble:
                    Payload[i] = new TagDouble(data);
                    break;
                case Type.TagByteArray:
                    Payload[i] = new TagByteArray(data);
                    break;
                case Type.TagString:
                    Payload[i] = new TagString(data);
                    break;
                case Type.TagList:
                    Payload[i] = new TagList(data);
                    break;
                case Type.TagCompound:
                    Payload[i] = new TagCompound(data, this);
                    break;
                case Type.TagIntArray:
                    Payload[i] = new TagIntArray(data);
                    break;
                case Type.TagLongArray:
                    Payload[i] = new TagLongArray(data);
                    break;
                default:
                    throw new Exception("Unrecognized tag type.");
            }
        }
    }

    public override void Write(Stream data)
    {
        TagId.Write(data);
        Length.Write(data);

        for (var i = 0; i < Length.Payload; i++)
        {
            Payload[i].Write(data);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (IsNamed)
            sb.AppendFormat("TAG_List(\"{0}\"): {1} entries of type {2}{3}", Name.PayloadString, Length.Payload, ((Type)TagId.Payload), Environment.NewLine);
        else
            sb.AppendFormat("TAG_List: {0} entries of type {1}{2}", Length.Payload, ((Type)TagId.Payload), Environment.NewLine);

        sb.AppendLine("{");

        for (var i = 0; i < Length.Payload; i++)
        {
            sb.Append(Payload[i].ToString());
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}

public class TagCompound : Tag
{
    public Dictionary<string, Tag> Payload;
    private bool _isRoot;
    private Tag _parent;

    public TagCompound()
    {
        _isRoot = false;
        _parent = null;
        Payload = null;
        Type = Type.TagCompound;
    }

    public TagCompound(Dictionary<string, Tag> payload)
        : this()
    {
        _isRoot = true;
        Payload = payload;
    }

    public TagCompound(Dictionary<string, Tag> payload, string name)
        : this(payload)
    {
        if (name != null && name.Length > 0)
        {
            Name = new TagString(name);
            IsNamed = true;
        }
    }

    public TagCompound(Stream data)
        : this()
    {
        _isRoot = true;
        Read(data);
    }

    internal TagCompound(Stream data, Tag parent)
        : this()
    {
        _parent = parent;
        Read(data);
    }

    public override Tag this[string key]
    {
        get
        {
            if (_isRoot && (!IsNamed || Name.Length == 0) && Payload.Count == 1)
            {
                var temp = Payload.GetEnumerator();
                temp.MoveNext();
                if (temp.Current.Value is TagCompound)
                {
                    return temp.Current.Value[key];
                }
            }
            return Payload[key];
        }
        set
        {
            if (_isRoot && (!IsNamed || Name.Length == 0) && Payload.Count == 1)
            {
                var temp = Payload.GetEnumerator();
                temp.MoveNext();
                if (temp.Current.Value is TagCompound)
                {
                    temp.Current.Value[key] = value;
                    return;
                }
            }
            Payload[key] = value;
        }
    }

    public override bool TryGetValue(string key, out Tag value)
    {
        if (_isRoot && (!IsNamed || Name.Length == 0) && Payload.Count == 1)
        {
            var temp = Payload.GetEnumerator();
            temp.MoveNext();
            if (temp.Current.Value is TagCompound)
            {
                return temp.Current.Value.TryGetValue(key, out value);
            }
        }
        return Payload.TryGetValue(key, out value);
    }

    public override void Read(Stream data)
    {
        Payload = new Dictionary<string, Tag>();
        while (true)
        {
            if (data.Position >= data.Length)
                break;
            var tagType = new TagByte(data);
            if (tagType.Payload == (sbyte)Type.TagEnd)
                break;
            var name = new TagString(data);
            Tag nextTag = null;
            switch ((Type)tagType.Payload)
            {
                case Type.TagByte:
                    nextTag = new TagByte(data);
                    break;
                case Type.TagShort:
                    nextTag = new TagShort(data);
                    break;
                case Type.TagInt:
                    nextTag = new TagInt(data);
                    break;
                case Type.TagLong:
                    nextTag = new TagLong(data);
                    break;
                case Type.TagFloat:
                    nextTag = new TagFloat(data);
                    break;
                case Type.TagDouble:
                    nextTag = new TagDouble(data);
                    break;
                case Type.TagByteArray:
                    nextTag = new TagByteArray(data);
                    break;
                case Type.TagString:
                    nextTag = new TagString(data);
                    break;
                case Type.TagList:
                    nextTag = new TagList(data);
                    break;
                case Type.TagCompound:
                    nextTag = new TagCompound(data, this);
                    break;
                case Type.TagIntArray:
                    nextTag = new TagIntArray(data);
                    break;
                case Type.TagLongArray:
                    nextTag = new TagLongArray(data);
                    break;
                default:
                    throw new Exception("Unrecognized tag type.");
            }
            nextTag.Name = name;
            nextTag.IsNamed = true;
            Payload.Add(name.PayloadString, nextTag);
        }
    }

    public override void Write(Stream data)
    {
        foreach (var pair in Payload)
        {
            data.WriteByte((byte)pair.Value.Type);
            if (pair.Value.IsNamed)
                pair.Value.Name.Write(data);
            pair.Value.Write(data);
        }
        data.WriteByte((byte)Type.TagEnd);
    }

    public override string ToString()
    {
        if ((_isRoot || (this._parent != null && this._parent is TagCompound compound && compound._isRoot)) && (!IsNamed || Name.Length == 0) && Payload.Count == 1)
        {
            var temp = Payload.GetEnumerator();
            temp.MoveNext();
            return temp.Current.Value.ToString();
        }
        var sb = new StringBuilder();
        if (IsNamed)
            sb.AppendFormat("TAG_Compound(\"{0}\"): {1} entries{2}", Name.PayloadString, Payload.Count, Environment.NewLine);
        else
            sb.AppendFormat("TAG_Compound: {0} entries{1}", Payload.Count, Environment.NewLine);

        sb.AppendLine("{");

        foreach (var t in Payload.Values)
        {
            sb.Append(t.ToString());
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}