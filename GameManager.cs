﻿namespace ConsoleProject2
{

    public class GameManager : InputManager
    {

        Player player;


        public EnemyInfo curEnemyInfo;

        Queue<Enemy> disabledEnemyQueue = new Queue<Enemy>();

        public List<Enemy> activeEnemies = new List<Enemy>();

        Queue<Tower> disabledTowerQueue = new Queue<Tower>();

        List<Tower> activeTowers = new List<Tower>();

        private GameManager()
        {



            InitObj();
            TimeManager.RoundEvent += SetEnemyStagePerSecond;
            TimeManager.NextStage += CurrentEnemyInfoSet;

        }

        public void ReStart()
        {
            
            disabledEnemyQueue.Clear();
            disabledTowerQueue.Clear();
            activeEnemies.Clear();
            activeTowers.Clear();
            player.PosX = Map.centerPos;
            player.PosY = Map.centerPos;

           
            InitObj();
            TimeManager.RoundEvent += SetEnemyStagePerSecond;
            TimeManager.NextStage += CurrentEnemyInfoSet;

            CurrentEnemyInfoSet();
        }

        public void SetPlayer(Player player)
        {
            this.player = player;
        }


        public void CheckEnemy()
        {
            if (activeEnemies.Count <= 0)
            {
                return;
            }
           


            int bossIndex = 0;
            for (int i = 0; i < activeEnemies.Count; i++)
            {

                
                if (activeEnemies[i].BossFlag)
                {
                   
                    bossIndex = i;
                }
                else if (10 + PixelType.ENEMY + 1 > Map.pixelNum[activeEnemies[i].PosY, activeEnemies[i].PosX]
                    && Map.pixelNum[activeEnemies[i].PosY, activeEnemies[i].PosX] >= PixelType.ENEMIES)
                {
                 
                    Map.pixelNum[activeEnemies[i].PosY, activeEnemies[i].PosX]++;
                }
                else if (Map.pixelNum[activeEnemies[i].PosY, activeEnemies[i].PosX] == PixelType.ENEMY)
                {
                 
                    Map.pixelNum[activeEnemies[i].PosY, activeEnemies[i].PosX] = PixelType.ENEMIES;
                }
             
                else
                {
                  
                    Map.pixelNum[activeEnemies[i].PosY, activeEnemies[i].PosX] = PixelType.ENEMY;
                }

            }
            if (CheckBossStage())
            {
                
                Map.pixelNum[activeEnemies[bossIndex].PosY, activeEnemies[bossIndex].PosX] = PixelType.BOSS;
            }
          
        }
        public void CheckTower()
        {
            for (int i = 0; i < activeTowers.Count; i++)
            {
                if(0.1<= activeTowers[i].AtkTick && activeTowers[i].AtkTick < activeTowers[i].AtkSpeed)

                {
                    Map.pixelNum[activeTowers[i].PosY, activeTowers[i].PosX] = PixelType.COOLDOWNTOWER;
                }
                else
                {
                    Map.pixelNum[activeTowers[i].PosY, activeTowers[i].PosX] = activeTowers[i].Grade;
                }
               
                Map.pixel[activeTowers[i].PosY, activeTowers[i].PosX] = activeTowers[i].Type;

            }
        }

        public bool CheckTowerCursor()
        {
            bool check = false;
            for (int i = 0; i < activeTowers.Count; i++)
            {
                if (activeTowers[i].PosX == player.PosX && activeTowers[i].PosY == player.PosY)
                {
                    check = true;
                    break;
                }

            }
            return check;
        }




        public void ChechkPlayer()
        {
            int playerNum = Map.pixelNum[player.PosY, player.PosX];
            if (playerNum == PixelType.BOSS)
            {
                Map.pixelNum[player.PosY, player.PosX] = PixelType.OVERLAPPLAYERBOSS;
            }
            else if ( playerNum  >= PixelType.ENEMY && playerNum < PixelType.RANDOMUSERSPACE)
            {
                Map.pixelNum[player.PosY, player.PosX] = PixelType.OVERLAPPLAYERENEMY + playerNum;
            }
            else if (playerNum == PixelType.RANDOMENEMYPATH)
            {
                Map.pixelNum[player.PosY, player.PosX] = PixelType.OVERLAPPLAYERPATH ;
            }

            else
            {
                Map.pixelNum[player.PosY, player.PosX] = PixelType.PLAYER;
            }

            
        }


        public void CheckDraw()
        {
            CheckEnemy();
            CheckTower();
            ChechkPlayer();
        }

        public void AttackCollider(Tower tower)
        {
            if (StageManager.gameoverCount == StageManager.enemyLimitCount && RandomTowerDefense.mode == 0)
            {
                return;
            }


          

           
            for (int e = 0; e < activeEnemies.Count; e++)
            {
                if (activeEnemies[e] != null)
                {
                    for (int i = tower.PosY - tower.Range; i < tower.PosY + tower.Range; i++)
                    {
                        for (int j = tower.PosX - tower.Range; j < tower.PosX + tower.Range; j++)
                        {
                                if(i < 0 || j <0 ||
                                   i > Map.height || j >Map.width
                                )
                                {
                                    continue;
                                }
                                if (e >= activeEnemies.Count)
                                {
                                    return;
                                }
                                if (activeEnemies[e].PosY == i && activeEnemies[e].PosX == j)
                                {
                                    if (activeEnemies[e].CurHp <= 0)
                                    {
                                        continue;
                                    }

                                    activeEnemies[e].TakeDamage(tower.Attack);
                                    tower.AtkTick = 0;

                                  

                                    return;
                                }

                          
                        }
                    }

                }


            }




        }

        public bool SetTower(int grade)
        {

            if (disabledTowerQueue.Count <= 0)
            {
                return false;
            }



            Random random = new Random();

            Tower tower = disabledTowerQueue.Dequeue();
            tower.AttackEvent += AttackCollider;

           
            
                switch (grade)
                {
                    case PixelType.GRADE_C:
                        tower.RandomInit(random.Next(PixelType.GRADE_B_START, PixelType.GRADE_B_END));
                        break;
                    case PixelType.GRADE_B:
                        tower.RandomInit(random.Next(PixelType.GRADE_A_START, PixelType.GRADE_A_END));
                        break;
                    case PixelType.GRADE_A:
                        tower.RandomInit(random.Next(PixelType.GRADE_S_START, PixelType.GRADE_S_END));
                        break;
                    default:
                        tower.RandomInit(random.Next(PixelType.GRADE_C_START, PixelType.GRADE_C_END));
                        break;

                }

            




            tower.TowerInitStatus(player.PosX, player.PosY);



            tower.DisableEvent += DisableTower;
            TimeManager.AddTowerAttackEvent(tower);

            activeTowers.Add(tower);

            return true;
        }

        public void DisableTower(Tower tower)
        {
            tower.DisableEvent -= DisableTower;
            TimeManager.RemoveTowerAttackEvent(tower);
            activeTowers.Remove(tower);
            disabledTowerQueue.Enqueue(tower);
        }

        public void SetEnemy()
        {
            if (disabledEnemyQueue.Count <= 0)
            {
                return;
            }
            Enemy enemy = disabledEnemyQueue.Dequeue();
            enemy.EnemyInitStatus(curEnemyInfo.hp, curEnemyInfo.moveSpeed, curEnemyInfo.dropGold);

            if(CheckBossStage())
            {
                enemy.BossFlag = true;
            }

            enemy.DisableEvent += DisableEnemy;
            TimeManager.AddEnemyMoveEvent(enemy);

            activeEnemies.Add(enemy);

            if (RandomTowerDefense.mode == 0)
            {
                StageManager.gameoverCount--;
            }

    
        }
        public void DisableEnemy(Enemy enemy)
        {

            
            enemy.DisableEvent -= DisableEnemy;
            TimeManager.RemoveEnemyMoveEvent(enemy);

            activeEnemies.Remove(enemy);

            disabledEnemyQueue.Enqueue(enemy);


            if (RandomTowerDefense.mode == 0)
            {
                StageManager.gameoverCount++;
            }
            else if (RandomTowerDefense.mode == 1)
            {
                // 적이 죽은게 아니고 목적지에 도착했을때
                if (enemy.CurHp > 0)
                {
                    StageManager.gameoverCount--;

                    if (enemy.BossFlag)
                    {
                        StageManager.gameoverCount = 0;
                    }

                    return;
                }
            }

            if (enemy.BossFlag)
            {
                RandomTowerDefense.clearFlag = true;
            }

            
            StageManager.SetGold(enemy.DropGold);


        }



        public void InitObj()
        {
            if (RandomTowerDefense.mode == 1)
            {
               
                for (int i = 0; i < StageManager.enemyLimitCount*2; i++)
                {
                    Enemy enemy = new Enemy();
                    enemy.RandomPath = Map.randomPath;
                    disabledEnemyQueue.Enqueue(enemy);

                }
            }
            else
            {

                for (int i = 0; i < StageManager.enemyLimitCount; i++)
                {
                    disabledEnemyQueue.Enqueue(new Enemy());

                }
            }
            for (int i = 0; i < StageManager.towerLimitCount; i++)
            {
                disabledTowerQueue.Enqueue(new Tower());
            }

          
        }

        public void SetEnemyStagePerSecond()
        {
            SetEnemy();

        }

        public void CurrentEnemyInfoSet()
        {
            curEnemyInfo = StageManager.EnemyInformations[StageManager.currentStage];
        }

        public bool CheckBossStage()
        {
            if (StageManager.currentStage == 10)
            {
                return true;
            }

            return false;
        }

        private static GameManager gmSingleton;
        public static GameManager Instance()
        {
            if (gmSingleton == null)
            {
                gmSingleton = new GameManager();


            }

            return gmSingleton;

        }

        public void InputKey()
        {

            //매끄러운 키 입력 구현
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo consoleKey = Console.ReadKey(true);

                switch (consoleKey.Key)
                {
                    case ConsoleKey.RightArrow:
                        MoveCursor(1, 0);
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveCursor(-1, 0);

                        break;

                    case ConsoleKey.UpArrow:
                        MoveCursor(0, -1);

                        break;
                    case ConsoleKey.DownArrow:
                        MoveCursor(0, 1);
                        break;
                    case ConsoleKey.Q:
                        GachaTower();
                        break;
                    case ConsoleKey.E:
                        MergeTower();
                        break;
                    case ConsoleKey.T:
                        SellTower();
                        break;
                    default:

                        break;
                }
            }
        }

        public void MoveCursor(int posX, int posY)
        {
            if (RandomTowerDefense.mode == 0)
            {

                if (player.PosX + posX < Map.centerPos || player.PosX + posX > Map.widthCenter)
                    return;
                if (player.PosY + posY < Map.centerPos || player.PosY + posY > Map.heightCenter)
                    return;

                player.PosX += posX;
                player.PosY += posY;
            }
            else if (RandomTowerDefense.mode == 1)
            {
                if (player.PosX + posX < Map.borderPos+1 || player.PosX + posX > Map.widthBorder-1)
                    return;
                if (player.PosY + posY < Map.borderPos+1 || player.PosY + posY > Map.heightBorder-1)
                    return;

                player.PosX += posX;
                player.PosY += posY;
            }

        }

        public void GachaTower()
        {
            if (StageManager.GetGold() >= 50 &&  Map.pixelNum[player.PosY, player.PosX] < PixelType.OVERLAPPLAYERENEMY)
            {
                if (CheckTowerCursor() == false)
                {
                    Random random = new Random();
                    double ran = random.Next(1, 100);

                    if(0 <= ran && ran <= 2.5)
                    {
                        SetTower(PixelType.GRADE_B);
                    }
                    else if (2.5 <= ran && ran < 11.6)
                    {
                        SetTower(PixelType.GRADE_C);
                    }
                    else if (11.6 <= ran && ran < 100)
                    {
                        SetTower(PixelType.GRADE_C_START);
                    }


                    StageManager.SetGold(-50);
                }
            }
        }

        public void SellTower()
        {
            for (int i = 0; i < activeTowers.Count; i++)
            {
                if (activeTowers[i].PosX == player.PosX && activeTowers[i].PosY == player.PosY)
                {
                    activeTowers[i].Disable();
                    StageManager.SetGold(30);
                    break;
                }

            }
        }

        public void MergeTower()
        {
            char type = 'N';
            Tower[] towerIndex = new Tower[3];
            for (int i = 0; i < activeTowers.Count; i++)
            {
                if (activeTowers[i].PosX == player.PosX && activeTowers[i].PosY == player.PosY)
                {
                    if (activeTowers[i].Grade >= PixelType.GRADE_S )
                    {
                        return;
                    }

                    type = activeTowers[i].Type;
                    towerIndex[0] = activeTowers[i];
                    break;
                }

            }
            if (type == 'N')
                return;



            int cnt = 1;
            for (int i = 0; i < activeTowers.Count; i++)
            {
                if (activeTowers[i].Type == type)
                {
                    if (towerIndex[0] == activeTowers[i])
                    {
                        continue;
                    }

                    towerIndex[cnt] = activeTowers[i];
                    cnt++;

                    if (cnt == 3)
                    {
                        break;
                    }
                }

            }

            if (cnt == 3)
            {
                int grade = towerIndex[0].Grade;
                for (int i = 0; i < cnt; i++)
                {

                    towerIndex[i].Disable();

                }


                SetTower(grade);

            }



        }
    }
}
