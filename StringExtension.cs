using System.Text;

namespace Cqrs;

public static class StringExtension
{
    public static string Repeat(this string str, int repeatNum)
    {
        switch (repeatNum)
        {
            case 0:
                return string.Empty;
            case 1:
                return str;
        }

        var sb = new StringBuilder(str);
        while (repeatNum > 1)
        {
            sb.Append(str);
            repeatNum--;
        }

        return sb.ToString();
    }
}