using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Ext.RustApi.Tools
{
    /// <summary>
    /// Very small and simple DI container.
    /// </summary>
    public class MicroContainer
    {
        private delegate object BuilderArgs();
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
        /// <typeparam name="TRegistration">Registration type (e.g. interface).</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        public MicroContainer Add<TRegistration, TImplementation>() where TImplementation : TRegistration
        {
            _registrations[typeof(TRegistration)] = () => DefaultBuilder(typeof(TImplementation));
            return this;
        }

        /// <summary>
        /// Add class to container.
        /// Duplicate will override previous registration.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type.</typeparam>
        public MicroContainer Add<TRegistration>()
        {
            _registrations[typeof(TRegistration)] = () => DefaultBuilder(typeof(TRegistration));
            return this;
        }

        /// <summary>
        /// Add instance to container.
        /// Duplicate will override previous registration.
        /// </summary>
        /// <typeparam name="TRegistration">Registration type.</typeparam>
        /// <param name="instance">Instance of registration type.</param>
        public MicroContainer Add<TRegistration>(TRegistration instance)
        {
            _registrations[typeof(TRegistration)] = () => instance;
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
        /// <param name="implementationType">Type to instantiate.</param>
        /// <returns></returns>
        private object DefaultBuilder(Type implementationType)
        {
            // let's see what we need to create an object
            var ctor = implementationType.GetConstructors()[0];
            var ctorParameters = ctor.GetParameters();

            // try to find all required object in container
            var parameters = ctorParameters.Select(x => Get(x.ParameterType)).ToArray();

            // create type instance
            return ctor.Invoke(parameters);
        }

        /// <summary>
        /// Looking for class by registration type.
        /// </summary>
        /// <param name="registrationType">Registration type.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">Can throw exception in case if requested class wasn't registered.</exception>
        private object Get(Type registrationType)
        {
            if (!_registrations.TryGetValue(registrationType, out var builder))
                throw new KeyNotFoundException(registrationType.FullName);

            return builder.Invoke();
        }
    }
}
