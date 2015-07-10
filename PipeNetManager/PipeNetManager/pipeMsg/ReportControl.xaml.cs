using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.ComponentModel;
using System.Windows.Xps.Packaging;
using System.Reflection;

namespace PipeNetManager.pipeMsg
{
    /// <summary>
    /// ReportControl.xaml 的交互逻辑
    /// </summary>
    public partial class ReportControl : UserControl
    {
        public ReportControl(List<string> listpath)
        {
            InitializeComponent();
            mListreportpath = listpath;
            ShowContent();
        }

        public ReportControl(string pipename)
        {
            InitializeComponent();
            this.pipename = pipename;

        }

        public void ShowContent()
        {
            if (mListreportpath==null||mListreportpath.Count <= 0)
                return;
            mCurpath = mListreportpath.ElementAt(0);
            progress.Visibility = Visibility.Visible;
            if (mCurpath == null || mCurpath.Length <= 0)
            {
                MessageBox.Show("加载报告出错！", "错误消息");
                return;
            }
            else
            {
                Dowork();
            }

        }

        /// <summary>
        /// chang the doc to xps.
        /// </summary>
        /// <param name="wordDocName"></param>
        /// <returns></returns>
        private XpsDocument ConvertWordToXPS(string wordDocName)
        {
            FileInfo fi = new FileInfo(wordDocName);
            XpsDocument result = null;
            string xpsDocName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), fi.Name);
            xpsDocName = xpsDocName.Replace(".docx", ".xps").Replace(".doc", ".xps");
            Microsoft.Office.Interop.Word.Application wordApplication = new Microsoft.Office.Interop.Word.Application();
            try
            {
                if (!File.Exists(xpsDocName))
                {
                    Object Nothing = Missing.Value;
                    wordApplication.Documents.Add(wordDocName, ref Nothing, ref Nothing, ref Nothing);
                    Document doc = wordApplication.ActiveDocument;
                    doc.ExportAsFixedFormat(xpsDocName, WdExportFormat.wdExportFormatXPS, false, WdExportOptimizeFor.wdExportOptimizeForPrint,
                        WdExportRange.wdExportAllDocument, 0, 0, WdExportItem.wdExportDocumentContent, true, true,
                        WdExportCreateBookmarks.wdExportCreateHeadingBookmarks, true, true, false, Type.Missing);
                    result = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);
                }
                if (File.Exists(xpsDocName))
                {
                    result = new XpsDocument(xpsDocName, FileAccess.Read);
                }
            }
            catch (System.Exception ex)
            {
                string error = ex.Message;
                MessageBox.Show(error, "错误消息");
                System.Console.WriteLine(error);
            }
            finally
            {
                wordApplication.Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
            return result;
        }

        private void Longtaskwork(object sender, DoWorkEventArgs e)                                 //invoke the long task 
        {
            document = ConvertWordToXPS(mCurpath);
        }

        private void LongtaskComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (document == null)
                return;
            reportviewer.Document = document.GetFixedDocumentSequence();
            reportviewer.FitToWidth();
            progress.Visibility = Visibility.Hidden;
        }

        private void Dowork()
        {
            backworkthread = new BackgroundWorker();
            backworkthread.WorkerSupportsCancellation = true;
            backworkthread.DoWork += new DoWorkEventHandler(Longtaskwork);
            backworkthread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LongtaskComplete);
            backworkthread.RunWorkerAsync();
        }

        private BackgroundWorker backworkthread;
        private List<string> mListreportpath;
        private string mCurpath;
        private XpsDocument document;
        private string pipename;
    }
}
