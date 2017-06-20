using Ryan.DelegatesAndEvents;
using static System.Console;

namespace Ryan.DelegatesAndEvents
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new list
            var list = new MyListWithChangeEvent();

            // Create a class that listens to the list's change event
            var listener = new EventListener(list);

            // Add and remove items from the list
            list.Add("item 1");
            list.Clear();
            listener.Detach();

            WriteLine("Press any key to stop...");
            ReadKey();
        }
    }
}
