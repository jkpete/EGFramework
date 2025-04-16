using System.Collections.Generic;
using System.IO;

namespace EGFramework{
    public class EGFtpSave : IEGSave, IEGSaveFile
    {
        public void InitSave(string path)
        {
            throw new System.NotImplementedException();
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