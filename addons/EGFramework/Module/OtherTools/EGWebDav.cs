using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WebDav;


namespace EGFramework{
    public class EGWebDav : IModule
    {
        public string ServerUrl { set; get; } = "";
        private string UserName { set; get; } = "";
        private string Password { set; get; } = "";
        public bool IsInit { set; get; }
        private WebDavClient WebDavClient { set; get; }
        private string CurrentPath { set; get; } = "/";

        public List<WebDavFileMsg> CurrentFileList { set; get; } = new List<WebDavFileMsg>();
        public void Init()
        {
            
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }

        public void InitClient(string serverUrl, string userName,string password){
            this.ServerUrl = serverUrl;
            this.UserName = userName;
            this.Password = password;
            Dictionary<string,string> headersAdd = new Dictionary<string, string>
            {
                { "Connection", "keep-alive" },
                { "Authorization", "Basic "+ EGWebDavExtension.EncodeCredentials(userName,password) }
            };
            WebDavClient = new WebDavClient(new WebDavClientParams
            {
                BaseAddress = new Uri(ServerUrl),
                Credentials = new NetworkCredential(userName, password),
                DefaultRequestHeaders = headersAdd
            });
            Console.WriteLine("Client has been init");
        }
        
        //---------download or upload from WebDav server---------//

        /// <summary>
        /// Download a file from dav path
        /// </summary>
        /// <param name="downloadUri">Such as /dav/Picture/Picture1.jpg</param>
        /// <param name="localPath">download destination,such as C:\Users\W35\Pictures</param>
        /// <param name="fileName">you can define file by this name,or by uri</param>
        /// <returns></returns>
        public async Task<bool> DownloadFile(string downloadUri,string localPath,string fileName = ""){
            if (fileName.Equals("")){
                fileName = Path.GetFileName(downloadUri);
            }
            using (var response = await WebDavClient.GetRawFile(downloadUri)) 
            {
                if(response.IsSuccessful == true){
                    // use response.Stream
                    using (FileStream DestinationStream = File.Create(localPath + "/" + fileName))
                    {
                        await response.Stream.CopyToAsync(DestinationStream);
                        //Print("【WebDav】" + fileName + "下载成功！");
                    }
                    return true;
                }else{
                    return false;
                }
            }
        }
        public async Task<bool> DownloadFilProcessed(string downloadUri,string localPath,string fileName = ""){
            if (fileName.Equals("")){
                fileName = Path.GetFileName(downloadUri);
            }
            using (var response = await WebDavClient.GetProcessedFile(downloadUri)) 
            {
                if(response.IsSuccessful == true){
                    // use response.Stream
                    using (FileStream DestinationStream = File.Create(localPath + "/" + fileName))
                    {
                        await response.Stream.CopyToAsync(DestinationStream);
                        //Print("【WebDav】" + fileName + "下载成功！");
                    }
                    return true;
                }else{
                    return false;
                }
            }
        }

        /// <summary>
        /// Upload a file by localUrl
        /// </summary>
        /// <param name="localUrl">Such as C:\Users\W35\Pictures\Picture1.jpg</param>
        /// <param name="uploadPath">upload destination,such as /dav/Picture</param>
        /// <param name="fileName">you can define file by this name,or by local url</param>
        /// <returns></returns>
        public async Task<bool> UploadFile(string localUrl,string uploadPath,string fileName = ""){
            if (fileName.Equals("")){
                fileName = Path.GetFileName(localUrl);
            }
            // use response.Stream
            var result = await WebDavClient.PutFile(uploadPath+"/"+fileName, File.OpenRead(localUrl));
            if(result.IsSuccessful){
                return true;
            }else{
                return false;
            }
        }

        //-----------operate disk-----------//
        
        /// <summary>
        /// Default root path is "/",any path should be start with "/"
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public async Task<List<WebDavFileMsg>> GetList(string currentPath){
            PropfindResponse result = await WebDavClient.Propfind(ServerUrl+currentPath);
            List<WebDavFileMsg> ResultFileList = new List<WebDavFileMsg>();
            if (result.IsSuccessful)
            {
                foreach (WebDavResource res in result.Resources)
                {
                    ResultFileList.Add(new WebDavFileMsg{
                        FileName = res.DisplayName ,
                        IsCollection = res.IsCollection ,
                        ContentLength = res.ContentLength ,
                        Uri = res.Uri ,
                        LastUpdateTime = res.LastModifiedDate
                    });
                }
            }
            return ResultFileList;
        }

        /// <summary>
        /// simple CD command, prop find all file message to CurrentFileList.
        /// </summary>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public async Task ChangeDictionary(string destinationPath){
            CurrentPath = destinationPath;
            PropfindResponse result = await WebDavClient.Propfind(ServerUrl+CurrentPath);
            CurrentFileList.Clear();
            if (result.IsSuccessful)
            {
                foreach (WebDavResource res in result.Resources)
                {
                    CurrentFileList.Add(new WebDavFileMsg{
                        FileName = res.DisplayName ,
                        IsCollection = res.IsCollection ,
                        ContentLength = res.ContentLength ,
                        Uri = res.Uri ,
                        LastUpdateTime = res.LastModifiedDate
                    });
                }
            }
        }

        /// <summary>
        /// create a directory
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <returns></returns>
        public async Task MakeDictionary(string dictionaryName){
            await WebDavClient.Mkcol(dictionaryName);
        }
        
        /// <summary>
        /// simple cp command, copy a file with differentName.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="copyFile"></param>
        /// <returns></returns>
        public async Task Copy(string sourceFile,string copyFile){
            await WebDavClient.Copy(sourceFile,copyFile);
        }

        /// <summary>
        /// simple mv command, move a file with change fileName or different path.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="moveFile"></param>
        /// <returns></returns>
        public async Task Move(string sourceFile,string moveFile){
            await WebDavClient.Move(sourceFile,moveFile);
        }

        /// <summary>
        /// simple rm command,delete a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task Remove(string fileName){
            await WebDavClient.Delete(fileName);
        }
    }

    public struct WebDavFileMsg : IEGFileMsg{
        public string FileName { set; get; }
        public bool IsCollection { set; get; }

        /// <summary>
        /// unit is kb
        /// </summary>
        public long? ContentLength { set; get; }
        public string Uri { set; get; }
        public DateTime? LastUpdateTime { set; get; }

        public DateTime? LastModify  { set; get; }

        public void Init(string fileName, bool isCollection, string uri, long? contentLength = null, DateTime? lastModify = null)
        {
            this.FileName = fileName;
            this.IsCollection = isCollection;
            this.Uri = uri;
            this.ContentLength = contentLength;
            this.LastModify = lastModify;
        }
    }

    public static class EGWebDavExtension{
        public static EGWebDav EGWebDav(this IEGFramework self)
        {
            return EGArchitectureImplement.Interface.GetModule<EGWebDav>();
        }
        public static string EncodeCredentials(string username, string password)
        {
            string credentials = $"{username}:{password}";
            byte[] credentialsBytes = System.Text.Encoding.UTF8.GetBytes(credentials);
            string encodedCredentials = Convert.ToBase64String(credentialsBytes);
            return encodedCredentials;
        }
    }
}