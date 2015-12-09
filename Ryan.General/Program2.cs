using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.General
{
    class Program2
    {

        private static List<Person> _allPeople = new List<Person>();
        private static List<Person> _selectedPeople = new List<Person>();

        static void Main2(string[] args)
        {
            HydratePeopleObjects();

            
            var item1 = _allPeople.FirstOrDefault(x => x.LastName == "Sample02");
            var item2 = _selectedPeople.FirstOrDefault(x => x.LastName == "Sample02");
            _selectedPeople.Add(new Person { ID = null, FirstName = "Sam02", LastName = "Sample02" });
            var item3 = _selectedPeople.LastOrDefault(x => x.LastName == "Sample02");
            Console.WriteLine("item1 and item2: {0}", item1.Equals(item2));
            Console.WriteLine("item1 and item3: {0}", item1.Equals(item3));
            Console.WriteLine("item2 and item3: {0}", item2.Equals(item3));
            
            // So the app doesn't close down in debug mode
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }

        private static void HydratePeopleObjects()
        {

            _allPeople = new List<Person> 
            {
                new Person { ID = null, FirstName = "Sam01", LastName = "Sample01" },
                new Person { ID = null, FirstName = "Sam02", LastName = "Sample02" },
                new Person { ID = null, FirstName = "Sam03", LastName = "Sample03" },
                new Person { ID = null, FirstName = "Sam04", LastName = "Sample04" },
                new Person { ID = null, FirstName = "Sam05", LastName = "Sample05" },
                new Person { ID = null, FirstName = "Sam06", LastName = "Sample06" },
                new Person { ID = null, FirstName = "Sam07", LastName = "Sample07" },
                new Person { ID = null, FirstName = "Sam08", LastName = "Sample08" },
                new Person { ID = null, FirstName = "Sam09", LastName = "Sample09" },
                new Person { ID = null, FirstName = "Sam10", LastName = "Sample10" }
            };

            _selectedPeople = new List<Person>();
            _selectedPeople.AddRange(_allPeople.Where(x => x.LastName == "Sample02" || 
                                       x.LastName == "Sample03" ||
                                       x.LastName == "Sample05" ||
                                       x.LastName == "Sample06").ToList());
            


        }


    }

    public class Person
    {

        public int? ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }

    public class PersonCollection
    {

        public List<Person> AllPeople { get; set; }
        public List<Person> SelectedPeople { get; set; }

    }

}
