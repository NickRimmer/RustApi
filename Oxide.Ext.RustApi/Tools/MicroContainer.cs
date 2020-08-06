﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Ext.RustApi.Tools
{
    /// <summary>
    /// Very small and simple DI container.
    /// </summary>
    public class MicroContainer
    {
        private delegate object BuilderArgs(Type requestType);
        private readonly Dictionary<Type, BuilderArgs> _registrations;

        /// <summary>
        /// Very small and simple DI container.
        /// </summary>
        public MicroContainer()
        {
            _registrations = new Dictionary<Type, BuilderArgs>();
        }

        /// <summary>
        /// Add class with registration by interfaces or inherited class.
        /// Duplicate will override previous registration.
        /// </summary>
        /// <param name="registrationType">Registration type (e.g. interface).</param>
        /// <param name="implementationType">Implementation type.</param>
        /// <returns></returns>
        public MicroContainer Add(Type registrationType, Type implementationType)
        {
            if (implementationType.IsInterface) throw new ArgumentException("Implementation can't be an interface", nameof(implementationType));

            _registrations[registrationType] = (requestType) => DefaultBuilder(requestType, implementationType);
            return this;
        }

        /// <summary>
        /// Add class with registration by interfaces or inherited class.
        /// Duplicate will override previous registration.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type (e.g. interface).</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        public MicroContainer Add<TRegistration, TImplementation>() where TImplementation : TRegistration =>
            Add(typeof(TRegistration), typeof(TImplementation));

        /// <summary>
        /// Add class to container.
        /// Duplicate will override previous registration.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type.</typeparam>
        public MicroContainer Add<TRegistration>() => Add<TRegistration, TRegistration>();

        /// <summary>
        /// Add singleton instance.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type (e.g. interface).</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <returns></returns>
        public MicroContainer AddSingle<TRegistration, TImplementation>() where TImplementation : TRegistration
        {
            if (typeof(TImplementation).IsInterface) throw new ArgumentException("Implementation can't be interface", nameof(TImplementation));

            var lazy = new Lazy<TImplementation>(() => (TImplementation)DefaultBuilder(typeof(TRegistration), typeof(TImplementation)));
            _registrations[typeof(TRegistration)] = (_) => lazy.Value;

            return this;
        }

        /// <summary>
        /// Add singleton instance.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type.</typeparam>
        /// <returns></returns>
        public MicroContainer AddSingle<TRegistration>() => AddSingle<TRegistration, TRegistration>();

        /// <summary>
        /// Add singleton instance to container.
        /// Duplicate will override previous registration.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type.</typeparam>
        /// <param name="instance">Instance of registration type.</param>
        public MicroContainer AddSingle<TRegistration>(TRegistration instance)
        {
            _registrations[typeof(TRegistration)] = (_) => instance;
            return this;
        }

        /// <summary>
        /// Get instance by registration type.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type.</typeparam>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">Can throw exception in case if requested class wasn't registered.</exception>
        public TRegistration Get<TRegistration>() => (TRegistration)Get(typeof(TRegistration));

        /// <summary>
        /// Default builder of instances.
        /// </summary>
        /// <param name="requestType">Request type.</param>
        /// <param name="implementationType">Type to instantiate.</param>
        /// <returns></returns>
        private object DefaultBuilder(Type requestType, Type implementationType)
        {
            var resultType = implementationType;

            if (implementationType.IsGenericTypeDefinition && requestType.IsGenericType)
            {
                var genericArguments = requestType.GenericTypeArguments;
                resultType = implementationType.MakeGenericType(genericArguments);
            }

            // let's see what we need to create an object
            var ctor = resultType.GetConstructors()[0];
            var ctorParameters = ctor.GetParameters();

            // try to find all required object in container
            var parameters = ctorParameters
                .Select(x => Get(x.ParameterType))
                .ToArray();

            // create type instance
            return ctor.Invoke(parameters);
        }

        /// <summary>
        /// Looking for class by registration type.
        /// </summary>
        /// <param name="requestType">Registration type.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">Can throw exception in case if requested class wasn't registered.</exception>
        private object Get(Type requestType)
        {
            if (!_registrations.TryGetValue(requestType, out var builder))
            {
                // only if type with generic we can try to find by definition
                if (!requestType.IsGenericType) throw new KeyNotFoundException(requestType.FullName);

                // try to find by definition
                if (!_registrations.TryGetValue(requestType.GetGenericTypeDefinition(), out builder))
                    throw new KeyNotFoundException(requestType.FullName);
            }

            return builder.Invoke(requestType);
        }
    }
}
