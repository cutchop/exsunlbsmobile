using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// getdeviceinfo 的摘要说明
    /// </summary>
    public class getdeviceinfo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            IDataBase db = DBConfig.GetDBObjcet();
            string sql = string.Format(@"select a.devicetype,b.drivername,b.driverphone,c.logintime,c.status,c.speed,c.direction,c.posx,c.posy,round(c.licheng/1000,0) as licheng,
                                a.owners_name,a.owners_tel,d.cur_dev_ip,d.conn_server_ip,dbo.getfuel(d.device_id,d.ll) as fuel,
                                c.regioninfo+''+(case when c.placeinfo is null then '' else c.placeinfo end) as placeinfo 
                                from deviceinfo a left join driverinfo b on a.DriverID=b.ID 
                                left join curtraceinfo c on a.PhoneNum=c.PhoneNum
                                left join device_status d on a.PhoneNum=d.device_id where a.phonenum='{0}'", context.Request["p"]);
            DataSet ds = db.ExecuteReturnDataSet(sql);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string status = dt.Rows[0]["status"].ToString();
                    if (status == "行驶")
                    {
                        string dir = ChangeDirection(dt.Rows[0]["direction"].ToString());
                        if (dir != "")
                        {
                            status = string.Format("以{0}km/h的速度向{1}行驶", dt.Rows[0]["speed"], dir);
                        }
                        else
                        {
                            status = string.Format("以{0}km/h的速度行驶", dt.Rows[0]["speed"]);
                        }
                    }
                    string server_ip = dt.Rows[0]["conn_server_ip"].ToString();
                    if (!string.IsNullOrEmpty(server_ip))
                    {
                        server_ip = server_ip.Substring(server_ip.IndexOf("H:")).Replace("H:", "").Replace(";P", "");
                    }
                    string ret = "{" + string.Format("\"t\":\"{0}\",\"dn\":\"{1}\",\"dp\":\"{2}\",\"l\":\"{3}\",\"time\":\"{4}\",\"s\":\"{5}\",\"lon\":\"{6}\",\"lat\":\"{7}\",\"sip\":\"{8}\",\"dip\":\"{9}\",\"oname\":\"{10}\",\"otel\":\"{11}\",\"lc\":\"{12}\",\"fuel\":\"{13}\",\"status\":\"{14}\"",
                        dt.Rows[0]["devicetype"], dt.Rows[0]["drivername"], dt.Rows[0]["driverphone"], dt.Rows[0]["placeinfo"], dt.Rows[0]["logintime"], status, dt.Rows[0]["posx"], dt.Rows[0]["posy"], server_ip, dt.Rows[0]["cur_dev_ip"], dt.Rows[0]["owners_name"], dt.Rows[0]["owners_tel"], dt.Rows[0]["licheng"], dt.Rows[0]["fuel"], status)
                        + "}";
                    context.Response.Write(ret);
                }
                else
                {
                    context.Response.Write("没有数据");
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

        private string ChangeDirection(string str)
        {
            try
            {
                float angle = float.Parse(str.Substring(2, str.Length - 3));
                if (angle <= 20)
                    return "北";
                else if (angle > 20 && angle <= 65)
                    return "东北";
                else if (angle > 65 && angle <= 110)
                    return "东";
                else if (angle > 110 && angle <= 155)
                    return "东南";
                else if (angle > 155 && angle <= 200)
                    return "南";
                else if (angle > 200 && angle <= 245)
                    return "西南";
                else if (angle > 245 && angle <= 290)
                    return "西";
                else if (angle > 290 && angle <= 335)
                    return "西北";
                else if (angle > 335)
                    return "北";
                else
                    return "";
            }
            catch { }
            return "";
        }
    }
}