using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Microsoft.MergenModules
{
    public class MergenLogger : IHttpModule
    {
        public void Dispose()
        {
        }
        public void Init(HttpApplication context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }
        private void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                HttpApplication app = sender as HttpApplication;
                OutputFilterStream filter = new OutputFilterStream(app.Response.Filter);
                app.Response.Filter = filter;
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    string innerMessage = (ex.InnerException != null)
                                          ? ex.InnerException.Message
                                          : "";
                    string message = ex.Message + Environment.NewLine + Environment.NewLine + innerMessage;
                    if (message.Length > 32766)
                    {
                        message = message.Substring(0, 32766);
                    }
                    eventLog.Source = "ASP.NET 4.0.30319.0";
                    eventLog.WriteEntry(message, EventLogEntryType.Error, 8875, 3);
                }
            }

        }
        private void context_EndRequest(Object sender, EventArgs e)
        {
            try
            {
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                Serializer ser = new Serializer();
                string xmlInputData = File.ReadAllText(path + "\\bin\\mergen.xml");
                Mergen mergenxml = ser.Deserialize<Mergen>(xmlInputData);
                HttpRequest appR = HttpContext.Current.Request;
                bool systemcheck = false;
                string response = "";
                string request = "";
                if (userName == @"NT AUTHORITY\SYSTEM")
                {
                    systemcheck = true;
                }
                foreach (var listen in mergenxml.Listener.Url)
                {
                    if ((listen.Method == "All" || listen.Method == "" || listen.Method == null) || listen.Method == appR.HttpMethod)
                    {
                        if (listen.Type == "IP")
                        {
                            var forwardedFor = appR.ServerVariables["HTTP_X_FORWARDED_FOR"];

                            var userIpAddress = String.IsNullOrWhiteSpace(forwardedFor) ?
                                appR.ServerVariables["REMOTE_ADDR"] : forwardedFor.Split(',').Select(s => s.Trim()).FirstOrDefault();

                            if (appR.UserHostAddress == listen.Text || userIpAddress == listen.Text)
                            {
                                request = request + "IP :" + appR.UserHostAddress;
                                request = request + Environment.NewLine;
                                request = request + appR.HttpMethod + " " + appR.RawUrl;
                                request = request + Environment.NewLine;
                                foreach (string key in appR.Headers.Keys)
                                {
                                    request = request + key;
                                    request = request + ": ";
                                    request = request + appR.Headers[key];
                                    request = request + Environment.NewLine;
                                }
                                byte[] bytes = appR.BinaryRead(appR.ContentLength);
                                if (bytes.Count() > 0)
                                {
                                    request = request + Encoding.ASCII.GetString(bytes);
                                }
                                if (listen.Response != "false")
                                {
                                    request = request + Environment.NewLine + Environment.NewLine;
                                    HttpResponse app = HttpContext.Current.Response;
                                    response = response + "Response Code: " + app.StatusCode.ToString();
                                    response = response + Environment.NewLine;
                                    foreach (string key in app.Headers.Keys)
                                    {
                                        response = response + key;
                                        response = response + ": ";
                                        response = response + app.Headers[key];
                                        response = response + Environment.NewLine;
                                    }
                                    response = response + ((OutputFilterStream)app.Filter).ReadStream();
                                    request = request + response;
                                }

                            }
                        }
                        else if (listen.Type == "Path")
                        {
                            if (appR.RawUrl.Contains(listen.Text))
                            {
                                request = request + "IP :" + appR.UserHostAddress;
                                request = request + Environment.NewLine;
                                request = request + appR.HttpMethod + " " + appR.RawUrl;
                                request = request + Environment.NewLine;
                                foreach (string key in appR.Headers.Keys)
                                {
                                    request = request + key;
                                    request = request + ": ";
                                    request = request + appR.Headers[key];
                                    request = request + Environment.NewLine;
                                }
                                byte[] bytes = appR.BinaryRead(appR.ContentLength);
                                if (bytes.Count() > 0)
                                {
                                    request = request + Encoding.ASCII.GetString(bytes);
                                }
                                if (listen.Response != "false")
                                {
                                    request = request + Environment.NewLine + Environment.NewLine;

                                    HttpResponse app = HttpContext.Current.Response;

                                    response = response + "Response Code: " + app.StatusCode.ToString();
                                    response = response + Environment.NewLine;
                                    foreach (string key in app.Headers.Keys)
                                    {
                                        response = response + key;
                                        response = response + ": ";
                                        response = response + app.Headers[key];
                                        response = response + Environment.NewLine;
                                    }
                                    response = response + ((OutputFilterStream)app.Filter).ReadStream();
                                    request = request + response;
                                }
                            }
                        }
                        else if (listen.Type == "All")
                        {
                            request = request + "IP :" + appR.UserHostAddress;
                            request = request + Environment.NewLine;
                            request = request + appR.HttpMethod + " " + appR.RawUrl;
                            request = request + Environment.NewLine;
                            foreach (string key in appR.Headers.Keys)
                            {
                                request = request + key;
                                request = request + ": ";
                                request = request + appR.Headers[key];
                                request = request + Environment.NewLine;
                            }
                            byte[] bytes = appR.BinaryRead(appR.ContentLength);
                            if (bytes.Count() > 0)
                            {
                                request = request + Encoding.ASCII.GetString(bytes);
                            }
                            if (listen.Response != "false")
                            {
                                request = request + Environment.NewLine + Environment.NewLine;
                                HttpResponse app = HttpContext.Current.Response;
                                response = response + "Response Code: " + app.StatusCode.ToString();
                                response = response + Environment.NewLine;
                                foreach (string key in app.Headers.Keys)
                                {
                                    response = response + key;
                                    response = response + ": ";
                                    response = response + app.Headers[key];
                                    response = response + Environment.NewLine;
                                }
                                response = response + Environment.NewLine;
                                response = response + ((OutputFilterStream)app.Filter).ReadStream();
                                request = request + response;
                            }
                        }
                    }
                    if (request != "")
                    {
                        if (request.Length > 32766)
                        {
                            //Event log max characters
                            request = request.Substring(0, 32766);
                        }
                        if (systemcheck)
                        {
                            string source = "IIS";
                            string log = "Microsoft-IIS-MergenLog";
                            if (!EventLog.SourceExists(source))
                            {
                                EventLog.CreateEventSource(source, log);
                                SetCustomLogMaxSize(log, 500000000);

                            }
                            EventLog demoLog = new EventLog(log);
                            demoLog.Source = source;
                            demoLog.WriteEntry(request, EventLogEntryType.Information, 8875);
                        }
                        else
                        {
                            using (EventLog eventLog = new EventLog("Application"))
                            {
                                eventLog.Source = "ASP.NET 4.0.30319.0";
                                eventLog.WriteEntry(request, EventLogEntryType.Information, 8875, 3);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    string innerMessage = (ex.InnerException != null)
                      ? ex.InnerException.Message
                      : "";
                    string message = ex.Message + Environment.NewLine + Environment.NewLine + innerMessage;
                    if (message.Length > 32766)
                    {
                        //Event log max characters
                        message = message.Substring(0, 32766);
                    }
                    eventLog.Source = "ASP.NET 4.0.30319.0";
                    eventLog.WriteEntry(message, EventLogEntryType.Error, 8875, 3);
                }
            }

        }
        public static void SetCustomLogMaxSize(string logName, int maxSize)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey
              (@"SYSTEM\CurrentControlSet\Services\Eventlog\" + logName, true);
            if (key == null)
            {
                Console.WriteLine(
                  "Registry key for this Event Log does not exist.");
            }
            else
            {
                key.SetValue("MaxSize", maxSize);
                Registry.LocalMachine.Close();
            }
        }
    }

    public class Serializer
    {
        public T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }
        public string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
    [XmlRoot(ElementName = "url")]
    public class Url
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "response")]
        public string Response { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "method")]
        public string Method { get; set; }
    }
    [XmlRoot(ElementName = "listener")]
    public class Listener
    {
        [XmlElement(ElementName = "url")]
        public List<Url> Url { get; set; }
    }
    [XmlRoot(ElementName = "mergen")]
    public class Mergen
    {
        [XmlElement(ElementName = "listener")]
        public Listener Listener { get; set; }
    }
}
