
using System;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotFileDialog : FileDialog, IEGFramework
    {
        private EasyEventOnce<string> OnFileSelect { set; get; } = new EasyEventOnce<string>();
        private EasyEventOnce<string> OnDirSelect { set; get; } = new EasyEventOnce<string>();
        private EasyEventOnce<string[]> OnFilesSelect { set; get; } = new EasyEventOnce<string[]>();

        private bool IsInit { set; get; } = false;
        public void Init()
        {
            if (!IsInit)
            {
                this.FileSelected += OnFileSelect.Invoke;
                this.DirSelected += OnDirSelect.Invoke;
                this.FilesSelected += OnFilesSelect.Invoke;
                this.Size = new Vector2I(480, 320);
                this.Access = AccessEnum.Filesystem;
                IsInit = true;
            }
        }
        public void InitFileSelect(Action<string> callback)
        {
            OnFileSelect.Register(callback);
            Init();
            this.FileMode = FileModeEnum.OpenFile;
        }

        public void InitDirSelect(Action<string> callback)
        {
            OnDirSelect.Register(callback);
            this.FileMode = FileModeEnum.OpenDir;
            Init();
        }

        public void InitFilesSelect(Action<string[]> callback)
        {
            OnFilesSelect.Register(callback);
            this.FileMode = FileModeEnum.OpenFiles;
            Init();
        }

        public void InitAnySelect(Action<string> singleSelectCallback, Action<string[]> multiSelectCallback)
        {
            OnFileSelect.Register(singleSelectCallback);
            OnDirSelect.Register(singleSelectCallback);
            OnFilesSelect.Register(multiSelectCallback);
            this.FileMode = FileModeEnum.OpenAny;
            Init();
        }

        public void InitSaveFileSelect(Action<string> callback)
        {
            OnFileSelect.Register(callback);
            this.FileMode = FileModeEnum.SaveFile;
            Init();
        }

        
        
    }
}
