using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum PartiotionKeys
    {
        [Description("One")]
        One = 1,
        [Description("Two")]
        Two,
        [Description("Three")]
        Three,
        [Description("Four")]
        Four,
        [Description("Five")]
        Five
    }

    public enum BankMembership
    {
        [Description("First")]
        First = 1,
        [Description("Second")]
        Second,
        [Description("Third")]
        Third,
        [Description("Fourth")]
        Fourth
    }
}
