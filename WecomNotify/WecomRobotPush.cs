using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using TaskScheduler.WecomNotify.Model;

namespace TaskScheduler.WecomNotify
{
    public class WecomRobotPush
    {
        private readonly HttpClient _client;
        public WecomRobotPush(HttpClient client)
        {
            _client = client;
            _client.BaseAddress ??= new Uri("https://qyapi.weixin.qq.com/");
        }

        public WecomRobotPush(IHttpClientFactory factory)
            : this(factory.CreateClient())
        {
        }

        public async Task SendMessage(string wecomRobotId, string message)
        {
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                msgtype = "text",
                text = new
                {
                    content = message,
                }
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"cgi-bin/webhook/send?key={wecomRobotId}", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task SendMessage(string wecomRobotId, string message, string[] mentionedList)
        {
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                msgtype = "text",
                text = new
                {
                    content = message,
                    mentioned_list = mentionedList
                }
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync($"cgi-bin/webhook/send?key={wecomRobotId}", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task SendFile(string wecomRobotId, string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                await SendFile(wecomRobotId, fileStream, Path.GetFileName(filePath));
            }
        }
        public async Task SendImage(string wecomRobotId, string imagePath)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("File not found", imagePath);

            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

            string md5Str;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(imageBytes);

                md5Str = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
            var base64Str = Convert.ToBase64String(imageBytes);
            var strContent = new StringContent(JsonSerializer.Serialize(new
            {
                msgtype = "image",
                image = new
                {
                    base64 = base64Str,
                    md5 = md5Str
                }
            }));
            await PostMessage($"https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key={wecomRobotId}", strContent);
        }
        public async Task SendFile(string wecomRobotId, Stream stream, string fileName)
        {
            var mediaId = await UploadFile(wecomRobotId, stream, fileName);
            var strContent = new StringContent(JsonSerializer.Serialize(new
            {
                msgtype = "file",
                file = new
                {
                    media_id = mediaId
                }
            }));
            await PostMessage($"https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key={wecomRobotId}", strContent);
        }
        private async Task PostMessage(string url, HttpContent requestContent, int tryCount = 0)
        {
            if (tryCount > 5)
            {
                throw new Exception($"<h3>Failed To Send Message after {tryCount} tries.</h3> <p>Url: {url}</p> <p>Content: {await requestContent.ReadAsStringAsync()}</p>");
            }
            var response = await _client.PostAsync(url, requestContent);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<WecomResponse>(content);
            if (res.ErrorCode == 0)
            {
                return;
            }
            else if (res.ErrorCode == 45011)
            {
                var rand = new Random();
                await Task.Delay(1000 * 60 + rand.Next(0, 60));
                await PostMessage(url, requestContent);
            }
            else if (res.ErrorCode == 45009)
            {
                throw new Exception($"Reach max api daily quota limit: {content}");
            }
            else
            {
                throw new Exception($"Unknown Exception: {content}");
            }
        }
        private async Task<string> UploadFile(string wecomRobotId, Stream stream, string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"cgi-bin/webhook/upload_media?key={wecomRobotId}&type=file");
            string boundary = Guid.NewGuid().ToString("N");
            var content = new MultipartFormDataContent(boundary);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse($"multipart/form-data;boundary={boundary}");
            var streamContent = new StreamContent(stream);
            content.Add(streamContent, "media", fileName);
            request.Content = content;
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            streamContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("filelength", stream.Length.ToString()));
            streamContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("name", $"\"media\""));
            streamContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("filename", $"\"{fileName}\""));
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<FileUploadResponse>(responseContent);
            return data.media_id;
        }

        public async Task SendMarkdownMessage(string wecomRobotId, string markdownContent)
        {
            var payload = new
            {
                msgtype = "markdown",
                markdown = new
                {
                    content = markdownContent
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key={wecomRobotId}", content);
            response.EnsureSuccessStatusCode();
        }

    }
}
