﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Core;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests.PassingFixtures
{
    namespace WithNamespaceSetup
    {
        [RunExternal(true, ExpectedTraceSequence = @"
running test 1
running test 2
TearDown NamespaceSetupB
")]
        public class supports_nunit_setup_by_namespace : TraceableNJasmineFixture
        {
            public override void Tests()
            {
                ResetTracing();

                it("test must run setup from NamespaceSetupB only, skipping NamespaceSetupA", delegate {
                    
                    Trace("running test 1");

                    NamespaceSetupA.SetupCount.Should().Equal(0);
                    NamespaceSetupB.SetupCount.Should().Equal(1);
                });

                it("should only run setup once", delegate
                {
                    Trace("running test 2");

                    NamespaceSetupA.SetupCount.Should().Equal(0);
                    NamespaceSetupB.SetupCount.Should().Equal(1);
                });
            }
        }

        [TestFixture]
        public class experimenting_with_nunit_namespace_setup
        {
            [Test]
            public void only_shows_second_namespace_setup_hmm()
            {
                
            }
        }

        [SetUpFixture]
        public class NamespaceSetupA
        {
            static public int SetupCount = 0;

            [SetUp]
            public void Setup()
            {
                SetupCount++;  // can't trace this step as it happens before trace reset
            }

            [TearDown]
            public void TearDown()
            {
                supports_nunit_setup_by_namespace.Trace("TearDown NamespaceSetupA");
            }
        }


        [SetUpFixture]
        public class NamespaceSetupB
        {
            static public int SetupCount = 0;

            [SetUp]
            public void Setup()
            {
                SetupCount++;  // can't trace this step as it happens before trace reset
            }

            [TearDown]
            public void TearDown()
            {
                supports_nunit_setup_by_namespace.Trace("TearDown NamespaceSetupB");
            }
        }
    }
}