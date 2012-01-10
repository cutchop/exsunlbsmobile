using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// gethisgrd 的摘要说明
    /// </summary>
    public class InspectionUpload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!string.IsNullOrEmpty(context.Request["p"]))
            {
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = string.Format("select phone from imei_phone where imei='{0}'",context.Request["p"]);
                DataTable dt = db.ExecuteReturnDataSet(sql).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    string phone = dt.Rows[0][0].ToString();
                    if (!string.IsNullOrEmpty(phone))
                    {
                        sql = string.Format("update device_status set lon={0},lat={1},data_time=getdate() where device_id='{2}'", context.Request["lon"], context.Request["lat"], phone);
                        try
                        {
                            db.ExecuteNonQuery(sql);
                            context.Response.Write("ok");
                        }
                        catch (Exception ex)
                        {
                            context.Response.Write(ex.Message);
                        }
                    }
                    else
                    {
                        context.Response.Write("平台上没有该号码");
                    }
                }
                else
                {
                    context.Response.Write("平台上没有该号码[" + context.Request["p"] + "]");
                }
            }
            else
            {
                context.Response.Write("缺少参数p");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}