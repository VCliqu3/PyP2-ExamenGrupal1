using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class CollectorStructure : Structure
    {
        private const string NAME = "Estructura de Recoleccion";
        private const int MAX_HEALTH = 200;
        private const int PRICE = 80;

        private const int MONEY_PER_TURN = 5;

        public CollectorStructure(Node position) : base(NAME, MAX_HEALTH, PRICE, position)
        {
            this.position = position;
        }

        public override int GetMaxHealth() => MAX_HEALTH;
        public int GetMoneyPerTurn() => MONEY_PER_TURN;
    }
}
