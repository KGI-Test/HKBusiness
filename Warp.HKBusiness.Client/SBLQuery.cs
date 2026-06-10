using EFramework.Client.WindowForms.WinForms;
using EFramework.EFrame.Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Warp.HKBusiness.Client
{
    //MG340027
    public partial class SBLQuery : WinForm
    {
        public SBLQuery()
        {
            InitializeComponent();
        }

        private void SBLQuery_Load(object sender, EventArgs e)
        {
            EFrame.SetUltraCombo(cmbTicker, EFrame.GetService("SG340024_GetTickers").Value as DataTable, "INSTR_INSNBR", "INSTR_CODE");

            dteTradeDate.Value = DateTime.Now;
        }

        private void SBLQuery_AfterLoad(object sender, WinFormParametersEventArgs e)
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

        private void SBLQuery_MenuButtonClick(object sender, WinFormMenuClickEventArgs e)
        {
            if (e.Menukey == "MG340027_Query")
            {
                LoadData();
            }

            if (e.Menukey == "MG340027_New")
            {
                EFrame.ShowForm("MG340028");
            }

            if (e.Menukey == "MG340027_Report")
            {
                DataTable dtSBLCSV = EFrame.GetService("SG340027_GetSBLCSV", new Dictionary<string, object>()
                {
                    ["HKSBL_TRADE_DATE"] = dteTradeDate.Value is null ? -1 : dteTradeDate.DateTime.ToDecimal(),
                    ["HKSBL_INSNBR"] = cmbTicker.SelectedRow is null ? -1 : cmbTicker.Value.ToDecimal()
                }).Value as DataTable;

                if (dtSBLCSV != null)
                {
                    string csv = ConvertDataTableToCsv(dtSBLCSV);
                    using (ReportViewer viewer = new ReportViewer(csv))
                    {
                        viewer.ShowDialog(this);
                    }
                }
            }
        }

        private string ConvertDataTableToCsv(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                {
                    string value = field.ToString();
                    if (value.Contains(" | ") || value.Contains("\"") || value.Contains("\n"))
                    {
                        return "\"" + value.Replace("\"", "\"\"") + "\"";
                    }
                    return value;
                });
                sb.AppendLine(string.Join(" / ", fields));
            }

            return sb.ToString();
        }

        private void LoadData()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                DataTable dtSBLQuery = EFrame.GetService("SG340027_GetSBL", new Dictionary<string, object>()
                {
                    ["HKSBL_TRADE_DATE"] = dteTradeDate.Value is null ? -1 : dteTradeDate.DateTime.ToDecimal(),
                    ["HKSBL_INSNBR"] = cmbTicker.SelectedRow is null ? -1 : cmbTicker.Value.ToDecimal()
                }).Value as DataTable;

                EFrame.SetUltraGrid(ugSBL, "Grid.MG340027_ugSBL.xml", dtSBLQuery);
            }
            catch (Exception ex) { EFrame.LogMessage(ex, LogType.Critical); throw ex; }
            finally { FormLock = false; }
        }

        private void SBLQuery_ReciveInternalMessage(object sender, WinFormReciveInternalMessageEventArgs e)
        {
            if (e.InternalTopic == "WAFT.dbo.HK_SBL_SBL")
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
