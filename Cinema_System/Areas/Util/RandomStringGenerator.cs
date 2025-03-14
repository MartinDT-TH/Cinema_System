namespace Cinema_System.Areas.Util;

using System;

public class RandomStringGenerator
{
    private static Random _random = new Random();

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        char[] stringChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[_random.Next(chars.Length)];
        }

        return new string(stringChars);
    }
}
