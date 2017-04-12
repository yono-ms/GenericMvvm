using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    class NameViewModelSample : NameViewModel
    {
        public NameViewModelSample()
        {
            FirstName = "鈴木";
            LastName = "一郎";
        }
    }
}
