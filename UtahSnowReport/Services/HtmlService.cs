using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace UtahSnowReport
{
    public interface IHtmlService
    {
        T48HrReport Build48HrReport();
        List<TMinus24hrData> GetTMinus24HrData();
        List<TPlus24hrData> GetTPlus24HrData();
        Stream DownloadExpectedSnowFallImage();
    }
    public class HtmlService : IHtmlService
    {

        public HtmlService() { }

        /*
        * 48hr_Data
        */
        public T48HrReport Build48HrReport()
        {
            var t48HrReport = new T48HrReport();
            t48HrReport.TMinus24HrData = GetTMinus24HrData();
            t48HrReport.TPlus24HrData = GetTPlus24HrData();
            t48HrReport.TimeStamp = DateTime.Now;
            return t48HrReport;
        }

        /*
        * T_Plus_24hr_Data
        */
        private static readonly string WeatherGov_SnowTable_Id = "snowTable";
        private static readonly string WeatherGov_Timestamp_Id = "timestamp3";
        private static readonly Dictionary<string, string> WeatherGov_Resorts_Values = new Dictionary<string, string>()
        {
            { "Alta", "Alta Ski Area, UT" },
            { "Snowbird", "Snowbird Ski and Summer Resort, UT" },
            { "Solitude", "Solitude Mountain Resort, UT" },
            { "Brighton", "Brighton Resort, UT" },
            { "Park City", "Park City Mountain Resort, UT" },
            { "Deer Valley", "Deer Valley, UT" },
            { "Snowbasin", "Snowbasin Resort, UT" }
        };

        public List<TPlus24hrData> GetTPlus24HrData()
        {
            var htmlDoc = new HtmlDocument();
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            using (var browser = new ChromeDriver(chromeOptions))
            {
                browser.Url = @"https://www.weather.gov/slc/winter#tab-2";
                var source = browser.PageSource;
                htmlDoc.LoadHtml(source);
            }

            //find snowtable node
            var snowTableNode = htmlDoc.GetElementbyId(WeatherGov_SnowTable_Id);

            //find timestamp node
            var timeStampNode = htmlDoc.GetElementbyId(WeatherGov_Timestamp_Id);
            var timeStampWords = timeStampNode.InnerText.Split(" ");
            TPlus24hrData.RangeStartTime = DateTime.TryParse(timeStampWords[0] + " 05:00:00 AM", out DateTime timestamp1) ? timestamp1 : DateTime.UnixEpoch;
            TPlus24hrData.RangeStopTime = DateTime.TryParse(timeStampWords[3] + " 05:00:00 AM", out DateTime timestamp2) ? timestamp2 : DateTime.UnixEpoch;

            List<TPlus24hrData> data = new List<TPlus24hrData>();
            foreach (KeyValuePair<string, string> resort_value in WeatherGov_Resorts_Values)
            {
                TPlus24hrData datum = new TPlus24hrData
                {
                    ResortName = resort_value.Key
                };

                //find resort node
                var resortNode = snowTableNode.Descendants("td").Where(d => d.InnerText == resort_value.Value).First();

                //find 24 hr snow data
                var snow24Node = resortNode.NextSibling.NextSibling;
                string snow24Text = snow24Node.InnerHtml;
                datum.Snow24hr_in = int.TryParse(snow24Text, out int temp1) ? temp1 : 0;

                //timestamp this sample
                datum.SampledTime = DateTime.Now;

                data.Add(datum);
            }

            return data;
        }

        /*
         * T_Minus_24hr_Data
        */
        private static readonly Dictionary<string, string> SkiUtah_Resorts_Ids = new Dictionary<string, string>()
        {
            { "Alta", "snow-report-summary-alta" },
            { "Snowbird", "snow-report-summary-snowbird" },
            { "Solitude", "snow-report-summary-solitude" },
            { "Brighton", "snow-report-summary-brighton" },
            { "Park City", "snow-report-summary-park-city-mountain" },
            { "Deer Valley", "snow-report-summary-deer-valley" },
            { "Snowbasin", "snow-report-summary-snowbasin" }
        };
        private static readonly string SkiUtah_TimeStamp = "u-type-caps SnowReportUpdate";
        private static readonly string SkiUtah_24hrSnow = "Conditions-condition SnowReportSummary-newSnow24";
        private static readonly string SkiUtah_SnowDepth = "Conditions-condition SnowReportSummary-baseSnowDepth";

        public List<TMinus24hrData> GetTMinus24HrData()
        {
            var html = @"https://www.skiutah.com/snowreport";
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);
            List<TMinus24hrData> data = new List<TMinus24hrData>();
            foreach (KeyValuePair<string, string> resort_id in SkiUtah_Resorts_Ids)
            {
                TMinus24hrData datum = new TMinus24hrData
                {
                    ResortName = resort_id.Key
                };

                //find resort node
                var resortNode = htmlDoc.GetElementbyId(resort_id.Value);

                //find timestamp node
                var timeStampNode = resortNode.Descendants("span").Where(d => d.Attributes["class"].Value == SkiUtah_TimeStamp).First();
                string timeStampText = timeStampNode.Descendants("strong").First().InnerHtml;
                string[] timeStampTextSplit = timeStampText.Split(" ");
                string[] timeStamp = timeStampTextSplit.Reverse().Take(2).Reverse().ToArray();
                datum.UpdatedTime = DateTime.TryParse(string.Join(" ", timeStamp[0], timeStamp[1]), out DateTime timestamp1) ? timestamp1 : DateTime.UnixEpoch;

                //find 24 hour snow node
                var snow24Node = resortNode.Descendants("div").Where(d => d.Attributes["class"].Value == SkiUtah_24hrSnow).First();
                string snow24Text = snow24Node.Descendants("span").First().InnerHtml;
                datum.Snow24hr_in = int.TryParse(snow24Text, out int temp1) ? temp1 : 0;

                //find snow depth node
                var snowDepthNode = resortNode.Descendants("div").Where(d => d.Attributes["class"].Value == SkiUtah_SnowDepth).First();
                string snowDepthText = snowDepthNode.Descendants("span").First().InnerHtml;
                datum.SnowDepth_in = int.TryParse(snowDepthText, out int temp2) ? temp2 : 0;

                //timestamp this sample
                datum.SampledTime = DateTime.Now;

                data.Add(datum);
            }

            return data;
        }

        public Stream DownloadExpectedSnowFallImage()
        {
            Uri uri = new Uri(@"https://www.weather.gov/images/slc/winter/StormTotalSnowWeb1.png");
            //WebClient client = new WebClient();
            //return client.OpenRead(uri);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            request.Accept = @"image/png";
            request.Referer = @"http://www.somesite.com/";
            request.Headers.Add("Accept-Language", "en-GB");
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
            request.Host = @"www.somesite.com";
            WebResponse response = request.GetResponse();
            return response.GetResponseStream();
        }
    }
}
