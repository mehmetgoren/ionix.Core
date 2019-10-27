namespace Ionix.Rest
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Collections.Generic;
    using Utils;

    //Bu Kısım Cache lenecek bir dictionary için bir sözleşme. RESTful da kullanılmıyOR. Ancak Kullanacak Kısımlar için eklendi.
    public interface ITokenTable
    {
        Guid? Login(Credentials credentials);
        bool Logout(Guid token);

        bool TryAuthenticateToken(Guid token, out User user);//RDBMS, Redis or .Net Dictionary
    }


    internal static class TokenTableSyncRoot
    {
        internal static readonly object SyncRootContainsToken = new object();
        internal static readonly object SyncRootAddToken = new object();
        internal static readonly object SyncRootRemove = new object();
        internal static readonly object SyncRootGetEnumerator = new object();
    }

    public enum AuthMode
    {
        Level0,
        Level1
    }

    public abstract class TokenTable<TDerived> : Singleton, ITokenTable
            where TDerived : TokenTable<TDerived>, new()
    {
        private sealed class TokenCache : IEnumerable<UserGuid>
        {
            private readonly UserGuidDic dic = new UserGuidDic();

            private readonly TokenTable<TDerived> parent;

            internal TokenCache(TokenTable<TDerived> parent)
            {
                this.parent = parent;
            }

            internal User ContainsToken(Guid token)
            {
                lock (TokenTableSyncRoot.SyncRootContainsToken)
                {
                    if (this.dic.TryGetByGuid(token, out User user))
                    {
                        if (this.parent.IsTimeOut(user.LastLoginTime))//timeout oldu ise logout yap.
                        {
                            this.Remove(user);
                            return null;
                        }
                        //Eğer Timeout olmamış ise logindate' i tıpkı session yapısı gibi yenile.
                        user.LastLoginTime = DateTime.Now;
                        return user;
                    }
                    return null;
                }
            }

            //Farklı browserlardan girmemesi içim gerekli kod
            internal Guid? AddToken0(User user)
            {
                Guid? ret = null;
                if (User.IsValid(user))
                {
                    lock (TokenTableSyncRoot.SyncRootAddToken)
                    {
                        Guid token = Guid.NewGuid();

                        Guid oldToken;
                        if (this.dic.TryGetByUser(user, out oldToken))//Önceden var ise ve yeniden login oldu ise.
                        {
                            this.Remove(user);
                        }

                        this.dic.Add(user, token);
                        ret = token;
                    }
                }
                return ret;
            }

            internal Guid? AddToken1(User user)
            {
                Guid? ret = null;
                if (User.IsValid(user))
                {
                    lock (TokenTableSyncRoot.SyncRootAddToken)
                    {
                        Guid oldToken;
                        if (this.dic.TryGetByUser(user, out oldToken)) //Önceden var ise ve yeniden login oldu ise.
                        {
                            ret = oldToken;
                            user.LastLoginTime = DateTime.Now;
                            // this.Remove(user);
                        }
                        else
                        {
                            Guid newToken = Guid.NewGuid();

                            this.dic.Add(user, newToken);
                            ret = newToken;
                        }
                    }
                }
                return ret;
            }

            internal void Remove(User user)
            {
                lock (TokenTableSyncRoot.SyncRootRemove)
                {
                    this.dic.RemoveByUser(user);
                }
            }

            public IEnumerator<UserGuid> GetEnumerator()
            {
                lock (TokenTableSyncRoot.SyncRootGetEnumerator)
                {
                    return this.dic.GetEnumerator();
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            //added at 06.03.2017
            internal int UserGuidDicCount => this.dic.Count;
            //
        }

        //will be Readed Form Config File
        private readonly bool timerEnabled;
        public bool TimerEnabled => this.timerEnabled;

        private readonly TimeSpan timeOut;
        public TimeSpan TimeOut => this.timeOut;

        private readonly Timer timer;
        public Timer Timer => this.timer;
        //

        private Func<User, Guid?> fAddToken; 
        private AuthMode mode;
        public AuthMode Mode
        {
            get { return this.mode; }
            set
            {
                switch (value)
                {
                    case AuthMode.Level0:
                        this.fAddToken = this.cache.AddToken0;
                        break;
                    case AuthMode.Level1:
                        this.fAddToken = this.cache.AddToken1;
                        break;
                    default:
                        throw new NotSupportedException(value.ToString());
                }
                this.mode = value;
            }
        }

        private readonly TokenCache cache;

        protected TokenTable()
        {
            this.cache = new TokenCache(this);
            this.Mode = AuthMode.Level0;

            this.timerEnabled = true;
            this.timeOut = TimeSpan.FromMinutes(TokenTableParams.SessionTimeout); //TimeSpan.FromSeconds(15);
            if (this.timerEnabled)
            {
                this.timer = new Timer(this.TimerCallback, null, this.timeOut, this.timeOut);
            }
        }

        //Yoğun testler Gerekli.
        private void TimerCallback(object o)
        {
            lock (this)
            {
                ConcurrentBag<UserGuid> tokenInfoList = new ConcurrentBag<UserGuid>(this.cache);
                foreach (var tokenInfo in tokenInfoList)
                {
                    if (this.IsTimeOut(tokenInfo.User.LastLoginTime))//if it expires
                    {
                        this.cache.Remove(tokenInfo.User);
                    }
                }   
            }
        }

        private bool IsTimeOut(DateTime loginTime)
        {
            return ((DateTime.Now - loginTime) > this.timeOut);
        }

        #region   |    ITokenTable    |

        public bool TryAuthenticateToken(Guid token, out User user)
        {
            user = this.cache.ContainsToken(token);
            return null != user;
        }

        //Artık DB de nasıl tutulursa tutulsun bu iki alan olacağından Bana 'Credentials' Nesnesi Döndürülecek.
        protected abstract User GetUserByCredentials(Credentials credentials);

        private static bool CheckCredentials(Credentials credentials)
        {
            return null != credentials && !String.IsNullOrWhiteSpace(credentials.Username) && !String.IsNullOrWhiteSpace(credentials.Password);
        }

        public Guid? Login(Credentials credentials)
        {
            if (CheckCredentials(credentials))
            {
                var user = this.GetUserByCredentials(credentials);
                if (null != user)
                {
                    if (user.Password == credentials.Password)
                    {
                        user.LastLoginTime = DateTime.Now;
                        //return this.cache.AddToken(user);
                        return this.fAddToken(user);
                    }

                    this.cache.Remove(user);//if reentered password is incorrect then singout.
                }
            }
            return null;
        }

        public bool Logout(Guid token)
        {
            if (token != default(Guid))
            {
                User user = this.cache.ContainsToken(token);
                if (null != user)
                {
                    this.cache.Remove(user);//if reentered password is incorrect then singout.
                    return true;
                }
            }

            return false;
        }

        #endregion


        //added at 06.03.2017
        public int ActiveUserCount => this.cache.UserGuidDicCount;
        //

        //added at 21.03.2017 to show 
        public IEnumerable<User> GetCurrentUserList()
        {
            List<User> ret = new List<User>(this.ActiveUserCount);
            foreach (UserGuid userGuid in this.cache)
            {
                  ret.Add(userGuid.User);  
            }
            return ret;
        }
        //

        public static readonly TDerived Instance = new TDerived();
    }

    //added at 07.03.2017
    public static class TokenTableParams
    {
        public static double SessionTimeout { get; set; } = 30.0;
    }
}
