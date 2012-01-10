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
    public class student_jx : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format("select stu_name,self_18 from student_info where self_18 is not null and stu_name like '%{0}%'", context.Request["k"]);
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
                        sb.AppendFormat("\"n\":\"{0}\",\"c\":\"{1}\"", dt.Rows[i]["stu_name"], dt.Rows[i]["self_18"]);
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