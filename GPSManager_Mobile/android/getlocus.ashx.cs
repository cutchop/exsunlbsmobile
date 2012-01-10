using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// getloucs 的摘要说明
    /// </summary>
    public class getlocus : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format(@"select logintime,logintime as logintime2,posx,posy,regioninfo+''+(case when placeinfo is null then '' else placeinfo end) as placeinfo 
                        from HISTRACEINFO where PhoneNum='{0}' and logintime>='{1} 00:00:00' 
                        and logintime<='{1} 23:59:59' order by LoginTime desc", context.Request["p"], context.Request["d"]);
            DataSet ds = db.ExecuteReturnDataSet(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"data\":[");
                if (dt.Rows.Count > 0)
                {
                    for (int i = 1; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["placeinfo"].Equals(dt.Rows[i - 1]["placeinfo"]))
                        {
                            dt.Rows[i - 1]["logintime2"] = dt.Rows[i]["logintime"];
                            dt.Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("{");
                        if (dt.Rows[i]["logintime"].Equals(dt.Rows[i]["logintime2"]))
                        {
                            sb.AppendFormat("\"t\":\"{0}\",\"l\":\"{1}\",\"lon\":\"{2}\",\"lat\":\"{3}\"", dt.Rows[i]["logintime"], dt.Rows[i]["placeinfo"], dt.Rows[i]["posx"], dt.Rows[i]["posy"]);
                        }
                        else
                        {
                            sb.AppendFormat("\"t\":\"{0}\",\"l\":\"{1}\",\"lon\":\"{2}\",\"lat\":\"{3}\"", dt.Rows[i]["logintime2"] + "至" + ((DateTime)dt.Rows[i]["logintime"]).ToString("HH:mm:ss"), dt.Rows[i]["placeinfo"], dt.Rows[i]["posx"], dt.Rows[i]["posy"]);
                        }
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