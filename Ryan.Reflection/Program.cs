using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.Reflection
{
    class Program
    {

        static void Main(string[] args)
        {
            var fieldTag = BuildFieldTagObject();
            var fieldTagTable = CreateDataTable(fieldTag);

            /*Console.WriteLine("MLS Number:  {0}", standardPropertyModel.MlsNumber);
            Console.WriteLine("Data Source: {0}", standardPropertyModel.DataSourceName);
            Console.WriteLine("Address:     {0}", standardPropertyModel.Address.AddressLine1);
            Console.WriteLine("             {0}, {1} {2}", standardPropertyModel.Address.City, standardPropertyModel.Address.State, standardPropertyModel.Address.Zip);

            Console.WriteLine("UserName:    {0}", standardPropertyModel.User.UserName);
            Console.WriteLine("Name:        {0} {1}", standardPropertyModel.User.Person.FirstName, standardPropertyModel.User.Person.LastName);

            Console.WriteLine();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();*/
        }

        public static List<FieldTag> BuildFieldTagObject()
        {
            return new List<FieldTag> {
                new FieldTag()
                {
                    FormProfileFieldId = 2,
                    FormProfileFieldTagId = 1,             // AboveGradeGLA
                    TagId = 20,                 // AboveGradeGLA
                    TagName = "AboveGradeGla",
                    Ordering = 1,
                    FieldTagOperatorList = new List<FieldTagOperator>{
                        new FieldTagOperator()
                        {
                            FieldTagOperatorId = 1,         
                            TagOperatorId = 8,          // NumericFormat
                            OperatorName = "NumericFormat",
                            FieldTagOperatorAttributeList = new List<FieldTagOperatorAttribute> {
                                new FieldTagOperatorAttribute()
                                {
                                    FieldTagOperatorAttributeId = 1,
                                    AttributeName = "Currency",
                                    AttributeValue = "C",
                                    AttributeDefaultValue = "C",
                                }
                            }
                        }
                    }
                }
            };

        }


        public static DataTable CreateDataTable<T>(IEnumerable<T> list)
        {
            Type type = typeof(T);
            var properties = type.GetProperties();

            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in properties)
            {
                dataTable.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }

            foreach (T entity in list)
            {
                object[] values = new object[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(entity);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }


    }


    public class FieldTag
    {
        public string EndResult { get; set; }
        public int FormProfileFieldId { get; set; }
        public int FormProfileFieldTagId { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
        public int Ordering { get; set; }
        public List<FieldTagOperator> FieldTagOperatorList { get; set; }
    }

    public class FieldTagOperator
    {
        public int FieldTagOperatorId { get; set; }
        public int TagOperatorId { get; set; }
        public string OperatorName { get; set; }
        public List<FieldTagOperatorAttribute> FieldTagOperatorAttributeList { get; set; }
    }

    public class FieldTagOperatorAttribute
    {
        public int FieldTagOperatorAttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeDefaultValue { get; set; }
        public string AttributeValue { get; set; }
    }

}
