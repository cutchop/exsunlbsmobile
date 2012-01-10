using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// command 的摘要说明
    /// </summary>
    public class command : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string op = context.Request["op"];
            if (!string.IsNullOrEmpty(op))
            {
                string phone = context.Request["phone"];
                if (!string.IsNullOrEmpty(phone))
                {
                    string name = context.Request["u"];
                    if (!string.IsNullOrEmpty(name))
                    {
                        IDataBase db = DBConfig.GetDBObjcet();
                        string sql = "";
                        DataTable dt = null;
                        switch (op)
                        {
                            case "dy"://断油
                                sql = string.Format("select * from userlogin where loginname='{0}' and passdyd='{1}'", name, context.Request["pw"]);
                                dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("update device_set set you_set=5 where device_id='{0}'", phone);
                                        db.ExecuteNonQuery(sql);
                                        context.Response.Write("ok");
                                    }
                                    else
                                    {
                                        context.Response.Write("设备号码错误");
                                    }
                                }
                                else
                                {
                                    context.Response.Write("密码错误");
                                }
                                break;
                            case "dd"://断电
                                sql = string.Format("select * from userlogin where loginname='{0}' and passdyd='{1}'", name, context.Request["pw"]);
                                dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("update device_set set dian_set=5 where device_id='{0}'", phone);
                                        db.ExecuteNonQuery(sql);
                                        context.Response.Write("ok");
                                    }
                                    else
                                    {
                                        context.Response.Write("设备号码错误");
                                    }
                                }
                                else
                                {
                                    context.Response.Write("密码错误");
                                }
                                break;
                            case "hf"://恢复油电
                                sql = string.Format("select * from userlogin where loginname='{0}' and passdyd='{1}'", name, context.Request["pw"]);
                                dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("update device_set set you_set=6,dian_set=6 where device_id='{0}'", phone);
                                        db.ExecuteNonQuery(sql);
                                        context.Response.Write("ok");
                                    }
                                    else
                                    {
                                        context.Response.Write("设备号码错误");
                                    }
                                }
                                else
                                {
                                    context.Response.Write("密码错误");
                                }
                                break;
                            case "pz"://拍照
                                sql = string.Format("select iscamera from deviceinfo where phonenum='{0}'", phone);
                                dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    if (dt.Rows[0][0].ToString() == "1")
                                    {
                                        sql = string.Format("update device_set set getphoto=1,photoset=5,photonumber=1 where device_id='{0}'", phone);
                                        db.ExecuteNonQuery(sql);
                                        context.Response.Write("ok");
                                    }
                                    else
                                    {
                                        context.Response.Write("设备没有摄像头");
                                    }
                                }
                                else
                                {
                                    context.Response.Write("设备号码错误");
                                }
                                break;
                            case "jt"://监听
                                if (!string.IsNullOrEmpty(context.Request["call"]))
                                {
                                    sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("update device_set set listen_flag=1,listen_phonenum='{0}' where device_id='{1}'", context.Request["call"], phone);
                                        db.ExecuteNonQuery(sql);
                                        context.Response.Write("ok");
                                    }
                                    else
                                    {
                                        context.Response.Write("设备号码错误");
                                    }
                                }
                                else
                                {
                                    context.Response.Write("缺少参数[call]");
                                }
                                break;
                            default:
                                context.Response.Write("请指定要进行的操作");
                                break;
                        }
                    }
                    else
                    {
                        context.Response.Write("缺少参数[u]");
                    }
                }
                else
                {
                    context.Response.Write("缺少参数[phone]");
                }
            }
            else
            {
                context.Response.Write("缺少参数[op]");
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