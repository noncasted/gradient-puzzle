namespace Common;

public static class IdExtensions
{
    public static Guid ToGuid(this long id)
    {
        var guidBytes = new byte[16];

        BitConverter.GetBytes(id).CopyTo(guidBytes, 0);
        BitConverter.GetBytes(id).CopyTo(guidBytes, 8);

        return new Guid(guidBytes);
    }
}