using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CronTask.MVVM.ViewModels
{
    public class MainsHandleViewModel: ViewModelBase
    {
        #region Private Fields
        private ICommand _ImportExls;
        #endregion

        #region Public Properties
        public string WindowTitle { get; set; }
        public string ImportFileLocation { get; set; }
        public DataTable ExlsTable { get; set; }
        #endregion

        #region Constructor
        public MainsHandleViewModel()
        {
            WindowTitle = "定时任务";
        }
        #endregion

        #region ICommand
        public ICommand ImportExls
        {
            get
            {
                _ImportExls = new RelayCommand(
                    param => ImportExlsFile());
                return _ImportExls;
            }
        }
        #endregion

        #region Mode

        private void ImportExlsFile()
        {
            try
            {
           
                var dlg = new CommonOpenFileDialog();
                dlg.Title = "选择一个文件";
                dlg.IsFolderPicker = false;
                dlg.Filters.Add(new CommonFileDialogFilter("TXT Files","*.txt"));
                dlg.InitialDirectory = ImportFileLocation;
                dlg.AddToMostRecentlyUsedList = false;
                dlg.AllowNonFileSystemItems = false;
                dlg.DefaultDirectory = ImportFileLocation;
                dlg.EnsureFileExists = true;
                dlg.EnsurePathExists = true;
                dlg.EnsureReadOnly = false;
                dlg.EnsureValidNames = true;
                dlg.Multiselect = false;
                dlg.ShowPlacesList = true;
               

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var folder = dlg.FileName;
                    // Do something with selected folder string
                    ImportFileLocation = folder.ToString();
                    OnPropertyChanged("ImportFileLocation");

                    string sheetName = "sheet1";//Excel的工作表名称
                    bool isColumnName = true;//判断第一行是否为标题列
                    IWorkbook workbook;//创建一个工作薄接口
                    string fileExt = Path.GetExtension(ImportFileLocation).ToLower();//获取文件的拓展名

                    //创建一个文件流
                    using (FileStream fs = new FileStream(ImportFileLocation, FileMode.Open, FileAccess.Read))
                    {
                        if (fileExt == ".xlsx")
                        {
                            workbook = new XSSFWorkbook(fs);
                        }
                        else
                        {
                            workbook = null;
                        }

                    }
                }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion
    }
}
