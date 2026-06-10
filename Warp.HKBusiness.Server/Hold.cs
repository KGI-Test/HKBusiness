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
    public class Hold : Service
    {

        /// <summary>
        /// SG340026_SaveHold Save Hold
        /// </summary>
        public CommonResult SaveHold(Dictionary<string, object> parameters)
        {
            if (CheckParameterExists(parameters, "HKOHD_ROWSEQ") == false)
                throw new Exception("Parameters must have HKOHD_ROWSEQ");

            //insert
            if (parameters["HKOHD_ROWSEQ"].ToDecimal() == 0)
            {
                parameters["HKOHD_ROWSEQ"] = DataAccess.DB["WARP"].Scalar("SELECT NEXT VALUE FOR WAFT.dbo.SEQ_HK_SBL_HOLD_HKOHD_ROWSEQ");
                parameters["HKOHD_CREATOR"] = LoginUser.UserID;
                parameters["HKOHD_UPDUSR"] = LoginUser.UserID;

                using (DBTransaction transaction = DataAccess.DB["WARP"].BeginTransaction())
                {
                    transaction.ExecuteSqlForInsert("WAFT.dbo.HK_SBL_HOLD", "", parameters);

                    transaction.Commit();
                }

                PublishInternalMessage("WAFT.dbo.HK_SBL_HOLD", new Dictionary<string, object>
                {
                    ["HKOHD_ROWSEQ"] = parameters["HKOHD_ROWSEQ"]
                });

                return new CommonResult(true, "", parameters["HKOHD_ROWSEQ"].ToString());
            }
            //update
            else
            {
                parameters["HKOHD_UPDUSR"] = LoginUser.UserID;
                parameters["HKOHD_UPDTME"] = DateTime.Now;

                using (DBTransaction transaction = DataAccess.DB["WARP"].BeginTransaction())
                {
                    transaction.ExecuteSqlForUpdate("WAFT.dbo.HK_SBL_HOLD", "", parameters, new Dictionary<string, object>
                    {
                        ["HKOHD_ROWSEQ"] = parameters["HKOHD_ROWSEQ"].ToDecimal()
                    });

                    transaction.Commit();
                }

                PublishInternalMessage("WAFT.dbo.HK_SBL_HOLD", new Dictionary<string, object>
                {
                    ["HKOHD_ROWSEQ"] = parameters["HKOHD_ROWSEQ"]
                });

                return new CommonResult(true, "", parameters["HKOHD_ROWSEQ"].ToDecimal());
            }
        }


        /// <summary>
        /// SG340026_DeleteHold Delete Hold
        /// </summary>
        public CommonResult DeleteHold(Dictionary<string, object> parameters)
        {
            if (CheckParameterExists(parameters, "HKOHD_ROWSEQ") == false)
                throw new Exception("Parameters must have HKOHD_ROWSEQ");

            using (DBTransaction transaction = DataAccess.DB["WARP"].BeginTransaction())
            {
                transaction.ExecuteSql("DELETE FROM WAFT.dbo.HK_SBL_HOLD WHERE HKOHD_ROWSEQ = @HKOHD_ROWSEQ", parameters);

                transaction.Commit();
            }

            PublishInternalMessage("WAFT.dbo.HK_SBL_HOLD", new Dictionary<string, object>
            {
                ["HKOHD_ROWSEQ"] = parameters["HKOHD_ROWSEQ"]
            });

            return new CommonResult(true, "", parameters["HKOHD_ROWSEQ"].ToDecimal());
        }



    }
}
