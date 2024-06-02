using System;
using System.Collections.Generic;

namespace Assets.TemplateLibrary.Helpers
{
    public class BaseActionWrapper
    {
        /// The delegate for repeating listeners
        public event Action<BaseActionWrapper, object[]> BaseListener = delegate { };

        /// The delegate for one-off listeners
        public event Action<BaseActionWrapper, object[]> OnceBaseListener = delegate { };

        public void Dispatch(object[] args)
        {
            BaseListener(this, args);
            OnceBaseListener(this, args);
            OnceBaseListener = delegate { };
        }

        public virtual List<Type> GetTypes() { return new List<Type>(); }

        public void AddListener(Action<BaseActionWrapper, object[]> callback)
        {
            foreach (Delegate del in BaseListener.GetInvocationList())
            {
                Action<BaseActionWrapper, object[]> action = (Action<BaseActionWrapper, object[]>)del;
                if (callback.Equals(action)) //If this callback exists already, ignore this addlistener
                    return;
            }

            BaseListener += callback;
        }

        public void AddOnce(Action<BaseActionWrapper, object[]> callback)
        {
            foreach (Delegate del in OnceBaseListener.GetInvocationList())
            {
                Action<BaseActionWrapper, object[]> action = (Action<BaseActionWrapper, object[]>)del;
                if (callback.Equals(action)) //If this callback exists already, ignore this addlistener
                    return;
            }
            OnceBaseListener += callback;
        }

        public void RemoveListener(Action<BaseActionWrapper, object[]> callback) { BaseListener -= callback; }

    }

    /// Base concrete form for a Signal with no parameters
    public class ActionWrapper : BaseActionWrapper
    {
        public event Action Listener = delegate { };
        public event Action OnceListener = delegate { };
        public void AddListener(Action callback) { Listener += callback; }
        public void AddOnce(Action callback) { OnceListener += callback; }
        public void RemoveListener(Action callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            return new List<Type>();
        }
        public void Dispatch()
        {
            Listener();
            OnceListener();
            OnceListener = delegate { };
            base.Dispatch(null);
        }
    }

    /// Base concrete form for a Signal with one parameter
    public class ActionWrapper<T> : BaseActionWrapper
    {
        public event Action<T> Listener = delegate { };
        public event Action<T> OnceListener = delegate { };
        public void AddListener(Action<T> callback) { Listener += callback; }
        public void AddOnce(Action<T> callback) { OnceListener += callback; }
        public void RemoveListener(Action<T> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            return retv;
        }
        public void Dispatch(T type1)
        {
            Listener(type1);
            OnceListener(type1);
            OnceListener = delegate { };
            object[] outv = { type1 };
            base.Dispatch(outv);
        }
    }

    /// Base concrete form for a Signal with two parameters
    public class ActionWrapper<T, U> : BaseActionWrapper
    {
        public event Action<T, U> Listener = delegate { };
        public event Action<T, U> OnceListener = delegate { };
        public void AddListener(Action<T, U> callback) { Listener += callback; }
        public void AddOnce(Action<T, U> callback) { OnceListener += callback; }
        public void RemoveListener(Action<T, U> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            return retv;
        }
        public void Dispatch(T type1, U type2)
        {
            Listener(type1, type2);
            OnceListener(type1, type2);
            OnceListener = delegate { };
            object[] outv = { type1, type2 };
            base.Dispatch(outv);
        }
    }

    /// Base concrete form for a Signal with three parameters
    public class ActionWrapper<T, U, V> : BaseActionWrapper
    {
        public event Action<T, U, V> Listener = delegate { };
        public event Action<T, U, V> OnceListener = delegate { };
        public void AddListener(Action<T, U, V> callback) { Listener += callback; }
        public void AddOnce(Action<T, U, V> callback) { OnceListener += callback; }
        public void RemoveListener(Action<T, U, V> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            retv.Add(typeof(V));
            return retv;
        }
        public void Dispatch(T type1, U type2, V type3)
        {
            Listener(type1, type2, type3);
            OnceListener(type1, type2, type3);
            OnceListener = delegate { };
            object[] outv = { type1, type2, type3 };
            base.Dispatch(outv);
        }
    }

    /// Base concrete form for a Signal with four parameters
    public class ActionWrapper<T, U, V, W> : BaseActionWrapper
    {
        public event Action<T, U, V, W> Listener = delegate { };
        public event Action<T, U, V, W> OnceListener = delegate { };
        public void AddListener(Action<T, U, V, W> callback) { Listener += callback; }
        public void AddOnce(Action<T, U, V, W> callback) { OnceListener += callback; }
        public void RemoveListener(Action<T, U, V, W> callback) { Listener -= callback; }
        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            retv.Add(typeof(U));
            retv.Add(typeof(V));
            retv.Add(typeof(W));
            return retv;
        }
        public void Dispatch(T type1, U type2, V type3, W type4)
        {
            Listener(type1, type2, type3, type4);
            OnceListener(type1, type2, type3, type4);
            OnceListener = delegate { };
            object[] outv = { type1, type2, type3, type4 };
            base.Dispatch(outv);
        }
    }
}