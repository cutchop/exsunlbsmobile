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
    public class command_jx : IHttpHandler
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
                            case "1"://设置驾校信息
                                sql = string.Format("select * from userlogin where loginname='{0}' and passdyd='{1}'", name, context.Request["pw"]);
                                dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("insert into device_download(device_id,cmd_type,cmd_remark,retry_times,download_content,out_datetime) values('{0}','set_jx_id','设置驾校ID',1,'11111111',60)", phone);
                                        db.ExecuteNonQuery(sql);
                                        sql = string.Format("insert into device_download(device_id,cmd_type,cmd_remark,retry_times,download_content,out_datetime) values('{0}','set_jx_name','设置驾校',1,'兰剑驾校',60)", phone);
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
                            case "2"://设置计费模式
                                sql = string.Format("select * from userlogin where loginname='{0}' and passdyd='{1}'", name, context.Request["pw"]);
                                dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("insert into device_download(device_id,cmd_type,cmd_remark,retry_times,download_content,out_datetime) values('{0}','set_jx_mode','设置驾校模式',1,'0',30)", phone);
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
                            case "3"://设置非计费模式
                                sql = string.Format("select * from userlogin where loginname='{0}' and passdyd='{1}'", name, context.Request["pw"]);
                                dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                if (dt.Rows.Count > 0)
                                {
                                    sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("insert into device_download(device_id,cmd_type,cmd_remark,retry_times,download_content,out_datetime) values('{0}','set_jx_mode','设置驾校模式',1,'1',30)", phone);
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
                            case "4"://重新发卡
                                if (!string.IsNullOrEmpty(context.Request["card"]))
                                {
                                    sql = string.Format("select * from userlogin where loginname='{0}' and passdyd='{1}'", name, context.Request["pw"]);
                                    dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                    if (dt.Rows.Count > 0)
                                    {
                                        sql = string.Format("select * from deviceinfo where phonenum='{0}'", phone);
                                        dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                        if (dt.Rows.Count > 0)
                                        {
                                            //根据卡号查学员姓名和剩余时长
                                            sql = string.Format("select a.stu_name,a.self_18,b.stu_shichang from (select stu_name,self_18 from student_info where self_18='{0}') a left join student_card b on a.self_18=b.stu_id", context.Request["card"].Trim());
                                            dt = db.ExecuteReturnDataSet(sql).Tables[0];
                                            if (dt.Rows.Count > 0)
                                            {
                                                //发送指令
                                                string stucard = ConvertToASCII(dt.Rows[0]["self_18"].ToString().Trim());
                                                string stuname = ConvertToGB2312(dt.Rows[0]["stu_name"].ToString().Trim());
                                                while (stuname.Length < 16)
                                                {
                                                    stuname += "20";
                                                }
                                                string stusysc = Convert.ToInt32(dt.Rows[0]["stu_shichang"].ToString()).ToString("X4");
                                                sql = string.Format("insert into device_download(device_id,cmd_type,cmd_remark,retry_times,download_content,out_datetime) values('{0}','set_ic_id','设置ic卡信息',1,'01{1}{2}313131313131313102{3}00000000',60)", phone, stucard, stuname, stusysc);
                                                db.ExecuteNonQuery(sql);
                                                context.Response.Write("ok");
                                            }
                                            else
                                            {
                                                context.Response.Write("学员卡号错误");
                                            }
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
                                }
                                else
                                {
                                    context.Response.Write("缺少参数[card]");
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
        //字母数字转ASCII
        private string ConvertToASCII(string str)
        {
            string ret = "";
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            for (int i = 0; i < str.Length; i++)
            {
                byte[] array = asciiEncoding.GetBytes(str[i].ToString());
                ret += array[0].ToString("X2");
            }
            return ret;
        }
        //汉字转GB2312
        private string ConvertToGB2312(string str)
        {
            string ret = "";
            for (int i = 0; i < str.Length; i++)
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(str[i].ToString());
                for (int j = 0; j < array.Length; j++)
                {
                    ret += array[j].ToString("X2");
                }
            }
            return ret;
        }
    }
}