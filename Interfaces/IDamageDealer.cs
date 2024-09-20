using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public interface IDamageDealer
    {
        public int GetDamage();
        public void DealDamage(IHasHealth iHasHealth);
    }
}
