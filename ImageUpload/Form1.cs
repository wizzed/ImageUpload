using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
namespace ImageUpload
{
    public partial class ImageUpload : Form
    {
        public ImageUpload()
        {
            InitializeComponent();
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(95, 18);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((char)e.KeyChar == (char)Keys.Escape)
                Application.Exit();
        }

        /*private void timer1_Tick(object sender, EventArgs e)
        {
            Application.Exit();
        }*/

        //FOR UPLOADING TO SERVER
        private void timer2_Tick(object sender, EventArgs e)
        {
            uploadToSite();
        }

        string path;

        bool uploading = false;
        private void uploadToSite()
        {
            if (Clipboard.ContainsImage() == true && !uploading)
            {
                label1.Text = "Uploading...";
                uploading = true;
                string website;
                path = Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".jpg";
                website = "http://wizzed.net/imageupload/uploadz.php";
                System.Net.WebClient client = new System.Net.WebClient();
                client.Headers.Add("Content-Type", "binary/octet-stream");
                
                Image test = Clipboard.GetImage();
                EncoderParameters parameters = new EncoderParameters(1);
                parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 80L);
                ImageCodecInfo codecInfo = GetEncoderInfo("image/jpeg");
                test.Save(path, codecInfo, parameters);
                Object o = new Object();
                Uri webUri = new Uri(website);
                client.UploadFileCompleted += new UploadFileCompletedEventHandler(UploadFileCallback);
                client.UploadFileAsync(webUri, "POST", path, o);
                //timer1.Start();
            }
        }

        private void UploadFileCallback(Object sender, UploadFileCompletedEventArgs e)
        {
            byte[] result = e.Result;
            string s = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);
            label1.Text = s.Split('$')[0] + " " + DateTime.Now.ToString("HH:mm:ss");
            System.IO.File.Delete(path);
            Clipboard.SetText(s.Split('$')[1]);
            uploading = false;
        }

       
        private void saveToFolder()
        {
            if (Clipboard.ContainsImage() == true)
            {
                string path =  Directory.GetCurrentDirectory() + "\\" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".png";
                
                //Image test = Clipboard.GetImage();

                try
                {
                    Image test = Clipboard.GetImage();
                    if (test != null)
                    {
                        test = Clipboard.GetImage();
                        Clipboard.Clear();
                        EncoderParameters parameters = new EncoderParameters(1);
                        parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
                        ImageCodecInfo codecInfo = GetEncoderInfo("image/png");
                        test.Save(path, codecInfo, parameters);
                        label1.Text = DateTime.Now.ToString("yyyyMMdd HHmmss") + ".png";
                        //timer1.Start();
                    }
                }
                catch(Exception e)
                {
                }
               
            }
        }

        #region drag to move window
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }

        }
        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        /*
        private void timer3_Tick(object sender, EventArgs e)
        {
            label1.Text = "";
            timer3.Stop();
        }*/

        private void ImageUpload_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("http://i.wizzed.net/images.php");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
