using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace EGFramework{
    /// <summary>
    /// This class is used to save files locally. such as u disk or other local path. used for backup or other purpose.
    /// </summary>
    public class EGLocalFileSave : IEGSave, IEGSaveFile
    {
        public string RootPath { get; set; } 

        public void InitSave(string path)
        {
            this.RootPath = path;    
        }

        public void CopyFile(string sourcePath, string copyPath)
        {
            if(IsRemoteFileExist(sourcePath)){
                string sourceCombinedPath = Path.Combine(RootPath, sourcePath);
                string copyCombinedPath = Path.Combine(RootPath, copyPath);
                File.Copy(sourceCombinedPath,copyCombinedPath,true);
            }else{
                throw new FileNotFoundException("File not exist!");
            }
        }

        public void DownloadFile(string remotePath, string localPath)
        {
            if(IsRemoteFileExist(remotePath)){
                string combinedPath = Path.Combine(RootPath, remotePath);
                File.Copy(combinedPath,localPath,true);
            }else{
                throw new FileNotFoundException("File not exist!");
            }
        }

        public Stream DownloadFile(string remotePath)
        {
            if(IsRemoteFileExist(remotePath)){
                string combinedPath = Path.Combine(RootPath, remotePath);
                FileStream fileStream = new FileStream(combinedPath,FileMode.Open,FileAccess.Read);
                return fileStream;
            }else{
                throw new FileNotFoundException("File not exist!");
            }
        }

        public bool IsRemoteDirectoryExist(string remotePath)
        {
            string combinedPath = Path.Combine(RootPath, remotePath);
            return Directory.Exists(combinedPath);
        }

        public bool IsRemoteFileExist(string remotePath)
        {
            string combinedPath = Path.Combine(RootPath, remotePath);
            return File.Exists(combinedPath);
        }

        public IEnumerable<IEGFileMsg> ListLocalFilePath(string localPath)
        {
            string [] filePaths = Directory.GetFiles(localPath);
            string [] directoryPaths = Directory.GetDirectories(localPath);
            List<IEGFileMsg> fileMsgs = new List<IEGFileMsg>();
            foreach(string filePath in filePaths){
                FileInfo fileInfo = new FileInfo(filePath);
                EGFileMsg msg = new EGFileMsg();
                msg.Init(fileInfo.Name,false,filePath,fileInfo.Length,fileInfo.LastWriteTime);
                fileMsgs.Add(msg);
            }
            foreach(string directoryPath in directoryPaths){
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                EGFileMsg msg = new EGFileMsg();
                msg.Init(directoryInfo.Name,true,directoryPath,null,directoryInfo.LastWriteTime);
                fileMsgs.Add(msg);
            }
            return fileMsgs;
        }

        public IEnumerable<IEGFileMsg> ListRemoteFilePath(string remotePath)
        {
            string combinedPath = Path.Combine(RootPath, remotePath);
            string [] filePaths = Directory.GetFiles(combinedPath);
            string [] directoryPaths = Directory.GetDirectories(combinedPath);
            List<IEGFileMsg> fileMsgs = new List<IEGFileMsg>();
            foreach(string filePath in filePaths){
                FileInfo fileInfo = new FileInfo(filePath);
                EGFileMsg msg = new EGFileMsg();
                msg.Init(fileInfo.Name,false,filePath,fileInfo.Length,fileInfo.LastWriteTime);
                fileMsgs.Add(msg);
            }
            foreach(string directoryPath in directoryPaths){
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                EGFileMsg msg = new EGFileMsg();
                msg.Init(directoryInfo.Name,true,directoryPath,null,directoryInfo.LastWriteTime);
                fileMsgs.Add(msg);
            }
            return fileMsgs;
        }

        public void MakeDirectory(string remotePath)
        {
            string combinedPath = Path.Combine(RootPath, remotePath);
            if(Directory.Exists(combinedPath) == false){
                Directory.CreateDirectory(combinedPath);
            }
        }

        public void MoveFile(string sourcePath, string movePath)
        {
            if(IsRemoteFileExist(sourcePath)){
                string sourceCombinedPath = Path.Combine(RootPath, sourcePath);
                string moveCombinedPath = Path.Combine(RootPath, movePath);
                File.Move(sourceCombinedPath,moveCombinedPath);
            }else{
                throw new FileNotFoundException("File not exist!");
            }
        }

        public void RemoveFile(string remotePath)
        {
            if(IsRemoteFileExist(remotePath)){
                string combinedPath = Path.Combine(RootPath, remotePath);
                File.Delete(combinedPath);
            }else{
                throw new FileNotFoundException("File not exist!");
            }
        }

        public void SyncFile(string remotePath, string localPath)
        {
            FileInfo fileInfo = new FileInfo(localPath);
            if(fileInfo.Exists == false){
                throw new FileNotFoundException("Local File not exist!");
            }
            FileInfo remoteFileInfo = new FileInfo(remotePath);
            if(remoteFileInfo.Exists == false){
                throw new FileNotFoundException("Remote File not exist!");
            }
            if(fileInfo.LastWriteTime > remoteFileInfo.LastWriteTime){
                string combinedPath = Path.Combine(RootPath, remotePath);
                File.Copy(localPath,combinedPath,true);
            }else if(fileInfo.LastWriteTime < remoteFileInfo.LastWriteTime){
                string combinedPath = Path.Combine(RootPath, remotePath);
                File.Copy(combinedPath,localPath,true);
            }else{
                EG.Print("File is same, no need to sync!");
            }
        }

        public void UploadFile(FileStream localFileStream, string remotePath)
        {
            string combinedPath = Path.Combine(RootPath, remotePath);
            FileStream fileStream = new FileStream(combinedPath,FileMode.Create,FileAccess.Write);
            localFileStream.CopyTo(fileStream);
            fileStream.Close();
            localFileStream.Close();
            fileStream.Dispose();
        }

        public void UploadFile(string localPath, string remotePath)
        {
            if(File.Exists(localPath)){
                string combinedPath = Path.Combine(RootPath, remotePath);
                File.Copy(localPath,combinedPath,true);
            }else{
                throw new FileNotFoundException("File not exist!");
            }
        }
    }
}