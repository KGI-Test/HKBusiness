using EFramework.Client.WindowForms.WinForms;
using EFramework.EFrame.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Warp.HKBusiness.Client
{
    //MG340026
    public partial class Hold : WinForm
    {
        private decimal RowSeq { get; set; } = 0;

        public Hold()
        {
            InitializeComponent();
        }

        private void Hold_Load(object sender, EventArgs e)
        {
            EFrame.SetUltraCombo(cmbTicker, EFrame.GetService("SG340024_GetTickers").Value as DataTable, "INSTR_INSNBR", "INSTR_CODE");

            EFrame.SetUltraCombo(cmbCounterparty, EFrame.GetService("SG340026_GetCounterparty").Value as DataTable, "FLGDAT_FLGVAR", "FLGDAT_FLGDTA");
        }

        private void Hold_AfterLoad(object sender, WinFormParametersEventArgs e)
        {
            if (e.Parameters.ContainsKey("HKOHD_ROWSEQ") == false)
            {
                RowSeq = 0;
                ClearHold();

                if (e.Parameters.ContainsKey("HKOHD_INSNBR"))
                {
                    cmbTicker.Value = e.Parameters["HKOHD_INSNBR"].ToDecimal();
                }
            }
            else
            {
                RowSeq = e.Parameters["HKOHD_ROWSEQ"].ToDecimal();
                LoadData(e);
            }
        }

        private void Hold_MenuButtonClick(object sender, WinFormMenuClickEventArgs e)
        {
            if (e.Menukey == "MG340026_New")
            {
                ClearHold();
                return;
            }

            if (e.Menukey == "MG340026_Save")
            {
                if (cmbCounterparty.SelectedRow == null)
                {
                    EFrame.ShowMessage(-4, lbCounterparty.Text);
                    return;
                }
                if (cmbTicker.SelectedRow == null)
                {
                    EFrame.ShowMessage(-4, lbTicker.Text);
                    return;
                }

                if (EFrame.ShowMessage(-3) != DialogResult.Yes) return;

                CommonResult commonResult = EFrame.GetService("SG340026_SaveHold", new Dictionary<string, object>
                {
                    ["HKOHD_ROWSEQ"] = RowSeq,
                    ["HKOHD_QUANTITY"] = numQuantity.Value.ToDecimal(),
                    ["HKOHD_SBL_RATE"] = numRate.Value.ToDecimal(),
                    ["HKOHD_DATE"] = dteDATE.DateTime.ToString("yyyyMMdd").ToDecimal(),
                    ["HKOHD_COUNTERPARTY"] = cmbCounterparty.Value.ToString(),
                    ["HKOHD_INSNBR"] = cmbTicker.Value.ToDecimal(),
                    ["HKOHD_DATE"] = dteDATE.DateTime.ToString("yyyyMMdd").ToString()
                });
                if (commonResult.Result == false)
                {
                    EFrame.ShowMessage(-8, commonResult.Message);
                    return;
                }

                EFrame.ShowMessage(-2);

                Close();
            }

            if (e.Menukey == "MG340026_Delete")
            {
                if (EFrame.ShowMessage(-10, numSEQ.Value.ToString()) != DialogResult.Yes) return;

                CommonResult commonResult = EFrame.GetService("SG340026_DeleteHold", new Dictionary<string, object>
                {
                    ["HKOHD_ROWSEQ"] = numSEQ.Value
                });
                if (commonResult.Result == false)
                {
                    EFrame.ShowMessage(-8, commonResult.Message);
                    return;
                }

                Close();
            }
        }

        private void ClearHold()
        {
            RowSeq = 0;
            numSEQ.Value = 0;
            numQuantity.Value = 0;
            numRate.Value = 0;
            cmbCounterparty.Value = null;
            cmbTicker.Value = null;
            dteDATE.Value = DateTime.Now;
        }

        private void LoadData(WinFormParametersEventArgs e)
        {
            DataTable table = EFrame.GetService("SG340026_GetHold",e.Parameters).Value as DataTable;

            if (table.Rows.Count == 0)
            {
                ClearHold();
                return;
            }
            else
            {
                RowSeq = table.Rows[0]["HKOHD_ROWSEQ"].ToDecimal();
                numSEQ.Value = RowSeq;
                numQuantity.Value = table.Rows[0]["HKOHD_QUANTITY"].ToDecimal();
                numRate.Value = table.Rows[0]["HKOHD_SBL_RATE"].ToDecimal();
                cmbCounterparty.Value = table.Rows[0]["HKOHD_COUNTERPARTY"].ToString();
                cmbTicker.Value = table.Rows[0]["HKOHD_INSNBR"].ToDecimal();
                dteDATE.Value = table.Rows[0]["HKOHD_DATE"].ToDecimal().ToDateTime();
            }
        }
    }
}
