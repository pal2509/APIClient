using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;


namespace APIClient
{
    class Program
    {
        public enum HTTP_Verb
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        static void Main(string[] args)
        {

            bool k = true;
            while(k)
            {
                Console.WriteLine("Opcões:\n");
                Console.WriteLine("1-Login\n");
                Console.WriteLine("2-FileList\n");
                Console.WriteLine("3-Download\n");
                Console.WriteLine("4-Upload\n");
                Console.WriteLine("5-Logout\n");
                Console.WriteLine("6-Sair\n");
                string op;
                op = Console.ReadLine();
                int n;
                if(int.TryParse(op, out n ))
                {
                    switch (n)
                    {
                        case 1:
                            {

                            }
                        case 2:
                            {

                            }
                        case 3:
                            {

                            }
                        case 4:
                            {

                            }
                        case 5:
                            {

                            }
                        case 6:
                            {
                                k = false;
                                break;
                            }
                    }
                        
                }

            }




            string endpoint = "http://localhost:53869/fileserver/FileUpload/5";
            string respValue = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
            request.Method = HTTP_Verb.POST.ToString();
            HttpWebResponse response = null;


            //----------------------------------------------------------------------

            string filename = "test.txt";
            request.ContentType = "multipart/form-data";
            request.Headers.Add("filename",filename);
            

            using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] buffer = new byte[1024 * 4];
                    int bytesLeft = 0;

                    while ((bytesLeft = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        requestStream.Write(buffer, 0, bytesLeft);
                    }

                }
            }

            //-----------------------------------------------------------------------
            try
            {
                response = (HttpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            Console.Write(reader.ReadToEnd());
                            Console.ReadLine();
                        }
                    }
                }

            }
            catch (Exception ex) { }


        }


        static public string  Login(string username, string password)
        {
            
        }
        



        static public void Upload(string uri, string filePath)
        {
            string formdataTemplate = "Content-Disposition: form-data; filename=\"{0}\";\r\nContent-Type: image/jpeg\r\n\r\n";
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ServicePoint.Expect100Continue = false;
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Headers.Add("filename", "test.jpg");

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, Path.GetFileName(filePath));
                    byte[] formbytes = Encoding.UTF8.GetBytes(formitem);
                    requestStream.Write(formbytes, 0, formbytes.Length);
                    byte[] buffer = new byte[1024 * 4];
                    int bytesLeft = 0;

                    while ((bytesLeft = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        requestStream.Write(buffer, 0, bytesLeft);
                    }

                }
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) { }

                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}
