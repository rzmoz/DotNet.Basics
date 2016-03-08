using System;

namespace DotNet.Basics.Ioc
{
    public class RegistrationsCollectionItem
    {
        public RegistrationsCollectionItem(ICsbRegistrations registration)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            Registration = registration;
            Type = Registration.GetType();
        }

        public Type Type { get; }
        public ICsbRegistrations Registration { get; }
        
        protected bool Equals(RegistrationsCollectionItem other)
        {
            return Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RegistrationsCollectionItem)obj);
        }

        public override int GetHashCode()
        {
            return (Type != null ? Type.GetHashCode() : 0);
        }

        public static bool operator ==(RegistrationsCollectionItem left, RegistrationsCollectionItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RegistrationsCollectionItem left, RegistrationsCollectionItem right)
        {
            return !Equals(left, right);
        }
    }
}
