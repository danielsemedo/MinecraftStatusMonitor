using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Data;
using AngleSharp.Dom.Html;

namespace MakeAGETRequest_charp
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class Class1
    {
        static void Main(string[] args)
        {
            int i = 0;

            while (i < 100)
            {

            
            string sURL;
            sURL = "https://mcapi.us/server/status?ip=13.92.44.209&port=25565";

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            WebProxy myProxy = new WebProxy("myproxy", 80);
            myProxy.BypassProxyOnLocal = true;

            wrGETURL.Proxy = WebProxy.GetDefaultProxy();

            Stream objStream;
                while (i != 287)
                { 
                     objStream = wrGETURL.GetResponse().GetResponseStream();
                       
                     StreamReader objReader = new StreamReader(objStream);

                      var st = new DataStorage();

                      var text = objReader.ReadLine();

                    
                        var serverStatus = DataStorage.getBetween(text, strStart: "\"online\":", strEnd: ",");
                        var maxPlayers = DataStorage.getBetween(text, strStart: "\"max\":", strEnd: ",");
                        var onlinePlayers = DataStorage.getBetween(text, strStart: "\"now\":", strEnd: "}");




                   

                        st.WriteLog(text);

                        Thread.Sleep(1000);
                }

            }


        }

        public class DataStorage
        {
            public void WriteLog(String rzad)
            {
                StreamWriter sw = new StreamWriter("log.csv", true);

                    if (!Convert.IsDBNull(rzad))
                    {
                        sw.Write(rzad);
                        sw.Write("\t");
                    }
                
                sw.Write("\n");
                sw.Flush();
                sw.Close();
            }

            public static string getBetween(string strSource, string strStart, string strEnd)
            {
                int Start, End;
                if (strSource.Contains(strStart) && strSource.Contains(strEnd))
                {
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                    End = strSource.IndexOf(strEnd, Start);
                    return strSource.Substring(Start, End - Start);
                }
                else
                {
                    return "";
                }
            }


        }

    


    }
}