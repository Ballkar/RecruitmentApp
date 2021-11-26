using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Services
{
    public interface IFileService
    {
        public string GetUserFilesUrl(int? id);
    }

    public class FileService : IFileService
    {
        public FileService()
        {

        }

        public string GetUserFilesUrl(int? id)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var path = $"{rootPath}/PrivateFiles/{id}/";
            if (!Directory.Exists(path))
            {
                DirectoryInfo di = Directory.CreateDirectory($"{rootPath}/PrivateFiles/{id}/");
            }
            return path;
        }
    }
}

