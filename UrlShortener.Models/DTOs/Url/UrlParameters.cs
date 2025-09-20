
﻿using UrlShortener.Models.DTOs.Paging;

namespace UrlShortener.Models.DTOs.Url
{
    public class UrlParameters : RequestParameters
    {

        public UrlParameters() => OrderBy = "ID";

        private int? _minVisitCount = 0;
        private int? _maxVisitCount = int.MaxValue;
        public int? MinVisitCount
        {
            get => _minVisitCount;
            set => _minVisitCount = _maxVisitCount < value ? 0 : value;
        }
        public int? MaxVisitCount
        {
            get => _maxVisitCount;
            set => _maxVisitCount = _minVisitCount > value ? int.MaxValue : value;
        }

        private DateTime? _minDate = null;
        private DateTime? _maxDate = null;

        public DateTime? MaxDate
        {
            get => _maxDate;
            set => _maxDate = (_minDate.HasValue && _minDate <= value) ? value : DateTime.MaxValue;
        }
        public DateTime? MinDate
        {
            get => _minDate;
            set => _minDate = (_maxDate.HasValue && _maxDate >= value) ? value : DateTime.MinValue;
        }

        public string? SearchTerm { get; set; }
        public string? OrderBy { get; set; }
    }
}
