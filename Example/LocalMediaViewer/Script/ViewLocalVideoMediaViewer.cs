using Godot;
using System;
using EGFramework;
using System.IO;

public partial class ViewLocalVideoMediaViewer : Node,IEGFramework
{
    public EGLocalFileSave localFileSave { get; set; }
    public VideoStreamPlayer ViewVideo { get; set; }
    public TextureRect ViewImage { get; set; }
    public VBoxContainer VideoList { get; set; }
    public FileDialog ImportImage { get; set; }
    public FileDialog ImportVideo { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ViewVideo = this.GetNode<VideoStreamPlayer>("ViewVideo");
        ViewImage = this.GetNode<TextureRect>("ViewImage");
        VideoList = this.GetNode<VBoxContainer>("Tab/Media/MediaList");
        ImportImage = this.GetNode<FileDialog>("ImportImage");
        ImportVideo = this.GetNode<FileDialog>("ImportVideo");
        LoadFileSystem();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
    }
    public void LoadFileSystem(){
        this.localFileSave = new EGLocalFileSave();
        localFileSave.RootPath = "SaveData";
        if(!localFileSave.IsRemoteDirectoryExist("Media")){
            localFileSave.MakeDirectory("Media");
        }
        RefreshList();
    }
    public void RefreshList(){
        string combinedPath = Path.Combine(localFileSave.RootPath, "Media");
        VideoList.ClearChildren();
        foreach (var file in localFileSave.ListRemoteFilePath("Media")){
            if(file.FileName.GetStrBehindSymbol('.')!="ogv" && file.FileName.GetStrBehindSymbol('.')!="jpg" && file.FileName.GetStrBehindSymbol('.')!="png"){
                continue;
            }
            Button button= new Button();
            button.Name = file.FileName;
            button.Text = file.FileName;
            string combinedFile = Path.Combine(combinedPath, button.Text);
            button.Connect("pressed",Callable.From(()=>{
                if(button.Text.GetStrBehindSymbol('.')=="ogv"){
                    PlayVideo(combinedFile);
                }
                if(button.Text.GetStrBehindSymbol('.')=="jpg" || button.Text.GetStrBehindSymbol('.')=="png"){
                    PlayImage(combinedFile);
                }
            }));
            VideoList.AddChild(button);
        }
    }

    public void PlayVideo(string videoName){
        ViewVideo.Visible = true;
        ViewImage.Visible = false;
        GD.Print("PlayVideo: " + videoName);
        ViewVideo.Stream.File = videoName;
        ViewVideo.Play();
    }
    public void PlayImage(string imageName){
        ViewVideo.Visible = false;
        ViewImage.Visible = true;
        GD.Print("PlayImage: " + imageName);
        Image image = Image.LoadFromFile(imageName);
        ImageTexture texture = ImageTexture.CreateFromImage(image);
        ViewImage.Texture = texture;
        // ViewImage.Texture = imageName;
    }

    public void OpenImportVideo(){
        ImportVideo.Popup();
    }
    public void OpenImportImage(){
        ImportImage.Popup();
    }
    public void UploadVideo(string localVideoPath){
        localFileSave.UploadFile(localVideoPath,"Media\\"+Path.GetFileName(localVideoPath));
        RefreshList();
        OS.Alert("Video Upload Success!","success");
    }
    public void UploadImage(string localImagePath){
        localFileSave.UploadFile(localImagePath,"Media\\"+Path.GetFileName(localImagePath));
        RefreshList();
        OS.Alert("Image Upload Success!","success");
    }
}
