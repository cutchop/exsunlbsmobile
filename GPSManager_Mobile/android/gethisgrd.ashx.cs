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
    public class gethisgrd : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format(@"select logintime,posx,posy,city+''+(case when posinfo is null then '' else posinfo end) as placeinfo,details,status from HISGRDINFO 
                        where phonenum='{0}' and logintime>='{1} 00:00:00' and logintime<='{1} 23:59:59' order by logintime desc", context.Request["p"], context.Request["d"]);
            DataSet ds = db.ExecuteReturnDataSet(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"data\":[");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"t\":\"{0}\",\"l\":\"{1}\",\"lon\":\"{2}\",\"lat\":\"{3}\",\"d\":\"{4}\"", dt.Rows[i]["logintime"], dt.Rows[i]["placeinfo"], dt.Rows[i]["posx"], dt.Rows[i]["posy"], dt.Rows[i]["details"]);
                        sb.Append("}");
                        if (i != dt.Rows.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                }
                sb.Append("]}");
                context.Response.Write(sb.ToString());
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