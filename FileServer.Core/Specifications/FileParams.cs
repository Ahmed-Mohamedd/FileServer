using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Specifications
{
    public class FileParams
    {
        const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;
        private int pageSize = 5;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

       // public string? Version { get; set; }
        public int? VersionNumber { get; set; }

        public string? MimeType { get; set; }
        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value.ToLower(); }
        }
    }
}
