using EFramework.Client.WindowForms.WinForms;
using EFramework.EFrame.Common;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Forms;

namespace Warp.HKBusiness.Client
{
    //MG340024
    public partial class Inventory : WinForm
    {
        public Inventory()
        {
            InitializeComponent();
        }

        private void Inventory_Load(object sender, EventArgs e)
        {
            DataTable dtTickers = EFrame.GetService("SG340024_GetTickers").Value as DataTable;

            EFrame.SetUltraCombo(cmbTicker, dtTickers, "INSTR_INSNBR", "INSTR_CODE");

            dteEXDate.Value = null;
        }

        private void Inventory_MenuButtonClick(object sender, WinFormMenuClickEventArgs e)
        {            
            if (e.Menukey == "MG340024_Query")
            {
                if (TabControl.ActiveTab.Text == "Single")
                {
                    if (cmbTicker.SelectedRow == null)
                    {
                        EFrame.ShowMessage(-4, lbTicker.Text);
                        return;
                    }

                    LoadDataSingle();
                }
                else if (TabControl.ActiveTab.Text == "Basket")
                {
                    if (txtTicker.Text == null)
                    {
                        EFrame.ShowMessage(-4, lbTicker2.Text);
                        return;
                    }

                    LoadDataBasket();
                }
            }
            else if (e.Menukey == "MG340024_Hold")
            {
                EFrame.ShowForm("MG340026", new Dictionary<string, object>
                {
                    ["HKOHD_INSNBR"] = cmbTicker.Value.ToDecimal()
                });
            }
            else if(e.Menukey == "MG340024_SBL")
            {
                EFrame.ShowForm("MG340028", new Dictionary<string, object>
                {
                    ["HKSBL_INSNBR"] = cmbTicker.Value.ToDecimal(),
                    ["HKSBL_PRICE"] = numHKDPrice.Value.ToDecimal()
                });
            }
        }

        private void LoadDataSingle()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                DataTable dtInventory = EFrame.GetService("SG340024_GetInventory", new Dictionary<string, object>()
                {
                    ["INSTR_INSNBR"] = cmbTicker.Value.ToDecimal()
                }).Value as DataTable;

                if (dtInventory.Rows.Count > 0)
                {
                    DataRow rowInventory = dtInventory.Rows[0];

                    txtStockName.Text = rowInventory["INSTR_NAME"].ToString();
                    txtIndex.Text = rowInventory["INDEX"].ToString();
                    dteEXDate.Value = rowInventory["HKBE_EX_DIVIDEND_DATE"].ToDecimal() == 0 ? (object)null : rowInventory["HKBE_EX_DIVIDEND_DATE"].ToDecimal().ToDateTime();
                    numHKDPrice.Value = rowInventory["HISPRS_CLOSE"].ToDecimal();
                    numUSDPrice.Value = rowInventory["PX_IN_USD"].ToDecimal();
                    numLot.Value = rowInventory["ROUND_LOT"].ToDecimal();
                    num5DaysFee.Value = rowInventory["5_DAYS_FEE"].ToDecimal();
                }

                DataTable dtSBLDetail = EFrame.GetService("SG340024_GetSBLDetail", new Dictionary<string, object>()
                {
                    ["INSTR_INSNBR"] = cmbTicker.Value.ToDecimal()
                }).Value as DataTable;

                EFrame.SetUltraGrid(ugSBLDetail, "Grid.MG340024_ugSBLDetail.xml", dtSBLDetail);

                DataTable dtDateDetail = EFrame.GetService("SG340024_GetDateDetail", new Dictionary<string, object>()
                {
                    ["INSTR_INSNBR"] = cmbTicker.Value.ToDecimal()
                }).Value as DataTable;

                EFrame.SetUltraGrid(ugDateDetail, "Grid.MG340024_ugDateDetail.xml", dtDateDetail);

                DataTable dtTradeDetail = EFrame.GetService("SG340024_GetTradeDetail", new Dictionary<string, object>()
                {
                    ["INSTR_INSNBR"] = cmbTicker.Value.ToDecimal()
                }).Value as DataTable;

                EFrame.SetUltraGrid(ugTradeDetail, "Grid.MG340024_ugTradeDetail.xml", dtTradeDetail);

            }
            catch (Exception ex) { EFrame.LogMessage(ex, LogType.Critical); throw ex; }
            finally { FormLock = false; }
        }

        private void LoadDataBasket()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                List<string> insnbrList = new List<string>();

                //insnbrList = txtTicker.Text.Split(',').Select(x => x.Trim()).ToList();
                if (txtTicker.Text != string.Empty)
                {
                    insnbrList = txtTicker.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    insnbrList = new List<string> { "ALL" };
                }

                DataTable dtBasket = EFrame.GetService("SG340024_GetBasket", new Dictionary<string, object>()
                {
                    ["INSTR_INSNBR"] = insnbrList
                }).Value as DataTable;

                EFrame.SetUltraGrid(ugBasket, "Grid.MG340024_ugBasket.xml", dtBasket);

                txtTicker.Focus();
            }
            catch (Exception ex) { EFrame.LogMessage(ex, LogType.Critical); throw ex; }
            finally { FormLock = false; }
        }

        private void ugBasket_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells["SHORT_SELL"].Value.ToDecimal() < 0)
                e.Row.Cells["SHORT_SELL"].Appearance.BackColor = System.Drawing.Color.Red;
        }

        private void Inventory_ReciveInternalMessage(object sender, WinFormReciveInternalMessageEventArgs e)
        {
            if (e.InternalTopic == "WAFT.dbo.HK_SBL_HOLD" || e.InternalTopic == "WAFT.dbo.HK_SBL_SBL")
            {
                if (TabControl.ActiveTab.Text == "Single" && cmbTicker.SelectedRow != null)
                {
                    StopReciveInternalMessage = true;
                    LoadDataSingle();
                    StopReciveInternalMessage = false;

                    return;
                }
            }

            if (e.InternalTopic == "EDAISYS.STATUS_EVENT" && e.Parameters.ContainsKey("STSEVT_NAME"))
            {
                if (e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.DailyImportSBLRate" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.DailyImportSBLRate_LastTradeDate" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HKSBLLocateFileAfterImportHold" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HKSBLLocateFileAfterImportHold_LastTradeDate" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HKBloombergBenefitEntitlement" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HKBloombergBenefitEntitlement_LastTradeDate" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.DailyImportSBLOpenTrades" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.DailyImportSBLOpenTrades_LastTradeDate" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.DailyImportSBLAvailableGSPS" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.DailyImportSBLAvailableGSPS_LastTradeDate" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HistoricalBalance5Weeks" ||
                    e.Parameters["STSEVT_NAME"].ToString() == "SG.SCH.HistoricalBalance5Weeks_LastTradeDate")
                {
                    if (TabControl.ActiveTab.Text == "Single" && cmbTicker.SelectedRow != null)
                    {
                        StopReciveInternalMessage = true;
                        LoadDataSingle();
                        StopReciveInternalMessage = false;

                        return;
                    }
                }
            }
        }

        private void ugSBLDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["SBL_TOTAL"].CellAppearance.TextHAlign = HAlign.Right;
            e.Layout.Bands[0].Columns["HOLD_TOTAL"].CellAppearance.TextHAlign = HAlign.Right;
        }
    }
}
