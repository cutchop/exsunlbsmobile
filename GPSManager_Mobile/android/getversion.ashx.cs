using System;
using System.Collections.Generic;
using System.Web;
using System.Data;

namespace GPSManager_Mobile.android
{
    /// <summary>
    /// 获取apk最新版本
    /// </summary>
    public class getversion : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(System.Configuration.ConfigurationManager.AppSettings["apkVersion"]);
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