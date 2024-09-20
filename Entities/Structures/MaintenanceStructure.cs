using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class MaintenanceStructure : Structure
    {
        private const string NAME = "Estructura de Mantenimiento";
        private const int MAX_HEALTH = 200;
        private const int PRICE = 50;

        private const int UNITS_PER_STRUCTURE = 3;

        public MaintenanceStructure(Node position) : base(NAME, MAX_HEALTH, PRICE, position)
        {
            this.position = position;
        }

        public override int GetMaxHealth() => MAX_HEALTH;

        public int GetUnitsPerStructure() => UNITS_PER_STRUCTURE;
    }
}
