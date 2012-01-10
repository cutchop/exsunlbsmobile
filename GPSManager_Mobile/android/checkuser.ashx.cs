using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// test 的摘要说明
    /// </summary>
    public class checkuser : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select * from userlogin where LoginName='{0}' and pwd='{1}'", context.Request["u"], context.Request["p"]);
            DataSet ds = db.ExecuteReturnDataSet(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    context.Response.Write("success");
                }
                else
                {
                    context.Response.Write("用户名或密码错误");
                }
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