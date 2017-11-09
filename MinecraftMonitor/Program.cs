using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Data;
using AngleSharp.Dom.Html;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace MakeAGETRequest_charp
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    class MinecraftMonitor
    {

        static double resultMaxPlayers = 0;
        static double resultOnlinePlayers = 0;
        static double sumMaxPlayers = 0;
        static double sumOnlinePlayers = 0;
        static string serverStatus1 = "";
        static string serverStatus2 = "";
        static string serverStatus3 = "";
        static int count = 0;

        public double SumOnlinePlayers { get => SumOnlinePlayers; set => SumOnlinePlayers = value; }
        public double SumMaxPlayers { get => sumMaxPlayers; set => sumMaxPlayers = value; }
        public double ResultOnlinePlayers { get => resultOnlinePlayers; set => resultOnlinePlayers = value; }
        public double ResultMaxPlayers { get => resultMaxPlayers; set => resultMaxPlayers = value; }
        public static string ServerStatus1 { get => serverStatus1; set => serverStatus1 = value; }
        public static int Count { get => count; set => count = value; }

        static void Main(string[] args)
        {
            int i = 0;

            Console.WriteLine("Telemetry started successfully...");

            while (i < 100)
            {

                GetTelemetryData("https://mcapi.us/server/status?ip=40.117.185.5&port=25565");
                GetTelemetryData("https://mcapi.us/server/status?ip=40.121.144.142&port=25565");
                GetTelemetryData("https://mcapi.us/server/status?ip=40.71.26.49&port=25565");
                AppInsightsSendTelemetryData(ServerStatus1, sumMaxPlayers, sumOnlinePlayers);


                if (Count == 1)
                    serverStatus1 = ServerStatus1;
                else if (Count == 2)
                    serverStatus2 = ServerStatus1;
                else if (Count == 3)
                    serverStatus3 = ServerStatus1;
                else if (Count == 4)
                    Count = 0;


                Console.WriteLine("Minecraft servers status: server1: {0} - server2: {1} - server3: {2} | Max Players: {3} | Online Players: {4}", serverStatus1, serverStatus2, serverStatus3, sumMaxPlayers, sumOnlinePlayers);

                sumMaxPlayers = 0;
                sumOnlinePlayers = 0;

                Thread.Sleep(10000);

                Count++;

            }





        }


    

    private static void AppInsightsSendTelemetryData(string serverStatus, double sumMaxPlayers, double sumOnlinePlayers)
    {
        var tc = new TelemetryClient();
        tc.Context.InstrumentationKey = "a327b24d-f1b8-4a60-8388-5611823df089";

        var properties = new Dictionary<string, string> { { "Game", "Minecraft" }, { "Server Status", serverStatus } };
        var measurements = new Dictionary<string, double> { { "MaxPlayers", sumMaxPlayers }, { "Online Players Now", sumOnlinePlayers } };
        tc.TrackEvent("Minecraft Server Status", properties, measurements);
        tc.TrackMetric("Online Players Now", sumOnlinePlayers, properties);


    }

    public static void GetTelemetryData(string url)
    {


        string sURL;
        sURL = url;

        WebRequest wrGETURL;
        wrGETURL = WebRequest.Create(sURL);

        WebProxy myProxy = new WebProxy("myproxy", 80);
        myProxy.BypassProxyOnLocal = true;

        wrGETURL.Proxy = WebProxy.GetDefaultProxy();

        Stream objStream;

        objStream = wrGETURL.GetResponse().GetResponseStream();

        StreamReader objReader = new StreamReader(objStream);


        var st = new LogTelemetryStorage();

        var text = objReader.ReadLine();


        ServerStatus1 = LogTelemetryStorage.getBetween(text, strStart: "\"online\":", strEnd: ",");
        var maxPlayers = LogTelemetryStorage.getBetween(text, strStart: "\"max\":", strEnd: ",");
        var onlinePlayers = LogTelemetryStorage.getBetween(text, strStart: "\"now\":", strEnd: "}");



        resultMaxPlayers = Convert.ToDouble(maxPlayers);
        resultOnlinePlayers = Convert.ToDouble(onlinePlayers);


        sumMaxPlayers = sumMaxPlayers + resultMaxPlayers;
        sumOnlinePlayers = sumOnlinePlayers + resultOnlinePlayers;

        st.WriteLog(text);



    }
}

        public class LogTelemetryStorage
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