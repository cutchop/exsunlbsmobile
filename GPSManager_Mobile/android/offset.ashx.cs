using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// offset 的摘要说明
    /// </summary>
    public class offset : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select dbo.get_offsetx({0},{1})", context.Request["lon"], context.Request["lat"]);
            DataSet ds = db.ExecuteReturnDataSet(sql);
            if (ds != null)
            {
                context.Response.Write(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                context.Response.Write("服务器异常");
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