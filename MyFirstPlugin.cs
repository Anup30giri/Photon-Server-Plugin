using Photon.Hive.Plugin;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyFirstPlugin
{
    public class MyFirstPlugin : PluginBase
    {
        public override string Name => "MyFirstPlugin";

        private IPluginLogger pluginLogger;

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            this.pluginLogger = host.CreateLogger(this.Name);
            return base.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            string eventData = "Hello from Photon Server ";
            this.pluginLogger.InfoFormat($"OnCreateGame {eventData} by user {1} Anup", eventData);
            info.Continue();
        }

        // Method for Sending Data to Client
        public void RaiseEvent(
        byte eventCode,
        object eventData,
        byte receiverGroup = ReciverGroup.All,
        int senderActorNumber = 0,
        byte cachingOption = CacheOperations.DoNotCache,
        byte interestGroup = 0,
        SendParameters sendParams = default(SendParameters))
        {
            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters.Add(245, eventData);
            parameters.Add(254, senderActorNumber);
            PluginHost.BroadcastEvent(receiverGroup, senderActorNumber, interestGroup, eventCode, parameters, cachingOption, sendParams);
        }

        // On Raise Event
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            base.OnRaiseEvent(info);
            this.pluginLogger.InfoFormat("onRaise Event Triggered");

            // Receiving Data from Client
            byte eventCode = info.Request.EvCode;
            if (eventCode == 13)
            {
                this.pluginLogger.Debug($"Data from event:{eventCode} with information {info.Request.Data}");
            }
             if (eventCode == 14)
            {
                this.pluginLogger.Debug($"Data from RPC {eventCode} with information {info.Request.Data}");
            }

            RaiseEvent(eventCode: 123, eventData: "Hello From Server");

            Dictionary<byte, object> row = new Dictionary<byte, object>() { { 55, "My Testing" }, { 254, 0 } };
            if (info.IsProcessed)
            {
                this.PluginHost.BroadcastEvent(
                recieverActors: new int[] { 1 },
                senderActor: 0,
                data: new Dictionary<byte, object>() { { 245, row } },
                evCode: 57,
                cacheOp: 0,
                sendParameters: new SendParameters() { Unreliable = false });
            }

            // Http Get Request
            var request = new HttpRequest
            {
                Url = "http://18.233.82.156/",
                Method = "GET",
                Accept = "application/json",
                ContentType = "application/json",
                Callback = OnHttpResponse,
                UserState = null,
                Async = true

            };

            PluginHost.HttpRequest(request,info);
        }

        private void OnHttpResponse(IHttpResponse response, object userState)
        {
            this.pluginLogger.Info($"ResponseData: {response.Status}");
            this.pluginLogger.Info($"ResponseData: {response.ResponseText}");
        }

    }
}
 