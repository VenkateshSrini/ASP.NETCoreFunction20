using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebJobs.Extension.File.Attribute
{
    public class FileAccessExtension : IExtensionConfigProvider
    {
        // Root path where files are written. 
        // Used when attribute.Root is blank 
        // This is an example of extension-global configuration. 
        // Generally, attributes should be able to override these settings. 
        // Make sure these settings are Json serialization friendly. 
        [JsonProperty("Root")]
        public string Root { get; set; }
        public void Initialize(ExtensionConfigContext context)
        {
            // Register converters. These help convert between the user's parameter type
            //  and the type specified by the binding rules. 

            // This allows a user to bind to IAsyncCollector<string>, and the sdk
            // will convert that to IAsyncCollector<SampleItem>
            context.AddConverter<string, FileContent>(ConvertToItem);
            // This is useful on input binding. 
            context.AddConverter<FileContent, string>(ConvertToString);

            // Create 2 binding rules for the Sample attribute.
            var rule = context.AddBindingRule<FileAccessAttribute>();
            //On input binding to read from the file
            rule.BindToInput<FileContent>(BuildItemFromAttr);
            //on output binding to make call to AddAsync method in IAyncCollector<FileContent>
            rule.BindToCollector<FileContent>(BuildCollector);
        }
        private FileContent ConvertToItem(string arg)
        {
            var parts = arg.Split(':');
            return new FileContent
            {
                FileName = parts[0],
                Content = parts[1]
            };
        }
        private string ConvertToString(FileContent item)
        {
            return item.Content;
        }
        private string GetRoot(FileAccessAttribute attribute)
        {
            var root = attribute.Folder ?? this.Root ?? Path.GetTempPath();
            return root;
        }
        private IAsyncCollector<FileContent> BuildCollector(FileAccessAttribute attribute)
        {
            var root = GetRoot(attribute);
            return new FileAccessAsyncCollector(root);
        }
        private FileContent BuildItemFromAttr(FileAccessAttribute attribute)
        {
            var root = GetRoot(attribute);
            var path = Path.Combine(root, attribute.FileName);
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
            var contents = System.IO.File.ReadAllText(path);
            return new FileContent
            {
                FileName = attribute.FileName,
                Content = contents
            };
        }
    }
}
