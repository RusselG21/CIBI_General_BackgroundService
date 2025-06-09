namespace Shared.Helper;

public static class ToTrimName
{
    public static string ToFirstName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        var nameParts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return string.Join(" ", nameParts.Take(nameParts.Length - 1)); ;
    }

    public static string ToLastName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        var nameParts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return nameParts[^1];
    }
}
