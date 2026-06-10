using EFramework.Client.WindowForms.WinForms;
using EFramework.EFrame.Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Warp.HKBusiness.Client
{
    //MG340025
    public partial class HoldQuery : WinForm
    {
        public HoldQuery()
        {
            InitializeComponent();
        }

        private void HoldQuery_Load(object sender, EventArgs e)
        {
            EFrame.SetUltraCombo(cmbTicker, EFrame.GetService("SG340024_GetTickers").Value as DataTable, "INSTR_INSNBR", "INSTR_CODE");

            dteTradeDate.Value = DateTime.Now;
        }

        private void HoldQuery_AfterLoad(object sender, WinFormParametersEventArgs e)
        {
            if (e.Parameters.ContainsKey("INSALA_INSTR_INSNBR"))
            {
                cmbTicker.Value = e.Parameters["INSALA_INSTR_INSNBR"].ToDecimal();
            }

            if (e.Parameters.ContainsKey("HKAVL_DATE"))
            {
                dteTradeDate.Value = e.Parameters["HKAVL_DATE"].ToDecimal().ToDateTime();
            }

            LoadData();
        }

        private void HoldQuery_MenuButtonClick(object sender, WinFormMenuClickEventArgs e)
        {
            if (e.Menukey == "MG340025_Query")
            {
                LoadData();
            }

            if (e.Menukey == "MG340025_New")
            {
                EFrame.ShowForm("MG340026");
            }
        }

        private void LoadData()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                DataTable dtHoldQuery = EFrame.GetService("SG340025_GetHold", new Dictionary<string, object>()
                {
                    ["HKOHD_DATE"] = dteTradeDate.Value is null ? -1 : dteTradeDate.DateTime.ToDecimal(),
                    ["HKOHD_INSNBR"] = cmbTicker.SelectedRow is null ? -1 : cmbTicker.Value.ToDecimal()
                }).Value as DataTable;

                EFrame.SetUltraGrid(ugHold, "Grid.MG340025_ugHold.xml", dtHoldQuery);
            }
            catch (Exception ex) { EFrame.LogMessage(ex, LogType.Critical); throw ex; }
            finally { FormLock = false; }
        }

        private void HoldQuery_ReciveInternalMessage(object sender, WinFormReciveInternalMessageEventArgs e)
        {
            if (e.InternalTopic == "WAFT.dbo.HK_SBL_HOLD" || e.InternalTopic == "WAFT.dbo.HK_SBL_SBL")
            {
                StopReciveInternalMessage = true;
                EFrame.ShowMessage(-13);
                LoadData();
                StopReciveInternalMessage = false;
                return;
            }

            if (e.InternalTopic == "EDAISYS.STATUS_EVENT" && e.Parameters.ContainsKey("STSEVT_NAME"))
            {
                if (e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HKSBLLocateFileAfterImportHold" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HKSBLLocateFileAfterImportHoldSource" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HKSBLLocateFileAfterImportHold_LastTradeDate")
                {
                    StopReciveInternalMessage = true;
                    EFrame.ShowMessage(-13);
                    LoadData();
                    StopReciveInternalMessage = false;
                    return;
                }
            }
        }
    }
}
