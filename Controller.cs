using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PyP2_ExamenGrupal1
{
    public class Controller
    {
        private int turnNumber = 1;
        private int money = 200;
        private int enemyUnitsTimesGenerated = 0;

        private const int NODES_NUMBER = 20;
        private const int INITIAL_MAINTENANCE_STRUCTURES = 3;

        private List<Node> nodes = new List<Node>();

        private List<Structure> playerStructures = new List<Structure>();
        private List<Unit> playerUnits = new List<Unit>();

        private List<Unit> enemyUnits = new List<Unit>();

        #region Generation Probabilities
        private const int TURN_TO_REACH_MAX_PROB = 15;

        private const float INIT_PROB_SOLDIER = 0.5f;
        private const float INIT_PROB_TANK = 0.3f;
        private const float INIT_PROB_HELICOPTER = 0.2f;

        private const float FINAL_PROB_SOLDIER = 0.33f;
        private const float FINAL_PROB_TANK = 0.33f;
        private const float FINAL_PROB_HELICOPTER = 0.33f;
        #endregion

        public void Execute()
        {

            GenerateNodes();
            GenerateInitialMaintenanceStructures();

            Console.WriteLine("Inicio del Juego");
            Console.WriteLine($"\nComienzas con ${money}");
            Console.WriteLine($"\nComienzas con 3 Estructuras de Mantenimiento");

            bool gameEnded = false;

            while (!gameEnded)
            {
                PlayerTurn();

                if (CheckPlayerWon())
                {
                    Console.WriteLine("\nHAS DERROTADO AL ENEMIGO!");
                    Console.WriteLine($"Has tardado {turnNumber} turnos");
                    gameEnded = true;
                }
                else
                {
                    EnemyTurn();

                    if (CheckPlayerDefeated())
                    {
                        Console.WriteLine("\nHAS SIDO DERROTADO!");
                        Console.WriteLine($"Has sobrevivido {turnNumber} turnos");
                        gameEnded = true;
                    }
                }

                turnNumber++;
            }
        }

        private void PlayerTurn()
        {
            Console.WriteLine("\nEs tu turno");

            bool turnEnded = false;

            while (!turnEnded)
            {
                Console.WriteLine("\nSelecciona una opción:");
                Console.WriteLine("1.- Ver tus estructuras");
                Console.WriteLine("2.- Ver tus unidades");
                Console.WriteLine("3.- Ver enemigos");
                Console.WriteLine("4.- Ver los nodos");
                Console.WriteLine("5.- Crear estructura");
                Console.WriteLine("6.- Crear unidad");
                Console.WriteLine("7.- Pasar Turno");

                int option = ChooseNumberOption(7);

                switch (option)
                {
                    case 1:
                        SeeStructures();
                        break;
                    case 2:
                        SeeUnits();
                        break;
                    case 3:
                        SeeEnemies();
                        break;
                    case 4:
                        SeeNodes();
                        break;
                    case 5:
                        CreateStructure();
                        //turnEnded = true;
                        break;
                    case 6:
                        CreateUnit();
                        //turnEnded = true;
                        break;
                    case 7:
                    default:
                        PassTurn();
                        turnEnded = true;
                        break;
                }
            }

            GainMoneyEndOfTurn();

            MovePlayerUnits();

            HandleCombat(true);
        }

        private void EnemyTurn()
        {
            Console.WriteLine("\nEs el turno del enemigo");

            GenerateEnemyUnits();
            MoveEnemyUnits();
            HandleCombat(false);
        }

        #region PlayerTurnOptions

        private void SeeStructures() 
        { 
            if(playerStructures.Count() == 0)
            {
                Console.WriteLine("\nNo cuentas con estructuras");
                return;
            }

            Console.WriteLine("\nEstructuras Aliadas:\n");

            int index = 0;

            foreach(Structure structure in playerStructures)
            {
                index++;
                Console.WriteLine($"{index}.- {structure._name} - {structure.health} puntos de vida - Posicion: Nodo {GetNodeIndexInList(structure.GetPosition())+1}");
            }
        }

        private void SeeUnits()
        {
            if (playerUnits.Count() == 0)
            {
                Console.WriteLine("\nNo cuentas con unidades");
                return;
            }

            Console.WriteLine("\nUnidades Aliadas:\n");

            int index = 0;

            foreach (Unit unit in playerUnits)
            {
                index++;
                Console.WriteLine($"{index}.- {unit._name} - {unit.health} puntos de vida - Posicion: Nodo {GetNodeIndexInList(unit.GetPosition()) + 1}");
            }
        }

        private void SeeEnemies()
        {
            if (enemyUnits.Count() == 0)
            {
                Console.WriteLine("\nEl enemigo no cuenta con unidades");
                return;
            }

            Console.WriteLine("\nUnidades Enemigos:\n");

            int index = 0;

            foreach (Unit unit in enemyUnits)
            {
                index++;
                Console.WriteLine($"{index}.- {unit._name} - {unit.health} puntos de vida - Posicion: Nodo {GetNodeIndexInList(unit.GetPosition()) + 1}");
            }
        }

        private void SeeNodes()
        {
            Console.WriteLine("\nNodos:\n");

            int index = 0;

            foreach (Node node in nodes)
            {
                index++;

                int playerStructures = node.playerStructures.Count;
                int playerUnits = node.playerUnits.Count;
                int enemyUnits = node.enemyUnits.Count;

                bool isPlayerBase = node == nodes[0];
                bool isEnemyBase = node == nodes[^1];
                bool hasEntities = playerStructures > 0 || playerUnits > 0 || enemyUnits > 0;

                string baseDescription = "";

                if (isPlayerBase) baseDescription = "BASE DEL JUGADOR - ";
                if (isEnemyBase) baseDescription = "BASE DEL ENEMIGO - ";

                string nodeName = $"Nodo {index}";

                string nodeDescription = $" - {playerStructures} Estructura(s) Aliadas || {playerUnits} Unidad(es) Aliadas || {enemyUnits} Unidad(es) Enemigas";
                if (!hasEntities) nodeDescription = " - Vacio";

                Console.WriteLine(baseDescription + nodeName + nodeDescription);
            }
        }

        private void CreateStructure()
        {
            if (!CheckCanBuyEntity()) return;

            Console.WriteLine($"\nCuentas con ${money}");

            CollectorStructure collectorStructure = new CollectorStructure(null);
            MaintenanceStructure maintenanceStructure = new MaintenanceStructure(null);
            DefenseStructure defenseStructure = new DefenseStructure(null);

            Console.WriteLine("\nSelecciona la estructura a construir");
            Console.WriteLine($"1.- Estructura de Recoleccion - Precio: ${collectorStructure.price}");
            Console.WriteLine($"2.- Estructura de Mantenimiento - Precio: ${maintenanceStructure.price}");
            Console.WriteLine($"3.- Estructura de Defensa - Precio: ${defenseStructure.price}");
            Console.WriteLine("4.- Volver");

            int option = ChooseNumberOption(4);
            Structure structure;

            switch (option)
            {
                case 1:
                    structure = collectorStructure;
                    break;
                case 2:
                    structure = maintenanceStructure;
                    break;
                case 3:
                    structure = defenseStructure;
                    break;
                case 4:
                default:
                    return;
            }

            if (!CanAfford(structure)) return;

            Console.WriteLine($"\nSelecciona el nodo donde contruir la estructura (Minimo 1 - Maximo {nodes.Count})");
            int nodeIndex = ChooseNumberOption(nodes.Count);

            BuyEntity(structure);

            structure.SetPosition(nodes[nodeIndex - 1]);
            nodes[nodeIndex - 1].playerStructures.Add(structure);
            playerStructures.Add(structure);

            Console.WriteLine($"\nSe ha construido una {structure._name} en el nodo {nodeIndex}");
        }

        private void CreateUnit()
        {
            if (!CheckCanBuyEntity()) return;

            Console.WriteLine($"\nCuentas con ${money}");

            Soldier soldier = new Soldier(null);
            Tank tank = new Tank(null);
            Helicopter helicopter = new Helicopter(null);

            Console.WriteLine("\nSelecciona la unidad a desplegar");
            Console.WriteLine($"1.- Soldado - Precio: ${soldier.price}");
            Console.WriteLine($"2.- Tanque - Precio: ${tank.price}");
            Console.WriteLine($"3.- Helicoptero - Precio: ${helicopter.price}");
            Console.WriteLine("4.- Volver");

            int option = ChooseNumberOption(4);
            Unit unit;

            switch (option)
            {
                case 1:
                    unit = soldier;
                    break;
                case 2:
                    unit = tank;
                    break;
                case 3:
                    unit = helicopter;
                    break;
                case 4:
                default:
                    return;
            }

            if (!CanAfford(unit)) return;

            //Las unidades se despliegan en la base

            BuyEntity(unit);

            unit.SetPosition(nodes[0]);
            nodes[0].playerUnits.Add(unit);
            playerUnits.Add(unit);

            Console.WriteLine($"\nSe ha desplegado un {unit._name} en la base aliada");
        }

        private void PassTurn()
        {
            //Nada
        }

        private void GainMoneyEndOfTurn()
        {
            int moneyGained = 0;

            foreach(Structure structure in playerStructures)
            {
                if (!(structure is CollectorStructure)) continue;
                
                moneyGained += ((CollectorStructure)structure).GetMoneyPerTurn();          
            }

            money += moneyGained;

            if(moneyGained > 0)
            {
                Console.WriteLine($"\nHas recolectado ${moneyGained} proveniente de tus Estructuras de Recoleccion");
            }
        }

        #endregion

        #region MoveUnits
        private void MovePlayerUnits()
        {
            foreach(Unit playerUnit in playerUnits)
            {
                MoveUnit(playerUnit, true);
            }
        }
        private void MoveEnemyUnits()
        {
            foreach (Unit enemyUnit in enemyUnits)
            {
                MoveUnit(enemyUnit, false);
            }
        }

        private void MoveUnit(Unit unit, bool isPlayerUnit)
        {
            for(int i = 1; i <= unit.GetSpeed(); i++) //Repetimos el proceso dependiendo de la velocidad de la unidad (número maximo de casillas que se puede mover en 1 turno)
            {
                int positionIndex = GetNodeIndexInList(unit.GetPosition());
                Node previousPosition = unit.GetPosition();

                if (isPlayerUnit)
                {
                    if (previousPosition.enemyUnits.Count > 0) return; //Si se encuentra con una unidad enemiga en la posicion, se deja de mover
                    if(previousPosition == nodes[^1]) return; //Si se encuentra en el ultimo nodo (base enemiga), se deja de mover

                    previousPosition.playerUnits.Remove(unit);

                    positionIndex++;
                }
                else
                {
                    if (previousPosition.playerStructures.Count > 0) return; //Si se encuentra con una estructura del jugador en la posicion, se deja de mover
                    if (previousPosition.playerUnits.Count > 0) return; //Si se encuentra con una unidad del jugador en la posicion, se deja de mover
                    if (previousPosition == nodes[0]) return; //Si se encuentra en el primer nodo (base del jugador) se deja de mover

                   previousPosition.enemyUnits.Remove(unit);

                    positionIndex--;
                }

                Node newPosition = nodes[positionIndex];

                if (isPlayerUnit)
                {
                    newPosition.playerUnits.Add(unit);
                }
                else
                {
                    newPosition.enemyUnits.Add(unit);
                }

                unit.SetPosition(newPosition);
            }
        }
        #endregion

        #region Combat
        private void HandleCombat(bool playerPriority)
        {         
            if(playerPriority)
            {
                PlayerAttackEnemy();
                EnemyAttackPlayer();
            }
            else
            {
                EnemyAttackPlayer();
                PlayerAttackEnemy();
            }
        }

        private void PlayerAttackEnemy()
        {
            DefenseStructuresAttackEnemy();
            PlayerUnitsAttackEnemy();
        }

        private void DefenseStructuresAttackEnemy()
        {
            foreach (Structure structure in playerStructures)
            {
                if (!(structure is DefenseStructure)) continue;

                Unit enemyUnit = GetRandomAliveUnitInList(enemyUnits);

                if (enemyUnit == null) return;

                ((DefenseStructure)structure).DealDamage(enemyUnit);
            }

            CheckEnemyUnitsDead();
        }

        private void PlayerUnitsAttackEnemy()
        {
            foreach (Node node in nodes)
            {
                foreach (Unit playerUnit in node.playerUnits)
                {
                    Unit enemyUnitInNode = GetRandomAliveUnitInListUnitCanAttack(playerUnit, node.enemyUnits);

                    if (enemyUnitInNode == null) continue;

                    playerUnit.DealDamage(enemyUnitInNode);
                }
            }

            CheckEnemyUnitsDead();
        }

        private void EnemyAttackPlayer()
        {
            EnemyAttackPlayerUnits();
            EnemyAttackPlayerDefenseStructures();
            EnemyAttackPlayerCollectorStructures();
            EnemyAttackPlayerMaintenanceStructures();
        }

        private void EnemyAttackPlayerUnits()
        {
            foreach (Node node in nodes)
            {
                foreach (Unit enemyUnit in node.enemyUnits)
                {
                    Unit playerUnitInNode = GetRandomAliveUnitInListUnitCanAttack(enemyUnit, node.playerUnits);

                    if (playerUnitInNode == null) continue;

                    enemyUnit.DealDamage(playerUnitInNode);
                }
            }

            CheckPlayerUnitsDead();
        }

        private void EnemyAttackPlayerDefenseStructures()
        {
            foreach (Node node in nodes)
            {
                foreach (Unit enemyUnit in node.enemyUnits)
                {
                    Structure playerStructureInNode = GetRandomAliveStructureInList(node.playerStructures);

                    if (playerStructureInNode == null) break;
                    if (!(playerStructureInNode is DefenseStructure)) continue;

                    enemyUnit.DealDamage(playerStructureInNode);
                }
            }

            CheckPlayerStructuresDead();
        }

        private void EnemyAttackPlayerCollectorStructures()
        {
            foreach (Node node in nodes)
            {
                foreach (Unit enemyUnit in node.enemyUnits)
                {
                    Structure playerStructureInNode = GetRandomAliveStructureInList(node.playerStructures);

                    if (playerStructureInNode == null) break;
                    if (!(playerStructureInNode is CollectorStructure)) continue;

                    enemyUnit.DealDamage(playerStructureInNode);
                }
            }

            CheckPlayerStructuresDead();
        }

        private void EnemyAttackPlayerMaintenanceStructures()
        {
            foreach (Node node in nodes)
            {
                foreach (Unit enemyUnit in node.enemyUnits)
                {
                    Structure playerStructureInNode = GetRandomAliveStructureInList(node.playerStructures);

                    if (playerStructureInNode == null) break;
                    if (!(playerStructureInNode is MaintenanceStructure)) continue;

                    enemyUnit.DealDamage(playerStructureInNode);
                }
            }

            CheckPlayerStructuresDead();
        }
        #endregion

        #region Generators

        private void GenerateNodes()
        {
            for(int i=1; i<=NODES_NUMBER; i++)
            {
                Node node = new Node();
                nodes.Add(node);
            }
        }

        private void GenerateInitialMaintenanceStructures()
        {
            for(int i=0; i<INITIAL_MAINTENANCE_STRUCTURES; i++)
            {
                MaintenanceStructure maintenanceStructure = new MaintenanceStructure(null);

                maintenanceStructure.SetPosition(nodes[i]);
                nodes[i].playerStructures.Add(maintenanceStructure);
                playerStructures.Add(maintenanceStructure);
            }
        }

        private void GenerateEnemyUnits()
        {
            if (enemyUnits.Count > 0)
            {
                Console.WriteLine($"\nEl enemigo no despliega unidades");
                return; //Solo genera unidades si no tiene unidades
            }

            enemyUnitsTimesGenerated++;
            int numberOfUnits = GetFibonacciTerm(enemyUnitsTimesGenerated);

            if(numberOfUnits > 0)
            {
                Console.WriteLine($"\nEl enemigo despliega las siguientes unidades en su base:");
            }
            else
            {
                Console.WriteLine($"\nEl enemigo no despliega unidades");
            }


            for (int i=1; i<=numberOfUnits; i++)
            {
                Unit unit = GenerateRandomUnit(turnNumber); //Se despliegan en la base enemiga

                unit.SetPosition(nodes[^1]);
                nodes[^1].enemyUnits.Add(unit);
                enemyUnits.Add(unit);

                Console.WriteLine($"{unit._name}");
            }        
        }

        private Unit GenerateRandomUnit(int turnNumber)
        {
            int turnInterpolator = turnNumber;
            turnInterpolator = turnInterpolator < 1? 1 : turnInterpolator;
            turnInterpolator = turnInterpolator > TURN_TO_REACH_MAX_PROB? TURN_TO_REACH_MAX_PROB: turnInterpolator;

            float soldierProbability = INIT_PROB_SOLDIER + (FINAL_PROB_SOLDIER - INIT_PROB_SOLDIER)*(float)(turnInterpolator-1)/(TURN_TO_REACH_MAX_PROB-1);
            float tankProbability = INIT_PROB_TANK + (FINAL_PROB_TANK - INIT_PROB_TANK) * (float)(turnInterpolator - 1) / (TURN_TO_REACH_MAX_PROB - 1);
            float helicopterProbability = INIT_PROB_HELICOPTER + (FINAL_PROB_HELICOPTER - INIT_PROB_HELICOPTER) * (float)(turnInterpolator - 1) / (TURN_TO_REACH_MAX_PROB - 1);

            float soldierThreshold = soldierProbability;
            float tankThreshold = soldierThreshold + tankProbability;
            float helicopterTreshold = tankThreshold + helicopterProbability;

            Random random = new Random();
            double randomValue = random.NextDouble();

            if(randomValue <= soldierThreshold)
            {
                return new Soldier(null);
            }
            else if(randomValue <= tankThreshold) 
            {
                return new Tank(null);
            }
            else
            {
                return new Helicopter(null);
            }
        }

        private int GetFibonacciTerm(int termNumber)
        {
            int a0 = 0;
            int a1 = 1;
            int a2 = a0 + a1;

            if (termNumber <= 1) return a0;
            if (termNumber == 2) return a1;

            for (int i = 3; i <= termNumber; i++)
            {
                a2 = a1 + a0;

                a0 = a1;
                a1 = a2;
            }

            return a2;
        }
        #endregion

        #region OptionHandlers
        private int ChooseNumberOption(int maxOptionNumber)
        {
            bool validOption = false;
            int option = 0;

            while (!validOption)
            {
                try
                {
                    int desiredOption = int.Parse(Console.ReadLine());

                    if (desiredOption > 0 && desiredOption <= maxOptionNumber)
                    {
                        validOption = true;
                        option = desiredOption;
                    }
                    else
                    {
                        Console.WriteLine($"Selecciona una opcion válida:");
                    }
                }
                catch
                {
                    Console.WriteLine($"Selecciona una opcion válida:");
                }
            }

            return option;
        }
        private bool ChooseYNOption()
        {
            Console.WriteLine("(Y/N):");

            bool desiredOption = false;
            bool validYN = false;

            while (!validYN)
            {
                string answer = Console.ReadLine();

                switch (answer)
                {
                    case "Y":
                    case "y":
                        validYN = true;
                        desiredOption = true;
                        break;
                    case "N":
                    case "n":
                        validYN = true;
                        desiredOption = false;
                        break;
                    default:
                        Console.WriteLine("\nEscoge una opción válida:");
                        break;
                }
            }

            return desiredOption;
        }
        #endregion

        #region Checks
        private bool CheckPlayerWon()
        {
            if (nodes[^1].playerUnits.Count > 0)
            {
                Console.WriteLine("\nHas llegado a la base del enemigo!");
                return true;
            }

            return false;
        }

        private bool CheckPlayerDefeated()
        {
            foreach(Node node in nodes)
            {
                if (node.playerStructures.Count > 0) return false;
            }

            Console.WriteLine("\nEl enemigo ha destrudo todas tus estructuras!");
            return true;

        }

        private bool CheckCanBuyEntity()
        {
            int maxEntities = 0;

            foreach(Structure structure in playerStructures)
            {
                if (!(structure is MaintenanceStructure)) continue;
                maxEntities += ((MaintenanceStructure)structure).GetUnitsPerStructure();
            }

            int currentEntities = playerStructures.Count + playerUnits.Count;

            if(currentEntities>= maxEntities)
            {
                Console.WriteLine($"\nSolo puedes construir un maximo de {maxEntities} unidades o estructuras - Cuentas con {playerUnits.Count} unidades y {playerStructures.Count} estructuras");
                return false;
            }

            Console.WriteLine($"\nPuedes construir {maxEntities - currentEntities} unidades o estructuras más");
            return true;
        }

        private bool CheckEntityDeath(Entity entity)
        {
            if(entity is Unit)
            {
                playerUnits.Remove((Unit)entity);
                enemyUnits.Remove((Unit)entity);

                Node deathPos = entity.GetPosition();

                deathPos.playerUnits.Remove((Unit)entity);
                deathPos.enemyUnits.Remove((Unit)entity);
            }

            if(entity is Structure)
            {
                playerStructures.Remove((Structure)entity);

                Node deathPos = entity.GetPosition();

                deathPos.playerStructures.Remove((Structure)entity);
            }

            return true;
        }

        private void CheckPlayerStructuresDead()
        {
            List<Structure> deadStructures = new List<Structure> ();

            foreach(Structure structure in playerStructures)
            {
                if (structure.IsAlive()) continue;
                deadStructures.Add(structure);

                Console.WriteLine($"Una {structure._name} fue destruida en el Nodo {GetNodeIndexInList(structure.GetPosition()) + 1}");
            }

            foreach(Structure deadStructure in deadStructures)
            {
                playerStructures.Remove(deadStructure);

                Node deathPos = deadStructure.GetPosition();

                deathPos.playerStructures.Remove(deadStructure);
            }
        }

        private void CheckPlayerUnitsDead()
        {
            List<Unit> deadUnits = new List<Unit>();

            foreach (Unit unit in playerUnits)
            {
                if (unit.IsAlive()) continue;
                deadUnits.Add(unit);

                Console.WriteLine($"Un {unit._name} aliado fue destruido en el Nodo {GetNodeIndexInList(unit.GetPosition()) + 1}");
            }

            foreach (Unit deadUnit in deadUnits)
            {
                playerUnits.Remove(deadUnit);

                Node deathPos = deadUnit.GetPosition();

                deathPos.playerUnits.Remove(deadUnit);
            }
        }

        private void CheckEnemyUnitsDead()
        {
            Console.Write("\n");

            List<Unit> deadUnits = new List<Unit>();

            foreach (Unit unit in enemyUnits)
            {
                if (unit.IsAlive()) continue;
                deadUnits.Add(unit);

                Console.WriteLine($"Un {unit._name} enemigo fue destruido en el Nodo {GetNodeIndexInList(unit.GetPosition()) + 1}");
            }

            foreach (Unit deadUnit in deadUnits)
            {
                enemyUnits.Remove(deadUnit);

                Node deathPos = deadUnit.GetPosition();

                deathPos.enemyUnits.Remove(deadUnit);
            }
        }

        private bool CheckAllUnitsDeadInList(List<Unit> units)
        {
            foreach(Unit unit in units)
            {
                if (unit.IsAlive()) return false;
            }

            return true;
        }

        private bool CheckUnitCanAttackUnitInList(Unit attackerUnit, List<Unit> units)
        {
            foreach (Unit unit in units)
            {
                if (!unit.IsAlive()) continue;
                if (!attackerUnit.CanAttackEntity(unit)) continue;

                return true;
            }

            return false;
        }

        private bool CheckAllStructuresDeadInList(List<Structure> structures)
        {
            foreach (Structure structure in structures)
            {
                if (structure.IsAlive()) return false;
            }

            return true;
        }
        #endregion

        #region Utilities
        private int GetNodeIndexInList(Node node)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (node == nodes[i]) return i;
            }

            return 0;
        }


        private bool CanAfford(Entity entity)
        {
            if(entity.price > money)
            {
                Console.WriteLine($"Necesitas ${entity.price} para comprar el/la {entity._name}. Cuentas con ${money}");
                return false;
            }

            return true;
        }

        private void BuyEntity(Entity entity) => money -= entity.price;

        private Unit GetRandomAliveUnitInList(List<Unit> units)
        {
            if (units.Count == 0) return null;

            if (CheckAllUnitsDeadInList(units)) return null;

            bool foundUnit = false;
            int index = 0;

            while (!foundUnit)
            {
                Random random = new Random();
                int randomIndex = random.Next(units.Count);

                if (units[randomIndex].IsAlive())
                {
                    foundUnit = true;
                    index = randomIndex;
                }
            }

            return units[index];
         
        }

        private Unit GetRandomAliveUnitInListUnitCanAttack(Unit attackerUnit, List<Unit> units)
        {
            if (units.Count == 0) return null;

            if (!CheckUnitCanAttackUnitInList(attackerUnit,units)) return null;

            bool foundUnit = false;
            int index = 0;

            while (!foundUnit)
            {
                Random random = new Random();
                int randomIndex = random.Next(units.Count);

                if (units[randomIndex].IsAlive() && attackerUnit.CanAttackEntity(units[randomIndex]))
                {
                    foundUnit = true;
                    index = randomIndex;
                }
            }

            return units[index];
        }

        private Structure GetRandomAliveStructureInList(List<Structure> structures)
        {
            if (structures.Count == 0) return null;

            if (CheckAllStructuresDeadInList(structures)) return null;

            bool foundStructures = false;
            int index = 0;

            while (!foundStructures)
            {
                Random random = new Random();
                int randomIndex = random.Next(structures.Count);
                if (structures[randomIndex].IsAlive())
                {
                    foundStructures = true;
                    index = randomIndex;
                }
            }

            return structures[index];
        }
        #endregion
    }
}
