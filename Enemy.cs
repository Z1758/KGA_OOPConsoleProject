﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleProject2
{
    public enum EnemyMoveState 
    {
        Down, Right, Up, Left, END
    }

    public class Enemy : IDynamicObject
    {
        private int posX;
        private int posY;
        private int maxHp;
        private int curHp;
        private double moveSpeed;
        private int dropGold;
        private bool bossFlag;
        private EnemyMoveState moveState = EnemyMoveState.Down;

        public int moveTick;

        private List<Point> randomPath;

        public int CurHp { get => curHp;  }
        public int PosX { get => posX;  }
        public int PosY { get => posY;  }
        public int DropGold { get => dropGold;  }
        public bool BossFlag { get => bossFlag; set => bossFlag = value; }
        public int MaxHp { get => maxHp; set => maxHp = value; }
        public List<Point> RandomPath { get => randomPath; set => randomPath = value; }

        public int pathListCnt;

        public event Action<Enemy> DisableEvent;

        public Enemy()
        {
            posX = 1; posY = 1;
            MaxHp = 0;
            curHp = MaxHp;
            moveSpeed = 0;
            dropGold = 0;
            bossFlag = false;
            moveTick = 0;
            pathListCnt = 0;
            RandomPath = null;
        }

        public void EnemyInitStatus(int maxHp, double moveSpeed, int dropGold )
        {
            moveState = EnemyMoveState.Down;
            posX = Map.enemyPathPos; posY = Map.enemyPathPos;

            moveTick = 0;

            this.MaxHp = maxHp;
            curHp = maxHp;
            this.moveSpeed = moveSpeed;
            this.dropGold = dropGold;
            pathListCnt = 0;
        }

        public void MoveAction(object sender, System.Timers.ElapsedEventArgs e)
        {

            moveTick++;


            if (moveTick < moveSpeed)
            {
                return;
            }
            moveTick = 0;

            if (RandomPath != null)
            {
                if (RandomPath.Count > pathListCnt)
                {
                    posY = RandomPath[pathListCnt].y;
                    posX = RandomPath[pathListCnt].x;
                    pathListCnt++;
                }
                else if (RandomPath.Count == pathListCnt && moveState != EnemyMoveState.END)
                {
                    moveState = EnemyMoveState.END;
                    Disable();
                }

            }
            else
            {

                switch (moveState)
                {
                    case EnemyMoveState.Down:
                        GoDown();
                        break;
                    case EnemyMoveState.Right:
                        GoRight();
                        break;
                    case EnemyMoveState.Up:
                        GoUp();
                        break;
                    case EnemyMoveState.Left:
                        GoLeft();
                        break;


                }
            }
        }
    
        public void TakeDamage(int atk)
        {
            if (CurHp <= 0 || moveState == EnemyMoveState.END)
            {
                return;
            }
            curHp -= atk;
   
            if (CurHp <= 0)
            {
                Disable();
            }
        }


        public void Disable()
        {
            DisableEvent(this);
        }
   
        public void GoDown()
        {
            posY++;

            if (PosY == Map.heightEnemyPath)
            {
                moveState = EnemyMoveState.Right;
            }


        }
        public void GoRight()
        {
            posX++;
            if (PosX == Map.widthEnemyPath)
            {
                moveState = EnemyMoveState.Up;
            }



        }

        public void GoUp()
        {
            posY--;
            if (PosY == Map.enemyPathPos)
            {
                moveState = EnemyMoveState.Left;
            }

        }

        public void GoLeft()
        {
            posX--;
            if (PosX == Map.enemyPathPos)
            {
                moveState = EnemyMoveState.Down;
            }

        }

      
    }
}
