﻿using System;

namespace Silphid.Injexit
{
    internal class Binding : IBinding
    {
        public static readonly IBinding Null = new NullBinding();
        
        public IContainer Container { get; }
        public Type AbstractionType { get; }
        public Type ConcretionType { get; }
        public Lifetime Lifetime { get; set; }
        public IResolver OverrideResolver { get; private set; }
        public bool IsOverrideResolverRecursive { get; private set; }
        public object Instance { get; set; }
        public bool InList { get; private set; }
        public string Name { get; private set; }
        public BindingId Id { get; private set; }
        public BindingId Reference { get; set; }

        public Binding(IContainer container, Type abstractionType, Type concretionType)
        {
            Container = container;
            AbstractionType = abstractionType;
            ConcretionType = concretionType;
        }

        public Binding(IContainer container, Type abstractionType, BindingId reference)
        {
            Container = container;
            AbstractionType = abstractionType;
            Reference = reference;
        }

        IBinding IBinding.InList()
        {
            if (InList)
                throw new InvalidOperationException("Already a list binding.");
                
            InList = true;
            return this;
        }

        public IBinding AsSingle()
        {
            if (Lifetime != Lifetime.Transient)
                throw new InvalidOperationException($"Lifetime already set to: {Lifetime}");
                
            Lifetime = Lifetime.Single;
            return this;
        }

        public IBinding AsEagerSingle()
        {
            if (Lifetime != Lifetime.Transient)
                throw new InvalidOperationException($"Lifetime already set to: {Lifetime}");

            Lifetime = Lifetime.EagerSingle;
            return this;
        }

        IBinding IBinding.Id(BindingId id)
        {
            if (Id != null)
                throw new InvalidOperationException("Already specified binding Name.");
                    
            Id = id;
            id.Binding = this;
            return this;
        }

        public IBinding Using(IResolver resolver)
        {
            if (OverrideResolver != null)
                throw new InvalidOperationException("Already specified binding overrides.");
                    
            OverrideResolver = resolver;
            return this;
        }

        public IBinding UsingRecursively(IResolver resolver)
        {
            if (OverrideResolver != null)
                throw new InvalidOperationException("Already specified binding overrides.");
                    
            OverrideResolver = resolver;
            IsOverrideResolverRecursive = true;
            return this;
        }

        IBinding IBinding.Named(string name)
        {
            if (Name != null)
                throw new InvalidOperationException("Already specified binding name.");
                    
            Name = Reflector.GetCanonicalName(name);
            return this;
        }

        public override string ToString()
        {
            if (Reference != null)
                return $"BindReference<{AbstractionType.Name}>({Reference})";
            
            var concretionType = ConcretionType?.Name ?? "???";
            var lifetime = Lifetime != Lifetime.Transient ? $".As{Lifetime}()" : "";
            var id = Id != null ? $".Id({Id})" : "";
            var list = InList ? ".InList()" : "";
            var instance = Instance != null ? "Instance" : "";
            var overrides = OverrideResolver != null ? ".Using(...)" : "";
            
            return $"Bind{instance}<{AbstractionType.Name}, {concretionType}>(){lifetime}{id}{list}{overrides}";
        }
    }
}