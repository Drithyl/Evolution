

public abstract class Sex
{
    public enum Types
    {
        Male,
        Female,
        Any
    };

    public static Types OppositeSex(Types type)
    {
        if (type == Types.Any)
            return Types.Any;

        else if (type == Types.Male)
            return Types.Female;

        else return Types.Male;
    }
}
