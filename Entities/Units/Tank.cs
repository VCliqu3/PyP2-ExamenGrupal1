using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class Tank : Unit
    {
        private const string NAME = "Tanque";
        private const int MAX_HEALTH = 100;
        private const int PRICE = 40;

        private const int DAMAGE = 20;
        private const int SPEED = 2;

        public Tank(Node position) : base(NAME, MAX_HEALTH, PRICE, DAMAGE, SPEED, position)
        {
            this.position = position;
        }

        public override int GetMaxHealth() => MAX_HEALTH;

        public override bool CanAttackEntity(Entity entity)
        {
            if (entity is Structure) return true;
            if (entity is Soldier) return true;

            return false;
        }
    }
}
