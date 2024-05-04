using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace GB_CSharp_Reflection
{
    internal class Program
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class CustomNameAttribute : Attribute
        {
            public string Name { get; }

            public CustomNameAttribute(string name)
            {
                Name = name;
            }
        }
        class TestClass
        {
            [CustomName("ICustom")]
            public int I { get; set; }
            [CustomName("SCustom")]
            public string S { get; set; }
            [CustomName("DCustom")]
            public decimal D { get; set; }
            [CustomName("CCastom")]
            public char[] C { get; set; }
            public TestClass()
            {
            }
            public TestClass(int i)
            {
                this.I = i;
            }
            public TestClass(int i, string s, decimal d, char[] c) : this(i)
            {
                this.S = s;
                this.D = d;
                this.C = c;
            }

            public static string ObjectToString(object obj)
            {
                var type = obj.GetType();
                var properties = type.GetProperties();
                string result = "";

                foreach (var property in properties)
                {
                    var attribute = (CustomNameAttribute)Attribute.GetCustomAttribute(property, typeof(CustomNameAttribute));
                    if (attribute != null)
                    {
                        var value = property.GetValue(obj);
                        if (value is char[])
                        {
                            value = string.Join("", (char[])value);
                        }
                        else if (value is string)
                        {
                            value = ((string)value).Split(' ')[0];
                        }
                        result += $"{attribute.Name}:{value} ";
                    }
                }

                return result.Trim();
            }

            public static void StringToObject(object obj, string str)
            {
                var type = obj.GetType();
                var properties = type.GetProperties();
                var pairs = str.Split(' ');

                foreach (var pair in pairs)
                {
                    var keyValue = pair.Split(':');
                    foreach (var property in properties)
                    {
                        var attribute = (CustomNameAttribute)Attribute.GetCustomAttribute(property, typeof(CustomNameAttribute));
                        if (attribute != null && attribute.Name == keyValue[0])
                        {
                            if (property.PropertyType == typeof(char[]))
                            {
                                property.SetValue(obj, keyValue[1].ToCharArray());
                            }
                            else
                            {
                                property.SetValue(obj, Convert.ChangeType(keyValue[1], property.PropertyType));
                            }
                        }
                    }
                }
            }
            static void Main(string[] args)
            {
                TestClass test = new TestClass() { I = 5, S = "kshgkhgkshdgk ", D = 1.154645m, C = new char[] { 'a', 'b','c' } };
                string testMsg = ObjectToString(test);
                Console.WriteLine(testMsg);
                var obj = new TestClass();
                StringToObject(obj,testMsg);
                Console.WriteLine(ObjectToString(obj));            
            }
        }
    }
}