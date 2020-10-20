using System.IO;
using System.Text;

namespace R8.Lib
{
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}