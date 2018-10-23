using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebJobs.Extension.File.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class FileAccessAttribute: System.Attribute
    {
        [AutoResolve]
        public string FileName { get; set; }
        [AppSetting(Default ="Directory")]
        public string Folder { get; set; }
    }
}
