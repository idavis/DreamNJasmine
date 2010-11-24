﻿using System;

namespace NJasmine.Core.FixtureVisitor
{
    public class DoNothingFixtureVisitor : INJasmineFixtureVisitor
    {
        public void visitDescribe(string description, Action action)
        {
        }

        public virtual void visitBeforeEach(Action action)
        {
        }

        public virtual void visitAfterEach(Action action)
        {
        }

        public virtual void visitIt(string description, Action action)
        {
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            return default(TFixture);
        }

        public TDisposable visitDisposing<TDisposable>() where TDisposable : class, IDisposable, new()
        {
            return default(TDisposable);
        }

        public TDisposable visitDisposing<TDisposable>(Func<TDisposable> factory) where TDisposable : class, IDisposable
        {
            return default(TDisposable);
        }
    }
}