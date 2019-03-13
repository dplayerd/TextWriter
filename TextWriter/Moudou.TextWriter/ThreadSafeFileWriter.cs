using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Moudou.TextWriter
{
    public class ThreadSafeFileWriter
    {
        #region "Static"
        /// <summary> 
        ///     Key: FilePath
        ///     Value: Writer
        /// </summary>
        private static Dictionary<string, ThreadSafeFileWriter> writerDic =
            new Dictionary<string, ThreadSafeFileWriter>();

        /// <summary> Write text into file </summary>
        /// <param name="filePath">File Path</param>
        /// <param name="content">Text will be write</param>
        public static void WriteFile(string filePath, string content)
        {
            WriteFile(filePath, new string[] { content });
        }

        /// <summary> Write text into file </summary>
        /// <param name="filePath">File Path</param>
        /// <param name="content">Text will be write</param>
        public static void WriteFile(string filePath, IEnumerable<string> content)
        {
            lock (writerDic)
            {
                if (!writerDic.ContainsKey(filePath))
                {
                    writerDic.Add(filePath, new ThreadSafeFileWriter() { _filePath = filePath });
                }
            }

            ThreadSafeFileWriter writer = writerDic[filePath];
            writer.Append(content);
        }


        /// <summary> Remove an instance from dic </summary>
        /// <param name="FilePath"></param>
        private static void RemoveWriter(string FilePath)
        {
            lock (writerDic)
            {
                if (writerDic.ContainsKey(FilePath))
                {
                    writerDic.Remove(FilePath);
                }
            }
        }
        #endregion


        #region "Instance method"
        private bool isWriting = false;

        private List<string> _writingPool =
            new List<string>();


        private string _filePath { get; set; }


        /// <summary> push texts into pool </summary>
        /// <param name="txts"></param>
        private void Append(IEnumerable<string> txts)
        {
            lock (this._writingPool)
            {
                this._writingPool.AddRange(txts);
            }

            WriteFile();
        }


        /// <summary> start writing </summary>
        private void WriteFile()
        {
            // Avoid multiple thread conflicts
            if (this.isWriting)
                return;

            this.isWriting = true;


            // writing thread
            Thread thread1 = new Thread(new ThreadStart(() =>
            {
                using (var fileStream = new FileStream(this._filePath, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    using (var streamWriter = new StreamWriter(fileStream, Encoding.Default))
                    {
                        while (this._writingPool.Count > 0)
                        {
                            List<string> temp = new List<string>();
                            lock (this._writingPool)
                            {
                                temp.AddRange(this._writingPool);
                                this._writingPool.Clear();
                            }

                            // it will faster than foreach
                            var text = string.Join("", temp);
                            streamWriter.Write(text);
                        }
                    }
                }

                this.isWriting = false;

                // remove self instance
                ThreadSafeFileWriter.RemoveWriter(this._filePath);
            }));

            thread1.Start();
        }
        #endregion
    }

}
