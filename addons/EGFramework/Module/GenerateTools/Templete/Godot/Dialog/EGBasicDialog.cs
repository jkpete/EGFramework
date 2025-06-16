using System;
using Godot;
namespace EGFramework.UI
{
    public static class EGBasicDialogExtension
    {
        public static void EGAlert(this Node self, string alertMsg, string title = "Alert")
        {
            AcceptDialog acceptDialog = self.SingletoneNode<AcceptDialog>("AlertDialog");
            acceptDialog.Title = title;
            acceptDialog.DialogText = alertMsg;
            acceptDialog.PopupCentered();
        }

        public static void EGConfirm(this Node self, string alertMsg, Action<bool> callback, string title = "Confirm")
        {
            ConfirmationDialog confirmDialog = self.SingletoneNode<ConfirmationDialog>("ConfirmDialog");
            confirmDialog.Title = title;
            confirmDialog.DialogText = alertMsg;
            confirmDialog.PopupCentered();
            confirmDialog.Connect("confirmed", Callable.From(() =>
            {
                callback(true);
            }));
            confirmDialog.Connect("canceled", Callable.From(() =>
            {
                callback(false);
            }));
        }

        public static void EGFileSingleSelect(this Node self, string filePath, Action<string> selectPath, string title = "FileSelect")
        {
            FileDialog fileDialog = self.SingletoneNode<FileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.Size = new Vector2I(480, 320);
            fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
            fileDialog.RootSubfolder = filePath;
            fileDialog.PopupCentered();
            fileDialog.Connect("file_selected", Callable.From<string>(path =>
            {
                selectPath(path);
            }));
        }
        
        public static void EGDocumentSelect(this Node self, string filePath, Action<string> selectPath, string title = "FileSelect")
        {
            FileDialog fileDialog = self.SingletoneNode<FileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.Size = new Vector2I(480, 320);
            fileDialog.FileMode = FileDialog.FileModeEnum.OpenDir;
            fileDialog.RootSubfolder = filePath;
            fileDialog.PopupCentered();
            fileDialog.Connect("file_selected", Callable.From<string>(path =>
            {
                selectPath(path);
            }));
        }
    }
}
