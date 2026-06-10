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
    //MG340028
    public partial class SBL : WinForm
    {
        private decimal RowSeq { get; set; } = 0;

        private decimal HoldSeq { get; set; } = 0;

        public SBL()
        {
            InitializeComponent();
        }

        private void SBL_Load(object sender, EventArgs e)
        {
            EFrame.SetUltraCombo(cmbTicker, EFrame.GetService("SG340024_GetTickers").Value as DataTable, "INSTR_INSNBR", "INSTR_CODE");

            EFrame.SetUltraCombo(cmbCounterparty, EFrame.GetService("SG340026_GetCounterparty").Value as DataTable, "FLGDAT_FLGVAR", "FLGDAT_FLGDTA");

            EFrame.SetUltraCombo(cmbDirection, EFrame.GetService("SG340028_GetDirection").Value as DataTable, "CODE", "NAME");
        }

        private void SBL_AfterLoad(object sender, WinFormParametersEventArgs e)
        {
            if (e.Parameters.ContainsKey("HKSBL_TRDNBR") == false)
            {
                RowSeq = 0;
                HoldSeq = 0;
                ClearSBL();

                if (e.Parameters.ContainsKey("HKSBL_INSNBR"))
                {
                    cmbTicker.Value = e.Parameters["HKSBL_INSNBR"].ToDecimal();

                    GetClosePrice();
                }

                if (e.Parameters.ContainsKey("HKOHD_INSNBR"))
                {
                    cmbTicker.Value = e.Parameters["HKOHD_INSNBR"].ToDecimal();

                    GetClosePrice();
                }

                if (e.Parameters.ContainsKey("HKOHD_COUNTERPARTY"))
                {
                    cmbCounterparty.Value = e.Parameters["HKOHD_COUNTERPARTY"].ToString();
                }

                if (e.Parameters.ContainsKey("HKOHD_QUANTITY"))
                {
                    numQuantity.Value = e.Parameters["HKOHD_QUANTITY"].ToDecimal();
                }

                if (e.Parameters.ContainsKey("HKOHD_SBL_RATE"))
                {
                    numRate.Value = e.Parameters["HKOHD_SBL_RATE"].ToDecimal();
                }

                if (e.Parameters.ContainsKey("HKOHD_ROWSEQ"))
                {
                    HoldSeq = e.Parameters["HKOHD_ROWSEQ"].ToDecimal();
                }

            }
            else
            {
                RowSeq = e.Parameters["HKSBL_TRDNBR"].ToDecimal();
                LoadData(e);
            }
        }

        private void SBL_MenuButtonClick(object sender, WinFormMenuClickEventArgs e)
        {
            if (e.Menukey == "MG340028_New")
            {
                ClearSBL();
                return;
            }

            if (e.Menukey == "MG340028_Save")
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
                if (cmbDirection.SelectedRow == null)
                {
                    EFrame.ShowMessage(-4, lbDirection.Text);
                    return;
                }

                if (HoldSeq > 0)
                {
                    DataTable table = EFrame.GetService("SG340028_CheckHoldQuantity", new Dictionary<string, object>
                    {
                        ["HKOHD_ROWSEQ"] = HoldSeq
                    }).Value as DataTable;

                    if (table.Rows.Count > 0)
                    {
                        if(table.Rows[0]["HKOHD_QUANTITY"].ToDecimal() < numQuantity.Value.ToDecimal())
                        {                            
                            EFrame.ShowMessage(-8, "Quantity cannot exceed the Hold Quantity.");
                            return;
                        }
                    }
                }


                if (EFrame.ShowMessage(-3) != DialogResult.Yes) return;

                CommonResult commonResult = EFrame.GetService("SG340028_SaveSBL", new Dictionary<string, object>
                {
                    ["HKSBL_ROWSEQ"] = RowSeq,
                    ["HKSBL_TRDNBR"] = RowSeq,
                    ["HKSBL_VER"] = 1,
                    ["HKSBL_DIRECTION"] = cmbDirection.Value.ToString(),
                    ["HKSBL_TRADE_DATE"] = dteTradeDate.DateTime.ToString("yyyyMMdd").ToDecimal(),
                    ["HKSBL_VALUE_DATE"] = dteValueDate.DateTime.ToString("yyyyMMdd").ToDecimal(),
                    ["HKSBL_COUNTERPARTY"] = cmbCounterparty.Value.ToString(),
                    ["HKSBL_INSNBR"] = cmbTicker.Value.ToDecimal(),
                    ["HKSBL_PRICE"] = numPrice.Value.ToDecimal(),
                    ["HKSBL_DIVIDEND"] = numDividend.Value.ToDecimal(),
                    ["HKSBL_QUANTITY"] = numQuantity.Value.ToDecimal(),
                    ["HKSBL_SBL_RATE"] = numRate.Value.ToDecimal(),
                    ["HKSBL_HOLD_SEQ"] = HoldSeq
                });
                if (commonResult.Result == false)
                {
                    EFrame.ShowMessage(-8, commonResult.Message);
                    return;
                }

                EFrame.ShowMessage(-2);

                Close();
            }

            if (e.Menukey == "MG340028_Delete")
            {
                if (EFrame.ShowMessage(-10, numTradeNumber.Value.ToString()) != DialogResult.Yes) return;

                CommonResult commonResult = EFrame.GetService("SG340028_DeleteSBL", new Dictionary<string, object>
                {
                    ["HKSBL_TRDNBR"] = numTradeNumber.Value
                });
                if (commonResult.Result == false)
                {
                    EFrame.ShowMessage(-8, commonResult.Message);
                    return;
                }

                Close();
            }
        }

        private void cmbTicker_Validated(object sender, EventArgs e)
        {
            GetClosePrice();
        }

        private void ClearSBL()
        {
            RowSeq = 0;
            HoldSeq = 0;
            numTradeNumber.Value = 0;
            numQuantity.Value = 0;
            numRate.Value = 0;
            numPrice.Value = 0;
            numDividend.Value = 100;
            cmbDirection.Value = null;
            cmbCounterparty.Value = null;
            cmbTicker.Value = null;
            dteTradeDate.Value = DateTime.Now;
            dteValueDate.Value = DateTime.Now;
        }

        private void LoadData(WinFormParametersEventArgs e)
        {
            DataTable table = EFrame.GetService("SG340028_GetSBL",e.Parameters).Value as DataTable;

            if (table.Rows.Count == 0)
            {
                ClearSBL();
                return;
            }
            else
            {
                RowSeq = table.Rows[0]["HKSBL_TRDNBR"].ToDecimal();
                numTradeNumber.Value = table.Rows[0]["HKSBL_TRDNBR"].ToDecimal();
                numQuantity.Value = table.Rows[0]["HKSBL_QUANTITY"].ToDecimal();
                numRate.Value = table.Rows[0]["HKSBL_SBL_RATE"].ToDecimal();
                numPrice.Value = table.Rows[0]["HKSBL_PRICE"].ToDecimal();
                numDividend.Value = table.Rows[0]["HKSBL_DIVIDEND"].ToDecimal();
                cmbDirection.Value = table.Rows[0]["HKSBL_DIRECTION"].ToString();
                cmbCounterparty.Value = table.Rows[0]["HKSBL_COUNTERPARTY"].ToString();
                cmbTicker.Value = table.Rows[0]["HKSBL_INSNBR"].ToDecimal();
                dteTradeDate.Value = table.Rows[0]["HKSBL_TRADE_DATE"].ToDecimal().ToDateTime();
                dteValueDate.Value = table.Rows[0]["HKSBL_VALUE_DATE"].ToDecimal().ToDateTime();
            }
        }

        private void GetClosePrice()
        {
            DataTable table = EFrame.GetService("SG120071_GetClosePrice", new Dictionary<string, object>
            {
                ["INSTR_INSNBR"] = cmbTicker.Value,
                ["DATA_DATE"] = dteTradeDate.DateTime.ToString("yyyyMMdd").ToDecimal()
            }).Value as DataTable;

            if (table.Rows.Count == 0)
            {
                EFrame.ShowMessage(-29);
                return;
            }

            numPrice.Value = table.Rows[0]["HISPRS_CLOSE"].ToDecimal();
        }
    }
}
