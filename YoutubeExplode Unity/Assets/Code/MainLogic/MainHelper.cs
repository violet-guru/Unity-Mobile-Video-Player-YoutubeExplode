using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace MainLogic
{
    public interface IBasicLog
    {
        bool HasExtras();
        void Log(string message, [CanBeNull] IDictionary<string, object> analyzeFields, string finalMessage);
    }

    public enum Hi
    {
        CurrentLanguage = 1,
        BigKernelSize = 2,
        ChangeMusicTrack = 3,
        FirstMusicTrack = 4,
        FinishTokensAnimation = 5,
        DuplexState = 6,
        PlayerScore = 7,
        WaitDoublePower = 8,
        LowSpeedExecution = 9,
        InitDuplex = 10,
        SolverBlockForPlayer = 11,
        SolverBackfire = 12,
        SolverBlocked = 13,
        NewSolver = 14,
        TestCountAzure = 15,
        SolverConveyor = 16,
        SolverThirdPartyBlocking = 17,
        SolverFinishUndefined = 18,
        SolverFinishLotLevels = 19,
        SolverWaitThinking = 20,
        SolverWaitSwitching = 21,
        SolverMovementIndex = 22,
        SolverCountdown = 23,
        SolverCheckLocking = 24,
        SolverBlockPlayer = 25,
        SolverCountdownTryAgain = 26,
        SolverCountdownFinish = 27,
        InitSolver = 28,
        VideoHtmlParser = 29,
        PurchaserInitSuccessfully = 30,
        PurchaserBuying = 31,
        PurchaserRestoreStarted = 32,
        SourceImageDisconnected = 33,
        RemoteTextsDisconnected = 34,
        LeaderBoardDisconnected = 35,
        PurchaserSuccessfull = 36,
        DeleteAllPrefs = 37,
        VideoMovieDisconnected = 38,
        VideoLinkInfo = 39,
        VideoLinkDisconnected = 40,
        StartMovieBase = 41,
        OrientationLandscapeLeft = 42,
        OrientationPortrait = 43,
        PurchaserOnInitializeFailed = 44,
        PurchaserProductNotFound = 45,
        PurchaserProductNotInit = 46,
        PurchaserReceiptValid = 47,
        PurchaserInvalidReceiptData = 48,
        PurchaserInvalidReceiptSecurity = 49,
        PurchaserOnPurchaseFailed = 50,
        PurchaserRestoreNotInit = 51,
        PurchaserRestoreNotSupported = 52,
        PurchaserOnTransactionsRestored = 53,
        AutoTestIndex = 54,
        AutoTestNumberFinished = 55,
        AutoTestNumber = 56,
        IsFrenzyFirstMove = 57,
        StartTokensAnimation = 58,
        ScoreFieldInvalid = 59,
        RemoteAccessLink = 60,
        HaltTokens = 61,
        TokenSwitching = 62,
        PowerDuplexStateFirst = 63,
        DuplexStart = 64,
        PositionTokensAndScore = 65,
        SaveButtonClick = 66,
        TestTokensToConsole = 67,
        TestsInit = 68,
        BuyProgressButtonClick = 69
    }

    public class SuperLog
    {
        private readonly List<List<MethodLog>> _logList = new List<List<MethodLog>>();
        private readonly bool _runLogs;
        private readonly IBasicLog _basicLog;

        public SuperLog(IBasicLog basicLog, bool runLogs)
        {
            _basicLog = basicLog;
            _logList.Add(new List<MethodLog>());
            _runLogs = runLogs;
        }

        private List<MethodLog> LastMethodLog => _logList[_logList.Count - 1];

        public void Send(bool isEnabled, Hi logEvent, bool isRed, int logExtra)
        {
            SendBase(isEnabled, logEvent, isRed, logExtra);
        }

        public void Send(bool isEnabled, Hi logEvent, bool isRed = false, string logExtra = null)
        {
            SendBase(isEnabled, logEvent, isRed, logExtra == "" ? null : logExtra);
        }

        private void SendBase<T>(bool isEnabled, Hi logEvent, bool isRed, T logExtra)
        {
            Dictionary<string, object> analyzeFields = null;
            if (logExtra != null)
            {
                analyzeFields = new Dictionary<string, object> { { "extra", logExtra } };
            }

            SendAnalyze(isEnabled, logEvent, isRed, analyzeFields);
        }

        public void FakeSend(string logEvent)
        {

        }

        public void SendAnalyze(bool isEnabled, Hi logEvent, bool isRed = false, IDictionary<string, object> analyzeFields = null)
        {
            if (!isEnabled)
            {
                return;
            }

            var dateNow = DateTime.Now.ToUniversalTime();

            var prefix = "";
            var suffix = "";
            if (isRed)
            {
                prefix = "<color=red>";
                suffix = "</color>";
            }

            var extraValue = _basicLog.HasExtras() ? prefix + "$$" + dateNow.ToString("u") + " " : "";
            var suffixValue = _basicLog.HasExtras() ? suffix : "";

            var textMessage = extraValue + Enum.GetName(typeof(Hi), logEvent) + suffixValue;
            _basicLog.Log(textMessage, analyzeFields, FinalMessage(textMessage, analyzeFields));
        }

        public MethodLog Add(string methodName, string childName = "")
        {
            if (!_runLogs)
            {
                return new MethodLog();
            }

            var returned = new MethodLog(methodName, childName);
            _logList[_logList.Count - 1].Add(returned);
            return returned;
        }

        public void WarningFinal(int warningMilliseconds)
        {
            if (!_runLogs)
            {
                return;
            }

            var mainMethodLog = LastMethodLog[0];
            if (mainMethodLog.MethodMilliseconds > warningMilliseconds)
            {
                var debugText = LastMethodLog.ConvertAll(t => t.ToString());
                Send(true, Hi.LowSpeedExecution, false, string.Join(Environment.NewLine, debugText.ToArray()));
            }

            _logList.Add(new List<MethodLog>());

            const int maxIndex = 20;
            if (_logList.Count > maxIndex)
            {
                _logList.RemoveAt(0);
            }
        }

        private string FinalMessage(string textMessage, [CanBeNull] IDictionary<string, object> analyzeFields)
        {
            var resultFields = new List<string>();
            if (analyzeFields != null)
            {
                foreach (var item in analyzeFields)
                {
                    var itemKey = "";
                    if (item.Key != "extra")
                    {
                        itemKey = item.Key + ": ";
                    }

                    resultFields.Add(itemKey + item.Value);
                }
            }

            var finalMessage = textMessage + (analyzeFields == null ? "" : "~ " + string.Join("| ", resultFields.ToArray()));
            return finalMessage;
        }
    }

    public class MethodLog
    {
        private readonly string _methodName;
        private readonly string _childName;
        private readonly DateTime _methodStart;
        private DateTime? _methodEnd;
        private readonly bool _runLogs;

        public MethodLog()
        {
        }

        public MethodLog(string methodName, string childName)
        {
            _methodName = methodName;
            _childName = childName;
            _methodStart = DateTime.Now;
            _runLogs = true;
        }

        public double MethodMilliseconds
        {
            get
            {
                //Comprobar que los items de la lista tengan MethodEnd
                if (!_methodEnd.HasValue)
                {
                    throw new Exception("$$ No MethodEnd in " + CompleteName);
                }

                return (_methodEnd.Value - _methodStart).TotalMilliseconds;
            }
        }

        private string CompleteName => _methodName + "," + _childName;

        public void Close()
        {
            if (!_runLogs)
            {
                return;
            }

            _methodEnd = DateTime.Now;
        }

        public override string ToString()
        {
            return CompleteName + "  " + MethodMilliseconds;
        }
    }

}