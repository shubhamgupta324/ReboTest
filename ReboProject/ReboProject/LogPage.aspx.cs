using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ReboProject
{
    public partial class LogPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

       

        protected void btnerror_Click(object sender, EventArgs e)
        {
            string filename = DateTime.Now.ToString("ddMMyyyy") + ".txt";
            string fullpath = Server.MapPath("~/ErrorLog/" + filename);
            if (File.Exists(fullpath))
            {
                using (StreamReader rd = new StreamReader(fullpath))
                {
                    TextBox1.Text = rd.ReadToEnd();
                }
            }
            else
            {
                TextBox1.Text = "No log file";
            }
        }

        protected void Btnprojecterror_Click(object sender, EventArgs e)
        {
            string filename = DateTime.Now.ToString("ddMMyyyy") + "_ProjectError.txt";
            string fullpath = Server.MapPath("~/ErrorProjectLog/" + filename);
            if (File.Exists(fullpath))
            {
                using (StreamReader rd = new StreamReader(fullpath))
                {
                    TextBox1.Text = rd.ReadToEnd();
                }
            }
            else
            {
                TextBox1.Text = "No log file";
            }
        }

        protected void btnpassproject_Click(object sender, EventArgs e)
        {
            string filename = DateTime.Now.ToString("ddMMyyyy") + "_ProjectSuccess.txt";
            string fullpath = Server.MapPath("~/SuccessProjectLog/" + filename);
            if (File.Exists(fullpath))
            {
                using (StreamReader rd = new StreamReader(fullpath))
                {
                    TextBox1.Text = rd.ReadToEnd();
                }
            }
            else
            {
                TextBox1.Text = "No log file";
            }
        }
    }
}