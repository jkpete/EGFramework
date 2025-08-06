using System;
using System.IO;
using Godot;
namespace EGFramework.UI
{
    public static class EGodotBasicDialogExtension
    {
        public static void EGAlert(this Node self, string alertMsg, string title = "Alert")
        {
            AcceptDialog acceptDialog = self.PopupNode<AcceptDialog>("AlertDialog");
            acceptDialog.Title = title;
            acceptDialog.DialogText = alertMsg;
        }

        public static void EGConfirm(this Node self, string alertMsg, Action<bool> callback, string title = "Confirm")
        {
            EGodotConfirmationDialog confirmDialog = self.PopupNode<EGodotConfirmationDialog>("ConfirmDialog");
            confirmDialog.Title = title;
            confirmDialog.DialogText = alertMsg;
            confirmDialog.Init(callback);
        }

        public static void EGFileOpen(this Node self, string filePath, Action<string> selectPath, string title = "Open a file")
        {
            EGodotFileDialog fileDialog = self.PopupNode<EGodotFileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.RootSubfolder = Path.GetDirectoryName(filePath);
            fileDialog.InitFileSelect(selectPath);
        }

        public static void EGFileSave(this Node self, string filePath, Action<string> selectPath, string title = "Save a file")
        {
            EGodotFileDialog fileDialog = self.PopupNode<EGodotFileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.CurrentFile = filePath;
            fileDialog.InitSaveFileSelect(selectPath);
        }

        public static void EGDocumentOpen(this Node self, string filePath, Action<string> selectPath, string title = "FileSelect")
        {
            EGodotFileDialog fileDialog = self.PopupNode<EGodotFileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.RootSubfolder = filePath;
            fileDialog.InitDirSelect(selectPath);
        }
    }
}
