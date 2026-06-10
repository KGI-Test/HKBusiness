using Core.Framework.Common;
using EFramework.Client.WindowForms.WinForms;
using EFramework.EFrame.Common;
using EFramework.EFrame.MSExcel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
namespace Warp.HKBusiness.Client
{
    public partial class ImportSBLRate : WinForm
    {
        private string _filePath = null;
        private bool _isCanNotify = false;
        private DataTable _dtAccountList = null;

        public ImportSBLRate()
        {
            InitializeComponent();
        }

        private void ImportStargateWebfrontEndReport_Load(object sender, EventArgs e)
        {
            dteDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Query:查詢時間區間資料,並且顯示在畫面上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportStargateWebfrontEndReport_MenuButtonClick(object sender, WinFormMenuClickEventArgs e)
        {
            DataTable dtDeposit = new DataTable();
            decimal decToday = DateTime.Now.ToDecimal();
            if (e.Menukey == "MG340014_Query")
            {
                Cursor.Current = Cursors.WaitCursor;
                decimal decDt = dteDate.DateTime.ToDecimal();
                //decimal decDtTo = dteToImportDate.DateTime.ToDecimal();
                dtDeposit = Query(decDt);
                EFrame.SetUltraGrid(ugDeposit, "Grid.MG340014_ugSBLRateData.xml", dtDeposit);
                return;
            }

            //查詢當日資料 
            if (e.Menukey == "MG340014_Import")
            {
                int dtNowHour = DateTime.Now.Hour;
                if (Import())
                {
                    dtDeposit = Query(decToday);
                    EFrame.SetUltraGrid(ugDeposit, "Grid.MG340014_ugSBLRateData.xml", dtDeposit);
                    //重新匯入則需等待比對後方可寄通知信
                    _isCanNotify = false;
                }
            }
        }

        /// <summary>
        /// Search SBL Rate Data
        /// </summary>
        /// <param name="decDt"></param>
        /// <param name="dectDtTo"></param>
        /// <returns></returns>
        private DataTable Query(decimal decDt)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            param["HKDSR_DATE"] = decDt;

            DataTable dtSblRateData = EFrame.GetService("SG340014_GetImportSBLRateData", param).Value as DataTable;
            return dtSblRateData;
        }

        /// <summary>
        /// 只能匯入excel
        /// </summary>
        private bool Import()
        {
            bool isImport = false;
            decimal decToday = DateTime.Now.ToString("yyyyMMdd").ToDecimal();
            if (this.txtFileName.Text.ToString().Trim() == "")
            {
                EFrame.ShowMessage(1);
                return false;
            }
            string file = this.txtFileName.Text.ToString();

            isImport = MapImportSBLRate(file);

            return isImport;
        }

        private void txtFileName_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            this.txtFileName.Text = openFileDialog1.FileName;
            _filePath = this.txtFileName.Text;
        }

        /// <summary>
        /// 取得選取的資料夾下的excel檔案,並且以B2為起始位置來讀資料
        /// 將讀取的資料新增到資料表裡,匯入時間當天
        /// </summary>
        /// <param name="file"></param>
        private bool MapImportSBLRate(string file)
        {
            DataTable dtContent = null;
            if (CheckFiles(file) == false)
                return false;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                using (ExcelApplication excelAP = new ExcelApplication(file))
                {
                    dtContent = excelAP.Sheets[0].GetDataTable("A1");
                }
            }
            catch (Exception ex)
            {
                EFrame.ShowMessage(8);
                return false;
            }

            EFramework.EFrame.Common.CommonResult commonResult = null;
            Dictionary<string, object> param = new Dictionary<string, object>
            {
                ["DATA"] = dtContent,
                ["IMPORT_DATE"] = DateTime.Now.ToDecimal(),
            };
            var result = false;
            try
            {
                commonResult = EFrame.GetService("SG340014_ImportSBLRateData", param);
                result = commonResult.Result;
            }
            catch
            {
                EFrame.ShowMessage(12);
            }

            return result;
        }

        private bool CheckFiles(string file_)
        {

            if (System.IO.File.Exists(file_.ToString()) == false)
            {
                EFrame.ShowMessage(-27, file_.ToString());
                return false;
            }

            if (CommonUtility.IsFileLocked(file_.ToString()))
            {
                EFrame.ShowMessage(-33, file_.ToString());
                return false;
            }

            return true;

        }

        private void ImportStargateWebfrontEndReport_ReciveInternalMessage(object sender, WinFormReciveInternalMessageEventArgs e)
        {
            //if (e.InternalTopic == "WAFT.dbo.TRD_TRADE" || e.InternalTopic == "WAFT.dbo.TRD_SECURITIES_DEPOSIT")
            //{
            //    if (e.Parameters.ContainsKey("TRD_TYPE") &&
            //        e.Parameters["TRD_TYPE"].ToString() == "SECURITIES_FUND_DEPOSIT_BANK_TRANSFER")
            //        loadData();
            //}
            if (e.InternalTopic == "EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE" && e.Parameters["IMPORT_DATE"].ToDecimal() == dteDate.Value.ToDecimal()) 
            {
                loadData();
            }
        }

        private void loadData() 
        {
            DataTable dtDeposit = new DataTable();
            Cursor.Current = Cursors.WaitCursor;
            decimal decDt = dteDate.DateTime.ToDecimal();
            dtDeposit = Query(decDt);
            EFrame.SetUltraGrid(ugDeposit, "Grid.MG340014_ugSBLRateData.xml", dtDeposit);
        }
    }
}
