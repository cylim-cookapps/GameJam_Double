using System;
using Cysharp.Text;
using UnityEngine;

namespace Pxp
{
    public static class Debug
    {
        #region Log

        /// <summary>
        /// Debug.Log
        /// </summary>
        /// <param name="msg"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void Log(object msg)
        {
            UnityEngine.Debug.Log(ZString.Format("[Pxp_Log] : {0}", msg));
        }

        /// <summary>
        /// Debug.LogFormat
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        /// <summary>
        /// Debug.Log (Message Color)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogColor(object message, Color? color = null)
        {
            var colorStr = "yellow";
            if (color != null)
            {
                colorStr = ColorUtility.ToHtmlStringRGBA(color.Value);
            }

            UnityEngine.Debug.Log(ZString.Format("[Pxp_Log] : <color='#{0}'>{1}</color>", colorStr, message) );
        }

        #endregion

        #region LogWarning

        /// <summary>
        /// Debug.LogWarning
        /// </summary>
        /// <param name="msg"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogWarning(object msg)
        {
            UnityEngine.Debug.LogWarning(ZString.Format("[Pxp_LogWarning] : {0}", msg));
        }

        /// <summary>
        /// Debug.LogWarningFormat
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        /// <summary>
        /// Debug.LogWarningFormat (Message Color)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogWarningColor(object message, Color? color = null)
        {
            var colorStr = "yellow";
            if (color != null)
            {
                colorStr = ColorUtility.ToHtmlStringRGBA(color.Value);
            }

            UnityEngine.Debug.LogWarning(
                ZString.Format("[Pxp_LogWarning] : <color='#{0}'>{1}</color>", colorStr, message));
        }

        #endregion

        #region LogError

        /// <summary>
        /// Debug.LogError
        /// </summary>
        /// <param name="msg"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogError(object msg)
        {
            UnityEngine.Debug.LogError(ZString.Format("[Pxp_LogError] : {0}", msg));
        }

        /// <summary>
        /// Debug.LogErrorFormat
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        /// <summary>
        /// Debug.LogError (Message Color)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        [System.Diagnostics.Conditional("__DEV")]
        public static void LogErrorColor(object message, Color? color = null)
        {
            var colorStr = "yellow";
            if (color != null)
            {
                colorStr = ColorUtility.ToHtmlStringRGBA(color.Value);
            }

            UnityEngine.Debug.LogError(ZString.Format("[Pxp_LogError] : <color='#{0}'>{1}</color>", colorStr, message));
        }

        #endregion

        #region LogException

        /// <summary>
        /// Debug.LogException
        /// </summary>
        /// <param name="e"></param>
        public static void LogException(Exception e)
        {
            UnityEngine.Debug.LogException(e);
            // Crashlytics.LogException(e);
        }

        /// <summary>
        /// Debug.LogException
        /// </summary>
        /// <param name="e"></param>
        /// <param name="context"></param>
        public static void LogException(Exception e, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogException(e, context);
            // Crashlytics.LogException(e);
        }

        #endregion

        #region Assert

        /// <summary>
        /// Debug.Assert
        /// </summary>
        /// <param name="condition"></param>
        public static void Assert(bool condition)
        {
            UnityEngine.Debug.Assert(condition);
        }

        /// <summary>
        /// Debug.Assert
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void Assert(bool condition, string message)
        {
            UnityEngine.Debug.Assert(condition, $"[Pxp_Assert] : {message}");
        }

        #endregion
    }
}
