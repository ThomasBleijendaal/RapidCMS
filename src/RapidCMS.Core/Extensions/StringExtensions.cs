using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RapidCMS.Core.Extensions
{
    internal static class StringExtensions
    {
        // https://stackoverflow.com/questions/25259/how-does-stack-overflow-generate-its-seo-friendly-urls
        public static string ToUrlFriendlyString(this string title)
        {
            if (title == null)
            {
                return "";
            }

            var len = title.Length;
            var prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (var i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    var prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length)
                    {
                        prevdash = false;
                    }
                }
            }

            return prevdash
                ? sb.ToString().Substring(0, sb.Length - 1)
                : sb.ToString();
        }

        private static string RemapInternationalCharToAscii(char c)
        {
            var s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }

        private readonly static SHA1CryptoServiceProvider Sha1 = new SHA1CryptoServiceProvider();
        private readonly static char[] Padding = { '=' };

        public static string ToSha1Base64String(this string text, string? salt = default)
        {
            return Sha1.ComputeHash(Encoding.UTF8.GetBytes($"{text}{salt}")).ToBase64String();
        }

        public static string ToBase64String(this string text)
        {
            return Encoding.UTF8.GetBytes(text).ToBase64String();
        }

        private static string ToBase64String(this byte[] text)
        {
            return Convert.ToBase64String(text)
                .TrimEnd(Padding)
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string FromBase64String(this string text)
        {
            var preString = text?.Replace('-', '+').Replace('_', '/');

            if (string.IsNullOrWhiteSpace(preString))
            {
                return string.Empty;
            }

            var originalString = Encoding.UTF8.GetString(
                Convert.FromBase64String($"{preString}{string.Join("", Enumerable.Repeat(Padding.First(), preString.Length % 4))}"));

            return originalString;
        }
    }
}
