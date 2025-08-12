using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.WecomNotify.Model
{
    public class FileUploadResponse
    {
        public int errCode { get; set; }
        public string errMsg { get; set; }
        public string type { get; set; }
        public string media_id { get; set; }
    }
}
