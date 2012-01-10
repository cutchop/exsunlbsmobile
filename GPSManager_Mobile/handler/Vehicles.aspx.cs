using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace GPSManager_Mobile
{
    public partial class Vehicles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string phone = Request.QueryString["p"];
            if (!string.IsNullOrEmpty(phone))
            {
                IDataBase db = DBConfig.GetDBObjcet();
                string sql = string.Format("select posx,posy,logintime,regioninfo+''+(case when placeinfo is null then '' else placeinfo end) as placeinfo from curtraceinfo where PhoneNum='{0}'", phone);
                DataTable dt = db.ExecuteReturnDataSet(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string lat = dt.Rows[0]["posy"].ToString();
                    string lon = dt.Rows[0]["posx"].ToString();
                    if (string.IsNullOrEmpty(lat)) lat = "0";
                    if (string.IsNullOrEmpty(lon)) lon = "0";
                    string output = "[{\"phone\":\"" + phone + "\",\"lat\":" + lat + ",\"lon\":" + lon + ",\"time\":\"" + dt.Rows[0]["logintime"] + "\",\"place\":\"" + dt.Rows[0]["placeinfo"] + "\"}]";
                    Response.Write(output);
                }
            }
        }
    }
}