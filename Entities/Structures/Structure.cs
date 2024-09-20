using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public abstract class Structure : Entity
    {
        public Structure(string _name, int maxHealth, int price, Node position) : base(_name, maxHealth, price, position) { }

    }
}
