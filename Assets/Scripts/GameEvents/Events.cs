public class GameEvent { }

public class OnDieResult : GameEvent
{
    public int result;
}

public class OnCreatePlayer : GameEvent
{
    public Player newPlayer;
}

public class OnPlayerHealthChange : GameEvent
{
    public int dealtaHealth;
    public Order? turnOrder;
}

public class OnTurnChange : GameEvent
{
    public Order turnOrder;
}

public class OnCameraShake : GameEvent
{
    public float duration;
    public float strength = 3;
    public int vibrato = 10;
    public float randomness = 90;
}

public class OnRoundEnd : GameEvent
{
    public Order winner;
}

public class OnGameEnd : GameEvent
{
    public Player winner;
}

public class OnResetTurn : GameEvent
{
    
}