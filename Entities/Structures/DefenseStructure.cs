using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class DefenseStructure :Structure , IDamageDealer
    {
        private const string NAME = "Estructura de Defensa";
        private const int MAX_HEALTH = 200;
        private const int PRICE = 200;

        private const int DAMAGE = 10;

        public DefenseStructure(Node position) : base(NAME, MAX_HEALTH, PRICE, position)
        {
            this.position = position;
        }

        public override int GetMaxHealth() => MAX_HEALTH;
        public int GetDamage() => DAMAGE;
        public void DealDamage(IHasHealth iHasHealth) => iHasHealth.TakeDamage(GetDamage());
    }
}
