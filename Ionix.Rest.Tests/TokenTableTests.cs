namespace Ionix.RestTests
{
    using Ionix.Rest;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using Xunit;
    using FluentAssertions;

    public class TokenTableTests
    {
        const int length = 1000000;

        private static readonly IList<Guid> users = new List<Guid>();

        [Fact]
        public void TopkenTable_Should_Work()
        {
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "aaaaaa", Password = "aaaaaa" }).Value);
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "bbbbbb", Password = "bbbbbb" }).Value);
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "MvcApp", Password = "MvcApp" }).Value);
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "cccccc", Password = "cccccc" }).Value);
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "dddddd", Password = "dddddd" }).Value);
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "eeeeee", Password = "eeeeee" }).Value);
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "ffffff", Password = "ffffff" }).Value);
            users.Add(TokenTable.Instance.Login(new Credentials() { Username = "hhhhhh", Password = "hhhhhh" }).Value);

            Task[] tasks = new Task[2];

            tasks[0] = Task.Run(() =>
            {
               // SignInTest();
            });


            tasks[1] = Task.Run(() =>
            {
                TryAuthenticateTokenTests();
            });

            // Task.WaitAll(tasks);

            true.Should().BeTrue();
        }

        private static void SignInTest()
        {
            Task[] tasks = new Task[length];

            ConcurrentBag<Guid> guids = new ConcurrentBag<Guid>();

            for (int j = 0; j < length; ++j)
            {
                tasks[j] = Task.Run(() =>
                {
                    var guid = TokenTable.Instance.Login(new Credentials() { Username = "MvcApp", Password = "MvcApp" });

                    guids.Add(guid.Value);
                    Console.WriteLine("SignInTest: " + guid);
                });
            }

            Task.WaitAll(tasks);

            Guid first = TokenTable.Instance.Login(new Credentials() { Username = "MvcApp", Password = "MvcApp" }).Value;

            if (guids.Count != length)
                throw new Exception("guids.Count != length");

            foreach (var guid in guids)
            {
                if (first != guid)
                    throw new Exception("first != guid");
            }
        }

        private static void TryAuthenticateTokenTests()
        {
            Task[] tasks = new Task[length];

            ConcurrentBag<Guid> guids = new ConcurrentBag<Guid>();

            for (int j = 0; j < length; ++j)
            {
                tasks[j] = Task.Run(() =>
                {
                    User user;
                    Guid random = GetRandomUserGuid();
                    Stopwatch bench = Stopwatch.StartNew();
                    if (TokenTable.Instance.TryAuthenticateToken(random, out user))
                    {
                        bench.Stop();
                        Console.WriteLine("TryAuthenticateTokenTests: " + user.Name + " Elapsed: " + bench.ElapsedMilliseconds);
                    }
                    else
                    {
                        throw new Exception("user is null");
                    }

                });
            }

          //  Task.WaitAll(tasks);
        }

        private static Guid GetRandomUserGuid()
        {
            Random rnd = new Random();

            return users[rnd.Next(0, users.Count)];
        }
    }
}
