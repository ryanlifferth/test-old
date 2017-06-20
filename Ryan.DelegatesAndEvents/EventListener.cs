using Ryan.DelegatesAndEvents;
using System;
using static System.Console;

namespace Ryan.DelegatesAndEvents
{
    public class EventListener
    {

        private MyListWithChangeEvent List;

        public EventListener(MyListWithChangeEvent list)
        {
            List = list;
            // Add "ListChanged" to the CHanged event on "List"
            List.Changed += new ChangedEventHandler(ListChanged);
        }

        /// <summary>
        ///     This will be called whenever the list changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListChanged(object sender, EventArgs e)
        {
            WriteLine("This is called when the event fires");
        }

        /// <summary>
        ///     Detach the event and delete the list
        /// </summary>
        public void Detach()
        {
            List.Changed -= new ChangedEventHandler(ListChanged);
            List = null;
        }

    }
}
