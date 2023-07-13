namespace Utilities
{
    public static class Constants
    {
        public enum Direction
        {
            Up,
            Left,
            Down,
            Right
        }

        public enum CellType
        {
            Empty,
            Wall,
            Red,
            Blue,
        } 
        
        public enum GameResult
        {
            Playing,
            Draw,
            Player1Win,
            Player2Win,
        }
        
        public enum TankType
        {
            Red,
            Blue
        }
    }
}