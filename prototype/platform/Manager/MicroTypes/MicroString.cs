using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.MicroTypes
{
    public abstract class MicroString
    {
        private readonly string value;

        public MicroString(string value)
        {
            this.value = value;
        }

        public static explicit operator string(MicroString obj)
        {
            return obj.value;
        }
    }

    public sealed class Email : MicroString
    {
        public Email(string email) : base(email) { }
    }

    public sealed class PhoneNumber : MicroString
    {
        public PhoneNumber(string email) : base(email) { }
    }
}
