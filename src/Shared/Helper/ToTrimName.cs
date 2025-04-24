namespace Shared.Helper;

public static class ToTrimName
{
    public static string ToFirstName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }
        var nameParts = name.Split(' ');
        if (nameParts.Length == 3)
        {
            return nameParts[1].Trim();
        }

        if (nameParts.Length == 2)
        {
            return nameParts[0].Trim();
        }
        return string.Empty;
    }

    public static string ToLastName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }
        var nameParts = name.Split(' ');

        if (nameParts.Length == 2)
        {
            return nameParts[1].Trim();
        }

        if (nameParts.Length == 3)
        {
            return nameParts[2].Trim();
        }
        return string.Empty;
    }
}
