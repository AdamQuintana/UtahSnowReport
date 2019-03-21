using System;
using System.Collections.Generic;
using System.Text;

namespace UtahSnowReport
{
    public class TPlus24hrData
    {
        public static DateTime RangeStartTime { get; set; }
        public static DateTime RangeStopTime { get; set; }
        public int Id { get; set; }
        public DateTime SampledTime { get; set; }
        public string ResortName { get; set; }
        public int Snow24hr_in { get; set; }

        public bool IsValid()
        {
            return DateTime.Now >= RangeStartTime && DateTime.Now <= RangeStopTime && Snow24hr_in > 0;
        }
        public static string HtmlHeader()
        {
            var columnNames = new List<string>() { "Resort", "Snowfall (in)" };
            StringBuilder builder = new StringBuilder();

            builder.Append("<h2>T + 24 Hr Snowfall</h2>");
            builder.AppendLine();

            builder.Append("<h3>");
            builder.Append(RangeStartTime.ToString("d"));
            builder.Append(" - ");
            builder.Append(RangeStopTime.ToString("d"));
            builder.Append("</h3>");
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
