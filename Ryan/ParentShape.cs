using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryan
{
    public abstract class ParentShape
    {

        public abstract string ShapeName();

        public double CalculateArea(double areaToBeCalculated)
        {
            return areaToBeCalculated * 2;
        }

    }

}
