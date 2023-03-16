using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class GameState
    {
        public int Rows { get; }
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }
        private readonly LinkedList<Position> snakePosition = new LinkedList<Position>();
        private readonly Random random = new Random();
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();

        public GameState(int rows, int col)
        {
            Rows = rows;
            Columns = col;
            Grid = new GridValue[rows, col];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for(int c = 1; c < 4; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePosition.AddFirst(new Position(r, c));
            }
        }

        private IEnumerable<Position> GetEmptyPosition()
        {
            for(int r = 0;r < Rows; r++)
            {
                for(int c = 0; c < Columns; c++)
                {
                    if(Grid[r,c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> emptyPosition = new List<Position>(GetEmptyPosition());
            if(emptyPosition.Count == 0)
            {
                return;
            }

            Position pos = emptyPosition[random.Next(emptyPosition.Count)];
            Grid[pos.Row, pos.Column] = GridValue.Food;
        }

        public Position GetSnakeHeadPosition()
        {
            return snakePosition.First.Value;
        }

        public Position GetTailPosition()
        {
            return snakePosition.Last.Value;
        }

        public IEnumerable<Position> GetSnakePosition()
        {
            return snakePosition;
        }

        private void AddHead(Position pos)
        {
            snakePosition.AddFirst(pos);
            Grid[pos.Row, pos.Column] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePosition.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePosition.RemoveLast();
        }

        private Direction GetLasDirection()
        {
            if(dirChanges.Count == 0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private bool canChangeDirection(Direction newDir)
        {
            if(dirChanges.Count == 2)
            {
                return false;
            }
            Direction lastdir = GetLasDirection();
            return newDir != lastdir && newDir != lastdir.Opposite();
        }
        public void ChangeDirection(Direction dir)
        {
            if (canChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
            
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
        }

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }

            if(newHeadPos == GetTailPosition())
            {
                return GridValue.Empty;
            }

            return Grid[newHeadPos.Row, newHeadPos.Column];
        }

        public void Move()
        {
            if(dirChanges.Count > 0)
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHeadPos = GetSnakeHeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if(hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if(hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if(hit == GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
