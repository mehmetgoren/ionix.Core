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

        public Logger Clear()
        {
            this.entity.ThreadId = Thread.CurrentThread.ManagedThreadId;
            this.entity.Code = null;
            this.entity.Message = null;
            this.entity.ObjJson = null;
            this.entity.HasError = null;
            this.entity.Elapsed = null;

            return this;
        }

        private readonly LogEntity entity;
        private Logger()
        {
            this.entity = new LogEntity();
        }

        internal Logger Type(string value)
        {
            this.entity.Type = value;
            return this;
        }

        internal Logger Method(string value)
        {
            this.entity.Method = value;
            return this;
        }

        internal Logger ThreadId(Thread thread)
        {
            this.entity.ThreadId = thread.ManagedThreadId;
            return this;
        }

        public Logger Code(int code)
        {
            this.entity.Code = code;
            return this;
        }

        public Logger Message(string value)
        {
            this.entity.Message = value;
            return this;
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

        public Logger HasError(bool value)
        {
            this.entity.HasError = value;
            return this;
        }

        public Logger Elapsed(long value)
        {
            this.entity.Elapsed = value;
            return this;
        }

        //Kısa yol.
        public Logger OnException(Exception ex)
        {
            if (null != ex)
            {
                this.Message(ex.FindRoot().Message)
                    .Object(ex)
                    .HasError(true);
            }
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
                    this.OnException(ex2);
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

                    this.Message(message)
                        .Elapsed(bench.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    this.OnException(ex);
                }
            }
            return this;
        }

        public Logger Bench(Action action)
        {
            return this.Bench(action, "Bench Result");
        }

        public void Save(bool saveByOtherThread = true)
        {
            if (saveByOtherThread)
            {
                this.SaveToDb((c, e) => Task.Run(() => c.Insert(e)));
            }
            else
            {
                this.SaveToDb((c, e) => c.Insert(e));
            }
        }

        public Task SaveAsync()
        {
            return this.SaveToDb((c, e) => c.InsertAsync(e));
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
            }
            catch (Exception ex)
            {
                try
                {
                    //olmadı text olarak imdat mesajı yazılsın.

                    string path = Assembly.GetExecutingAssembly().Location;

                    File.WriteAllText(path, ex.FindRoot().Message);

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
