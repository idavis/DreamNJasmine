﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NUnitFixtureCollection
    {
        readonly NUnitFixtureCollection _parent;
        List<TestPosition> _positions = new List<TestPosition>();  // storing position keys separately by order of existence
        List<TestPosition> _setupPositions = new List<TestPosition>();
        List<TestPosition> _teardownPositions = new List<TestPosition>();
        Dictionary<TestPosition, Type> _fixtures = new Dictionary<TestPosition, Type>();
        Dictionary<TestPosition, Action> _fixtureSetupMethods = new Dictionary<TestPosition, Action>();
        Dictionary<TestPosition, Action> _fixtureTeardownMethods = new Dictionary<TestPosition, Action>();
        Dictionary<TestPosition, object> _instances = new Dictionary<TestPosition, object>();

        public Exception ExceptionFromOnetimeSetup { get; private set; }

        public NUnitFixtureCollection()
        {
            _parent = null;
        }

        public NUnitFixtureCollection(NUnitFixtureCollection parent)
        {
            _parent = parent;
        }

        public void DoOnetimeSetUp()
        {
            try
            {
                foreach (var setupAction in _setupPositions.Select(p => _fixtureSetupMethods[p]))
                {
                    setupAction();
                }
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                ExceptionFromOnetimeSetup = e.InnerException;
            }
            catch (Exception e)
            {
                ExceptionFromOnetimeSetup = e;
            }
        }

        public void DoOnetimeTearDown()
        {
            foreach (var action in _teardownPositions.Select(p => _fixtureTeardownMethods[p]).Reverse())
            {
                action();
            }
        }

        public void DoSetUp(TestPosition position)
        {
            var instance = GetInstance(position);

            RunMethodsWithAttribute(instance, NUnitFramework.SetUpAttribute);
        }

        public void DoTearDown(TestPosition position)
        {
            var instance = GetInstance(position);

            RunMethodsWithAttribute(instance, NUnitFramework.TearDownAttribute);
        }

        public void AddFixture(TestPosition position, Type type)
        {
            if (_fixtures.ContainsKey(position))
            {
                throw new InvalidOperationException();
            }

            _positions.Add(position);
            _fixtures[position] = type;

            AddSetup(position, delegate
            {
                RunMethodsWithAttribute(GetInstance(position), NUnitFramework.FixtureSetUpAttribute);
            });

            AddTearDown(position, delegate
            {
                RunMethodsWithAttribute(GetInstance(position), NUnitFramework.FixtureTearDownAttribute);
            });
        }

        public void AddSetup(TestPosition position, Action action)
        {
            _setupPositions.Add(position);
            _fixtureSetupMethods[position] = action;
        }

        public void AddTearDown(TestPosition position, Action action)
        {
            _teardownPositions.Add(position);
            _fixtureTeardownMethods[position] = action;
        }

        public object GetInstance(TestPosition position)
        {
            if (_positions.Contains(position))
            {
                object instance;

                if (!_instances.TryGetValue(position, out instance))
                {
                    var type = _fixtures[position];

                    instance = type.GetConstructor(new Type[0]).Invoke(EmptyObjectArray);
                    _instances[position] = instance;
                }

                return instance;
            }
            else if (_parent != null)
            {
                return _parent.GetInstance(position);
            }
            else
            {
                throw new InvalidOperationException("NUnit fixture instance requested not found.");
            }
        }

        void RunMethodsWithAttribute(object instance, string attribute)
        {
            var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                     attribute, true);

            foreach (var method in methods)
            {
                method.Invoke(instance, EmptyObjectArray);
            }
        }

        readonly static object[] EmptyObjectArray = new object[0];
    }
}
