namespace sockets
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    class Program
    {
        static readonly string WebRoot = FindProjectRootContaining("webroot");

        static string FindProjectRootContaining(string folderName)
        {
            string current = Directory.GetCurrentDirectory();
            while (current != null)
            {
                string potential = Path.Combine(current, folderName);
                if (Directory.Exists(potential))
                {
                    return potential;
                }
                current = Directory.GetParent(current)?.FullName;
            }
            throw new DirectoryNotFoundException($"'{folderName}' directory not found in any parent folder.");
        }


        static readonly string LogFile = "server.log";
        static readonly int Port = 8080;

        static readonly Dictionary<string, string> MimeTypes = new()
        {
            [".html"] = "text/html",
            [".css"] = "text/css",
            [".js"] = "application/javascript"
        };

        static void Main()
        {
            TcpListener server = new TcpListener(IPAddress.Any, Port);
            server.Start();
            Console.WriteLine($"Server started on port {Port}...");


            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        static void HandleClient(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream);
            using StreamWriter writer = new(stream) { AutoFlush = true };

            try
            {
                string requestLine = reader.ReadLine();
                if (string.IsNullOrEmpty(requestLine)) return;

                LogRequest(requestLine);

                string[] tokens = requestLine.Split(' ');
                if (tokens.Length != 3)
                {
                    SendResponse(writer, "400 Bad Request", "text/html", "<h1>400 Bad Request</h1>");
                    return;
                }

                string method = tokens[0];
                string url = Uri.UnescapeDataString(tokens[1]);
                string filePath = Path.Combine(WebRoot, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (method != "GET")
                {
                    SendResponse(writer, "405 Method Not Allowed", "text/html", ErrorPage("405 Method Not Allowed"));
                    return;
                }

                if (!filePath.StartsWith(WebRoot))
                {
                    SendResponse(writer, "403 Forbidden", "text/html", ErrorPage("403 Forbidden"));
                    return;
                }

                string extension = Path.GetExtension(filePath).ToLower();
                if (!MimeTypes.ContainsKey(extension))
                {
                    SendResponse(writer, "403 Forbidden", "text/html", ErrorPage("403 Forbidden"));
                    return;
                }

                if (!File.Exists(filePath))
                {
                    SendResponse(writer, $"404 Not Found {filePath}", "text/html", ErrorPage($"404 Not Found"));
                    return;
                }

                byte[] content = File.ReadAllBytes(filePath);
                string mime = MimeTypes[extension];
                SendBinaryResponse(stream, "200 OK", mime, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        static void SendResponse(StreamWriter writer, string status, string contentType, string content)
        {
            writer.WriteLine($"HTTP/1.1 {status}");
            writer.WriteLine($"Content-Type: {contentType}");
            writer.WriteLine($"Content-Length: {Encoding.UTF8.GetByteCount(content)}");
            writer.WriteLine();
            writer.WriteLine(content);
        }

        static void SendBinaryResponse(NetworkStream stream, string status, string contentType, byte[] content)
        {
            StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);
            writer.WriteLine($"HTTP/1.1 {status}");
            writer.WriteLine($"Content-Type: {contentType}");
            writer.WriteLine($"Content-Length: {content.Length}");
            writer.WriteLine();
            writer.Flush();

            stream.Write(content, 0, content.Length);
        }

        static string ErrorPage(string title)
        {
            string errorPage = Path.Combine(WebRoot, "error.html");
            if (File.Exists(errorPage))
            {
                string template = File.ReadAllText(errorPage);
                return template.Replace("{{error}}", title);
            }
            return $"<html><body><h1>{title}</h1></body></html>";
        }

        static void LogRequest(string line)
        {
            File.AppendAllText(LogFile, $"{DateTime.Now} - {line}{Environment.NewLine}");
        }
    }

}
