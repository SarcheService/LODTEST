using System;
using System.Linq;

namespace LOD_APR.Helpers
{
    public static class RandomString
    {
        public static string GetRandomString(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopkrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetRandomIDString(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopkrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string pre = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            string post = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"{pre}-{post}";
        }
    }
}