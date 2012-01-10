using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Data;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// getdevicelist 的摘要说明
    /// </summary>
    public class getdevicelist : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format(@"select a.cmpgroupid,a.cmpgroupname,b.phonenum,b.vehicleno,b.devicetype,c.posx,c.posy,c.status,
            c.regioninfo+''+(case when c.placeinfo is null then '' else c.placeinfo end) as placeinfo,c.logintime,getdate() as now from 
            (select cmpgroupid,cmpgroupname,sortingid from usergroup where cmpgrouproot like 
            (select b.cmpgrouproot+cast(a.managergroupid as varchar)+'%' from userlogin a left join usergroup b on a.managergroupid=b.cmpgroupid where a.loginname='{0}')
            or cmpgroupid=(select managergroupid from userlogin where loginname='{0}')) a inner join deviceinfo b on a.cmpgroupid=b.belonggroupid
            left join curtraceinfo c on b.phonenum=c.phonenum order by a.cmpgroupid,a.sortingid", context.Request["u"]);
            DataSet ds = db.ExecuteReturnDataSet(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"data\":[");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, int> group = new Dictionary<string, int>();
                    string gid = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        gid = dt.Rows[i]["cmpgroupid"].ToString();
                        if (group.ContainsKey(gid))
                        {
                            group[gid]++;
                        }
                        else
                        {
                            group.Add(gid, 1);
                        }
                    }
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i == 0 || dt.Rows[i]["cmpgroupid"].ToString() != dt.Rows[i - 1]["cmpgroupid"].ToString())
                        {
                            sb.Append("{");
                            sb.AppendFormat("\"g\":\"{0}({1})\"", dt.Rows[i]["cmpgroupname"], group[dt.Rows[i]["cmpgroupid"].ToString()]);
                            sb.Append("},");
                        }
                        sb.Append("{");
                        sb.AppendFormat("\"v\":\"{0}\",\"ph\":\"{1}\",\"pl\":\"{2}\",\"t\":\"{3}\",\"lon\":\"{4}\",\"lat\":\"{5}\""
                            , dt.Rows[i]["vehicleno"], dt.Rows[i]["phonenum"], dt.Rows[i]["placeinfo"] + "状态:" + dt.Rows[i]["status"], DateStringFromNow(dt.Rows[i]["logintime"], dt.Rows[i]["now"]), dt.Rows[i]["posx"], dt.Rows[i]["posy"]);
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

        private string DateStringFromNow(object datetime,object now)
        {
            try
            {
                DateTime dt = (DateTime)datetime;
                TimeSpan span = (DateTime)now - dt;
                if (span.TotalDays > 90)
                {
                    return dt.ToShortDateString();
                }
                else if (span.TotalDays > 1)
                {
                    return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                }
                else if (span.TotalHours > 1)
                {
                    return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                }
                else if (span.TotalMinutes > 1)
                {
                    return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                }
                else if (span.TotalSeconds >= 1)
                {
                    return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                }
                else
                {
                    return "1秒前";
                }
            }
            catch
            {
                return "";
            }
        }
    }
}