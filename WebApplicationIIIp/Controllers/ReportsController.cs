using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Controllers
{
    public class ReportsController : ApiController
    {
        // GET: Reports
        // POST api/
        [System.Web.Http.HttpGet]
        public Response<string> CreateApplication(int id)
        {
            var res = new Response<string>();
            var result = "";
            var file = "";
            try
            {
                string rdlcFilePathPlus = @"Report\Report1.rdlc";
                LocalReport lr = new LocalReport();
                lr.ReportPath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, rdlcFilePathPlus);
                lr.Refresh();

                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = lr.Render("PDF", null, out mimeType,
                        out encoding, out extension, out streamids, out warnings);

                
                Guid ns = Guid.NewGuid();
                var baseDirPA = AppDomain.CurrentDomain.BaseDirectory + @"\Temp\Pdf\";
                var descPathPA = baseDirPA + DateTime.Now.ToString("yyyyMMdd") + "\\" + ns.ToString();
                if (!Directory.Exists(descPathPA))
                {
                    Directory.CreateDirectory(descPathPA);
                }

                string fileName = "TEST9" + ".pdf";

                descPathPA += "\\" + fileName;
                using (FileStream fs = File.Create(descPathPA))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }

                res.isSuccess = true;
                if (ConfigurationManager.AppSettings["TestLocal"] == "Y")
                {
                    var url = ConfigurationManager.AppSettings["URL"];
                    result = DateTime.Now.ToString("yyyyMMdd") + "//" + ns.ToString();
                    res.data = url + "/Temp/pdf/" + result + "/" + fileName;  //20240509/0d606bbd-5ee3-40b1-8602-e598aa644586

                }
                else
                {
                    Byte[] bytesGen = File.ReadAllBytes(descPathPA);
                    file = Convert.ToBase64String(bytesGen);
                    res.data = file;
                }

            }
            catch (Exception ex)
            {
                
                res.isSuccess = false;
                res.messageError = ex.Message;
            }
            return res;
        }
    }
}