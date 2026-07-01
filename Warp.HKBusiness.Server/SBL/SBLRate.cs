using EFramework.EFrame;
using EFramework.EFrame.Common;
using EFramework.EFrame.Logging;
using EFramework.EFrame.MSExcel;
using EFramework.Server.Servicies.ServiceSpace;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Warp.HKBusiness.Server
{
    public class SBLRate : Service
    {
        /// <summary>
        /// 2.2	LIST OF ELIGIBLE FOR SHORT SELLING & SBL RATE (SS LIST)
        /// SG340013_DailyImportSBLRate
        /// SG.SCH.DailyImportSBLRate
        /// </summary>
        /// <returns></returns>
        public CommonResult DailyImportSBLRate(Dictionary<string, object> parameters)
        {
            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            var fileName = Model.FlagData["HKSBL_SBL_RATE", "ImportFileName"].FlagData;

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    excelAp.Close();
                }
            }

            var date = parameters["PROCESS_DATE"].ToDecimal();
            //var date = dtContent.Columns[0].ColumnName.ToString()?.ToDateTime().ToDecimal();

            // 出HK_SBL_DAILY_SBL_RATE datatable
            DataTable dtDailySBLRate = DataAccess.DB["WARP"].QueryDataTable($"select HKDSR_CREATOR,HKDSR_CREATME,HKDSR_UPDUSR,HKDSR_UPDTME,HKDSR_DATE,HKDSR_STOCK_CODE,HKDSR_STOCK_SHORT_NAME,HKDSR_CURRENCY,HKDSR_TICKER,HKDSR_SBL_RATE_1,HKDSR_SBL_RATE_2 from EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE WHERE 1=2");
            dtDailySBLRate.TableName = "EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE";
            dtDailySBLRate.PrimaryKey = new DataColumn[] { dtDailySBLRate.Columns["HKDSR_DATE"], dtDailySBLRate.Columns["HKDSR_STOCK_CODE"] };

            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtDailySBLRate.NewRow();
                dr["HKDSR_CREATOR"] = dr["HKDSR_UPDUSR"] = "SYSTEM";
                dr["HKDSR_CREATME"] = dr["HKDSR_UPDTME"] = DateTime.Now;
                dr["HKDSR_DATE"] = date;
                dr["HKDSR_STOCK_CODE"] = dtContent.Rows[i][0].ToString();
                dr["HKDSR_STOCK_SHORT_NAME"] = dtContent.Rows[i][1].ToString();
                dr["HKDSR_CURRENCY"] = dtContent.Rows[i][2].ToString();
                dr["HKDSR_TICKER"] = dtContent.Rows[i][3].ToString();

                var rate1 = dtContent.Rows[i][4].ToString();
                var rate2 = dtContent.Rows[i][5].ToString();
                //var rate2 = "0";

                decimal decimalRate1 = 0;
                decimal decimalRate2 = 0;

                dr["HKDSR_SBL_RATE_1"] = (decimal.TryParse(rate1, out decimalRate1) ? decimalRate1 : 0) / 100;
                dr["HKDSR_SBL_RATE_2"] = (decimal.TryParse(rate2, out decimalRate2) ? decimalRate2 : 0) / 100;
                dtDailySBLRate.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE WHERE HKDSR_DATE = @DATE", new Dictionary<string, object>() { { "DATE", date } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtDailySBLRate);

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// 2.2.1	IMPORT SBL RATE
        /// SG340014_ImportSBLRateData
        /// SG.SCH.ImportSBLRateData
        /// 不需要SCHEDULE 可刪除
        /// </summary>
        /// <returns></returns>
        public CommonResult ImportSBLRateData(Dictionary<string, object> parameters)
        {
            var dtContent = parameters["DATA"] as DataTable;

            var date = parameters["IMPORT_DATE"].ToDecimal();

            // 出HK_SBL_DAILY_SBL_RATE datatable
            DataTable dtDailySBLRate = DataAccess.DB["WARP"].QueryDataTable($"select HKDSR_CREATOR,HKDSR_CREATME,HKDSR_UPDUSR,HKDSR_UPDTME,HKDSR_DATE,HKDSR_STOCK_CODE,HKDSR_STOCK_SHORT_NAME,HKDSR_CURRENCY,HKDSR_TICKER,HKDSR_SBL_RATE_1,HKDSR_SBL_RATE_2 from EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE WHERE 1=2");
            dtDailySBLRate.TableName = "EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE";
            dtDailySBLRate.PrimaryKey = new DataColumn[] { dtDailySBLRate.Columns["HKDSR_DATE"], dtDailySBLRate.Columns["HKDSR_STOCK_CODE"] };

            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtDailySBLRate.NewRow();
                dr["HKDSR_CREATOR"] = dr["HKDSR_UPDUSR"] = "SYSTEM";
                dr["HKDSR_CREATME"] = dr["HKDSR_UPDTME"] = DateTime.Now;
                dr["HKDSR_DATE"] = date;
                dr["HKDSR_STOCK_CODE"] = dtContent.Rows[i][0].ToString();
                dr["HKDSR_STOCK_SHORT_NAME"] = dtContent.Rows[i][1].ToString();
                dr["HKDSR_CURRENCY"] = dtContent.Rows[i][2].ToString();
                dr["HKDSR_TICKER"] = dtContent.Rows[i][3].ToString();

                var rate1 = dtContent.Rows[i][4].ToString();
                var rate2 = dtContent.Rows[i][5].ToString();
                //var rate2 = "0";

                decimal decimalRate1 = 0;
                decimal decimalRate2 = 0;

                dr["HKDSR_SBL_RATE_1"] = (decimal.TryParse(rate1, out decimalRate1) ? decimalRate1 : 0) / 100;
                dr["HKDSR_SBL_RATE_2"] = (decimal.TryParse(rate2, out decimalRate2) ? decimalRate2 : 0) / 100;
                dtDailySBLRate.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE WHERE HKDSR_DATE = @DATE", new Dictionary<string, object>() { { "DATE", date } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtDailySBLRate);

            PublishInternalMessage("EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE", new Dictionary<string, object>
            {
                { "IMPORT_DATE", date }
            });

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// 2.2	LIST OF ELIGIBLE FOR SHORT SELLING & SBL RATE (SS LIST)
        /// SG340013_HKEligibleShortSelling
        /// SG.SCH.HKEligibleShortSelling
        /// </summary>
        /// <returns></returns>
        public CommonResult HKEligibleShortSelling(Dictionary<string, object> parameters)
        {
            //var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;
            var processDate = parameters["PROCESS_DATE"].ToString();

            var fileUrl = Model.FlagData["HKSBL_SBL_SHORT_SELLING", "ImportFileUrl"].FlagData.Replace("{DATE}", processDate);

            DataTable dtContent = null;
            try
            {
                var bytes = new byte[0];
                using (var client = new WebClient())
                {
                    bytes = client.DownloadData(fileUrl);
                }

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
                {
                    using (ExcelApplication excelAp = new ExcelApplication(ms))
                    {
                        if (excelAp.Sheets["Sheet1"] != null)
                            dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A5");
                    }
                }


            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("404"))
                {
                    return new CommonResult(true, "Today HKEX no Eligible Short Selling Data", null);
                }

                throw;
            }

            // 出HK_SBL_SHORT_SELLING_LIST datatable
            DataTable dtDailySBLShortSelling = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_SHORT_SELLING_LIST WHERE 1=2");
            dtDailySBLShortSelling.TableName = "EXTSRC.dbo.HK_SBL_SHORT_SELLING_LIST";
            dtDailySBLShortSelling.PrimaryKey = new DataColumn[] { dtDailySBLShortSelling.Columns["HKSSL_DATE"], dtDailySBLShortSelling.Columns["HKSSL_STOCK_CODE"] };

            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtDailySBLShortSelling.NewRow();
                dr["HKSSL_CREATOR"] = dr["HKSSL_UPDUSR"] = "SYSTEM";
                dr["HKSSL_CREATME"] = dr["HKSSL_UPDTME"] = DateTime.Now;

                dr["HKSSL_DATE"] = processDate.ToDecimal();
                dr["HKSSL_STOCK_CODE"] = dtContent.Rows[i][1].ToString();
                dr["HKSSL_STOCK_SHORT_NAME"] = dtContent.Rows[i][2].ToString();
                dr["HKSSL_CURRENCY"] = dtContent.Rows[i][3].ToString();
                dr["HKSSL_TYPE"] = dtContent.Rows[i][4].ToString();
                dr["HKSSL_EXEMPT_FROM_TICK_RULE"] = dtContent.Rows[i][5].ToString();
                dr["HKSSL_REMARKS"] = dtContent.Rows[i][6].ToString();

                dtDailySBLShortSelling.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_SHORT_SELLING_LIST WHERE HKSSL_DATE = @DATE", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtDailySBLShortSelling);

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// 2.3	DAILY SBL AVAILABLE BALANCE (DATA)
        /// SG340015_DailyImportSBLAvailableGSPS
        /// SG.SCH.DailyImportSBLAvailableGSPS
        /// </summary>
        /// <returns></returns>
        public CommonResult DailyImportSBLAvailableGSPS(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();
            var parseDateStr = DateTime.ParseExact(processDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-TW")).ToString("MMddyy");

            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            var fileName = Model.FlagData["HKSBL_SBL_AVAILABLE_GSPS", "ImportFileName"].FlagData.Replace("MMddyy", parseDateStr);

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出 HK_SBL_AVAILABLE_GSPS datatable
            DataTable dtSBLAvailableGSPS = DataAccess.DB["WARP"].QueryDataTable($"select HKAVL_CREATOR,HKAVL_CREATME,HKAVL_UPDUSR,HKAVL_UPDTME,HKAVL_DATE,HKAVL_VALUE_DATE,HKAVL_REUTER_CODE,HKAVL_STOCK_NAME,HKAVL_MARGIN_AC_BALANCE,HKAVL_SHORT_SELLING_AC_BALANCE,HKAVL_MAX_OF_02,HKAVL_EXCOUDED_HOLDING,HKAVL_NET_MARGIN_HOLDING,HKAVL_NO_OF_MARGIN_HOLDER,HKAVL_STOCK_BOARD_LOTS,HKAVL_SBL_AVAILABLE_LOTS,HKAVL_SBL_AVAILABLE_BALANCE from EXTSRC.dbo.HK_SBL_AVAILABLE_GSPS WHERE 1=2");
            dtSBLAvailableGSPS.TableName = "EXTSRC.dbo.HK_SBL_AVAILABLE_GSPS";
            dtSBLAvailableGSPS.PrimaryKey = new DataColumn[] { dtSBLAvailableGSPS.Columns["HKAVL_DATE"], dtSBLAvailableGSPS.Columns["HKAVL_REUTER_CODE"] };

            // map xlsx欄位to datatable欄位
            for (int i = 1; i < dtContent.Rows.Count; i++)
            {
                var dr = dtSBLAvailableGSPS.NewRow();
                dr["HKAVL_CREATOR"] = dr["HKAVL_UPDUSR"] = "SYSTEM";
                dr["HKAVL_CREATME"] = dr["HKAVL_UPDTME"] = DateTime.Now;
                dr["HKAVL_DATE"] = dtContent.Rows[i][0].ToDateTime().ToDecimal();
                dr["HKAVL_VALUE_DATE"] = dtContent.Rows[i][1].ToDateTime().ToDecimal();
                var reuterCode = dtContent.Rows[i][2].ToString();
                // 9826.HK格式有問題 暫時跳過
                // 20260401修正 註解
                //if (reuterCode == "9826.HK")
                //    continue;
                dr["HKAVL_REUTER_CODE"] = reuterCode;
                dr["HKAVL_STOCK_NAME"] = dtContent.Rows[i][3].ToString();
                dr["HKAVL_MARGIN_AC_BALANCE"] = dtContent.Rows[i][4].ToDecimal();
                dr["HKAVL_SHORT_SELLING_AC_BALANCE"] = dtContent.Rows[i][5].ToDecimal();
                dr["HKAVL_MAX_OF_02"] = dtContent.Rows[i][6].ToDecimal();
                dr["HKAVL_EXCOUDED_HOLDING"] = dtContent.Rows[i][7].ToDecimal();
                dr["HKAVL_NET_MARGIN_HOLDING"] = dtContent.Rows[i][8].ToDecimal();
                dr["HKAVL_NO_OF_MARGIN_HOLDER"] = dtContent.Rows[i][9].ToDecimal();
                dr["HKAVL_STOCK_BOARD_LOTS"] = dtContent.Rows[i][10].ToDecimal();
                dr["HKAVL_SBL_AVAILABLE_LOTS"] = dtContent.Rows[i][11].ToDecimal();
                dr["HKAVL_SBL_AVAILABLE_BALANCE"] = dtContent.Rows[i][12].ToDecimal();

                dtSBLAvailableGSPS.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_AVAILABLE_GSPS WHERE HKAVL_DATE = @DATE", new Dictionary<string, object> { { "DATE", dtSBLAvailableGSPS.Rows[0]["HKAVL_DATE"] } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtSBLAvailableGSPS);

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// 2.4	SBL OPEN TRADES (OPEN TRADES)
        /// SG340016_DailyImportSBLOpenTrades
        /// SG.SCH.DailyImportSBLOpenTrades
        /// </summary>
        /// <returns></returns>
        public CommonResult DailyImportSBLOpenTrades(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();

            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            var fileName = Model.FlagData["HKSBL_TRADE_DATA", "ImportFileName"].FlagData.Replace("{DATE}", processDate);

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    if (excelAp.Sheets["gvMain"] != null)
                        dtContent = excelAp.Sheets["gvMain"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出 HK_SBL_FI_TRADE_DATA datatable
            DataTable dtSBLTradeData = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_FI_TRADE_DATA WHERE 1=2");
            dtSBLTradeData.TableName = "EXTSRC.dbo.HK_SBL_FI_TRADE_DATA";
            dtSBLTradeData.PrimaryKey = new DataColumn[] { dtSBLTradeData.Columns["HKOPT_DATE"], dtSBLTradeData.Columns["HKOPT_TRADE_ID"] };

            // map xlsx欄位to datatable欄位
            for (int i = 1; i < dtContent.Rows.Count; i++)
            {
                var dr = dtSBLTradeData.NewRow();
                dr["HKOPT_CREATOR"] = dr["HKOPT_UPDUSR"] = "SYSTEM";
                dr["HKOPT_CREATME"] = dr["HKOPT_UPDTME"] = DateTime.Now;

                dr["HKOPT_DATE"] = processDate.ToDecimal();
                dr["HKOPT_TRADE_TYPE"] = dtContent.Rows[i][0].ToString();
                dr["HKOPT_TRADE_ID"] = dtContent.Rows[i][1].ToDecimal();
                dr["HKOPT_UNDERLYING"] = dtContent.Rows[i][2].ToString();
                dr["HKOPT_UND_ISSUER"] = dtContent.Rows[i][3].ToString();
                dr["HKOPT_ISIN"] = dtContent.Rows[i][4].ToString();
                //dr["HKOPT_LEG_TYPE"] = dtContent.Rows[i][4].ToString();
                dr["HKOPT_TRADE_CCY"] = dtContent.Rows[i][5].ToString();
                dr["HKOPT_STOCK_CCY"] = dtContent.Rows[i][6].ToString();
                dr["HKOPT_OPEN"] = dtContent.Rows[i][7].ToString();
                var tradeDate = dtContent.Rows[i][8].ToDateTime();
                dr["HKOPT_TRADE_DATE"] = tradeDate.ToDecimal();
                dr["HKOPT_VALUE_DATE"] = dtContent.Rows[i][9].ToDateTime().ToDecimal();
                dr["HKOPT_START"] = dtContent.Rows[i][10].ToDateTime().ToDecimal();
                dr["HKOPT_END"] = dtContent.Rows[i][11].ToDateTime().ToDecimal();
                dr["HKOPT_SIDE"] = dtContent.Rows[i][12].ToString();
                dr["HKOPT_QUANTITY"] = dtContent.Rows[i][13].ToDecimal();

                var stockCcyPrice = dtContent.Rows[i][14].ToString();
                dr["HKOPT_STOCK_CCY_PRICE"] = decimal.TryParse(stockCcyPrice, out decimal dStockCcyPrice) ? dStockCcyPrice : 0;
                dr["HKOPT_COLLATERAL_PRICE"] = dtContent.Rows[i][15].ToDecimal();

                var price = dtContent.Rows[i][16].ToString();
                dr["HKOPT_PRICE"] = decimal.TryParse(price, out decimal dPrice) ? dPrice : 0;
                dr["HKOPT_FX"] = dtContent.Rows[i][17].ToDecimal();
                dr["HKOPT_REF_VALUE"] = dtContent.Rows[i][18].ToDecimal();

                var fee = dtContent.Rows[i][19].ToString();
                dr["HKOPT_FEE"] = decimal.TryParse(fee, out decimal dFee) ? dFee : 0;

                var haircut = dtContent.Rows[i][20].ToString();
                dr["HKOPT_HAIRCUT"] = decimal.TryParse(haircut, out decimal dHaircut) ? dHaircut : 0;

                var rebate = dtContent.Rows[i][21].ToString();
                dr["HKOPT_REBATE"] = decimal.TryParse(rebate, out decimal dRebate) ? dRebate : 0;
                dr["HKOPT_COUNTERPARTY"] = dtContent.Rows[i][22].ToString();
                dr["HKOPT_PORTFOLIO"] = dtContent.Rows[i][23].ToString();

                var dailyInterest = dtContent.Rows[i][24].ToString();
                dr["HKOPT_DAILY_INTEREST"] = decimal.TryParse(dailyInterest, out decimal dDailyInterest) ? dDailyInterest : 0;

                dr["HKOPT_TRADER"] = dtContent.Rows[i][25].ToString();
                dr["HKOPT_ACQUIRER"] = dtContent.Rows[i][26].ToString();
                dr["HKOPT_STATUS"] = dtContent.Rows[i][27].ToString();
                dr["HKOPT_SETTLE_DATE_RULE"] = dtContent.Rows[i][28].ToString();
                dr["HKOPT_SOURCE"] = dtContent.Rows[i][29].ToString();

                dtSBLTradeData.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_FI_TRADE_DATA WHERE HKOPT_DATE = @DATE", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtSBLTradeData);

            return new CommonResult(true, "", dtContent);
        }

        /// <summary>
        /// 2.5	HISTORICAL BALANCE (BALANCE)
        /// SG340017_HistoricalBalance5Weeks
        /// SG.SCH.HistoricalBalance5Weeks
        /// </summary>
        /// <returns></returns>
        public CommonResult HistoricalBalance5Weeks(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();
            var parseDateStr = DateTime.ParseExact(processDate, "yyyyMMdd", new System.Globalization.CultureInfo("zh-TW")).ToString("MMddyy");

            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            //Margin_Balance_Report_5weeks_MMddyy.xlsx
            var fileName = Model.FlagData["HKSBL_HISTORICAL_BALANCE_5WEEKS", "ImportFileName"].FlagData.Replace("MMddyy", parseDateStr);

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出 HK_SBL_BALANCE_FIVE_WEEKS datatable
            DataTable dtBalabceFiveWeeks = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_BALANCE_FIVE_WEEKS WHERE 1=2");
            dtBalabceFiveWeeks.TableName = "EXTSRC.dbo.HK_SBL_BALANCE_FIVE_WEEKS";
            dtBalabceFiveWeeks.PrimaryKey = new DataColumn[] { dtBalabceFiveWeeks.Columns["HKBFW_DATE"], dtBalabceFiveWeeks.Columns["HKBFW_GLOSS_INSTRUMENT_CODE"] };

            // map xlsx欄位to datatable欄位
            for (int i = 1; i < dtContent.Rows.Count; i++)
            {
                var dr = dtBalabceFiveWeeks.NewRow();
                dr["HKBFW_CREATOR"] = dr["HKBFW_UPDUSR"] = "SYSTEM";
                dr["HKBFW_CREATME"] = dr["HKBFW_UPDTME"] = DateTime.Now;

                dr["HKBFW_DATE"] = processDate.ToDecimal();
                dr["HKBFW_GLOSS_INSTRUMENT_CODE"] = dtContent.Rows[i][0].ToString();
                dr["HKBFW_HKSE_CODE"] = dtContent.Rows[i][1].ToDecimal();

                IFormatProvider formatProvider1 = new System.Globalization.CultureInfo("zh-TW");
                var date1 = DateTime.ParseExact(dtContent.Columns[2].ColumnName, "MM/dd/yyyy", formatProvider1);
                dr["HKBFW_DATE1"] = date1.ToDecimal();
                dr["HKBFW_QUANTITY1"] = dtContent.Rows[i][2].ToDecimal();

                IFormatProvider formatProvider2 = new System.Globalization.CultureInfo("zh-TW");
                var date2 = DateTime.ParseExact(dtContent.Columns[3].ColumnName, "MM/dd/yyyy", formatProvider2);
                dr["HKBFW_DATE2"] = date2.ToDecimal();
                dr["HKBFW_QUANTITY2"] = dtContent.Rows[i][3].ToDecimal();

                IFormatProvider formatProvider3 = new System.Globalization.CultureInfo("zh-TW");
                var date3 = DateTime.ParseExact(dtContent.Columns[4].ColumnName, "MM/dd/yyyy", formatProvider3);
                dr["HKBFW_DATE3"] = date3.ToDecimal();
                dr["HKBFW_QUANTITY3"] = dtContent.Rows[i][4].ToDecimal();

                IFormatProvider formatProvide4 = new System.Globalization.CultureInfo("zh-TW");
                var date4 = DateTime.ParseExact(dtContent.Columns[5].ColumnName, "MM/dd/yyyy", formatProvide4);
                dr["HKBFW_DATE4"] = date4.ToDecimal();
                dr["HKBFW_QUANTITY4"] = dtContent.Rows[i][5].ToDecimal();

                IFormatProvider formatProvide5 = new System.Globalization.CultureInfo("zh-TW");
                var date5 = DateTime.ParseExact(dtContent.Columns[6].ColumnName, "MM/dd/yyyy", formatProvide5);
                dr["HKBFW_DATE5"] = date5.ToDecimal();
                dr["HKBFW_QUANTITY5"] = dtContent.Rows[i][6].ToDecimal();

                IFormatProvider formatProvide6 = new System.Globalization.CultureInfo("zh-TW");
                var date6 = DateTime.ParseExact(dtContent.Columns[7].ColumnName, "MM/dd/yyyy", formatProvide6);
                dr["HKBFW_DATE6"] = date6.ToDecimal();
                dr["HKBFW_QUANTITY6"] = dtContent.Rows[i][7].ToString();

                dtBalabceFiveWeeks.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_BALANCE_FIVE_WEEKS WHERE HKBFW_DATE = @DATE", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtBalabceFiveWeeks);

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// 2.6	DAILY OUTSTANDING POSITION
        /// SG340018_DailyOutstandingPosition
        /// SG.SCH.DailyOutstandingPosition
        /// </summary>
        /// <returns></returns>
        public CommonResult DailyOutstandingPosition(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();

            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            var fileName = Model.FlagData["HKSBL_OUTSTANDING_POSITION", "ImportFileName"].FlagData.Replace("{DATE}", processDate);

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    if (excelAp.Sheets["Oustanding"] != null)
                        dtContent = excelAp.Sheets["Oustanding"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出 EXTSRC.dbo.HK_SBL_DAILY_OUTSTANDING datatable
            DataTable dtOutstandingPosition = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_DAILY_OUTSTANDING WHERE 1=2");
            dtOutstandingPosition.TableName = "EXTSRC.dbo.HK_SBL_DAILY_OUTSTANDING";
            dtOutstandingPosition.PrimaryKey = new DataColumn[] { dtOutstandingPosition.Columns["HKDOS_DATE"], dtOutstandingPosition.Columns["HKDOS_COUNTERPARTY"], dtOutstandingPosition.Columns["HKDOS_UNDERLYING"] };

            var groups = dtContent.AsEnumerable().Skip(1) // 跳過 header row
                .GroupBy(r => new
                {
                    Date = processDate.ToDecimal(), // 或如果要用檔案內的日期，改成 r[0].ToDateTime().ToDecimal() 或對應欄位
                    Counterparty = r[0].ToString().Trim(),
                    Underlying = r[1].ToString().Trim()
                });

            foreach (var g in groups)
            {
                var totalQty = g.Sum(r => r[3].ToDecimal());
                var totalCollateral = g.Sum(r => r[6].ToDecimal());

                var dr = dtOutstandingPosition.NewRow();
                dr["HKDOS_CREATOR"] = dr["HKDOS_UPDUSR"] = "SYSTEM";
                dr["HKDOS_CREATME"] = dr["HKDOS_UPDTME"] = DateTime.Now;
                dr["HKDOS_DATE"] = g.Key.Date;
                dr["HKDOS_COUNTERPARTY"] = g.Key.Counterparty;
                dr["HKDOS_UNDERLYING"] = g.Key.Underlying;
                dr["HKDOS_CURRENCY"] = g.First()[2].ToString(); // 若 currency 在各筆相同
                dr["HKDOS_QUANTITY"] = totalQty;
                dr["HKDOS_LASTEST_CLOSING_PRICE"] = g.First()[4];

                var marginStr = g.First()[5].ToString().Trim('%');
                dr["HKDOS_MARGIN"] = marginStr.ToDecimal() * 0.01m;
                dr["HKDOS_COLLATERAL_VALUE"] = totalCollateral;

                dtOutstandingPosition.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_DAILY_OUTSTANDING WHERE HKDOS_DATE = @DATE", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtOutstandingPosition);

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// no use
        /// 2.7	LOCATE FILE
        /// SG340019_HKSBLLocateFile
        /// SG.SCH.HKSBLLocateFile
        /// </summary>
        /// <returns></returns>
        public CommonResult HKSBLLocateFile(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();

            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            // {DATE}_response.csv
            var fileName = Model.FlagData["HKSBL_LOCATE_FILE", "ImportFileName"].FlagData.Replace("{DATE}", processDate);

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    var a = Path.GetFileNameWithoutExtension(fileName);

                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出 HK_SBL_LOCATE_GRAVITON datatable
            DataTable dtLocate = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_LOCATE_GRAVITON WHERE 1=2");
            dtLocate.TableName = "EXTSRC.dbo.HK_SBL_LOCATE_GRAVITON";
            dtLocate.PrimaryKey = new DataColumn[] { dtLocate.Columns["HKLFG_DATE"], dtLocate.Columns["HKLFG_TICKER"] };

            // map xlsx欄位to datatable欄位
            for (int i = 1; i < dtContent.Rows.Count; i++)
            {
                var dr = dtLocate.NewRow();
                dr["HKLFG_CREATOR"] = dr["HKLFG_UPDUSR"] = "SYSTEM";
                dr["HKLFG_CREATME"] = dr["HKLFG_UPDTME"] = DateTime.Now;

                dr["HKLFG_DATE"] = processDate.ToDecimal();
                dr["HKLFG_ISIN"] = dtContent.Rows[i][0].ToString();
                dr["HKLFG_TICKER"] = dtContent.Rows[i][1].ToString();
                dr["HKLFG_QUANTITY"] = dtContent.Rows[i][2].ToDecimal();

                var sblRate = dtContent.Rows[i][3].ToString().Trim('%');
                dr["HKLFG_SBL_RATE"] = sblRate.ToDecimal();


                dtLocate.Rows.Add(dr);
            }

            // 全刪
            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_LOCATE_GRAVITON");
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtLocate);

            return new CommonResult(true, "", null);
        }
        /// <summary>
        /// 20260504 jet add
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommonResult HKSBLLocateFileSource(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();
            var source = parameters["SOURCE"].ToString();
            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;
            var shareFolderGrasshopper = Model.FlagData["SBL_GAM_HK_CLIENT_DATA", "Response_File_Path_toHK"].FlagData;
            // QE{DATE}_response.csv
            var fileName = Model.FlagData["HKSBL_LOCATE_FILE", source].FlagData.Replace("{DATE}", processDate);

            string fullFilePath = shareFolder + fileName;
            if (source == "Grasshopper")
            {
                fullFilePath = shareFolderGrasshopper + fileName;
            }
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    var a = Path.GetFileNameWithoutExtension(fileName);

                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出 HK_SBL_LOCATE_GRAVITON datatable
            DataTable dtLocate = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_LOCATE WHERE 1=2");
            dtLocate.TableName = "EXTSRC.dbo.HK_SBL_LOCATE";
            dtLocate.PrimaryKey = new DataColumn[] { dtLocate.Columns["HKLFG_DATE"], dtLocate.Columns["HKLFG_TICKER"] };

            // map xlsx欄位to datatable欄位
            for (int i = 1; i < dtContent.Rows.Count; i++)
            {
                var dr = dtLocate.NewRow();
                dr["HKLFG_CREATOR"] = dr["HKLFG_UPDUSR"] = "SYSTEM";
                dr["HKLFG_CREATME"] = dr["HKLFG_UPDTME"] = DateTime.Now;

                dr["HKLFG_DATE"] = processDate.ToDecimal();
                if (source == "Grasshopper")
                {
                    dr["HKLFG_ISIN"] = "";
                    string value = dtContent.Rows[i][0].ToString(); // 0175.HK 
                    string[] parts = value.Split('.');
                    string stockNo = parts[0].TrimStart('0');
                    string result = stockNo + " " + parts[1];

                    dr["HKLFG_TICKER"] = result;
                    dr["HKLFG_QUANTITY"] = dtContent.Rows[i][2].ToDecimal();
                    dr["HKLFG_SBL_RATE"] = 0.25;
                }
                else
                {
                    dr["HKLFG_ISIN"] = dtContent.Rows[i][0].ToString();
                    dr["HKLFG_TICKER"] = dtContent.Rows[i][1].ToString();
                    dr["HKLFG_QUANTITY"] = dtContent.Rows[i][2].ToDecimal();
                    var sblRate = dtContent.Rows[i][3].ToString().Trim('%');
                    dr["HKLFG_SBL_RATE"] = sblRate.ToDecimal();
                }
                dr["HKLFG_SOURCE"] = source;

                dtLocate.Rows.Add(dr);
            }

            // 全刪
            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_LOCATE WHERE HKLFG_SOURCE=@SOURCE", new Dictionary<string, object>() { { "SOURCE", source } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtLocate);

            return new CommonResult(true, "", null);
        }
        /// <summary>
        /// 20260504 jet add
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommonResult HKSBLLocateFileAPAC(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();
            var source = parameters["SOURCE"].ToString();
            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            string filePrefix = "KGIHK_SBL_APAC_out_{DATE}".Replace("{DATE}", processDate);

            // 搜尋符合前綴的檔案
            string fullFilePath = null;
            try
            {
                var matchingFiles = Directory.GetFiles(shareFolder, filePrefix + "*");
                if (matchingFiles.Length == 0)
                {
                    LogMessage($"File not found with prefix: {filePrefix}*", LogType.Warning);
                    return new CommonResult(true, $"File not found with prefix: {filePrefix}", null);
                }

                // 取得第一個符合的檔案
                fullFilePath = matchingFiles[0];
            }
            catch (Exception ex)
            {
                LogMessage($"Error searching for file with prefix {filePrefix}: {ex.Message}", LogType.Warning);
                return new CommonResult(true, $"Error searching for file: {ex.Message}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    // var a = Path.GetFileNameWithoutExtension(fileName);

                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出 HK_SBL_LOCATE_GRAVITON datatable
            DataTable dtLocate = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_AVAILABLE_APEC WHERE 1=2");
            dtLocate.TableName = "EXTSRC.dbo.HK_SBL_AVAILABLE_APEC";
            dtLocate.PrimaryKey = new DataColumn[] { dtLocate.Columns["HKAVA_DATE"], dtLocate.Columns["HKAVA_LOCATE_ID"] };

            // map xlsx欄位to datatable欄位
            for (int i = 1; i < dtContent.Rows.Count; i++)
            {
                var dr = dtLocate.NewRow();
                dr["HKAVA_CREATOR"] = dr["HKAVA_UPDUSR"] = "SYSTEM";
                dr["HKAVA_CREATME"] = dr["HKAVA_UPDTME"] = DateTime.Now;
                dr["HKAVA_DATE"] = processDate.ToDecimal();
                dr["HKAVA_LOCATE_ID"] = dtContent.Rows[i][1].ToString();
                dr["HKAVA_REQUEST_REFERENCE_ID"] = dtContent.Rows[i][2].ToString();
                dr["HKAVA_CLIENT_CODE"] = dtContent.Rows[i][3].ToString();
                dr["HKAVA_SECURITY_ID"] = dtContent.Rows[i][4].ToString();
                dr["HKAVA_REQUESTED_QUANTITY"] = dtContent.Rows[i][5].ToDecimal();
                dr["HKAVA_STATUS"] = dtContent.Rows[i][6].ToString();
                dr["HKAVA_APPROVED_QUANTITY"] = dtContent.Rows[i][7].ToDecimal();
                dr["HKAVA_APPROVED_RATE"] = dtContent.Rows[i][8].ToDecimal();
                dr["HKAVA_APPROVED_RATE_TYPE"] = dtContent.Rows[i][9].ToString();
                DateTime dt;

                if (DateTime.TryParseExact(
                        dtContent.Rows[i][10].ToString(),
                        "M/d/yyyy H:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out dt))
                {
                    dr["HKAVA_APPROVED_DATETIME"] = dt;
                }
                else
                {
                    dr["HKAVA_APPROVED_DATETIME"] = DBNull.Value; // 或你要的預設值
                }
                dtLocate.Rows.Add(dr);
            }

            // 全刪
            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_AVAILABLE_APEC ", new Dictionary<string, object>() { { "SOURCE", source } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtLocate);

            return new CommonResult(true, "", null);
        }
        /// <summary>
        /// 20260504 Jet add 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommonResult HKSBLLocateImportHoldAPAC(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();
            var source = parameters["SOURCE"].ToString();
            var dtContent = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_AVAILABLE_APEC WHERE HKAVA_DATE=@DATE  ", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() }, { "SOURCE", source } });

            var listInsNbr = DataAccess.DB["WARP"].QueryDataTableFromFile("SG340019_GetTickers.sql").AsEnumerable().Select(x => new { INSTR_INSNBR = x.Field<decimal>("INSTR_INSNBR"), INSALA_CODE = x.Field<string>("INSALA_CODE") }).ToList();

            // 出 HK_SBL_LOCATE_GRAVITON datatable
            DataTable dtHold = DataAccess.DB["WARP"].QueryDataTable($"select * from WAFT.dbo.HK_SBL_HOLD WHERE 1=2");
            dtHold.TableName = "WAFT.dbo.HK_SBL_HOLD";
            dtHold.PrimaryKey = new DataColumn[] { dtHold.Columns["HKOHD_ROWSEQ"] };

            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                if (dtContent.Rows[i]["HKAVA_APPROVED_QUANTITY"].ToDecimal() > 0)
                {
                    var dr = dtHold.NewRow();
                    dr["HKOHD_CREATOR"] = dr["HKOHD_UPDUSR"] = "SYSTEM";
                    dr["HKOHD_CREATME"] = dr["HKOHD_UPDTME"] = DateTime.Now;

                    dr["HKOHD_ROWSEQ"] = DataAccess.DB["WARP"].Scalar("SELECT NEXT VALUE FOR WAFT.dbo.SEQ_HK_SBL_HOLD_HKOHD_ROWSEQ");
                    dr["HKOHD_DATE"] = processDate.ToDecimal();
                    dr["HKOHD_TYPE"] = "Auto";
                    dr["HKOHD_COUNTERPARTY"] = source;
                    var insNbr = listInsNbr.FirstOrDefault(x => x.INSALA_CODE == dtContent.Rows[i]["HKAVA_SECURITY_ID"].ToString())?.INSTR_INSNBR;
                    if (insNbr == null || insNbr == 0)
                    {
                        LogMessage($"No matching INSTR_INSNBR found for ticker: {dtContent.Rows[i]["HKAVA_SECURITY_ID"].ToString()}", LogType.Warning);
                        continue;
                    }
                    dr["HKOHD_INSNBR"] = insNbr;
                    dr["HKOHD_QUANTITY"] = 0 - dtContent.Rows[i]["HKAVA_APPROVED_QUANTITY"].ToDecimal();
                    var sblRate = dtContent.Rows[i]["HKAVA_APPROVED_RATE"].ToDecimal();
                    dr["HKOHD_SBL_RATE"] = sblRate;
                    dtHold.Rows.Add(dr);
                }
            }

            // 全刪 不要刪OnHold 刪當天 Graviton就好
            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM WAFT.dbo.HK_SBL_HOLD WHERE HKOHD_DATE=@DATE AND HKOHD_COUNTERPARTY=@SOURCE AND HKOHD_TYPE = 'Auto'", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() }, { "SOURCE", source } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtHold);

            return new CommonResult(true, "", null);
        }
        /// <summary>
        /// 20260504 Jet add 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommonResult HKSBLLocateImportHoldSource(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();
            // var source="Quadeye";
            var source = parameters["SOURCE"].ToString();
            var dtContent = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_LOCATE WHERE HKLFG_DATE=@DATE AND HKLFG_SOURCE=@SOURCE ", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() }, { "SOURCE", source } });

            var listInsNbr = DataAccess.DB["WARP"].QueryDataTableFromFile("SG340019_GetTickers.sql").AsEnumerable().Select(x => new { INSTR_INSNBR = x.Field<decimal>("INSTR_INSNBR"), INSALA_CODE = x.Field<string>("INSALA_CODE") }).ToList();

            // 出 HK_SBL_LOCATE_GRAVITON datatable
            DataTable dtHold = DataAccess.DB["WARP"].QueryDataTable($"select * from WAFT.dbo.HK_SBL_HOLD WHERE 1=2");
            dtHold.TableName = "WAFT.dbo.HK_SBL_HOLD";
            dtHold.PrimaryKey = new DataColumn[] { dtHold.Columns["HKOHD_ROWSEQ"] };

            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtHold.NewRow();
                dr["HKOHD_CREATOR"] = dr["HKOHD_UPDUSR"] = "SYSTEM";
                dr["HKOHD_CREATME"] = dr["HKOHD_UPDTME"] = DateTime.Now;

                dr["HKOHD_ROWSEQ"] = DataAccess.DB["WARP"].Scalar("SELECT NEXT VALUE FOR WAFT.dbo.SEQ_HK_SBL_HOLD_HKOHD_ROWSEQ");
                dr["HKOHD_DATE"] = processDate.ToDecimal();
                dr["HKOHD_TYPE"] = "Auto";
                dr["HKOHD_COUNTERPARTY"] = source;
                var insNbr = listInsNbr.FirstOrDefault(x => x.INSALA_CODE == dtContent.Rows[i]["HKLFG_TICKER"].ToString())?.INSTR_INSNBR;
                if (insNbr == null || insNbr == 0)
                {
                    LogMessage($"No matching INSTR_INSNBR found for ticker: {dtContent.Rows[i]["HKLFG_TICKER"].ToString()}", LogType.Warning);
                    continue;
                }

                dr["HKOHD_INSNBR"] = insNbr;
                dr["HKOHD_QUANTITY"] = dtContent.Rows[i]["HKLFG_QUANTITY"].ToDecimal();

                var sblRate = dtContent.Rows[i]["HKLFG_SBL_RATE"].ToDecimal();
                dr["HKOHD_SBL_RATE"] = sblRate;


                dtHold.Rows.Add(dr);
            }

            // 全刪 不要刪OnHold 刪當天 Graviton就好
            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM WAFT.dbo.HK_SBL_HOLD WHERE HKOHD_DATE=@DATE AND HKOHD_COUNTERPARTY=@SOURCE AND HKOHD_TYPE = 'Auto'", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() }, { "SOURCE", source } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtHold);

            return new CommonResult(true, "", null);
        }
        /// <summary>
        /// 20260505 Jet add
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommonResult DailyImportSBLRateData(Dictionary<string, object> parameters)
        {
            //var dtContent = parameters["DATA"] as DataTable;
            var processDate = parameters["PROCESS_DATE"].ToString();
            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;
            // {DATE}.csv
            string file = "{DATE}.xlsx";
            var fileName = file.Replace("{DATE}", processDate);

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            // 出HK_SBL_DAILY_SBL_RATE datatable
            DataTable dtDailySBLRate = DataAccess.DB["WARP"].QueryDataTable($"select HKDSR_CREATOR,HKDSR_CREATME,HKDSR_UPDUSR,HKDSR_UPDTME,HKDSR_DATE,HKDSR_STOCK_CODE,HKDSR_STOCK_SHORT_NAME,HKDSR_CURRENCY,HKDSR_TICKER,HKDSR_SBL_RATE_1,HKDSR_SBL_RATE_2 from EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE WHERE 1=2");
            dtDailySBLRate.TableName = "EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE";
            dtDailySBLRate.PrimaryKey = new DataColumn[] { dtDailySBLRate.Columns["HKDSR_DATE"], dtDailySBLRate.Columns["HKDSR_STOCK_CODE"] };

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    var a = Path.GetFileNameWithoutExtension(fileName);

                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }
            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtDailySBLRate.NewRow();
                dr["HKDSR_CREATOR"] = dr["HKDSR_UPDUSR"] = "SYSTEM";
                dr["HKDSR_CREATME"] = dr["HKDSR_UPDTME"] = DateTime.Now;
                dr["HKDSR_DATE"] = processDate;
                dr["HKDSR_STOCK_CODE"] = dtContent.Rows[i][0].ToString();
                dr["HKDSR_STOCK_SHORT_NAME"] = dtContent.Rows[i][1].ToString();
                dr["HKDSR_CURRENCY"] = dtContent.Rows[i][2].ToString();
                dr["HKDSR_TICKER"] = dtContent.Rows[i][3].ToString();

                var rate1 = dtContent.Rows[i][4].ToString();
                var rate2 = dtContent.Rows[i][5].ToString();
                //var rate2 = "0";

                decimal decimalRate1 = 0;
                decimal decimalRate2 = 0;

                dr["HKDSR_SBL_RATE_1"] = (decimal.TryParse(rate1, out decimalRate1) ? decimalRate1 : 0) / 100;
                dr["HKDSR_SBL_RATE_2"] = (decimal.TryParse(rate2, out decimalRate2) ? decimalRate2 : 0) / 100;
                dtDailySBLRate.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE WHERE HKDSR_DATE = @DATE", new Dictionary<string, object>() { { "DATE", processDate } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtDailySBLRate);

            PublishInternalMessage("EXTSRC.dbo.HK_SBL_DAILY_SBL_RATE", new Dictionary<string, object>
            {
                { "IMPORT_DATE", processDate }
            });

            return new CommonResult(true, "", null);
        }
        /// <summary>
        /// 20260506 Jet add  DailyImportIndexData
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommonResult DailyImportIndexData(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();
            string month = processDate.Substring(4, 2);
            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;
            // {DATE}.csv
            string file = "HSI HSCEI HSCI Index {MM}.xlsx";
            var fileName = file.Replace("{MM}", month);

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            // 出HK_SBL_DAILY_SBL_RATE datatable
            DataTable dtDailySBLRate = DataAccess.DB["WARP"].QueryDataTable($"select FLGDAT_CREATOR, FLGDAT_CREATME , FLGDAT_UPDUSR , FLGDAT_UPDTME , FLGDAT_FLGNAM, FLGDAT_FLGVAR, FLGDAT_FLGDTA, FLGDAT_FLGDSC , FLGDAT_FLGNBR, FLGDAT_ORDERS  from EDAISYS.dbo.FLAGDATAS WHERE 1=2");
            dtDailySBLRate.TableName = "EDAISYS.dbo.FLAGDATAS";
            dtDailySBLRate.PrimaryKey = new DataColumn[] { dtDailySBLRate.Columns["FLGDAT_FLGNAM"], dtDailySBLRate.Columns["FLGDAT_FLGVAR"], dtDailySBLRate.Columns["FLGDAT_FLGDTA"] };
            DataTable dtContent = null;
            DataTable dtContent2 = null;
            DataTable dtContent3 = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    var a = Path.GetFileNameWithoutExtension(fileName);

                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");
                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent2 = excelAp.Sheets["Sheet1"].GetDataTable("B1");
                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent3 = excelAp.Sheets["Sheet1"].GetDataTable("C1");

                    //excelAp.Close();
                }
            }
            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtDailySBLRate.NewRow();
                dr["FLGDAT_CREATOR"] = "SYSTEM";
                dr["FLGDAT_CREATME"] = DateTime.Now;
                dr["FLGDAT_UPDUSR"] = "SYSTEM";
                dr["FLGDAT_UPDTME"] = DateTime.Now;
                dr["FLGDAT_FLGNAM"] = "HK_SBL_INDEX";
                dr["FLGDAT_FLGVAR"] = "HSI INDEX";
                dr["FLGDAT_FLGDTA"] = dtContent.Rows[i][0].ToString();
                dr["FLGDAT_FLGDSC"] = "";
                dr["FLGDAT_FLGNBR"] = 0;
                dr["FLGDAT_ORDERS"] = 0;
                dtDailySBLRate.Rows.Add(dr);
            }
            for (int i = 0; i < dtContent2.Rows.Count; i++)
            {
                var dr = dtDailySBLRate.NewRow();
                dr["FLGDAT_CREATOR"] = "SYSTEM";
                dr["FLGDAT_CREATME"] = DateTime.Now;
                dr["FLGDAT_UPDUSR"] = "SYSTEM";
                dr["FLGDAT_UPDTME"] = DateTime.Now;
                dr["FLGDAT_FLGNAM"] = "HK_SBL_INDEX";
                dr["FLGDAT_FLGVAR"] = "HSCEI INDEX";
                dr["FLGDAT_FLGDTA"] = dtContent2.Rows[i][0].ToString();
                dr["FLGDAT_FLGDSC"] = "";
                dr["FLGDAT_FLGNBR"] = 0;
                dr["FLGDAT_ORDERS"] = 0;
                dtDailySBLRate.Rows.Add(dr);
            }
            for (int i = 0; i < dtContent3.Rows.Count; i++)
            {
                var dr = dtDailySBLRate.NewRow();
                dr["FLGDAT_CREATOR"] = "SYSTEM";
                dr["FLGDAT_CREATME"] = DateTime.Now;
                dr["FLGDAT_UPDUSR"] = "SYSTEM";
                dr["FLGDAT_UPDTME"] = DateTime.Now;
                dr["FLGDAT_FLGNAM"] = "HK_SBL_INDEX";
                dr["FLGDAT_FLGVAR"] = "HSCI INDEX";
                dr["FLGDAT_FLGDTA"] = dtContent3.Rows[i][0].ToString();
                dr["FLGDAT_FLGDSC"] = "";
                dr["FLGDAT_FLGNBR"] = 0;
                dr["FLGDAT_ORDERS"] = 0;
                dtDailySBLRate.Rows.Add(dr);
            }
            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM  EDAISYS.dbo.FLAGDATAS WHERE FLGDAT_FLGNAM='HK_SBL_INDEX'", new Dictionary<string, object>() { { "DATE", processDate } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtDailySBLRate);

            return new CommonResult(true, "", null);
        }
        /// <summary>
        /// 2.7	LOCATE FILE AFTER ON HOLD UPDATE
        /// SG340019_HKSBLLocateFileAfterImportHold
        /// SG.SCH.HKSBLLocateFileAfterImportHold
        /// </summary>
        /// <returns></returns>
        public CommonResult HKSBLLocateImportHold(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();

            var dtContent = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_SBL_LOCATE_GRAVITON WHERE HKLFG_DATE=@DATE", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() } });

            var listInsNbr = DataAccess.DB["WARP"].QueryDataTableFromFile("SG340019_GetTickers.sql").AsEnumerable().Select(x => new { INSTR_INSNBR = x.Field<decimal>("INSTR_INSNBR"), INSALA_CODE = x.Field<string>("INSALA_CODE") }).ToList();

            // 出 HK_SBL_LOCATE_GRAVITON datatable
            DataTable dtHold = DataAccess.DB["WARP"].QueryDataTable($"select * from WAFT.dbo.HK_SBL_HOLD WHERE 1=2");
            dtHold.TableName = "WAFT.dbo.HK_SBL_HOLD";
            dtHold.PrimaryKey = new DataColumn[] { dtHold.Columns["HKOHD_ROWSEQ"] };

            // map xlsx欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtHold.NewRow();
                dr["HKOHD_CREATOR"] = dr["HKOHD_UPDUSR"] = "SYSTEM";
                dr["HKOHD_CREATME"] = dr["HKOHD_UPDTME"] = DateTime.Now;

                dr["HKOHD_ROWSEQ"] = DataAccess.DB["WARP"].Scalar("SELECT NEXT VALUE FOR WAFT.dbo.SEQ_HK_SBL_HOLD_HKOHD_ROWSEQ");
                dr["HKOHD_DATE"] = processDate.ToDecimal();
                dr["HKOHD_TYPE"] = "Auto";
                dr["HKOHD_COUNTERPARTY"] = "Graviton";
                var insNbr = listInsNbr.FirstOrDefault(x => x.INSALA_CODE == dtContent.Rows[i]["HKLFG_TICKER"].ToString())?.INSTR_INSNBR;
                if (insNbr == null || insNbr == 0)
                {
                    LogMessage($"No matching INSTR_INSNBR found for ticker: {dtContent.Rows[i]["HKLFG_TICKER"].ToString()}", LogType.Warning);
                    continue;
                }

                dr["HKOHD_INSNBR"] = insNbr;
                dr["HKOHD_QUANTITY"] = dtContent.Rows[i]["HKLFG_QUANTITY"].ToDecimal();

                var sblRate = dtContent.Rows[i]["HKLFG_SBL_RATE"].ToDecimal();
                dr["HKOHD_SBL_RATE"] = sblRate;


                dtHold.Rows.Add(dr);
            }

            // 全刪 不要刪OnHold 刪當天 Graviton就好
            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM WAFT.dbo.HK_SBL_HOLD WHERE HKOHD_DATE=@DATE AND HKOHD_COUNTERPARTY='Graviton' AND HKOHD_TYPE = 'Auto'", new Dictionary<string, object>() { { "DATE", processDate.ToDecimal() } });
            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtHold);

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// 2.1	BLOOMBERG DATA
        /// SG340012_HKBloombergBenefitEntitlement
        /// SG.SCH.HKBloombergBenefitEntitlement
        /// </summary>
        /// <returns></returns>
        public CommonResult HKBloombergBenefitEntitlement(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();

            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            // CCNAN05_COMMON_HKMK_{DATE}xxxxxx.TXT
            // 模糊檔名
            var fileName = Model.FlagData["HKSBL_BLOOMBERG_BENEFIT_ENTITLEMENT", "ImportFileName"].FlagData.Replace("{DATE}", processDate);
            // 取得實際檔名
            Directory.GetFiles(shareFolder).ToList().ForEach(file =>
            {
                var currentFileName = Path.GetFileName(file);

                if (currentFileName.StartsWith(Path.GetFileNameWithoutExtension(fileName)) && currentFileName.EndsWith(".TXT"))
                {
                    fileName = Path.GetFileName(currentFileName);
                }
            });

            //shareFolder + fileName
            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            var lsContent = File.ReadLines(fullFilePath).ToList();

            // 只留101001開頭的資料
            lsContent = lsContent.Where(x => x.StartsWith("101001")).ToList();

            // 出 HK_BENEFIT_ENTITLEMENT datatable
            DataTable dtBenefit = DataAccess.DB["WARP"].QueryDataTable($"select * from EXTSRC.dbo.HK_BENEFIT_ENTITLEMENT WHERE 1=2");
            dtBenefit.TableName = "EXTSRC.dbo.HK_BENEFIT_ENTITLEMENT";
            dtBenefit.PrimaryKey = new DataColumn[] { dtBenefit.Columns["HKBE_ANNOUNCEMENT_NUMBER"] };

            // map xlsx欄位to datatable欄位
            for (int i = 0; i < lsContent.Count(); i++)
            {
                var row = lsContent[i];

                var dr = dtBenefit.NewRow();
                dr["HKBE_CREATOR"] = dr["HKBE_UPDUSR"] = "SYSTEM";
                dr["HKBE_CREATME"] = dr["HKBE_UPDTME"] = DateTime.Now;

                dr["HKBE_RECORD_TYPE"] = row.Substring(0, 3);
                dr["HKBE_RECORD_SUBTYPE"] = row.Substring(3, 3);
                dr["HKBE_ANNOUNCEMENT_TYPE"] = row.Substring(6, 2);
                var key = row.Substring(8, 9);
                if (string.IsNullOrWhiteSpace(key) || dtBenefit.AsEnumerable().Where(r => r["HKBE_ANNOUNCEMENT_NUMBER"].ToString() == key).Any())
                {
                    LogMessage($"Duplicate or empty announcement number found: {key}", LogType.Warning);
                    //new Exception($"Duplicate announcement number found: {key}");
                    continue;
                }

                dr["HKBE_ANNOUNCEMENT_NUMBER"] = key;
                dr["HKBE_FILLER"] = row.Substring(17, 2);
                dr["HKBE_STOCK"] = row.Substring(19, 5).ToDecimal();
                dr["HKBE_ISIN"] = row.Substring(24, 12).Trim();
                dr["HKBE_ANNOUNCEMENT_SUMMARY"] = row.Substring(36, 80).Trim();
                dr["HKBE_PAYABLE_DATE"] = row.Substring(116, 8).ToDecimal();
                dr["HKBE_OPTION"] = row.Substring(124, 1).Trim();
                dr["HKBE_CASH_SCRIP_OPTION"] = row.Substring(125, 1).Trim();
                dr["HKBE_CURRENCY_OPTION"] = row.Substring(126, 1).Trim();
                dr["HKBE_BOOK_CLOSE_PERIOD_FROM"] = row.Substring(127, 8).ToDecimal();
                dr["HKBE_BOOK_CLOSE_PERIOD_TO"] = row.Substring(135, 8).ToDecimal();
                dr["HKBE_ELECTION_PERIOD_FROM"] = row.Substring(143, 8).ToDecimal();
                dr["HKBE_ELECTION_PERIOD_TO"] = row.Substring(151, 8).ToDecimal();
                dr["HKBE_ELECTION_PERIOD_END_TIME"] = row.Substring(159, 5);
                dr["HKBE_EX_DIVIDEND_DATE"] = row.Substring(164, 8).ToDecimal();
                dr["HKBE_SHAREHOLDING_DATE"] = row.Substring(172, 8).ToDecimal();
                dr["HKBE_ISSURE_RECORD_DATE"] = row.Substring(180, 8).ToDecimal();
                dr["HKBE_ISSURE_DEADLINE_FOR_ELECTION_DATE"] = row.Substring(188, 8).ToDecimal();
                dr["HKBE_ISSURE_DEADLINE_FOR_ELECTION_TIME"] = row.Substring(196, 5);
                dr["HKBE_FRACTION_DISTRIBUTED_IN_CASH"] = row.Substring(201, 1);
                dr["HKBE_LAST_REGISTRATION_DATE"] = row.Substring(202, 8).ToDecimal();
                dr["HKBE_SCRIP_FEE_COLLECTION"] = row.Substring(210, 1).Trim();
                dr["HKBE_YEAR_END"] = row.Substring(211, 8).ToDecimal();
                dr["HKBE_FIRST_ANOUNCEMENT_DATE"] = row.Substring(219, 8).ToDecimal();
                dr["HKBE_FURTHER_ANOUNCEMENT_DATE"] = row.Substring(227, 8).ToDecimal();
                dr["HKBE_ANOUNCEMENT_CREATION_DATE"] = row.Substring(235, 8).ToDecimal();
                dr["HKBE_LAST_UPDATED_DATE"] = row.Substring(243, 8).ToDecimal();
                dr["HKBE_RECORD_CHECKSUM"] = row.Substring(251, 20).Trim();
                dr["HKBE_FILLER1"] = row.Substring(271, 143).Trim();
                dr["HKBE_FILLER2"] = row.Substring(414, 3).Trim();

                dtBenefit.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].BulkInsertOrUpdate(dtBenefit, "EXTSRC.dbo.HK_BENEFIT_ENTITLEMENT");

            return new CommonResult(true, "", null);
        }

        /// <summary>
        /// 2.1	BLOOMBERG DATA
        /// SG340012_HKBloombergLotSize
        /// SG.SCH.HKBloombergLotSize
        /// </summary>
        /// <returns></returns>
        public CommonResult HKBloombergLotSize(Dictionary<string, object> parameters)
        {
            var processDate = parameters["PROCESS_DATE"].ToString();

            var shareFolder = Model.FlagData["KGIHK_SBL", "SFTP"].FlagData;

            var fileName = Model.FlagData["HKSBL_BLOOMBERG_LOT_SIZE", "ImportFileName"].FlagData.Replace("{DATE}", DateTime.Now.ToDecimal().ToString());

            string fullFilePath = shareFolder + fileName;
            if (!File.Exists(fullFilePath))
            {
                LogMessage($"File not found: {fullFilePath}", LogType.Warning);
                return new CommonResult(true, $"File not found: {fileName}", null);
            }

            DataTable dtContent = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fullFilePath)))
            {
                using (ExcelApplication excelAp = new ExcelApplication(ms))
                {
                    var a = Path.GetFileNameWithoutExtension(fileName);

                    if (excelAp.Sheets["Sheet1"] != null)
                        dtContent = excelAp.Sheets["Sheet1"].GetDataTable("A1");

                    //excelAp.Close();
                }
            }

            // 出Flagdatas datatable
            DataTable dtFlagdatas = DataAccess.DB["WARP"].QueryDataTable($"select * from EDAISYS.dbo.FLAGDATAS WHERE 1=2");
            dtFlagdatas.TableName = "EDAISYS.dbo.FLAGDATAS";

            // map csv欄位to datatable欄位
            for (int i = 0; i < dtContent.Rows.Count; i++)
            {
                var dr = dtFlagdatas.NewRow();
                dr["FLGDAT_CREATOR"] = dr["FLGDAT_UPDUSR"] = "SYSTEM";
                dr["FLGDAT_CREATME"] = dr["FLGDAT_UPDTME"] = DateTime.Now;

                dr["FLGDAT_FLGNAM"] = "SEHK_LOTSIZE";
                dr["FLGDAT_FLGVAR"] = dtContent.Rows[i][0].ToDecimal();
                dr["FLGDAT_FLGNBR"] = dtContent.Rows[i][1].ToDecimal();
                dr["FLGDAT_FLGDTA"] = dr["FLGDAT_FLGDSC"] = "";
                dr["FLGDAT_ORDERS"] = 0;

                dtFlagdatas.Rows.Add(dr);
            }

            DataAccess.DB["WARP"].ExecuteSql("DELETE FROM EDAISYS.dbo.FLAGDATAS WHERE FLGDAT_FLGNAM = 'SEHK_LOTSIZE'");
            DataAccess.DB["WARP"].BulkInsert(dtFlagdatas);

            return new CommonResult(true, "", null);
        }
    }
}
