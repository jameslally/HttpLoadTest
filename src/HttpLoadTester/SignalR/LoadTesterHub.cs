using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace HttpLoadTester.SignalR
{
    public class DashboardHub : Hub
    {
        private readonly string _sourceUrl;
        private readonly ServiceActions _statusService;

        public DashboardHub(IConfiguration configuration, ServiceActions statusService)
        {
            _sourceUrl = (string)configuration["HttpSourceLocation"];
            _statusService = statusService;
        }

        //public DashboardHub(IConfiguration configuration)
        //{
        //    _sourceUrl = (string)configuration["HttpSourceLocation"];

        //}

        public string LeaveGroup(string connectionId, string groupName)
        {
            Groups.Remove(connectionId, groupName).Wait();
            return connectionId + " removed from " + groupName;
        }

        public string sendStartToHub(string code)
        {
            _statusService.StartService(code);//("PFMUser");//
            return "";
        }

        public string sendStopToHub(string code)
        {
            _statusService.StopService(code);
            return "";
        }

        public int SendMessageCountToAll(int messageCount, int sleepTime)
        {
            if (sleepTime > 0)
            {
                Thread.Sleep(sleepTime);
            }

            Clients.All.displayMessagesCount(++messageCount, Context.ConnectionId).Wait();
            return messageCount;
        }

        public int SendMessageCountToGroup(int messageCount, string groupName, int sleepTime)
        {
            if (sleepTime > 0)
            {
                Thread.Sleep(sleepTime);
            }

            Clients.Group(groupName).displayMessagesCount(++messageCount, Context.ConnectionId).Wait();
            return messageCount;
        }


        public int SendMessageCountToCaller(int messageCount, int sleepTime)
        {
            if (sleepTime > 0)
            {
                Thread.Sleep(sleepTime);
            }

            Clients.Caller.displayMessagesCount(++messageCount, Context.ConnectionId).Wait();
            return messageCount;
        }
    }
}
