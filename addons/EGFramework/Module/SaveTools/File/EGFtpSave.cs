using System.Collections.Generic;
using System.IO;
using FluentFTP;

namespace EGFramework{
    public class EGFtpSave : IEGSave, IEGSaveFile
    {
        public FtpClient FTPClient { set; get; }

        public void InitSave(string host)
        {
            this.FTPClient = new FtpClient(host);
        }
        public void InitUser(string user, string password)
        {
            this.FTPClient.Credentials = new System.Net.NetworkCredential(user, password);
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

        public IEnumerable<IEGFileMsg> ListRemoteFilePath(string remotePath) 
        {
            FTPClient.Connect();
            FtpListItem[] nameList = FTPClient.GetListing(remotePath);
            List<IEGFileMsg> fileList = new List<IEGFileMsg>();
            foreach (var item in nameList)
            {
                IEGFileMsg fileMsg = new EGFileMsg();
                fileMsg.Init(item.Name, item.Type == FtpObjectType.Directory, item.FullName, item.Size/1024, item.Modified);
                fileList.Add(fileMsg);
            }
            FTPClient.Disconnect();
            return fileList;
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
            FTPClient.Connect();
            FTPClient.DownloadFile(localPath, remotePath, FtpLocalExists.Overwrite);
            FTPClient.Disconnect();
        }

        public void UploadFile(FileStream localFileStream, string remotePath)
        {
            throw new System.NotImplementedException();
        }

        public void UploadFile(string localPath, string remotePath)
        {
            FTPClient.Connect();
            FTPClient.UploadFile(localPath, remotePath, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            FTPClient.Disconnect();
        }
    }
}