using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF
{
    public class Method
    {
        public MethodInfo MethodData { get; }
        public bool IsSelected { get; set; } = false;

        public Method(MethodInfo methodData)
        {
            MethodData = methodData;
        }
    }
}
