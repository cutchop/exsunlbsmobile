using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace GPSManager_Mobile
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] != null)
            {
                if (!IsPostBack)
                {
                    IDataBase db = DBConfig.GetDBObjcet();
                    string sql = "select a.managergroupid,b.cmpgroupname,a.lat,a.lon from userlogin a left join usergroup b on a.ManagerGroupID=b.CmpGroupID where a.LoginName='" + Session["username"] + "'";
                    DataTable dt = db.ExecuteReturnDataSet(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        int l1 = 0, l2 = 0, l3 = 0;
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<script type=\"text/javascript\">var _cname='");
                        sb.Append(dt.Rows[0]["cmpgroupname"].ToString());
                        string _centerlat = dt.Rows[0]["lon"].ToString();
                        string _centerlon = dt.Rows[0]["lat"].ToString();
                        if (string.IsNullOrEmpty(_centerlat)) _centerlat = "114.436263";
                        if (string.IsNullOrEmpty(_centerlon)) _centerlon = "30.497476";
                        sb.AppendFormat("';var _centerlat={0},_centerlon={1};var structure = [", _centerlat, _centerlon);
                        string groupid = dt.Rows[0]["managergroupid"].ToString();
                        sql = string.Format("select cmpgroupid,cmpgroupname from usergroup where RootTreeID='{0}' order by sortingid", groupid);
                        dt = db.ExecuteReturnDataSet(sql).Tables[0];
                        string groupid2 = "";
                        DataTable dt2 = null;
                        int i = 0;
                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            sb.Append("{");
                            sb.AppendFormat("text:'{0}',items:[", dt.Rows[i]["cmpgroupname"]);
                            groupid2 = dt.Rows[i]["cmpgroupid"].ToString();
                            sql = string.Format("select cmpgroupid,cmpgroupname from usergroup where RootTreeID='{0}' order by sortingid", groupid2);
                            dt2 = db.ExecuteReturnDataSet(sql).Tables[0];
                            DataTable dt3 = null;
                            int j = 0;
                            for (j = 0; j < dt2.Rows.Count; j++)
                            {
                                sb.Append("{");
                                sb.AppendFormat("text:'{0}',items:[", dt2.Rows[j]["cmpgroupname"]);
                                sql = string.Format("select phonenum,vehicleno,devicetype from DeviceInfo where BelongGroupID='{0}' order by vehicleno", dt2.Rows[j]["cmpgroupid"]);
                                dt3 = db.ExecuteReturnDataSet(sql).Tables[0];
                                for (int k = 0; k < dt3.Rows.Count; k++)
                                {
                                    sb.Append("{");
                                    sb.AppendFormat("text:'{0}',phone:'{1}'", dt3.Rows[k]["vehicleno"], dt3.Rows[k]["phonenum"]);
                                    sb.Append(",leaf:true},");
                                    l1++;
                                    l2++;
                                    l3++;
                                }
                                if (sb.ToString().EndsWith(","))
                                {
                                    sb.Remove(sb.Length - 1, 1);
                                }
                                sb.Append("],count:" + l3 + "},");
                                l3 = 0;
                            }
                            sql = string.Format("select phonenum,vehicleno,devicetype from DeviceInfo where BelongGroupID='{0}' order by vehicleno", groupid2);
                            dt2 = db.ExecuteReturnDataSet(sql).Tables[0];
                            for (j = 0; j < dt2.Rows.Count; j++)
                            {
                                sb.Append("{");
                                sb.AppendFormat("text:'{0}',phone:'{1}'", dt2.Rows[j]["vehicleno"], dt2.Rows[j]["phonenum"]);
                                sb.Append(",leaf:true},");
                                l1++;
                                l2++;
                            }
                            if (sb.ToString().EndsWith(","))
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                            sb.Append("],count:" + l2 + "},");
                            l2 = 0;
                        }
                        sql = string.Format("select phonenum,vehicleno,devicetype from DeviceInfo where BelongGroupID='{0}' order by vehicleno", groupid);
                        dt = db.ExecuteReturnDataSet(sql).Tables[0];
                        for (i = 0; i < dt.Rows.Count; i++)
                        {
                            sb.Append("{");
                            sb.AppendFormat("text:'{0}',phone:'{1}'", dt.Rows[i]["vehicleno"].ToString(), dt.Rows[i]["phonenum"].ToString());
                            sb.Append(",leaf:true},");
                            l1++;
                        }
                        sb.Append("{text:'注销',phone:'logout',leaf:true}");
                        sb.Append("];var _total=" + l1 + ";</script>");
                        Response.Write(sb.ToString());
                    }
                }
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }
    }
}