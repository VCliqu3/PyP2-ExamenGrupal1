using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class Soldier : Unit
    {
        private const string NAME = "Soldado";
        private const int MAX_HEALTH = 50;
        private const int PRICE = 20;

        private const int DAMAGE = 10;
        private const int SPEED = 1;

        public Soldier(Node position) : base(NAME, MAX_HEALTH, PRICE, DAMAGE, SPEED, position)
        {
            this.position = position;
        }

        public override int GetMaxHealth() => MAX_HEALTH;

        public override bool CanAttackEntity(Entity entity)
        {
            if (entity is Structure) return true;
            if (entity is Helicopter) return true;

            return false;
        }
    }
}
