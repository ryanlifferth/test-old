using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ryan.AngularJS.Models
{
    public class AjaxResponse
    {
        public bool MulitplePropertiesFound { get; set; }
        public string StatusMessage { get; set; }
        public List<DuplicateParcelInfo> DuplicateParcelInfoList { get; set; }
    }

    public class DuplicateParcelInfo
    {
        public string FullAddress { get; set; }
        public string Apn { get; set; }
        public string MlsNumber { get; set; }
        public Guid PropertySourceId { get; set; }
        public Guid RawSourceId { get; set; }
    }
}