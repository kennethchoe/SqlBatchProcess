using System;
using System.Collections.Generic;
using System.Data;

namespace SqlBatchProcess
{
    internal class ParameterCollectionMock : List<DataParameterMock>, IDataParameterCollection
    {
        public bool Contains(string parameterName)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(string parameterName)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(string parameterName)
        {
            throw new NotImplementedException();
        }

        public object this[string parameterName]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}