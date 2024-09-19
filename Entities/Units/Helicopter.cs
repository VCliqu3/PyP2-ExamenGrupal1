using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class Helicopter : Unit
    {
        private const string NAME = "Helicoptero";
        private const int MAX_HEALTH = 150;
        private const int PRICE = 60;

        private const int DAMAGE = 30;
        private const int SPEED = 3;

        public Helicopter(Node position) : base(NAME, MAX_HEALTH, PRICE, DAMAGE, SPEED, position) 
        { 
            this.position = position;
        }

        public override int GetMaxHealth() => MAX_HEALTH;

        public override bool CanAttackEntity(Entity entity)
        {
            if (entity is Structure) return true;
            if (entity is Tank) return true;

            return false;
        }
    }
}
