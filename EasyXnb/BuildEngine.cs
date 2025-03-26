using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;

namespace EasyXnb
{
    public class BuildEngine : IBuildEngine
    {
        private string _logFile = "";
        private StreamWriter _logWriter;
        private List<string> _errors;
        public bool ShouldLog;

        public BuildEngine()
        {
            _errors = new List<string>();
            ShouldLog = true;
        }

        public BuildEngine(string logFile)
        {
            _logFile = logFile;
            ShouldLog = true;
            try
            {
                _logWriter = new StreamWriter(logFile, true);
            }
            catch { ShouldLog = false; }
        }

        public void Begin()
        {
            if (ShouldLog)
            {
                _errors = new List<string>();
            }
        }

        private void Log(string message)
        {
            if (ShouldLog)
            {
                try
                {
                    _logWriter.WriteLine(message);
                }
                catch { }
            }
        }

        public void End()
        {
            if (ShouldLog)
            {
                try
                {
                    _logWriter.Flush();
                    _logWriter.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// Returns a list of the errors recorded while processing files.
        /// </summary>
        public List<string> GetErrors()
        {
            return _errors;
        }

        //We don't need this, but we need it to be defined.
        public bool BuildProjectFile(string projectFileName, string[] targetNames, System.Collections.IDictionary globalProperties, System.Collections.IDictionary targetOutputs)
        {
            return true;
        }

        //We don't need this, but we need it to be defined.
        public int ColumnNumberOfTaskNode => 0;

        //We don't need this, but we need it to be defined.
        public bool ContinueOnError => true;

        //We don't need this, but we need it to be defined.
        public int LineNumberOfTaskNode => 0;

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            if (ShouldLog)
            {
                Log($"Custom Event at {DateTime.Now}: {e.Message}");
            }
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            if (ShouldLog)
            {
                Log($"Error at {DateTime.Now}: {e.File}: {e.Message}");
            }
            _errors.Add($"{e.File}: {e.Message}");
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            if (ShouldLog)
            {
                Log($"Message at {DateTime.Now}: {e.Message}");
            }
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            if (ShouldLog)
            {
                Log($"Warning at {DateTime.Now}: {e.Message}");
            }
        }

        //We don't need this, but we need it to be defined.
        public string ProjectFileOfTaskNode => string.Empty;
    }
}