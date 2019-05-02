using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemUtility

{
    public class FieldBooleanType : FieldType
    {

        public FieldBooleanType()
        {
            Initialize(1, "T", "F", "");
        }

        public FieldBooleanType(int length, string trueValue, string falseValue, string nullValue)
        {
            Initialize(length, trueValue, falseValue, nullValue);
        }

        protected void Initialize(int length, string trueValue, string falseValue, string nullValue)
        {
        }
    }
}
