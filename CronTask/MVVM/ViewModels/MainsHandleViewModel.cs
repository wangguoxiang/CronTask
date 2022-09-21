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
using CronTask.MVVM;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace CronTask.MVVM.ViewModels
{
    public class MainsHandleViewModel: ViewModelBase
    {
        #region Private Fields
        private ICommand _ImportExls;
        private ICommand _GetDns;
        #endregion

        #region Public Properties
        public string WindowTitle { get; set; }
        public string ImportFileLocation { get; set; }
        public List<int> ID { get; set; }
        public List<string> DNS { get; set; }
        public List<string> HaveTime { get; set; }
        public List<string> IsChecker { get; set; }
        public List<string>  Alerte { get; set; }

        private ObservableCollection<HttpsData> _productCollection = new ObservableCollection<HttpsData>();
        public ObservableCollection<HttpsData> ProductCollection
        {
            get { return _productCollection; }
            set { _productCollection = value; OnPropertyChangedNew(); }
        }

        #endregion

        #region Constructor
        public MainsHandleViewModel()
        {
            WindowTitle = "定时任务";
            ID = new List<int>();
            DNS = new List<string>();
            HaveTime = new List<string>();
            IsChecker = new List<string>();
            Alerte = new List<string>();
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

        public ICommand GetDns
        {
            get
            {
                _GetDns = new RelayCommand(
                    param => LoadDns());
                return _GetDns;
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
                    ProductCollection.Clear();
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
                         /*   ICell cell = headerRow.GetCell(3);
                            if (cell != null && !string.IsNullOrWhiteSpace(cell.ToString()))
                            {
                            // XlsxTable.Columns.Add(cell.ToString());
                            MessageBox.Show(cell.ToString());
                            }*/
                        // }

                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null) continue;
                            if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                /*  if (row.GetCell(j) != null)
                                  {
                                      if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                                      {
                                          rowList.Add(row.GetCell(j).ToString());
                                      }
                                  }*/

                                if (row.GetCell(j) == null) continue;
                                if ( j == 3 && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                                {
                                    DNS.Add(row.GetCell(j).ToString());
                                    ID.Add(DNS.Count);
                                    IsChecker.Add("");
                                    HaveTime.Add("");
                                    Alerte.Add("");

                                  
                                    ProductCollection.Add(new HttpsData
                                    {
                                        ID = DNS.Count,
                                        DNS = row.GetCell(j).ToString(),
                                        IsChecker = "",
                                        HaveTime = "",
                                        Alerte = ""
                                    });

                                    OnPropertyChanged("ID");
                                    OnPropertyChanged("DNS");
                                    OnPropertyChanged("IsChecker");
                                    OnPropertyChanged("Alerte");
                                    OnPropertyChanged("HaveTime");
                                }
                            }
                            //if (rowList.Count > 0)
                              

                            rowList.Clear();
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadDns()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                ThreadDns();
            });

        }

        private void ThreadDns()
        {
            MatchCollection DnsList;
            int index = 0;
            string haveTime;
            string ptn = "^((?!-)[A-Za-z0-9-]{1,63}(?<!-)\\.)+[A-Za-z]{2,6}$";
            int count = 0;
            Regex r = new Regex(ptn, RegexOptions.ECMAScript);

            lock (ProductCollection)
            {
                if (ProductCollection.Count > 0)
                {
                    foreach (var row in ProductCollection)
                    {
                        ProductCollection[index].IsChecker = "开始测试";
                        OnPropertyChangedNew("ProductCollection");

                        haveTime = "";

                        DnsList = r.Matches(row.DNS);
                        foreach (var dns in DnsList)
                        {
                            try
                            {
                                X509Certificate2 x509 = HttpCertificate.DownloadSslCertificate(dns.ToString());
                                if (x509 != null)
                                {
                                    if (string.IsNullOrWhiteSpace(haveTime))
                                    {
                                        haveTime = string.Format("{0}-{1}", x509.NotAfter.ToString(), x509.NotBefore.ToString());
                                    }
                                    else
                                    {
                                        haveTime += string.Format(",{0}-{1}", x509.NotAfter.ToString(), x509.NotBefore.ToString());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                haveTime = ex.ToString();
                                continue;
                            }

                            ProductCollection[index].IsChecker = haveTime;
                            OnPropertyChangedNew("ProductCollection");
                            index++;

                        }
                    }

                    MessageBox.Show("检测完成");
                }
            } 
        }
        #endregion
    }   
}
