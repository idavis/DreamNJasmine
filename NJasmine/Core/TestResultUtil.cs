﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;
using NUnit.Core;

namespace NJasmine.Core
{
    public class TestResultUtil
    {
        public static void Error(TestResult testResult, Exception exception, FailureSite failureSite = FailureSite.Test)
        {
            SetResult(testResult, ResultState.Error, exception, failureSite);
        }

        private static void SetResult(TestResult testResult, ResultState resultState, Exception ex, FailureSite failureSite)
        {
            if (resultState == ResultState.Cancelled)
            {
                testResult.SetResult(resultState, "Test cancelled by user", BuildStackTrace(ex, testResult.Test), failureSite);
            }
            else
            {
                if (resultState == ResultState.Error)
                {
                    testResult.SetResult(resultState, BuildMessage(ex), BuildStackTrace(ex, testResult.Test), failureSite);
                }
                else
                {
                    testResult.SetResult(resultState, ex.Message, ex.StackTrace, failureSite);
                }
            }
        }

        private static string BuildStackTrace(Exception exception, ITest test)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SPECIFICATION:");
            if (test.Properties.Contains(TestExtensions.MultilineNameProperty))
            {
                foreach (var line in ((string)test.Properties[TestExtensions.MultilineNameProperty]).Split('\n'))
                    sb.AppendLine("    " + line);
            }

            sb.AppendLine();

            sb.AppendLine(GetStackTrace(exception));

            for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                sb.Append(Environment.NewLine);
                sb.Append("--");
                sb.Append(innerException.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append(GetStackTrace(innerException));
            }
            return sb.ToString();
        }

        private static string BuildMessage(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} : {1}", exception.GetType().ToString(), exception.Message);

            for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.AppendFormat("  ----> {0} : {1}", innerException.GetType().ToString(), innerException.Message);
            }
            return stringBuilder.ToString();
        }

        private static string GetStackTrace(Exception exception)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                string stackTraceToTrim = "at NJasmine.Core";

                foreach(var line in exception.StackTrace.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!line.Trim().StartsWith(stackTraceToTrim))
                        sb.AppendLine(line);
                }
            }
            catch (Exception)
            {
                sb.Append("No stack trace available");
            }

            return sb.ToString();
        }
    }
}