using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryan.AddressUtility.Models;

namespace Ryan.Maps.Win.ViewModels
{
    public class CropMapViewModel : ViewModelBase
    {
        private Address _subject;
        private List<Models.Comp> _comps;

        public Address Subject
        {
            get { return _subject; }
            set
            {
                if (value == _subject) return;

                _subject = value;
                OnPropertyChanged("Subject");
            }
        }

        public List<Models.Comp> Comps
        {
            get { return _comps; }
            set
            {
                if (value == _comps) return;

                _comps = value;
                OnPropertyChanged("Comps");
            }
        }
    }
}
