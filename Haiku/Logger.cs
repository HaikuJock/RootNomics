using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Haiku
{
    public class NonBlockingLogger : Logging
    {
        readonly private BlockingCollection<Tuple<long, string>> messageQueue = new BlockingCollection<Tuple<long, string>>();
        readonly bool toConsole;

        public NonBlockingLogger(bool toConsole = false)
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    OutputFromMessageQueue();
                }
            });
            thread.IsBackground = true;
            thread.Start();
            this.toConsole = toConsole;
        }

        public void WriteLine(string message)
        {
            messageQueue.Add(new Tuple<long, string>(DateTime.Now.Ticks, message));
        }

        public void Flush()
        {
            while (messageQueue.Count > 0)
            {
                OutputFromMessageQueue();
            }
        }

        void OutputFromMessageQueue()
        {
            var timeStampedMessage = messageQueue.Take();
            var timeStamp = new DateTime(timeStampedMessage.Item1).ToString("HH:mm:ss.fff");
            var message = timeStampedMessage.Item2;

            OutputLine("[" + timeStamp + "] " + message);
        }

        protected virtual void OutputLine(string output)
        {
            if (toConsole)
            {
                Console.WriteLine(output);
            }
            else
            {
                Debug.WriteLine(output);
            }
        }
    }

    public class NonBlockingFileLogger : NonBlockingLogger
    {
        const int FlushAtLength = 16 * 1024;
        const int FlushAfterTimeInSeconds = 20;
        readonly StringBuilder stringBuilder;
        readonly string fileName;
        DateTime lastFlushTime;

        public NonBlockingFileLogger(string fileName)
            : base()
        {
            stringBuilder = new StringBuilder();
            this.fileName = fileName;
            lastFlushTime = DateTime.UtcNow;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }

        public new void Flush()
        {
            while (stringBuilder.Length > 0)
            {
                FlushLogToFile();
            }
        }

        protected override void OutputLine(string output)
        {
            stringBuilder.AppendLine(output);

            if (stringBuilder.Length >= FlushAtLength
                || (DateTime.UtcNow - lastFlushTime).TotalSeconds > FlushAfterTimeInSeconds)
            {
                FlushLogToFile();
            }
        }

        void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            FlushLogToFile();
        }

        void FlushLogToFile()
        {
            File.AppendAllText(fileName, stringBuilder.ToString());
            stringBuilder.Clear();
            lastFlushTime = DateTime.UtcNow;
        }
    }

    public class Logger : Logging
    {
        public void WriteLine(string message)
        {
            //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message);
            Debug.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + message);
        }

        public void Flush() { }
    }
}
