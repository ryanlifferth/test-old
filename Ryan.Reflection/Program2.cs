using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Reflection
{
    class Program2
    {
        static void Main2(string[] args)
        {

            var translationMap = BuildTranslationMap();
            var standardPropertyModel = new StandardPropertyModel();

            foreach (var mapItem in translationMap)
            {
                MapToStandardPropertyModel(standardPropertyModel, mapItem.StandardPropertyItemName, mapItem.Value);
            }

            Console.WriteLine("MLS Number:  {0}", standardPropertyModel.MlsNumber);
            Console.WriteLine("Data Source: {0}", standardPropertyModel.DataSourceName);
            Console.WriteLine("Address:     {0}", standardPropertyModel.Address.AddressLine1);
            Console.WriteLine("             {0}, {1} {2}", standardPropertyModel.Address.City, standardPropertyModel.Address.State, standardPropertyModel.Address.Zip);

            Console.WriteLine("UserName:    {0}", standardPropertyModel.User.UserName);
            Console.WriteLine("Name:        {0} {1}", standardPropertyModel.User.Person.FirstName, standardPropertyModel.User.Person.LastName);

            Console.WriteLine();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }


        private static List<DataSourceTranslationMap> BuildTranslationMap()
        {
            return new List<DataSourceTranslationMap> { 
                new DataSourceTranslationMap{ StandardPropertyItemName = "MlsNumber",            DataSourceColumnName = "ListingID",         TranslateToType ="string", Value = "1234" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "DataSourceName",       DataSourceColumnName = "DataSourceName",    TranslateToType ="string", Value = "Portland MLS" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.AddressLine1", DataSourceColumnName = "FullStreetAddress", TranslateToType ="string", Value = "630 N FIRST ST" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.HouseNumber",  DataSourceColumnName = "StreetNumber",      TranslateToType ="string", Value = "630" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.StreetPrefix", DataSourceColumnName = "StreetDirPrefix",   TranslateToType ="string", Value = "N" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.StreetName",   DataSourceColumnName = "StreetName",        TranslateToType ="string", Value = "FIRST" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.StreetSuffix", DataSourceColumnName = "StreetDirSuffix",   TranslateToType ="string", Value = "" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.StreetType",   DataSourceColumnName = "StreetTypeSuffix",  TranslateToType ="string", Value = "ST" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.UnitNumber",   DataSourceColumnName = "UnitNumber",        TranslateToType ="string", Value = "" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.City",         DataSourceColumnName = "City",              TranslateToType ="string", Value = "Drain" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.County",       DataSourceColumnName = "County",            TranslateToType ="string", Value = "Douglas" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.State",        DataSourceColumnName = "State",             TranslateToType ="string", Value = "OR" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.Zip",          DataSourceColumnName = "ZipCode",           TranslateToType ="string", Value = "97435" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.Latitude",     DataSourceColumnName = "Latitude",          TranslateToType ="string", Value = "43.666419" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "Address.Longitude",    DataSourceColumnName = "Longitude",         TranslateToType ="string", Value = "-123.313435" },

                new DataSourceTranslationMap{ StandardPropertyItemName = "User.UserName",         DataSourceColumnName = "",                 TranslateToType ="string", Value = "ryantest" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "User.Person.FirstName", DataSourceColumnName = "",                 TranslateToType ="string", Value = "Ryan" },
                new DataSourceTranslationMap{ StandardPropertyItemName = "User.Person.LastName",  DataSourceColumnName = "",                 TranslateToType ="string", Value = "Test" }
                
            };

        }

        /// <summary>
        ///     Maps (sets) the value from the data source in the StandardPropertyModel using reflection.  Matches 
        ///     are done according to the mapping names.  For root-level objects a simple SetValue is performed.  For 
        ///     complex/custom data types (e.g. Address.City) need to get the complex data type, instantiate it
        ///     if it is null then set the value.
        /// </summary>
        /// <param name="standardPropertyModel"></param>
        /// <param name="standardPropertyItemName"></param>
        /// <param name="sourceValue"></param>
        private static void MapToStandardPropertyModel(StandardPropertyModel standardPropertyModel, string standardPropertyItemName, object sourceValue)
        {
            var parts = standardPropertyItemName.Split('.');
            object parentObject = standardPropertyModel;  // parent object to get info on using reflection
            
            foreach (var part in parts)
            {
                // As we iterate through each object, if it is the first object (the root)
                // or the last object (the target) then we don't need to 
                // instantiate the object.  All other objects need to check to see
                // if they need to be instantiated.
                // If it is last object, then just return the property and set the value
                if (part == parts.Last())
                {
                    var propertyInfo = parentObject.GetType().GetProperty(part);
                    propertyInfo.SetValue(parentObject, sourceValue);
                }
                else
                {
                    // Get the property info and then see if that object needs to be instantiated
                    // If it does, instantiate it and set it on the parent object
                    PropertyInfo propertyInfo = parentObject.GetType().GetProperty(part);
                    var targetObject = propertyInfo.GetValue(parentObject, null);

                    if (targetObject == null)
                    {
                        targetObject = Activator.CreateInstance(propertyInfo.PropertyType);
                        propertyInfo.SetValue(parentObject, targetObject);
                    }
                    parentObject = targetObject;  // Set this for the next use inside this loop

                }
            }

        }

      

    }
}
