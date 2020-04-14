using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;

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
            string startpoint = "http://localhost:";
            Console.WriteLine("LocalHost number?");
            string host = Console.ReadLine();
            startpoint = startpoint + host;



            bool k = true, l = true;
            while (k)
            {
                Console.WriteLine("Opcões:");
                Console.WriteLine("1-Login");
                Console.WriteLine("2-Sair");
                string op;
                int n;
                op = Console.ReadLine();
                if (int.TryParse(op, out n))
                {
                    int token = 0;
                    switch (n)
                    {
                        case 1:
                            {
                                Console.WriteLine("Username?");
                                string username = Console.ReadLine();
                                Console.WriteLine("Password?");
                                string password = Console.ReadLine();

                                string r = Login(username, password, startpoint, ref token);
                                Console.WriteLine(r);
                                break;
                            }
                        case 2:
                            {
                                k = false;
                                break;
                            }
                    }

                    if (token > 0)
                    {
                        while (l)
                        {
                            Console.WriteLine("1-FileList");
                            Console.WriteLine("2-Download");
                            Console.WriteLine("3-Upload");
                            Console.WriteLine("4-Logout");
                            op = Console.ReadLine();

                            if (int.TryParse(op, out n))
                            {
                                switch (n)
                                {
                                    case 1:
                                        {
                                            Console.WriteLine(FileList(ref token, startpoint));
                                            break;
                                        }
                                    case 2:
                                        {
                                            Console.WriteLine("Nome do ficheiro?");
                                            string filename = Console.ReadLine();
                                            Download(startpoint, token, filename);
                                            break;
                                        }
                                    case 3:
                                        {
                                            Console.WriteLine("Nome do ficheiro?");
                                            string filename = Console.ReadLine();

                                            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + filename))
                                            {
                                                Upload(startpoint, token, filename);
                                                break;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Ficheiro não existe!!!");
                                                break;
                                            }
                                
                                        }
                                    case 4:
                                        {
                                            Console.WriteLine(Logout(ref token, startpoint, ref l));
                                            break;
                                        }
                                }

                            }
                        }
                    }
                }




 

            }

        }
        /// <summary>
        /// Metodo de login do utilizador
        /// </summary>
        /// <param name="username">Nome do utilizador</param>
        /// <param name="password">Password do utilizador</param>
        /// <param name="startpoint">Link startpoint com o localhost</param>
        /// <returns></returns>
        static public string Login(string username, string password, string startpoint, ref int token)
        {
            //Criação do pedido http
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/login/" + username + "/" + password);
            request.Method = HTTP_Verb.GET.ToString(); //Verbo do pedido http

            try
            {
                //Resposta do pedido
                HttpWebResponse res = (HttpWebResponse)request.GetResponse();
                
                //Criação do fluxo para a resposta
                using (Stream responseStream = res.GetResponseStream())
                {
                    if (responseStream != null)//Verificação se a resposta está vazia
                    {
                        string r;
                        //Criação do leitor da resposta
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            r = reader.ReadToEnd();//Leitura da resposta para uma string                          
                        }
                        //Interpretação das respostas
                        if (r.CompareTo("-1") == 0)
                        {
                            token = -1;
                            return "Username e/ou password errados!!!";
                        }
                        else if (r.CompareTo("-2") == 0)
                        {
                            token = -2;
                            return "User já está loged in";
                        }
                        else
                        {
                            token = int.Parse(r);
                            return "Login com sucesso!!!";
                        }
                    }
                    else
                    {
                        token = -3;
                        return "Erro inesperado!!!";
                    }
                }
            }
            catch (Exception e)
            {
                token = -4;
                return e.ToString();
            }
        }


        /// <summary>
        /// Método para fazer o pedido da lista de ficherios da api
        /// </summary>
        /// <param name="token">token do utilizador</param>
        /// <param name="startpoint">começo do link para o pedido</param>
        /// <returns>retorna uma string com os nomes dos ficherios</returns>
        static public string FileList(ref int token, string startpoint)
        {
            //Criação do pedido http
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/dir/" + token.ToString());
            request.Method = HTTP_Verb.GET.ToString(); //Verbo do pedido http

            try
            {
                //Resposta do pedido
                HttpWebResponse res = (HttpWebResponse)request.GetResponse();

                //Criação do fluxo para a resposta
                using (Stream responseStream = res.GetResponseStream())
                {
                    if (responseStream != null)//Verificação se a resposta está vazia
                    {
                        string r;
                        //Criação do leitor da resposta
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            r = reader.ReadToEnd();//Leitura da resposta para uma string                          
                        }
                         return r;                       
                    }
                    else
                    {
                        return "Erro inesperado!!!";
                    }
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// Método para o logout do utilizador
        /// </summary>
        /// <param name="token">token do utilizador</param>
        /// <param name="startpoint">link de inicio</param>
        /// <param name="l">variável que controla a paragem do ciclo</param>
        /// <returns>string com a resposta</returns>
        static public string Logout(ref int token, string startpoint,  ref bool l)
        {
            //Criação do pedido http
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/logout/" + token.ToString());
            request.Method = HTTP_Verb.GET.ToString(); //Verbo do pedido http

            try
            {
                //Resposta do pedido
                HttpWebResponse res = (HttpWebResponse)request.GetResponse();

                //Criação do fluxo para a resposta
                using (Stream responseStream = res.GetResponseStream())
                {
                    if (responseStream != null)//Verificação se a resposta está vazia
                    {
                        string r;
                        //Criação do leitor da resposta
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            r = reader.ReadToEnd();//Leitura da resposta para uma string                          
                        }
                        //Interpretação das respostas
                        if (r.CompareTo("true") == 0)
                        {
                            l = false;
                            token = 0;
                            return "Logout com sucesso.";
                        }
                        else
                        {
                            l = false;
                            token = 0;
                            return "Já está loged out";
                        }
                    }
                    else
                    {
                        l = true;
                        return "Erro inesperado!!!";
                    }
                }
            }
            catch (Exception e)
            {
                l = true;
                return e.ToString();
            }
        }

        /// <summary>
        /// Método para upload de ficheiros
        /// </summary>
        /// <param name="startpoint">link de inicio</param>
        /// <param name="token">token do utilizador</param>
        /// <param name="filename">nome do ficherio para ser uploaded</param>
        static public void Upload(string startpoint, int token, string filename)
        {
            string endpoint = startpoint + "/fileserver/FileUpload/" + token.ToString();//Criação do link completo
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);//Criação do pedido http
            request.Method = HTTP_Verb.POST.ToString();//Método do pedido
            HttpWebResponse response = null;
            //----------------------------------------------------------------------
            request.ContentType = "multipart/form-data";//
            request.Headers.Add("filename", filename);//Por o nome do ficheiro num campo do pedido

            //Stream para escrita do ficheiro pedido
            using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                //Stream para leitura da stream que é recebida
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] buffer = new byte[1024 * 4];//Buffer de leitura
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
                response = (HttpWebResponse)request.GetResponse();//Receber a resposta

                using (Stream responseStream = response.GetResponseStream())//Stram para ler a resposta
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))//Leitura da stream de resposta
                        {
                            Console.Write(reader.ReadToEnd());
                        }
                    }
                }

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Metodo de download
        /// </summary>
        /// <param name="startpoint"></param>
        /// <param name="token"></param>
        /// <param name="filename"></param>
        static public void Download(string startpoint, int token, string filename)
        {
            string endpoint = startpoint + "/fileserver/FileDownload/" + token;//Criação do link
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);//Criação do pedido http
            request.Method = HTTP_Verb.GET.ToString();//Metodo do pedido
            HttpWebResponse response = null;
            request.ContentType = "multipart/form-data";//Tipo de dados
            request.Headers.Add("filename", filename);//Nome do ficheiro

            response = (HttpWebResponse)request.GetResponse();//Receber a resposta
            var filePath = AppDomain.CurrentDomain.BaseDirectory + filename;//Caminho do ficheiro
            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))//Criação de uma stream para escrita do ficheiro
            {
                response.GetResponseStream().CopyTo(fs);//Receber a resposta e copia-la para a stream fs;
            }

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
