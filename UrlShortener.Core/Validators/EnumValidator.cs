using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Core.Validators
{
    public class EnumValidator<T> where T : Enum
    {
        public static bool IsDefined(object Value)
            => Enum.IsDefined(typeof(T), Value);

    }
}
