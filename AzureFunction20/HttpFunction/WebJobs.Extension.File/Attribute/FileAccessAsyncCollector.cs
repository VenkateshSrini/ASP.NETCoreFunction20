using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebJobs.Extension.File.Attribute
{
    public class FileAccessAsyncCollector : IAsyncCollector<FileContent>
    {
        string RootPath { get; }
        public FileAccessAsyncCollector(string rootDirectory)
        {
            RootPath = rootDirectory;
        }
        public Task AddAsync(FileContent item, CancellationToken cancellationToken = default(CancellationToken))
        {
            var fullPath = Path.Combine(RootPath, item.FileName);
            Directory.CreateDirectory(Path.GetFullPath(RootPath));
            System.IO.File.AppendAllText(fullPath, $"{item.MessageID}:{item.Content}");
            return Task.CompletedTask;
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }


}
