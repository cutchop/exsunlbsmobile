using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace GPSManager_Mobile.handler
{
    public partial class users : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string type = Request.QueryString["type"];
            switch (type)
            {
                case "login":
                    try
                    {
                        string username = Request.Params["username"];
                        string password = Request.Params["password"];
                        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                        {
                            IDataBase db = DBConfig.GetDBObjcet();
                            string sql = string.Format("select * from userlogin where loginname='{0}' and pwd='{1}'", username, password);
                            DataTable dt = db.ExecuteReturnDataSet(sql).Tables[0];
                            if (dt.Rows.Count > 0)
                            {
                                Session["username"] = dt.Rows[0]["loginname"];
                                Session["nikename"] = dt.Rows[0]["nikename"];
                                Session["groupid"] = dt.Rows[0]["managergroupid"];
                                Response.Write("[{\"success\":true}]");
                            }
                            else
                            {
                                Response.Write("[{\"success\":false,\"msg\":\"帐号或密码错误\"}]");
                            }
                        }
                    }
                    catch { }
                    break;
                default: break;
            }
        }
    }
}