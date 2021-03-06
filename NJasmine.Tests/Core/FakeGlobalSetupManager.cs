﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;

namespace NJasmineTests.Core
{
    class FakeGlobalSetupManager : IGlobalSetupManager
    {
        public void Cleanup(TestPosition position)
        {
        }

        public void PrepareForTestPosition(TestPosition position, out Exception existingError)
        {
            existingError = null;
        }

        public T GetSetupResultAt<T>(TestPosition position)
        {
            return default(T);
        }

        public IEnumerable<string> GetTraceMessages()
        {
            return new List<string>();
        }
    }
}
