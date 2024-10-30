using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{
    public class FileHandler
    {
        public async Task<bool> FileExists(string path)
        {
            return await Task.Run(()=> File.Exists(path));
        }

        public async Task<string> GetFileName(string path)
        {
            if (await FileExists(path))
            {
                return new FileInfo(path).Name;
            }

            throw new Exception("File does not exist");
        }

        public async Task<long> GetFileSize(string path)
        {
            if (await FileExists(path))
            {
                return new FileInfo(path).Length;
            }

            throw new Exception("File does not exist");
        }

        public async Task DeleteFile(string path)
        {
            if (await FileExists(path))
            {
                File.Delete(path);
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }
    }
}