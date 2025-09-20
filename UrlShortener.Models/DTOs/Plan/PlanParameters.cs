
﻿using System.Linq.Expressions;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.Enums;

namespace UrlShortener.Models.DTOs.Plan
{
    public class PlanParameters : RequestParameters
    {

        public PlanParameters() => OrderBy = "name";

        public static enSupportLevel[] SupportLevelList = Enum.GetValues<enSupportLevel>();
        private uint _maxPrice = int.MaxValue;
        private uint _minPrice = 0;
        
        public uint MinPrice
        {
            get => _minPrice;
            set => _minPrice = _maxPrice < value ? 0 : value;
        }
        public uint MaxPrice
        {
            get => _maxPrice;
            set => _maxPrice = _minPrice > value ? int.MaxValue : value;
        }

        public bool? HasCustomSlugs { get; set; } = null;
        public IEnumerable<enSupportLevel>? SupportLevels { get; set; }

        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; }
    }
}