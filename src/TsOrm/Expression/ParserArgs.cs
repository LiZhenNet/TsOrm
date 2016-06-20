using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace TsOrm.Expression
{
    public class ParserArgs
    {
        public ParserArgs()
        {
            Builder = new StringBuilder();
            Parameters = new List<DbParameter>();
        }
        public StringBuilder Builder { get; private set; }
        public List<DbParameter> Parameters { get; set; }
    }
}
