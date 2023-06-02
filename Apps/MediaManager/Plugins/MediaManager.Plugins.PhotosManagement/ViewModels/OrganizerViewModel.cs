using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VeraSoft.Plugins.PhotosManagement.Config;
using VeraSoft.Wpf.Core;
using VeraSoft.Wpf.Utils;

namespace VeraSoft.Plugins.PhotosManagement.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class OrganizerViewModel : ViewModel
    {
        public OrganizerViewModel()
        {
            InitialSize = new System.Windows.Size(1200, 400);
            PageName = CurrentDictionary.Organizer;
            SourceCommand = new RelayCommand(o => PerformSourceCommand(o));
            DestinationCommand = new RelayCommand(o => PerformDestinationCommand(o));
            ExportCommand = new RelayCommand(o => PerformExportCommand(o));
            ExportByDateCommand = new RelayCommand(o => PerformExportByDateCommand(o));
        }

        public ICommand SourceCommand { get; set; }
        public ICommand DestinationCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ExportByDateCommand { get; set; }
        PhotosManagementConfig Config { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public bool IsPhotosChecked { get; set; }
        public bool IsVideosChecked { get; set; }
        public bool IsMusicChecked { get; set; }
        public bool IsDocumentsChecked { get; set; }
        public bool IsOthersChecked { get; set; }

        private void PerformSourceCommand(object o)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                dialog.SelectedPath = path;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                Source = dialog.SelectedPath;
            }
        }

        private void PerformDestinationCommand(object o)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                dialog.SelectedPath = path;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                Destination = dialog.SelectedPath;
            }
        }

        private void PerformExportCommand(object o)
        {
            if (IsOthersChecked)
            {
                FilesManager.CopyFolder(Source, Destination);
                return;
            }
            List<string> itemsToExport = GetItemsToExport();
            FilesManager.CopyFolder(Source, Destination, itemsToExport);
            MessageBox.Show(CurrentDictionary.ExportedOk + " " + Destination, CurrentDictionary.FilesExport, MessageBoxButton.OK);
            // TODO borrar carpetas vacias
        }
        // ExportByDateCommand
        private void PerformExportByDateCommand(object o)
        {
            if (IsOthersChecked)
            {
                FilesManager.CopyFolderByDate(Source, Destination);
                MessageBox.Show(CurrentDictionary.ExportedOk + " " + Destination, CurrentDictionary.FilesExport, MessageBoxButton.OK);
                return;
            }
            List<string> itemsToExport = GetItemsToExport();
            FilesManager.CopyFolderByDate(Source, Destination, itemsToExport);
            MessageBox.Show(CurrentDictionary.ExportedOk + " " + Destination, CurrentDictionary.FilesExport, MessageBoxButton.OK);
        }

        private List<string> GetItemsToExport()
        {
            List<string> itemsToExport = new List<string>();
            itemsToExport.AddRange(IsCheckedToExport(IsPhotosChecked, Config.PhotoTypesList));
            itemsToExport.AddRange(IsCheckedToExport(IsVideosChecked, Config.VideoTypesList));
            itemsToExport.AddRange(IsCheckedToExport(IsMusicChecked, Config.MusicTypesList));
            itemsToExport.AddRange(IsCheckedToExport(IsDocumentsChecked, Config.DocumentTypesList));
            return itemsToExport;
        }

        private List<string> IsCheckedToExport(bool isChecked, List<string> itemsToAdd)
        {
            if (isChecked && itemsToAdd != null)
            {
                return itemsToAdd;
            }
            else
            {
                return new List<string>();
            }
        }

        public override void Load(object objectToEdit)
        {
        }

        public override void Save()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            Source = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\atest";
            Destination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\atest";
            //PhotosManagementConfig
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\PhotosManagementConfig.xml");
            Config = XmlTools.Deserialize<PhotosManagementConfig>(configPath);
        }
    }
}
