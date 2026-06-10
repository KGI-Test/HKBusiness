using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using EFramework.EFrame;
using EFramework.EFrame.DB;
using EFramework.EFrame.Logging;
using EFramework.EFrame.Common;
using EFramework.Server.Servicies.ServiceSpace;
using EFramework.DBModel;
using System.Diagnostics;

namespace Warp.HKBusiness.Server
{
    public class SBL : Service
    {

        /// <summary>
        /// SG340028_SaveSBL Save SBL
        /// </summary>
        public CommonResult SaveSBL(Dictionary<string, object> parameters)
        {
            if (CheckParameterExists(parameters, "HKSBL_TRDNBR") == false)
                throw new Exception("Parameters must have HKSBL_TRDNBR");

            //insert
            if (parameters["HKSBL_TRDNBR"].ToDecimal() == 0)
            {
                parameters["HKSBL_TRDNBR"] = DataAccess.DB["WARP"].Scalar("SELECT NEXT VALUE FOR WAFT.dbo.SEQ_HK_SBL_SBL_HKSBL_ROWSEQ");
                parameters["HKSBL_ROWSEQ"] = parameters["HKSBL_TRDNBR"];
                parameters["HKSBL_CREATOR"] = LoginUser.UserID;
                parameters["HKSBL_UPDUSR"] = LoginUser.UserID;

                using (DBTransaction transaction = DataAccess.DB["WARP"].BeginTransaction())
                {
                    transaction.ExecuteSqlForInsert("WAFT.dbo.HK_SBL_SBL", "", parameters);

                    transaction.Commit();
                }

                PublishInternalMessage("WAFT.dbo.HK_SBL_SBL", new Dictionary<string, object>
                {
                    ["HKSBL_TRDNBR"] = parameters["HKSBL_TRDNBR"]
                });
            }
            //update
            else
            {
                parameters["HKSBL_UPDUSR"] = LoginUser.UserID;
                parameters["HKSBL_UPDTME"] = DateTime.Now;

                using (DBTransaction transaction = DataAccess.DB["WARP"].BeginTransaction())
                {
                    transaction.ExecuteSqlForUpdate("WAFT.dbo.HK_SBL_SBL", "", parameters, new Dictionary<string, object>
                    {
                        ["HKSBL_TRDNBR"] = parameters["HKSBL_TRDNBR"].ToDecimal()
                    });

                    transaction.Commit();
                }

                PublishInternalMessage("WAFT.dbo.HK_SBL_SBL", new Dictionary<string, object>
                {
                    ["HKSBL_TRDNBR"] = parameters["HKSBL_TRDNBR"]
                });
            }

            string countrypartyMail = DataAccess.DB["WARP"].Scalar(@"SELECT ISNULL(FLGDAT_FLGDSC, '') FROM EDAISYS.dbo.FLAGDATAS WHERE FLGDAT_FLGNAM = 'HK_SBL_COUNTERPARTY' AND FLGDAT_FLGVAR = @HKSBL_COUNTERPARTY", parameters).ToString();

            if (countrypartyMail != string.Empty)
            {
                DataTable dtcountrypartyMail = new DataTable();
                dtcountrypartyMail.Columns.Add("Trade Number", typeof(decimal));
                dtcountrypartyMail.Columns.Add("Direction", typeof(string));
                dtcountrypartyMail.Columns.Add("Trade Date", typeof(decimal));
                dtcountrypartyMail.Columns.Add("Value Date", typeof(decimal));
                dtcountrypartyMail.Columns.Add("Counterparty", typeof(string));
                dtcountrypartyMail.Columns.Add("Ticker", typeof(string));
                dtcountrypartyMail.Columns.Add("Price", typeof(decimal));
                dtcountrypartyMail.Columns.Add("Dividend", typeof(decimal));
                dtcountrypartyMail.Columns.Add("Quantity", typeof(decimal));
                dtcountrypartyMail.Columns.Add("Rate", typeof(decimal));

                DataRow drcountrypartyMail = dtcountrypartyMail.NewRow();
                drcountrypartyMail["Trade Number"] = parameters["HKSBL_TRDNBR"];
                drcountrypartyMail["Direction"] = parameters["HKSBL_DIRECTION"];
                drcountrypartyMail["Trade Date"] = parameters["HKSBL_TRADE_DATE"];
                drcountrypartyMail["Value Date"] = parameters["HKSBL_VALUE_DATE"];
                drcountrypartyMail["Counterparty"] = parameters["HKSBL_COUNTERPARTY"];
                drcountrypartyMail["Ticker"] = DataAccess.DB["WARP"].Scalar(@"SELECT INSTR_NAME FROM WAFT.dbo.INS_INSTRUMENT WHERE INSTR_VER = 1 AND INSTR_INSNBR = @HKSBL_INSNBR", parameters).ToString();
                drcountrypartyMail["Price"] = parameters["HKSBL_PRICE"];
                drcountrypartyMail["Dividend"] = parameters["HKSBL_DIVIDEND"];
                drcountrypartyMail["Quantity"] = parameters["HKSBL_QUANTITY"];
                drcountrypartyMail["Rate"] = parameters["HKSBL_SBL_RATE"];

                dtcountrypartyMail.Rows.Add(drcountrypartyMail);

                DataAccess.MSG.SendEMail($"{CommonUtility.Environment.SystemName}_SBL Confirmation_{CommonUtility.Environment.Environment}({CommonUtility.Environment.IP})",
$@"<html>
<p>{CommonUtility.DataTableToHtml(dtcountrypartyMail)}</p>
</html>",
                        countrypartyMail, System.Net.Mail.MailPriority.Normal);
            }

            return new CommonResult(true, "", parameters["HKSBL_TRDNBR"].ToDecimal());
        }


        /// <summary>
        /// SG340028_DeleteSBL Delete SBL
        /// </summary>
        public CommonResult DeleteSBL(Dictionary<string, object> parameters)
        {
            if (CheckParameterExists(parameters, "HKSBL_TRDNBR") == false)
                throw new Exception("Parameters must have HKSBL_TRDNBR");

            using (DBTransaction transaction = DataAccess.DB["WARP"].BeginTransaction())
            {
                transaction.ExecuteSql("DELETE FROM WAFT.dbo.HK_SBL_SBL WHERE HKSBL_TRDNBR = @HKSBL_TRDNBR", parameters);

                transaction.Commit();
            }

            PublishInternalMessage("WAFT.dbo.HK_SBL_SBL", new Dictionary<string, object>
            {
                ["HKSBL_TRDNBR"] = parameters["HKSBL_TRDNBR"]
            });

            return new CommonResult(true, "", parameters["HKSBL_TRDNBR"].ToDecimal());
        }



    }
}
