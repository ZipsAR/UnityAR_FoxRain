using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace ZipsAR
{
    public static class Logger
    {
        // [Project Setting] - [ Other Setting ] - [Scripting Define Symbols]에 ENABLE_LOG 등록 필요함
        // 등록 안하는 경우, Log를 컴파일하지 않아 성능 저하를 방지할 수 있다
        public const string ENABLE_LOGS = "ENABLE_LOG";


        public static bool isDebugBuild
        {
            get { return UnityEngine.Debug.isDebugBuild; }
        }


        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void Log(object message)
        {
            // 이전 함수명 
            string prevFuncName = new StackFrame(1, true).GetMethod().Name;
            // 이전 Class명
            string prevClassName = new StackTrace().GetFrame(1).GetMethod().ReflectedType?.Name;
            Debug.Log($"ZipsAR-[{prevClassName} / {prevFuncName}] : {message}");
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void Log(object message, UnityEngine.Object context)
        {
            // 이전 함수명 
            string prevFuncName = new StackFrame(1, true).GetMethod().Name;
            // 이전 Class명
            string prevClassName = new StackTrace().GetFrame(1).GetMethod().ReflectedType?.Name;
            Debug.Log($"[{prevClassName} / {prevFuncName}] : {message}", context);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogFormat(string message, params object[] args)
        {
            Debug.LogFormat(message, args);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogFormat(UnityEngine.Object context, string message, params object[] args)
        {
            Debug.LogFormat(context, message, args);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogWarning(object message)
        {
            // 이전 함수명 
            string prevFuncName = new StackFrame(1, true).GetMethod().Name;
            // 이전 Class명
            string prevClassName = new StackTrace().GetFrame(1).GetMethod().ReflectedType?.Name;
            Debug.LogWarning($"[{prevClassName} / {prevFuncName}] : {message}");
            // Debug.LogWarning(message);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogWarning(object message, UnityEngine.Object context)
        {
            // 이전 함수명 
            string prevFuncName = new StackFrame(1, true).GetMethod().Name;
            // 이전 Class명
            string prevClassName = new StackTrace().GetFrame(1).GetMethod().ReflectedType?.Name;
            Debug.LogWarning($"[{prevClassName} / {prevFuncName}] : {message}", context);
            // Debug.LogWarning(message, context);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogWarningFormat(string message, params object[] args)
        {
            Debug.LogWarningFormat(message, args);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogWarningFormat(UnityEngine.Object context, string message, params object[] args)
        {
            Debug.LogWarningFormat(context, message, args);
        }



        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogError(object message)
        {
            // 이전 함수명 
            string prevFuncName = new StackFrame(1, true).GetMethod().Name;
            // 이전 Class명
            string prevClassName = new StackTrace().GetFrame(1).GetMethod().ReflectedType?.Name;
            Debug.LogError($"[{prevClassName} / {prevFuncName}] : {message}");
            // Debug.LogError(message);
        }



        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogError(object message, UnityEngine.Object context)
        {
            // 이전 함수명 
            string prevFuncName = new StackFrame(1, true).GetMethod().Name;
            // 이전 Class명
            string prevClassName = new StackTrace().GetFrame(1).GetMethod().ReflectedType?.Name;
            Debug.LogError($"[{prevClassName} / {prevFuncName}] : {message}", context);
            // Debug.LogError(message, context);
        }



        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogErrorFormat(string message, params object[] args)
        {
            Debug.LogErrorFormat(message, args);
        }


        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogErrorFormat(UnityEngine.Object context, string message, params object[] args)
        {
            Debug.LogErrorFormat(context, message, args);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogException(System.Exception exception)
        {
            Debug.LogException(exception);
        }

        [System.Diagnostics.Conditional(ENABLE_LOGS)]
        public static void LogException(System.Exception exception, UnityEngine.Object context)
        {
            Debug.LogException(exception, context);

        }
    }
}