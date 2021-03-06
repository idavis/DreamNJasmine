﻿using System;
using System.Linq.Expressions;
using System.Threading;
using NJasmine.Core;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine
{
    public abstract class GivenWhenThenFixture : SpecificationFixture
    {
        /// <summary>
        /// Branches the current test specification
        /// </summary>
        /// <param name="description">Description text used to name the test branch.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void describe(string description, Action specification)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitFork(SpecElement.describe, description, specification, position);
            });
        }

        /// <summary>
        /// Branches the current test specification
        /// </summary>
        /// <param name="description">Description text used to name the test branch -- will be prefixed with 'given'.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void given(string description, Action specification)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitFork(SpecElement.given, "given " + description, specification, position);
            });
        }

        /// <summary>
        /// Branches the current test specification.
        /// </summary>
        /// <param name="description">Description text used to name the test branch -- will be prefixed with 'when'.</param>
        /// <param name="specification">The branched portion of the specification.</param>
        public void when(string description, Action specification)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitFork(SpecElement.when, "when " + description, specification, position);
            });
        }

        /// <summary>
        /// Adds a test.
        /// </summary>
        /// <param name="description">The description that names the test -- will be prefixed with 'then'.</param>
        /// <param name="test">The test implementation.</param>
        public void then(string description, Action test)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitTest(SpecElement.then, "then " + description, test, position);
            });
        }

        /// <summary>
        /// Adds an unimplemented test.
        /// </summary>
        /// <param name="description">The description that names the test -- will be prefixed with 'then'.</param>
        public void then(string description)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitTest(SpecElement.then, "then " + description, null, position);
            });
        }

        /// <summary>
        /// Adds a test
        /// </summary>
        /// <param name="description">The description that names the test.</param>
        /// <param name="action">The test implementation.</param>
        public void it(string description, Action action)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitTest(SpecElement.it, description, action, position);
            });
        }

        /// <summary>
        /// Adds an unimplemented test.
        /// </summary>
        /// <param name="description">The description that names the test.</param>
        public void it(string description)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitTest(SpecElement.it, description, null, position);
            });
        }

        /// <summary>
        /// Adds cleanup code to be ran after each test in the following context.
        /// </summary>
        /// <param name="cleanup">The cleanup code.</param>
        public void afterEach(Action cleanup)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitAfterEach(SpecElement.afterEach, cleanup, position);
            });
        }

        /// <summary>
        /// Adds cleanup code to be ran after each test in the following context.
        /// </summary>
        /// <param name="cleanup">The cleanup code.</param>
        public void cleanup(Action cleanup)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitAfterEach(SpecElement.cleanup, cleanup, position);
            });
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void beforeEach(Action action)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitBeforeEach(SpecElement.beforeEach, delegate()
                {
                    action();
                    return (string)null;
                },
                    position);
            });
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public T beforeEach<T>(Func<T> action)
        {
            return SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                return base.Visitor.visitBeforeEach(SpecElement.beforeEach, action, position);
            });
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void arrange(Action action)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitBeforeEach(SpecElement.arrange, delegate()
                {
                    action();
                    return (string)null;
                }, position);
            });
        }

        /// <summary>
        /// Functionally the same as arrange(), has the semantics of exercising the sut.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void act(Action action)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitBeforeEach(SpecElement.act, delegate()
                {
                    action();
                    return (string)null;
                }, position);
            });
        }

        /// <summary>
        /// Adds initialization code to be before after each test in the following context.
        /// A return value can be used in the remainder of the test.
        /// If that return value supports IDisposable, it will be disposed when the test is done.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        /// <returns>The result of the initialization code.</returns>
        public T arrange<T>(Func<T> arrangeAction)
        {
            return SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                return base.Visitor.visitBeforeEach(SpecElement.arrange, arrangeAction, position);
            });
        }

        /// <summary>
        /// Adds initialization code to instantiate a class before after each test in the following context.
        /// If that class supports IDisposable, it will be disposed when the test is done.
        /// </summary>
        /// <typeparam name="TArranged">The class to be created.</typeparam>
        /// <returns>The class instance created.</returns>
        public TArranged arrange<TArranged>() where TArranged : class, new()
        {
            return SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                Func<TArranged> factory = delegate { return new TArranged(); };

                return base.Visitor.visitBeforeEach(SpecElement.arrange, factory, position);
            });
        }

        /// <summary>
        /// Adds initialization code to be ran once before all tests in the following context.
        /// </summary>
        /// <param name="action">The initialization code.</param>
        public void beforeAll(Action action)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitBeforeAll<string>(SpecElement.beforeAll, delegate
                    {
                        action();
                        return (string)null;
                    }, 
                    position);
            });
        }

        /// <summary>
        /// Adds initialization code to be ran once before all tests in the following context.
        /// The initialization code can return a value, which will be made available to every test
        /// in the following context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The initialization code.</param>
        /// <returns>The return value of the initialization code.</returns>
        public T beforeAll<T>(Func<T> action)
        {
            return SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                return base.Visitor.visitBeforeAll(SpecElement.beforeAll, action, position);
            });
        }

        /// <summary>
        /// Adds cleanup code to be ran once after all tests in the following context.
        /// </summary>
        /// <param name="action">The cleanup code.</param>
        public void afterAll(Action action)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitAfterAll(SpecElement.afterAll, action, position);
            });
        }

        /// <summary>
        /// Indicates that the tests in the following specification context should not
        /// be ran unless the user explicitly asks for them to be ran.  Similar to
        /// NUnit's ExplicitAttribute.
        /// </summary>
        /// <param name="reason"></param>
        public void ignoreBecause(string reason)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitIgnoreBecause(SpecElement.ignore, reason, position);
            });
        }

        /// <summary>
        /// Verifies a particular expecation when the tests run.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public void expect(Expression<Func<bool>> expectation)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitExpect(SpecElement.expect, expectation, position);
            });
        }

        private int _msWaitMax = 1000;
        private int _msWaitIncrement = 250;

        /// <summary>
        /// Modifies the default timeouts used by waitUntil and expectEventually.
        /// </summary>
        /// <param name="msWaitMax">The maximum time to wait, in milliseconds.</param>
        public void setWaitTimeout(int msWaitMax)
        {
            var originalWaitMax = msWaitMax;

            _msWaitMax = msWaitMax;

            cleanup(delegate
            {
                _msWaitMax = originalWaitMax;
            });
        }

        /// <summary>
        /// Modifies the default polling interval used by waitUntil and expectEventually.
        /// </summary>
        /// <param name="msWaitIncrement">The polling interval, in seconds.</param>
        public void setWaitIncrement(int msWaitIncrement)
        {
            var originalWaitIncrement = msWaitIncrement;

            _msWaitIncrement = Math.Min(msWaitIncrement, 1);

            cleanup(delegate
            {
                _msWaitIncrement = originalWaitIncrement;
            });
        }

        /// <summary>
        /// Verifies a particular expectation is true as the test runs.  Will wait for a timeout
        /// if the expectation is not initially true.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="msWaitMax">The time to wait, in milliseconds.</param>
        /// <param name="msWaitIncrement">The polling interval, in milliseconds.</param>
        public void expectEventually(Expression<Func<bool>> expectation, int? msWaitMax = null, int? msWaitIncrement = null)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitWaitUntil(SpecElement.expectEventually, expectation, msWaitMax ?? _msWaitMax, msWaitIncrement ?? _msWaitIncrement, position);
            });
        }

        /// <summary>
        /// Verifies a particular expectation is true as the test runs.  Will wait for a timeout
        /// if the expectation is not initially true.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="msWaitMax">The time to wait, in milliseconds.</param>
        /// <param name="msWaitIncrement">The polling interval, in milliseconds.</param>
        public void waitUntil(Expression<Func<bool>> expectation, int? msWaitMax = null, int? msWaitIncrement = null)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitWaitUntil(SpecElement.waitUntil, expectation, msWaitMax ?? _msWaitMax, msWaitIncrement ?? _msWaitIncrement, position);
            });
        }

        public void withCategory(string category)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitWithCategory(SpecElement.withCategory, category, position);
            });
        }

        public void trace(string message)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitTrace(SpecElement.trace, message, position);
            });
        }

        public void leakDisposable(IDisposable disposable)
        {
            SetPositionForNestedCall_Run_Then_SetPositionForNextSibling(position =>
            {
                base.Visitor.visitLeakDisposable(SpecElement.leakDisposable, disposable, position);
            });
        }
    }
}