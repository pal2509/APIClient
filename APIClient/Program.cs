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



            bool k = true, l = false;
            while (k)
            {
                Console.WriteLine("Opcões:");
                Console.WriteLine("1-Login");
                Console.WriteLine("2-Sair");
                Console.WriteLine("3-Registo");
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

                                int r = Login(username, password, startpoint, ref token);
                                if (r == 1)
                                {
                                    Console.WriteLine("Login com sucesso!!!");
                                    l = true;
                                }
                                if (r == -1) Console.WriteLine("Username ou password não existe.");
                                if (r == -2) Console.WriteLine("Já está loged in.");
                                if (r < -3) Console.WriteLine("Erro inesperado!!!");

                                break;
                            }
                        case 2:
                            {
                                k = false;
                                break;
                            }
                        case 3:
                            {
                                Console.WriteLine("Username?");
                                string username = Console.ReadLine();
                                Console.WriteLine("Password?");
                                string password = Console.ReadLine();
                                int r = Registration(username, password, startpoint);
                                if (r == 1) Console.WriteLine("Sucesso");
                                if (r == -1) Console.WriteLine("Nome de utilizador já existe!!!");
                                if (r == -4) Console.WriteLine("Erro - Inesperado");
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
                            Console.WriteLine("4-Delete File");
                            Console.WriteLine("5-Lista de pedidos de registo");
                            Console.WriteLine("6-Aceitar pedidos");
                            Console.WriteLine("7-Logout");
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
                                            int r = Download(startpoint, token, filename);
                                            if (r == 1) Console.WriteLine("Sucesso");
                                            if (r == -1) Console.WriteLine("Token inválido");
                                            if (r == -2) Console.WriteLine("Ficheiro não existe!!!");
                                            
                                            break;
                                        }
                                    case 3:
                                        {
                                            Console.WriteLine("Nome do ficheiro?");
                                            TimeSpan time = new TimeSpan(-1);
                                            string filename = Console.ReadLine();
                                            Console.WriteLine("Adicionar tempo de vida?(Y-Sim N-Não");
                                            string y = Console.ReadLine();
                                            if (y.CompareTo("Y") == 0 || y.CompareTo("y") == 0)
                                            {
                                                Console.WriteLine("Tempo?()");
                                                time = TimeSpan.Parse(Console.ReadLine());
                                            }
                                            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + filename))
                                            {
                                                int r = Upload(startpoint, token, filename, time);
                                                if (r == 1) Console.WriteLine("Sucesso");
                                                if (r == -1) Console.WriteLine("Token inválido");
                                                if (r == -2) Console.WriteLine("Ficheiro não existe!!!");
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
                                            Console.WriteLine("Nome do ficheiro?");
                                            string filename = Console.ReadLine();
                                            int r = DeleteFile(filename, startpoint, token);
                                            if (r == 1) Console.WriteLine("Ficheiro " + filename + " eliminado com sucesso.");
                                            if (r == -1) Console.WriteLine("Token inválido");
                                            if (r == -2) Console.WriteLine("Ficheiro não existe!!!");
                                            if (r == -3) Console.WriteLine("Não foi possivel chegar ao seu workspace.");
                                            if (r == -4) Console.WriteLine("Não foi possivel obter resposta");
                                            break;
                                        }
                                    case 5:
                                        {
                                            RequestList(startpoint, token);
                                            break;
                                        }
                                    case 6:
                                        {
                                            Console.WriteLine("Nome de utilizador?");
                                            string username = Console.ReadLine();
                                            int r = AcceptRequest(startpoint, token, username);
                                            if (r == 1) Console.WriteLine("Utilizador " + username + " aceite com sucesso.");
                                            if (r == -1) Console.WriteLine("Token inválido");
                                            if (r == -2) Console.WriteLine("Não tem permissão para aceitar utilizadores");
                                            if (r == -3) Console.WriteLine("Não existe nehnum pedido com esse nome de utilizador!!!");
                                            if (r == -4) Console.WriteLine("Não foi possivel obter resposta");
                                            break;
                                        }
                                    case 7:
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

        static public int AcceptRequest(string startpoint, int token, string username)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/RequestManagement/" + token.ToString());
            request.Method = HTTP_Verb.POST.ToString(); //Verbo do pedido http
            request.ContentType = "multipart/form-data";//
            request.Headers.Add("username", username);
            request.ContentLength = 0;

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
                    return int.Parse(r);
                }
                else return -4;
            }
        }


        static public void RequestList(string startpoint, int token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/RequestList/" + token.ToString());
            request.Method = HTTP_Verb.GET.ToString(); //Verbo do pedido http


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
                    Console.WriteLine(r);
                }
                else Console.WriteLine("Erro - Tente novamente");


            }
        }

        /// <summary>
        /// Metodo de login do utilizador
        /// </summary>
        /// <param name="username">Nome do utilizador</param>
        /// <param name="password">Password do utilizador</param>
        /// <param name="startpoint">Link startpoint com o localhost</param>
        /// <returns></returns>
        static public int Login(string username, string password, string startpoint, ref int token)
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
                            return -1;
                        }
                        else if (r.CompareTo("-2") == 0)
                        {
                            token = -2;
                            return -2;
                        }
                        else
                        {
                            token = int.Parse(r);
                            return 1;
                        }
                    }
                    else
                    {
                        token = -3;
                        return -3;
                    }
                }
            }
            catch (Exception e)
            {
                token = -4;
                return - 4;
            }
        }

        static public int DeleteFile(string filename, string startpoint, int token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/FileDelete/" + token.ToString());
            request.Method = HTTP_Verb.POST.ToString(); //Verbo do pedido http
            request.ContentType = "multipart/form-data";//
            request.Headers.Add("filename", filename);
            request.ContentLength = 0;

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

                    return int.Parse(r);

                }
                else return -4;
            }

        }

        static public int Registration(string usrnm, string psswd, string startpoint)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/SignUp");
            request.Method = HTTP_Verb.POST.ToString(); //Verbo do pedido http

            request.ContentType = "multipart/form-data";//
            request.Headers.Add("username", usrnm);
            request.Headers.Add("password", psswd);
            request.ContentLength = 0;

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

                    return int.Parse(r);
                }
                else return -4;
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
        static public int Upload(string startpoint, int token, string filename, TimeSpan time)
        {
            string endpoint = startpoint + "/fileserver/FileUpload/" + token.ToString();//Criação do link completo
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);//Criação do pedido http
            request.Method = HTTP_Verb.POST.ToString();//Método do pedido
            HttpWebResponse response = null;
            //----------------------------------------------------------------------
            request.ContentType = "multipart/form-data";//
            request.Headers.Add("filename", filename);//Por o nome do ficheiro num campo do pedido
            if (time.CompareTo(new TimeSpan(-1)) == 0)
            {
                request.Headers.Add("ttl", null);
            }
            else request.Headers.Add("ttl", time.ToString());
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + filename))return -2;
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
            string r = "-3";
          
               response = (HttpWebResponse)request.GetResponse();//Receber a resposta

                using (Stream responseStream = response.GetResponseStream())//Stream para ler a resposta
                {
                    if (responseStream != null)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))//Leitura da stream de resposta
                        {
                            r = reader.ReadToEnd();
                        }
                    }
                }
                return int.Parse(r);
        }

        /// <summary>
        /// Metodo de download
        /// </summary>
        /// <param name="startpoint"></param>
        /// <param name="token"></param>
        /// <param name="filename"></param>
        static public int Download(string startpoint, int token, string filename)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(startpoint + "/fileserver/FileDownload/" + token.ToString());
            request.Method = HTTP_Verb.POST.ToString(); //Verbo do pedido http
            request.ContentType = "multipart/form-data";//
            request.Headers.Add("filename", filename);
            request.ContentLength = 0;

            HttpWebResponse res = (HttpWebResponse)request.GetResponse();

            var filePath = AppDomain.CurrentDomain.BaseDirectory + filename;//Caminho do ficheiro
            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))//Criação de uma stream para escrita do ficheiro
            {
                res.GetResponseStream().CopyTo(fs);//Receber a resposta e copia-la para a stream fs;
            }
            return 1;
        }


 
    }
}
