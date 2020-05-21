using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.JRSClient.Extensions
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
