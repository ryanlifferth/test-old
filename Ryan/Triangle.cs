using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan
{
    public class Triangle : ParentShape
    {

        public Triangle()
        {
            // DO something here
            var triangleArea = CalculateArea(2);
        }



        public override string ShapeName()
        {
            return "this is a triangle.";
        }
    }
}
