﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UtahSnowReport
{
    public class TMinus24hrData
    {
        public int Id { get; set; }
        public string ResortName { get; set; }
        public DateTime SampledTime { get; set; }
        public int Snow24hr_in { get; set; }
        public int SnowDepth_in { get; set; }
        public DateTime UpdatedTime { get; set; }

        public bool IsValid()
        {
            DateTime today = DateTime.Today;
            return UpdatedTime.Date == today.Date;
        }
        public static string HtmlHeader()
        {
            var columnNames = new List<string>() { "Resort", "Snowfall (in)", "Total Snow Depth (in)" };
            StringBuilder builder = new StringBuilder();

            builder.Append("<h2>T - 24 Hr Snowfall</h2>");
            builder.AppendLine();

            builder.Append("<table  style=\"width:100%\">");
            builder.AppendLine();

            builder.Append("<tr>");
            builder.AppendLine();
            foreach (var name in columnNames)
            {
                builder.AppendFormat("<th>{0}</th>", name);
                builder.AppendLine();
            }
            builder.Append("</tr>");
            builder.AppendLine();

            return builder.ToString();
        }

        public string ToHtml()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<tr>");
            builder.AppendLine();

            builder.AppendFormat("<td>{0}</td>", ResortName);
            builder.AppendLine();

            builder.AppendFormat("<td>{0}</td>", Snow24hr_in);
            builder.AppendLine();

            builder.AppendFormat("<td>{0}</td>", SnowDepth_in);
            builder.AppendLine();

            builder.Append("</tr>");
            builder.AppendLine();

            return builder.ToString();
        }

        public static string HtmlFooter()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("</table>");
            builder.AppendLine();

            return builder.ToString();
        }
    }
}
