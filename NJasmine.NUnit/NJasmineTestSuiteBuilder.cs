using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.NUnit.TestElements;

namespace NJasmine.NUnit
{
    class NJasmineTestSuiteBuilder : ISpecPositionVisitor
    {
        readonly INativeTestFactory _nativeTestFactory;
        private readonly TestBuilder _parent;
        readonly FixtureDiscoveryContext _buildContext;
        private readonly GlobalSetupManager _globalSetup;
        List<TestBuilder> _accumulatedDescendants;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(INativeTestFactory nativeTestFactory, TestBuilder parent, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup)
        {
            _nativeTestFactory = nativeTestFactory;
            _parent = parent;
            _buildContext = buildContext;
            _globalSetup = globalSetup;
            _accumulatedDescendants = new List<TestBuilder>();
            _accumulatedCategories = new List<string>();
            _ignoreReason = null;
        }

        public void VisitAccumulatedTests(Action<TestBuilder> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        private void ApplyCategoryAndIgnoreIfSet(TestBuilder result)
        {
            if (_ignoreReason != null)
            {
                result.AddIgnoreReason(_ignoreReason);
            }

            foreach (var category in _accumulatedCategories)
            {
                result.AddCategory(category);
            }
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var result = new TestBuilder(_nativeTestFactory.ForUnimplementedTest(position));

                _buildContext.NameGenator.NameTest(description, _parent, result);

                ApplyCategoryAndIgnoreIfSet(result);

                _accumulatedDescendants.Add(result);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(_nativeTestFactory, position, _globalSetup);

                var resultBuilder = new TestBuilder(_nativeTestFactory.ForSuite(position, () => _globalSetup.Cleanup(position)));

                ApplyCategoryAndIgnoreIfSet(resultBuilder);

                _buildContext.NameGenator.NameFork(description, _parent, resultBuilder);

                var finalResultBuilder = subSuite.RunSuiteAction(_buildContext, _globalSetup, action, false, resultBuilder);

                _accumulatedDescendants.Add(finalResultBuilder);
            }
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var buildResult = new TestBuilder(_nativeTestFactory.ForUnimplementedTest(position));

                _buildContext.NameGenator.NameTest(description, _parent, buildResult);

                ApplyCategoryAndIgnoreIfSet(buildResult);
                
                _accumulatedDescendants.Add(buildResult);
            }
            else
            {
                var buildResult = _buildContext.CreateTest(this._globalSetup, _parent, position, description);

                ApplyCategoryAndIgnoreIfSet(buildResult);

                _accumulatedDescendants.Add(buildResult);
            }
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
        {
            if (_accumulatedDescendants.Count > 0)
            {
                _ignoreReason = reason;
            }
            else
            {
                _parent.AddIgnoreReason(reason);
            }
        }

        public void visitExpect(SpecElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
        }

        public void visitWaitUntil(SpecElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
        }

        public void visitWithCategory(SpecElement origin, string category, TestPosition position)
        {
            _accumulatedCategories.Add(category);
        }

        public void visitTrace(SpecElement origin, string message, TestPosition position)
        {
        }

        public void visitLeakDisposable(SpecElement origin, IDisposable disposable, TestPosition position)
        {
        }
    }
}