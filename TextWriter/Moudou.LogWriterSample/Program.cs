using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moudou.LogWriterSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.GO();
        }


        #region "Sample"
        public static void GO()
        {
            for (var i = 0; i < 1024; i++)
            {
                Thread writeThread = new Thread(new ThreadStart(() =>
                {
                    XmlFileLogger.AppendLog(Guid.NewGuid(), new XmlFileLogger.LogContent()
                    {
                        Duration = 48763,
                        Message = "Message",
                        Title = "Title"
                    });
                }));


                writeThread.Start();
            }
        }
        #endregion
    }
}
