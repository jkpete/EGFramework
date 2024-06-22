using Godot;
using System;
using EGFramework;
using WebDav;
using System.Threading.Tasks;
using System.Net;

public partial class EGMDnsTest : Node, IEGFramework
{
    private string serverUrl = "http://192.168.1.170:5212/dav";
    private string username = "ZK@zk.com";
    private string password = "uQTl7lzLSMZQ1QBd7sZZMlG2Gya65XKM";

    public override void _Ready()
    {
        //TestDav();
    }

    public async void TestDav(){
        this.EGWebDav().InitClient(serverUrl,username,password);
        await InitClient();
    }
    public async Task<bool> InitClient()
    {
        string result = EncodeCredentials(username,password);
        GD.Print(result);
        //ScanWebDavAndDownload();
        //await this.EGWebDav().DownloadFile("/dav/Picture/风景1.jpg",ProjectSettings.GlobalizePath("user://"),"风景1.jpg");
        //Print("download success!");
        //await this.EGWebDav().UploadFile(ProjectSettings.GlobalizePath("user://4.mp4"),"/dav/Picture","4.mp4");
        await this.EGWebDav().UploadFile(ProjectSettings.GlobalizePath("user://PPT_Test.jpg"),"/dav/Picture","PPT_Test.jpg");
        GD.Print("upload success!");
        //var result = await this.EGWebDav().GetList("/Video");
        //Print(JsonConvert.SerializeObject(result));
        return true;
    }
    public static string EncodeCredentials(string username, string password)
    {
        string credentials = $"{username}:{password}";
        byte[] credentialsBytes = System.Text.Encoding.UTF8.GetBytes(credentials);
        string encodedCredentials = Convert.ToBase64String(credentialsBytes);
        return encodedCredentials;
    }

}
