using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public abstract class Entity : IHasHealth
    {
        public string _name;
        public int health;
        public int price;

        public Node position;

        public Entity(string _name, int maxHealth, int price, Node position)
        {
            this._name = _name;
            health = maxHealth;
            this.price = price;
            this.position = position;
        }

        public int GetHealth() => health;
        public abstract int GetMaxHealth();

        public void IncreaseHealth(int quantity)
        {
            health = health + quantity > GetMaxHealth() ? GetMaxHealth() : health + quantity;
        }

        public void TakeDamage(int quantity)
        {
            health = health - quantity < 0 ? 0 : health - quantity;
        }
        public bool IsAlive() => health > 0;
        public Node GetPosition() => position;
        public void SetPosition(Node position) => this.position = position;
    }
}
