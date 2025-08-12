using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.WecomNotify
{
    public static class WecomRPAGate
    {
        private static readonly string _wecomApiUrl = "https://ros.reginamiracle.com:8118/wecomNotify/sendMessage";

        public static async Task SendMessage(string wecomRobotId, string message, string tag = "")
        {
            var payload = new { wecomRobotId, message, tag};
            using HttpClient httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(_wecomApiUrl, payload);
            response.EnsureSuccessStatusCode();
        }
    }
}
