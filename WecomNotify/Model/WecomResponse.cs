using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TaskScheduler.WecomNotify.Model
{
    public class WecomResponse
    {
        [JsonPropertyName("errcode")]
        public int ErrorCode { get; set; }
        [JsonPropertyName("errmsg")]
        public string ErrorMessage { get; set; }
    }
}
