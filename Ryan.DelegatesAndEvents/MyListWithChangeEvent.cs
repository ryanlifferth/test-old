using System;
using System.Collections;

namespace Ryan.DelegatesAndEvents
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    ///     A class that works just like an ArrayList but sends event notifications whenever the list changes
    /// </summary>
    public class MyListWithChangeEvent : ArrayList
    {

        /// <summary>
        ///     An event that clients can use to be notified whenever the elements of the list change
        /// </summary>
        public event ChangedEventHandler Changed;

        /// <summary>
        ///     Invoke the Changed event; called whenever list changes
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        /// <summary>
        ///     Overrides the Add method of ArrayList so that we can call the changed event
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override int Add(object value)
        {
            int i = base.Add(value);
            OnChanged(EventArgs.Empty);
            return i;
        }

        /// <summary>
        ///     Overrides the Clear method of ArrayList so that we can call the changed event
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Overrides the this property so that we can call the changed event
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override object this[int index]
        {
            set
            {
                base[index] = value;
                OnChanged(EventArgs.Empty);
            }
        }

    }
}
