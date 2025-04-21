using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Renci.SshNet;

namespace EGFramework{
    public struct EGSftpHost
    {
        public string Host { set; get; }
        public string User { set; get; }
        public string Password { set; get; }
        public int Port { set; get; }
    }
    public class EGSftpSave : IEGSave,IEGSaveFile
    {
        public SftpClient Sftp { set; get; }
        
        /// <summary>
        /// Host is a json string, such as {"Host":"","User":"","Password":"","Port":22}
        /// </summary>
        /// <param name="hostJson"></param>
        public void InitSave(string hostJson)
        {
            EGSftpHost host = JsonConvert.DeserializeObject<EGSftpHost>(hostJson);
            if(host.Port == 0){
                host.Port = 22;
            }
            this.Sftp = new SftpClient(host.Host, host.Port, host.User, host.Password);
            // this.Sftp = new SftpClient(path);
        }

        public void CopyFile(string sourcePath, string copyPath)
        {
            throw new System.NotImplementedException();
        }

        public void DownloadFile(string remotePath, string localPath)
        {
            throw new System.NotImplementedException();
        }

        public Stream DownloadFile(string remotePath)
        {
            throw new System.NotImplementedException();
        }


        public bool IsRemoteDirectoryExist(string remotePath)
        {
            throw new System.NotImplementedException();
        }

        public bool IsRemoteFileExist(string remotePath)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IEGFileMsg> ListLocalFilePath(string localPath)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IEGFileMsg> ListRemoteFilePath(string remotePath)
        {
            throw new System.NotImplementedException();
        }

        public void MakeDirectory(string remotePath)
        {
            throw new System.NotImplementedException();
        }

        public void MoveFile(string sourcePath, string movePath)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveFile(string remotePath)
        {
            throw new System.NotImplementedException();
        }

        public void SyncFile(string remotePath, string localPath)
        {
            throw new System.NotImplementedException();
        }

        public void UploadFile(FileStream localFileStream, string remotePath)
        {
            throw new System.NotImplementedException();
        }

        public void UploadFile(string localPath, string remotePath)
        {
            throw new System.NotImplementedException();
        }
    }
}