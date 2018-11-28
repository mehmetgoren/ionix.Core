namespace SQLog
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.IO;
    using ionix.Data;
    using ionix.Utils.Extensions;
    using Microsoft.Extensions.Configuration;

    //Tamamen Exception Safety olmalı.
    public class Logger
    {
        public static Logger Create(StackTrace stackTrace)
        {
            Logger ret = new Logger();
            ret.ThreadId(Thread.CurrentThread);
            if (null != stackTrace)
            {
                ret.SetReflectedValues(stackTrace);
            }
            return ret;
        }

        /// <summary>
        /// if you use anonymous methods, chose this methos instead.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Logger Create(string type, string method)
        { 
            return new Logger().ThreadId(Thread.CurrentThread).Type(type??"").Method(method??"");
        }

        private static readonly Lazy<IConfigurationRoot> _appSettings = new Lazy<IConfigurationRoot>(() =>
        {
            try
            {
                return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            }
            catch
            {
                return null;
            }

        }, true);
        internal static IConfigurationRoot AppSettings => _appSettings.Value;


        private static readonly object syncEnable = new object();
        private static bool? _enable;
        public static bool Enable {
            get
            {
                if (!_enable.HasValue)
                {
                    lock(syncEnable)
                    {
                        if (!_enable.HasValue)
                        {
                            bool value = true;
                            var appSettings = AppSettings;
                            if (null != appSettings)
                            {
                                string enableStr = appSettings["SQLog:Enable"];
                                if (!String.IsNullOrEmpty(enableStr))
                                {
                                   Boolean.TryParse(enableStr, out value);
                                }
                            }

                            _enable = value;
                        }
                    }
                }

                return _enable.Value;
            }
            set { _enable = value;  }
        }

        public Logger Clear()
        {
            this.entity.ThreadId = Thread.CurrentThread.ManagedThreadId;
            this.entity.Clear();

            return this;
        }

        private readonly LogEntity entity;
        private Logger()
        {
            this.entity = new LogEntity();
        }

        private Logger Type(string value)
        {
            this.entity.Type = value;
            return this;
        }

        private Logger Method(string value)
        {
            this.entity.Method = value;
            return this;
        }

        private Logger ThreadId(Thread thread)
        {
            this.entity.ThreadId = thread.ManagedThreadId;
            return this;
        }

        public Logger Code(int code)
        {
            this.entity.Code = code;
            return this;
        }

        public Logger Info(string value)
        {
            this.entity.LogType = nameof(Info);
            this.entity.Message = value;
            return this;
        }

        public Logger Warning(string value)
        {
            this.entity.LogType = nameof(Warning);
            this.entity.Message = value;
            return this;
        }

        public Logger Error(string value)
        {
            this.entity.LogType = nameof(Error);
            this.entity.Message = value;
            return this;
        }

        public Logger Error(Exception ex)
        {
            if (null != ex)
            {
                this.Error(ex.FindRoot().Message)
                    .Object(ex);
            }
            return this;
        }

        public Logger Trace(string name, object obj)
        {
            this.entity.LogType = nameof(Trace);
            this.entity.Message = name;
            return this.Object(obj);
        }

        public Logger Object(object obj)
        {
            if (null != obj)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(obj);
                    this.entity.ObjJson = json;
                }
                catch { }
            }
            return this;
        }


        public Logger Elapsed(long value)
        {
            this.entity.Elapsed = value;
            return this;
        }



        //Exception Çıkabilacek Durumlarda loglama için.
        public Logger Check(Action action, out Exception ex)
        {
            ex = null;
            if (null != action)
            {
                try
                {
                    action();
                }
                catch (Exception ex2)
                {
                    ex = ex2;
                    this.Error(ex2);
                }
            }
            return this;
        }

        public Logger Check(Action action)
        {
            Exception ex;
            return this.Check(action, out ex);
        }

        public Logger Bench(Action action, string message)
        {
            if (null != action)
            {
                try
                {
                    Stopwatch bench = Stopwatch.StartNew();
                    action();
                    bench.Stop();

                    this.Info(message)
                        .Elapsed(bench.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                }
            }
            return this;
        }

        public Logger Bench(Action action)
        {
            return this.Bench(action, "Bench Result");
        }


        private Action<string> _action;
        private void ExecuteActionSafely(string msg)
        {
            try
            {
                this._action?.Invoke(msg);
            }
            catch { }

        }

        /// <summary>
        /// if you need to do something with the message text after save operation complete. i.e: to write message to Console
        /// </summary>
        /// <param name="action">it gives you the message text</param>
        /// <returns></returns>
        public Logger OnSaveCompleted(Action<string> action)
        {
            this._action = action;

            return this;
        }

        public void Save()
        {
            if (Enable)
              this.SaveToDb((c, e) => c.Insert(e));
        }

        public Task SaveAsync()
        {
            return Enable ? this.SaveToDb((c, e) => c.InsertAsync(e)) : Task.FromResult(0);
        }

        private T SaveToDb<T>(Func<ICommandAdapter, LogEntity, T> func)
        {
            T ret = default(T);
            LogEntity l = this.entity.Copy();
            l.OpDate = DateTime.Now;

            DbClient c = null;
            try
            {
                c = ionixFactory.CreateDbClient();

                ret = func(c.Cmd, l);
                // c.Cmd.Insert(l);
                this.ExecuteActionSafely(l.Message);
            }
            catch (Exception ex)
            {
                try
                {
                    //olmadı text olarak imdat mesajı yazılsın.

                    string path = Assembly.GetExecutingAssembly().Location + "\\SQLog_Critical.txt";

                    string msg = ex.FindRoot().Message;
                    this.ExecuteActionSafely(l.Message);
                    File.WriteAllText(path, msg);            

                    //string sSource = "SQLog";
                    //string sLog = "Application";


                    //if (!EventLog.SourceExists(sSource))
                    //    EventLog.CreateEventSource(sSource, sLog);

                    //EventLog.WriteEntry(sSource, ex.Message);
                    //EventLog.WriteEntry(sSource, ex.Message,
                    //    EventLogEntryType.Error, 1983);
                }
                catch (Exception)
                {

                }
            }
            finally
            {
                if (null != c)
                {
                    try
                    {
                        c.Dispose();
                    }
                    catch (Exception) { }
                }
            }

            return ret;
        }


        private void SetReflectedValues(StackTrace stackTrace)
        {
            if (null != stackTrace)
            {
                try
                {
                    StackFrame sf = stackTrace.GetFrames()?.FirstOrDefault();
                    if (null != sf)
                    {
                        var mi = sf.GetMethod();
                        this.Type(mi.ReflectedType.Name).Method(mi.Name);
                    }
                }
                catch
                {
                }
            }
        }


        public static class Logs
        {
            public static IList<dynamic> Query(SqlQuery query)
            {
                if (null != query)
                {
                    using (var c = ionixFactory.CreateDbClient())
                    {
                        return c.Cmd.Query<dynamic>(query);
                    }
                }

                return  new List<dynamic>();
            }
        }
    }
}
