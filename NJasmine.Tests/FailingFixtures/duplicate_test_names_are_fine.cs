﻿using System;
using System.Linq;
using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedTraceSequence = @"
outer it 1
repeated inner it 1
outer it 2
repeated inner it 2
outer it 3
repeated inner it 3
", ExpectedStrings = new [ ]
    {
        "Test Error : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated outer test",
        "Test Error : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated outer test`2",
        "Test Error : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated outer test`3",
        "Skipped : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated unimplemented outer test",
        "Skipped : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated unimplemented outer test`2",
        "Skipped : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated unimplemented outer test`3",
        "Test Failure : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine.repeated describe.repeated inner describe",
        "Test Failure : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine.repeated describe.repeated inner describe`2",
        "Test Failure : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine.repeated describe.repeated inner describe`3",
        "Skipped : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated outer unimplemented describe",
        "Skipped : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated outer unimplemented describe`2",
        "Skipped : NJasmineTests.FailingFixtures.duplicate_test_names_are_fine repeated outer unimplemented describe`3"
    }
)]
    public class duplicate_test_names_are_fine : TraceableNJasmineFixture
    {
        public override void Tests()
        {
            importNUnit<PerClassTraceResetFixture>();

            foreach (var i in Enumerable.Range(1, 3))
            {
                it("repeated outer test", delegate
                {
                    var value = "outer it " + i;
                    Trace(value);
                    throw new Exception(value);
                });

                it("repeated unimplemented outer test");

                describe("repeated describe", delegate
                {
                    it("repeated inner it", delegate
                    {
                        Trace("repeated inner it " + i);
                    });

                    describe("repeated inner describe", delegate
                    {
                        throw new Exception();
                    });
                });

                describe("repeated outer unimplemented describe");
            }
        }
    }
}
