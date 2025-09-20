
﻿using UrlShortener.Models.DTOs.Paging;

namespace UrlShortener.Models.DTOs.User
{
    public class UserParameters : RequestParameters
    {

        public UserParameters() => OrderBy = "Id";
        public IEnumerable<int> PlanId { get; set; }

        private DateTime? _minDate = null;
        private DateTime? _maxDate = null;

        public DateTime? MaxDate 
        { 
            get => _maxDate;
            set => _maxDate = (_minDate.HasValue && _minDate <= value) ? value : DateTime.MaxValue; 
        }
        public DateTime? MinDate { 
            get => _minDate;
            set => _minDate = (_maxDate.HasValue && _maxDate >= value) ? value : DateTime.MinValue; 
        }

        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; }
    }
}