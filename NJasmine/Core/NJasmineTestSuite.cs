﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest, INJasmineFixturePositionVisitor
    {
        readonly NJasmineFixture _fixture;
        readonly TestPosition _position;
        readonly NUnitFixtureCollection _nunitImports;
        readonly List<Test> _accumulatedDescendants;

        bool _haveReachedAnIt;

        public NJasmineTestSuite(NJasmineFixture fixture, string baseName, string name, TestPosition position, NUnitFixtureCollection parentNUnitImports) 
            : base(baseName, name)
        {
            _fixture = fixture;
            _position = position;
            _nunitImports = new NUnitFixtureCollection(parentNUnitImports);
            _accumulatedDescendants = new List<Test>();
            _haveReachedAnIt = false;

            maintainTestOrder = true;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public void BuildSuite(Action action)
        {
            Exception exception = null;

            _fixture.PushVisitor(new VisitorPositionAdapter(_position.GetFirstChildPosition(), this));

            try
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    exception = e;
                }

                if (exception == null)
                {
                    foreach (var sibling in _accumulatedDescendants)
                        this.Add(sibling);
                }
                else
                {
                    this.Add(new NJasmineInvalidTestSuite(TestName.FullName, "Exception thrown within test definition", exception, _position));
                }
            }
            finally
            {
                _fixture.PopVisitor();
            }
        }

        private void MakeNameUnique(TestMethod test)
        {
            
        }
        
        public void visitDescribe(string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                _accumulatedDescendants.Add(new NJasmineUnimplementedTestMethod(TestName.FullName + " " + description, description,
                                                                               position));
            }
            else
            {
                var describeSuite = new NJasmineTestSuite(_fixture, TestName.FullName, description, position, _nunitImports);

                describeSuite.BuildSuite(action);

                _accumulatedDescendants.Add(describeSuite);
            }
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.beforeEach);
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.afterEach);
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            var testName = description;
            var testFullName = TestName.FullName + " " + description;

            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(testFullName, testName, position);

                MakeNameUnique(nJasmineUnimplementedTestMethod);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var testMethod = new NJasmineTestMethod(_fixture, position, _nunitImports);

                testMethod.TestName.Name = testName;
                testMethod.TestName.FullName = testFullName;

                _accumulatedDescendants.Add(testMethod);
            }

            _haveReachedAnIt = true;
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.importNUnit);

            _nunitImports.AddFixture(position, typeof(TFixture));

            return null;
        }

        public TArranged visitArrange<TArranged>(TestPosition position) where TArranged : class, new()
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.arrange);

            return null;
        }

        public TArranged visitArrange<TArranged>(Func<TArranged> factory, TestPosition position)
        {
            if (_haveReachedAnIt)
                throw WrongMethodAfterItMethod(SpecMethod.arrange);

            return default(TArranged);
        }

        protected override void DoOneTimeSetUp(TestResult suiteResult)
        {
            _nunitImports.DoOnetimeSetUp();
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            _nunitImports.DoOnetimeTearDown();
        }

        InvalidOperationException WrongMethodAfterItMethod(SpecMethod innerSpecMethod)
        {
            return new InvalidOperationException("Called " + innerSpecMethod + "() after " + SpecMethod.arrange + "().");
        }
    }
}
