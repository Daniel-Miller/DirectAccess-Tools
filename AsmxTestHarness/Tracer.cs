using System;
using System.Diagnostics;
using System.Text;

namespace AsmxTestHarness
{
    public class Tracer
    {
        private static StringBuilder _html;

        public static void Error(string message)
        {
            TraceEvent(TraceEventType.Error, message);
            HtmlError(message);
        }

        public static void Info(string message)
        {
            TraceEvent(TraceEventType.Information, message);
            HtmlInfo(message);
        }

        public static void Reset()
        {
            _html = new StringBuilder();
        }

        public static void Warning(string message)
        {
            TraceEvent(TraceEventType.Warning, message);
        }

        private static void TraceEvent(TraceEventType type, string message)
        {
            if (TraceFilePath != null)
            {
                var level = new SourceSwitch(SourceName) {Level = Level};
                var trace = new TraceSource(SourceName) {Switch = level};

                trace.Listeners.Clear();

                using (var file = new TextWriterTraceListener(TraceFilePath))
                {
                    trace.Listeners.Add(file);
                    trace.TraceEvent(type, 0, $"{DateTime.Now:HH:mm:ss.fff} {message}");
                    file.Flush();
                }

                trace.Close();
            }
        }

        #region Properties

        public static StringBuilder Html => _html ?? (_html = new StringBuilder());

        public static SourceLevels Level { get; set; } = SourceLevels.Verbose;

        public static string SourceName { get; set; } = "AsmxTestHarness";

        public static string TraceFilePath { get; set; }

        #endregion

        #region Methods (HTML)

        public static void HtmlInfo(string text)
        {
            var info = text.Replace("\n", "\n<br/>");

            Html.AppendLine($"<li>[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] Information: {info}</li>");
        }

        public static void HtmlError(string text)
        {
            var info = text.Replace("\n", "\n<br/>");

            Html.AppendLine($"<li style='color:red;'>[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] Error: {info}</li>");
        }

        #endregion
    }
}