﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    class ignore_is_ignored : ObservableNJasmineFixture
    {
        public override void Tests()
        {
            Observe("1");

            ignore("shouldnt run", () =>
            {
                Observe("-1");
            });

            ignore(() =>
            {
                Observe("-1");
            });

            Observe("2");
        }
    }

    public class NJasmineTestMethod_implements_ignore : NJasmineFixture
    {
        public override void Tests()
        {
            ignore_is_ignored fixture = new ignore_is_ignored();

            it("ignore() isn't run ", delegate() 
            {
                var sut = NJasmineTestMethod.Create(fixture, new TestPosition(1, 3, 2));

                sut.Run();

                expect(fixture.Observations).to.Equal(new List<string> {"1", "2"});
            });
        }
    }
}