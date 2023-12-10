using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngenicoTestTCP
{
    public enum ComponentEnum : int
    {
        none = 0,
        btnPaymentStart = 1,

        //   UnActive = 2
    }
    internal class ComponentClass
    {
        public ComponentEnum CompValue { get; set; }
        public bool ComponentEnabled { get; set; }
        public ComponentClass(ComponentEnum enum_a, bool enabled_a )
        {
            CompValue = enum_a;
            ComponentEnabled = enabled_a;
        }
    }
  }
