using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public abstract class Unit : Entity, IDamageDealer
    {
        public int damage;
        public int speed;

        public Unit(string _name, int maxHealth, int price, int damage, int speed, Node position) : base(_name, maxHealth, price, position)
        {
            this.damage = damage;
            this.speed = speed;
        }

        public void DealDamage(IHasHealth iHasHealth)
        {
            iHasHealth.TakeDamage(GetDamage());
        }

        public int GetDamage() => damage;
        public int GetSpeed() => speed;

        public abstract bool CanAttackEntity(Entity entity);
    }
}
