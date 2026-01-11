using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.Exceptions;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Debugging;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Web
{
    [NodeTint("#2b5c4b")]
    public abstract class BaseWebRequestNode : AsyncNode
    {
        [Input(ShowBackingValue.Always), SerializeField] private string _url;

        [Output, SerializeField] private long _responseCode;
        [Output, SerializeField] private string _error;
        [Output, SerializeField] private NodeException _nodeException;

        [Output, SerializeField] private NodePort _onSuccess;
        [Output, SerializeField] private NodePort _onFailure;

        public string Url
        {
            get
            {
                var input = GetInputValue<string>(nameof(_url));
                if (string.IsNullOrEmpty(input)) return _url;
                return input;
            }
        }
        public NodePort OnSuccess => GetOutputPort(nameof(_onSuccess));
        public NodePort OnFailure => GetOutputPort(nameof(_onFailure));

        public override void Execute()
        {
            base.Execute();
            SendRequestAsync(TokenSource.Token).Forget();
        }

        protected async UniTaskVoid SendRequestAsync(CancellationToken token)
        {
            string targetUrl = Url?.Trim();
            Stopwatch sw = Stopwatch.StartNew();

            if (string.IsNullOrEmpty(targetUrl))
            {
                HandleError("URL is null or empty");
                return;
            }

            using (UnityWebRequest request = CreateRequest(targetUrl))
            {
                try
                {
                    await request.SendWebRequest().WithCancellation(token);

                    sw.Stop();
                    _responseCode = request.responseCode;

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        LogSuccess(request, sw.ElapsedMilliseconds);
                        OnRequestSuccess(request);
                        ExecuteNext(OnSuccess);
                    }
                    else
                    {
                        HandleError($"{request.error} (Code: {_responseCode})", sw.ElapsedMilliseconds);
                    }
                }
                catch (OperationCanceledException)
                {
                    NovelGameDebug.LogWarning($"[WebRequest] Cancelled: {targetUrl}");
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message, sw.ElapsedMilliseconds);
                }
                finally
                {
                    StopTask();
                }
            }
        }

        private void LogSuccess(UnityWebRequest request, long elapsedMs)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<b>[WebRequest Success]</b>");
            sb.AppendLine($"Node: {name} ({GUID})");
            sb.AppendLine($"URL: {request.url}");
            sb.AppendLine($"Method: {request.method}");
            sb.AppendLine($"Response Code: {request.responseCode}");
            sb.AppendLine($"Time: {elapsedMs} ms");
            sb.AppendLine($"Size: {FormatBytes(request.downloadedBytes)}");

            NovelGameDebug.Log(sb.ToString());
        }

        private void HandleError(string errorMessage, long elapsedMs = 0)
        {
            _error = errorMessage;
            _nodeException = new NodeException(_error, this);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<b>[WebRequest Error]</b>");
            sb.AppendLine($"Node: {name} ({GUID})");
            sb.AppendLine($"Error: {_error}");
            if (elapsedMs > 0) sb.AppendLine($"Time: {elapsedMs} ms");

            NovelGameDebug.LogError(sb.ToString());

            OnRequestFailed();
            ExecuteNext(OnFailure);
        }

        private string FormatBytes(ulong bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{(bytes / 1024f):F2} KB";
            return $"{(bytes / (1024f * 1024f)):F2} MB";
        }

        protected abstract UnityWebRequest CreateRequest(string targetUrl);
        protected abstract void OnRequestSuccess(UnityWebRequest request);
        protected virtual void OnRequestFailed() { }

        protected void ExecuteNext(NodePort port)
        {
            if (port?.Connection?.node is BaseNode nextNode)
            {
                nextNode.Execute();
            }
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_responseCode)) return _responseCode;
            if (port.fieldName == nameof(_error)) return _error;
            if (port.fieldName == nameof(_nodeException)) return _nodeException;
            return null;
        }
    }
}