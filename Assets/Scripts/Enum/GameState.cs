public enum GameState
{
    Setup,        // инициализация (кладём героя, стартовую руку)
    PlayerTurn,   // ход игрока
    EnemyTurn,    // ход противника
    GameOver
}