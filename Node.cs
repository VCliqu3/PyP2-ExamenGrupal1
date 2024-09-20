using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyP2_ExamenGrupal1
{
    public class Node
    {
        public List<Structure> playerStructures;
        public List<Unit> playerUnits;
        public List<Unit> enemyUnits;

        public Node()
        {
            playerStructures = new List<Structure>();
            playerUnits = new List<Unit>();
            enemyUnits = new List<Unit>();
        }

        public List<Structure> GetPlayerStructures() => playerStructures;
        public List<Unit> GetPlayerUnits() => playerUnits;
        public List<Unit> GetEnemyUnits() => enemyUnits;
    }
}
