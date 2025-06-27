using System;
using Godot;
namespace EGFramework.UI
{
    public static class EGodotBasicDialogExtension
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
            EGodotConfirmationDialog confirmDialog = self.SingletoneNode<EGodotConfirmationDialog>("ConfirmDialog");
            confirmDialog.Title = title;
            confirmDialog.DialogText = alertMsg;
            confirmDialog.PopupCentered();
            confirmDialog.Init(callback);
        }

        public static void EGFileOpen(this Node self, string filePath, Action<string> selectPath, string title = "Open a file")
        {
            EGodotFileDialog fileDialog = self.SingletoneNode<EGodotFileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.RootSubfolder = filePath;
            fileDialog.PopupCentered();
            fileDialog.InitFileSelect(selectPath);
        }

        public static void EGFileSave(this Node self, string filePath, Action<string> selectPath, string title = "Save a file")
        {
            EGodotFileDialog fileDialog = self.SingletoneNode<EGodotFileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.RootSubfolder = filePath;
            fileDialog.PopupCentered();
            fileDialog.InitSaveFileSelect(selectPath);
        }

        public static void EGDocumentOpen(this Node self, string filePath, Action<string> selectPath, string title = "FileSelect")
        {
            EGodotFileDialog fileDialog = self.SingletoneNode<EGodotFileDialog>("FileDialog");
            fileDialog.Title = title;
            fileDialog.RootSubfolder = filePath;
            fileDialog.PopupCentered();
            fileDialog.InitDirSelect(selectPath);
        }
    }
}
