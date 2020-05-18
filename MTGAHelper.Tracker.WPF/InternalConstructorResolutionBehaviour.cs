using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Linq;
using System.Reflection;

namespace MTGAHelper.Tracker.WPF
{
    public class InternalConstructorResolutionBehavior : IConstructorResolutionBehavior
    {
        private IConstructorResolutionBehavior original;

        public InternalConstructorResolutionBehavior(Container container)
        {
            this.original = container.Options.ConstructorResolutionBehavior;
        }

        public ConstructorInfo GetConstructor(Type implementationType)
        {
            if (!implementationType.GetConstructors().Any())
            {
                var internalCtors = implementationType.GetConstructors(
                    BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(c => !c.IsPrivate)
                    .ToArray();

                if (internalCtors.Length == 1) return internalCtors.First();
            }

            return this.original.GetConstructor(implementationType);
        }
    }
}
