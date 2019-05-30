using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OPIDChecks.Models
{
    public class FileViewModel
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
    }
}