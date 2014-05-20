using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPact.MowblyFramework.Core.Log.Format
{
    class JsonFormatter  : ILogFormatter
    {
        public String getContentType()
        {
            return "text/plain";
        }

        public String getHeader()
        {
            return null;
        }

        public String getFooter()
        {
            return null;
        }

        public class LOG_TO_SERVER
        {
            public string type { get; set; }
            public string level { get; set; }
            public string tag { get; set; }
            public string message { get; set; }
            public string username { get; set; }
            public string space { get; set; }
            public string timestamp { get; set; }
        }



        public virtual string Format(LogEvent e)
        {
            string currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
            List<LOG_TO_SERVER> log = new List<LOG_TO_SERVER>
            {
                new LOG_TO_SERVER
                {
                    type = e.Type, 
                    level = e.Level.ToString().ToUpper(), 
                    tag = e.Tag, 
                    message = e.Message,
                    username = e.Username, 
                    space = "",
                    timestamp = (e.TimeStamp - 60*60*1000).ToString()
                }
             };

            string log_string = JsonConvert.SerializeObject(log);

            log_string = log_string.Remove(log_string.Length - 1, 1);

            log_string = log_string.Remove(0, 1);

            log_string = log_string + ",";

            return log_string;
        }
    }
}
