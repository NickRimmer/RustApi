using Oxide.Ext.RustApi.Attributes;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Oxide.Ext.RustApi.Tests.Unit
{
    public class Temp
    {

        [Fact]
        public void T1()
        {
            var type = typeof(Temp);
            var allMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var result = allMethods
                .Where(x => x.GetCustomAttribute(typeof(ApiCommandAttribute)) != null)
                .Select(x => new { Method = x, Attribute = x.GetCustomAttribute(typeof(ApiCommandAttribute)) })
                .ToArray();
        }

        [ApiCommand("c1")]
        private void Temp1() { }

        [ApiCommand("c2")]
        public void Temp2() { }
    }
}
