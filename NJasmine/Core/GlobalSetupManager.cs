﻿using System;
using System.Linq;
using System.Threading;

namespace NJasmine.Core
{
    public class GlobalSetupManager : IGlobalSetupManager
    {
        Func<SpecificationFixture> _fixtureFactory;
        Thread _thread;
        GlobalSetupVisitor _visitor;
        AutoResetEvent _runningLock;
        private TestPosition _targetPosition;

        public void Initialize(Func<SpecificationFixture> fixtureFactory)
        {
            _fixtureFactory = fixtureFactory;
            _thread = null;
            _runningLock = new AutoResetEvent(false);
            _visitor = new GlobalSetupVisitor(_runningLock);
        }

        public void Cleanup()
        {
            if (_thread != null)
            {
                PrepareForTestPosition(new TestPosition());
            }
        }

        public void PrepareForTestPosition(TestPosition position)
        {
            if (position == null)
                throw new ArgumentException("Parameter is required", "position");

            _targetPosition = position;

            if (_visitor.SetTargetPosition(position))
                return;

            do
            {
                if (_thread == null)
                {
                    _thread = new Thread(ThreadProc);
                    _thread.Start();
                }

                _runningLock.Set();
                Thread.Sleep(0);
                _runningLock.WaitOne(-1);
            } while (!_visitor.SetTargetPosition(position));
        }

        public T GetSetupResultAt<T>(TestPosition position)
        {
            return (T)_visitor.GetSetupResultAt(position);
        }

        public void ThreadProc()
        {
            _runningLock.WaitOne(-1);

            try
            {
                while (true)
                {
                    if (_targetPosition.Coordinates.Count() == 0)
                    {
                        return;
                    }

                    var fixture = _fixtureFactory();
                    fixture.CurrentPosition = new TestPosition(0);
                    try
                    {
                        fixture.Visitor = _visitor;
                        fixture.Run();
                    }
                    catch (NJasmineTestMethod.TestFinishedException e)
                    {
                    }
                    finally
                    {
                        _visitor.FinishCleanup();
                    }
                }
            }
            finally 
            {
                _thread = null;

                _runningLock.Set();
            }
        }
    }
}