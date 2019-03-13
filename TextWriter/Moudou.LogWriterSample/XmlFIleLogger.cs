using Moudou.TextWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moudou.LogWriterSample
{
    public class XmlFileLogger
    {
        public class LogContent
        {
            public string Title { get; set; }

            public string Message { get; set; }

            public long Duration { get; set; }
        }

        private const string _thisHourFormat = "yyyyMMdd_HH";
        private const string _dateTimeFormat = "yyyy/MM/dd HH:mm:ss ffffff";
        private const string _logFolderPath = "D:\\logs\\";


        /// <summary> write logs </summary>
        /// <param name="uniqueKey"></param>
        /// <param name="log"></param>
        public static void AppendLog(Guid uniqueKey, LogContent log)
        {
            AppendLog(uniqueKey, new LogContent[] { log });
        }

        /// <summary> write logs </summary>
        /// <param name="uniqueKey"></param>
        /// <param name="logs"></param>
        public static void AppendLog(Guid uniqueKey, IEnumerable<LogContent> logs)
        {
            DateTime cDate = DateTime.Now;
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;


            string fileName =
                System.IO.Path.Combine(
                    XmlFileLogger._logFolderPath,
                    DateTime.Now.ToString(XmlFileLogger._thisHourFormat) + ".log");

            IEnumerable<string> items = GetWritingVal(uniqueKey, logs, cDate, threadID);
            ThreadSafeFileWriter.WriteFile(fileName, items);
        }


        // convert format
        private static IEnumerable<string> GetWritingVal(Guid uniqueKey, IEnumerable<LogContent> logs, DateTime cDate, int threadID)
        {
            foreach (var log in logs)
            {
                var outputText =
    $@"<log> <title> { log.Title } </title> <executeTime> { DateTime.Now.ToString(XmlFileLogger._dateTimeFormat) } </executeTime> <message> <![CDATA[{ log.Message }]]> </message> <duration> { log.Duration } </duration><uniqueKey> { uniqueKey } </uniqueKey><thread> { threadID } </thread></log>
";

                yield return outputText;
            }
        }
    }

}
