using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public interface IHasHealth
    {
        public int GetHealth();
        public int GetMaxHealth();
        public void TakeDamage(int quantity);
        public void IncreaseHealth(int quantity);
        public bool IsAlive();

    }
}
