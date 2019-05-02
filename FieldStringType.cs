using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemUtility
{
    public class FieldStringType : FieldType
    {
        int? minLength;
        int? maxLength;

        public FieldStringType(int length)
        {
            Initialize(length, null, length);
        }

        public FieldStringType(int length, int? minLength, int? maxLength)
        {
            Initialize(length, minLength, maxLength);
        }

        protected void Initialize(int length, int? minLength, int? maxLength)
        {
            this.length = length;
            this.minLength = minLength;
            this.maxLength = maxLength;
        }
    }
}
