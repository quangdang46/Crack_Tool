using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace demoactivexnet
{
    public partial class Form1:Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string RunCMD(string cmd)
        {
            Process cmdProcess;
            cmdProcess = new Process();
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.Arguments = "/c " + cmd;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.CreateNoWindow = true;
            cmdProcess.Start();
            string output = cmdProcess.StandardOutput.ReadToEnd();
            cmdProcess.WaitForExit();
            if (String.IsNullOrEmpty(output))
                return "";
            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string output = RunCMD("wmic diskdrive get serialNumber"); // check số serial ổ cứng
            using (StreamWriter HDD = new StreamWriter("HDD.txt", true))
            {
                HDD.WriteLine(output);
                HDD.Close();
            }
            string[] lines = File.ReadAllLines("HDD.txt");
            File.Delete("HDD.txt");
            string str = Regex.Replace(lines[2], @"\s", ""); // lấy serial đầu tiên

            string outputs = RunCMD("wmic bios get serialnumber"); // check số serial bios
            using (StreamWriter BIOS = new StreamWriter("bios.txt", true))
            {
                BIOS.WriteLine(outputs);
                BIOS.Close();
            }
            string[] liness = File.ReadAllLines("bios.txt");
            File.Delete("bios.txt");
            string strs = Regex.Replace(liness[2], @"\s", ""); // lấy serial đầu tiên

            string keys = string.Concat(strs, str);

            HttpClient httpClient = new HttpClient();
            string text2 = keys;
            string requestUri2 = "https://docs.google.com/spreadsheets/d/1h1Fc2_VPofeNakNWC4vCFV5zQOfQNRt-mFF7Of4xGhQ/edit?usp=sharing";
            string text3 = httpClient.GetAsync(requestUri2).Result.Content.ReadAsStringAsync().Result.ToString();
            Match match2 = Regex.Match(text3.ToString(), text2 + ".*?(?=ok)");
            bool flag2 = match2 != Match.Empty;
            if (flag2)
            {
                string[] array = match2.ToString().Split(new char[]
                {
                            '|'
                });
                //string siteurlold = "https://mmosorfware.com/time.php";
                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //string htmlold = new System.Net.WebClient().DownloadString(siteurlold);
                //string[] arrayn = htmlold.ToString().Split(new char[]
                //{
                //             '/'
                //});
                //int dayn = Int32.Parse(arrayn[0]);
                //int monthn = Int32.Parse(arrayn[1]);
                //int yearn = Int32.Parse(arrayn[2]);

                DateTime time = DateTime.Now;
                int dayn = time.Day;
                int monthn = time.Month;
                int yearn = time.Year;

                string[] arrays = array[1].ToString().Split(new char[]
               {
                            '/'
               });

                int dayt = Int32.Parse(arrays[0]);
                int montht = Int32.Parse(arrays[1]);
                int yeart = Int32.Parse(arrays[2]);

                System.DateTime now = new System.DateTime(yearn, monthn, dayn);
                System.DateTime then = new System.DateTime(yeart, montht, dayt);
                System.TimeSpan diff1 = then.Subtract(now);


                int days = (int)Math.Ceiling(diff1.TotalDays);

                bool flag3 = days <= 0;
                if (flag3)
                {
                    MessageBox.Show("Vui lòng liên hệ admin để gia hạn.", "Phần mềm hết hạn" + days , MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.label3.Text = array[1].ToString();
                    this.ngayconlai.Text = days.ToString() + " ngày";
                }
                else
                {
                    MessageBox.Show("Đăng Nhập Thành Công .", "Còn lại: " + days + " ngày!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.label3.Text = array[1].ToString();
                    this.ngayconlai.Text = days.ToString() + " ngày";

                }
            }

            else
            {
                MessageBox.Show(string.Format("Bạn chưa mua bản quyền tool, vui lòng bấm Ctrl + C và gửi mã \"{0}\" cho chúng tôi để kích hoạt tool!", keys), "Thông báo active bản quyền!", MessageBoxButtons.OK);
                this.textBox1.Text = keys;
                //Environment.Exit(Environment.ExitCode);
            }


        }
    }
}
