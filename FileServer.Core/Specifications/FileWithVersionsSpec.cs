using f= FileServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileServer.Core.Specifications
{
    public class FileWithVersionsSpec:BaseSpecifications<f.File>
    {
        public FileWithVersionsSpec(FileParams param):base(
            f => (string.IsNullOrEmpty(param.Search) || (f.FileName.ToLower().Contains(param.Search.ToLower())))&&
                 (!param.VersionNumber.HasValue || f.FileVersions.Any(v => v.VersionNumber == param.VersionNumber))&&
                 (string.IsNullOrEmpty(param.MimeType)|| (f.MimeType.ToLower().Contains(param.MimeType.ToLower())))
        )
        {
            Includes.Add(f => f.FileVersions);
            AddOrderBy(f => f.UploadDate);
            ApplyPagination(param.PageSize *(param.PageIndex-1), param.PageSize);
        }

        // get specified product
        public FileWithVersionsSpec(string id) : base(f => f.FileId.ToString() == id)
        {
            Includes.Add(p => p.FileVersions);
        }


    }
}
