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
using NPOI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using CronTask.MVVM.Models;

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
        public List<HttpsData> XlsxTable { get; set; }
      
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
                dlg.Filters.Add(new CommonFileDialogFilter("Excel Files", "*.xlsx"));
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

                    List<string> rowList = new List<string>();
                    ISheet sheet;

                    string fileExt = Path.GetExtension(ImportFileLocation).ToLower();//获取文件的拓展名

                    //创建一个文件流
                    using (var stream = new FileStream(ImportFileLocation, FileMode.Open, FileAccess.ReadWrite))
                    {
                        stream.Position = 0;
                        XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                        sheet = xssWorkbook.GetSheetAt(1);
                        IRow headerRow = sheet.GetRow(0);
                        int cellCount = headerRow.LastCellNum;
                        //for (int j = 0; j < cellCount; j++)
                        //{
                            ICell cell = headerRow.GetCell(3);
                            if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                            {
                            // XlsxTable.Columns.Add(cell.ToString());
                            MessageBox.Show(cell.ToString());
                            }
                        // }

                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;
                            if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                {
                                    if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                                    {
                                        rowList.Add(row.GetCell(j).ToString());
                                    }
                                }
                                if ( j == 3 )
                                {
                                   XlsxTable.Add(HttpsData{
                                                        ID: i
                                                        DNS: row.GetCell(j).ToString()
                                                        HaveTime: ""
                                                        IsChecker: ""
                                                        Alerte: ""
                                                       });
                                }
                            }
                            //if (rowList.Count > 0)
                              

                            rowList.Clear();
                        }
                    }

                    OnPropertyChanged("XlsxTable");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        #endregion
    }
}
