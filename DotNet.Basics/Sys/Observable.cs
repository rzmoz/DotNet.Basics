using System;
using System.Runtime.Serialization;

namespace DotNet.Basics.Sys
{
    [DataContract]
    public class Observable<T>
    {
        private T _value;

        public Observable()
        {
            var type = typeof(T);
            bool isPrimitive = type.IsPrimitive;
            _notifyListeners = true;
            if (isPrimitive)
                _setValue = ValueTypeSetter;
            else
                _setValue = ReferenceTypeSetter;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        private bool _notifyListeners;

        private readonly Func<T, bool> _setValue;
        public event EventHandler<EventArgs<T>> Updating;
        public event EventHandler<EventArgs<T>> Updated;

        public void Reset()
        {
            _notifyListeners = false;
            Value = default(T);
            _notifyListeners = true;
        }

        public T Value
        {
            get { return _value; }
            set { _setValue(value); }
        }

        private void SetValue(T t)
        {
            if (_notifyListeners && Updating != null)
                Updating(this, new EventArgs<T>(_value));

            _value = t;

            if (_notifyListeners && Updated != null)
                Updated(this, new EventArgs<T>(_value));
        }

        private bool ValueTypeSetter(T t)
        {
            if (_value.Equals(t))
                return false;
            SetValue(t);
            return true;
        }

        private bool ReferenceTypeSetter(T t)
        {
            //t will never be value type since this was decided in constructor
            if (_value as object == null && t as object == null)
                return false;

            if (_value as object != null && t as object != null && _value.Equals(t))
                return false;

            SetValue(t);
            return true;
        }
    }
}
