using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtahSnowReport
{
    public class T48HrReport
    {
        public int Id { get; set; }
        public List<TMinus24hrData> TMinus24HrData { get; set; }
        public List<TPlus24hrData> TPlus24HrData { get; set; }
        public DateTime TimeStamp { get; set; }

        public bool IsValid()
        {
            return TMinus24HrData.Any(d => d.IsValid()) || TPlus24HrData.Any(d => d.IsValid());
        }

        public string ToHtml()
        {
            var builder = new StringBuilder();
            builder.Append("<html>");
            builder.AppendLine();
            builder.Append("<head>");
            builder.AppendLine();
            builder.Append("<style>");
            builder.AppendLine();
            builder.Append("table, th, td {");
            builder.AppendLine();
            builder.Append("border: 1px solid black;");
            builder.AppendLine();
            builder.Append("border-collapse: collapse;");
            builder.AppendLine();
            builder.Append("}");
            builder.AppendLine();
            builder.Append("</style>");
            builder.AppendLine();
            builder.Append("</head>");
            builder.AppendLine();
            builder.Append("<body>");
            builder.AppendLine();

            builder.Append("<h2>");
            builder.AppendLine();
            builder.Append(TimeStamp.ToString());
            builder.Append("</h2>");
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine();

            builder.Append(TMinus24hrData.HtmlHeader());
            foreach (var tMinusData in TMinus24HrData)
            {
                if (tMinusData.IsValid())
                {
                    builder.Append(tMinusData.ToHtml());
                }
            }
            builder.Append(TMinus24hrData.HtmlFooter());

            builder.AppendLine();
            builder.AppendLine();

            builder.Append(TPlus24hrData.HtmlHeader());
            foreach (var tPlusData in TPlus24HrData)
            {
                if (tPlusData.IsValid())
                {
                    builder.Append(tPlusData.ToHtml());
                }
            }
            builder.Append(TPlus24hrData.HtmlFooter());

            builder.Append("</body></html>");

            return builder.ToString();
        }
    }
}
