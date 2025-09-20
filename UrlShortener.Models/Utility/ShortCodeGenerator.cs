
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Models.Utility
{
    public static class ShortCodeGenerator
    {
       private static readonly Random _random = new Random();
       private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
       public static int Length = 5; // 64 ^ 5 = 1 Billion unique url
        public static string Get()
       {
           
           return new string(Enumerable.Repeat(_chars, Length)
               .Select(s => s[_random.Next(s.Length)]).ToArray());
       }
    }
}

