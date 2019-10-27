namespace Ionix.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    //Bu Kısım Server da tutulacak. Client da yalnızca Token değeri olacak. Dolayısıyla Password burada tutulabilir. Cache lendiği yer 
    public class User : IEquatable<User>
    {
        public static bool IsValid(User user)
        {
            return null != user && !String.IsNullOrEmpty(user.Name);
        }

        public User()
        {
            this.Extended = new ExpandoObject();
        }

        public string Name { get; set; }
        public string Password { get; set; }
        public DateTime LastLoginTime { get; set; }

        //Bundan Sonra istediğin veriyi buraya aktarabilirsin.
        public string Role { get; set; }
        public bool IsAdmin { get; set; }

        //Burası tek değer. İleride web api gibi rol bazlı geliştirilebilir.
        public bool CanUseWebSockets { get; set; }

        public dynamic Extended { get; }

        public bool Equals(User other)
        {
            if (null != other)
                return String.Equals(this.Name, other.Name);
            return  false;
        }

        public override int GetHashCode()
        {
            return this.Name != null ? this.Name.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as User);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    internal sealed class UserEqualityComparer : IEqualityComparer<User>
    {
        internal static readonly UserEqualityComparer Instance = new UserEqualityComparer();

        private UserEqualityComparer() { }

        public bool Equals(User x, User y)
        {
            return String.Equals(x.Name, y.Name);
        }

        public int GetHashCode(User obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
