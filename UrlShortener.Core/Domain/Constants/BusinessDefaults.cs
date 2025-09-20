using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Models.Enums;

namespace UrlShortener.Core.Domain.Constants
{
    public static class BusinessDefaults
    {
        public static enRole       DefaultRole = enRole.User;
        public static enPlan       DefaultPlan = enPlan.Basic;
        public static enPermission DefaultPermission = 0;
    }
}
