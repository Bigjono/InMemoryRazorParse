using System.Dynamic;
using System.Text;

namespace InMemoryRazorParser.Core
{
    public abstract class DynamicContentGeneratorBase
    {
        private StringBuilder _buffer;


        protected DynamicContentGeneratorBase()
        {
            Model = new ExpandoObject();
        }

        public dynamic Model { get; set; }

        public abstract void Execute();

        protected void Write(object value)
        {
            WriteLiteral(value);
        }


        protected void WriteLiteral(object value)
        {
            _buffer.Append(value);
        }


        public string GetContent()
        {
            _buffer = new StringBuilder(1024);
            Execute();
            return _buffer.ToString();
        }

    }
}
