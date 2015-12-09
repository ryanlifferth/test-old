using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan.LinqStuff
{
    class Program
    {

        private static DataTable _dt;

        static void Main(string[] args)
        {

            CreateDatatable();

            DataRow drow = _dt.AsEnumerable().Where(p => p.Field<int>("ID") == 1).FirstOrDefault();

            var nameValueOfSelectedRow = drow.Field<string>("Name");
            var idValueOfSelectedRow = drow.Field<int>("ID");
            Console.WriteLine("DataRow values for ID = 1");
            Console.WriteLine("ID = {0}; Name = {1}", idValueOfSelectedRow, nameValueOfSelectedRow);
            //Console.WriteLine("Name: {0}", nameValueOfSelectedRow);
            Console.WriteLine();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

        }

        private static void CreateDatatable()
        {
            //Create DataTable 
            _dt = new DataTable();
            _dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("ID",  typeof(int)),
                new DataColumn("Name", typeof(string))

            });

            //Fill with data

            _dt.Rows.Add(new Object[] { 1, "Test1" });
            _dt.Rows.Add(new Object[] { 2, "Test2" });
        }
    }
}
