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
            FTPClient.Connect();
            Stream fileStream = FTPClient.OpenRead(sourcePath);
            FTPClient.UploadStream(fileStream, copyPath, FtpRemoteExists.Overwrite, true);
            fileStream.Close();
            FTPClient.Disconnect();
        }

        public void DownloadFile(string remotePath, string localPath)
        {
            FTPClient.Connect();
            FTPClient.DownloadFile(localPath, remotePath, FtpLocalExists.Overwrite);
            FTPClient.Disconnect();
        }

        public Stream DownloadFile(string remotePath)
        {
            FTPClient.Connect();
            Stream fileStream = FTPClient.OpenRead(remotePath);
            FTPClient.Disconnect();
            return fileStream;
        }

        public bool IsRemoteDirectoryExist(string remotePath)
        {
            FTPClient.Connect();
            bool isExist = FTPClient.DirectoryExists(remotePath);
            FTPClient.Disconnect();
            return isExist;
        }

        public bool IsRemoteFileExist(string remotePath)
        {
            FTPClient.Connect();
            bool isExist = FTPClient.FileExists(remotePath);
            FTPClient.Disconnect();
            return isExist;
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
            FTPClient.Connect();
            FTPClient.CreateDirectory(remotePath, true);
            FTPClient.Disconnect();
        }

        public void MoveFile(string sourcePath, string movePath)
        {
            FTPClient.Connect();
            FTPClient.MoveFile(sourcePath, movePath ,FtpRemoteExists.Overwrite);
            FTPClient.Disconnect();
        }

        public void RemoveFile(string remotePath)
        {
            FTPClient.Connect();
            FTPClient.DeleteFile(remotePath);
            FTPClient.Disconnect();
        }

        public void SyncFile(string remotePath, string localPath)
        {
            FTPClient.Connect();
            FTPClient.DownloadFile(localPath, remotePath, FtpLocalExists.Overwrite);
            FTPClient.Disconnect();
        }

        public void UploadFile(FileStream localFileStream, string remotePath)
        {
            FTPClient.Connect();
            FTPClient.UploadStream(localFileStream, remotePath, FtpRemoteExists.Overwrite, true);
            FTPClient.Disconnect();
        }

        public void UploadFile(string localPath, string remotePath)
        {
            FTPClient.Connect();
            FTPClient.UploadFile(localPath, remotePath, FtpRemoteExists.Overwrite, true, FtpVerify.Retry);
            FTPClient.Disconnect();
        }
    }
}