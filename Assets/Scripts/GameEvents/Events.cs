using System.Collections.Generic;

public class GameEvent { }

public class OnCameraShake : GameEvent
{
    public float duration;
    public float strength = 3;
    public int vibrato = 10;
    public float randomness = 90;
}

public class OnDieResult : GameEvent
{
    public int result;
}

public class OnCreateActor : GameEvent
{
    public Actor newActor;
}

public class OnActorHealthChange : GameEvent
{
    public int dealtaHealth;
    public int? turnIndex;
}

public class OnImmediateTurnChange : GameEvent
{
    public int turnIndex;
}

public class OnTurnStart : GameEvent
{
    public Actor currentActor;
}

public class OnRoundEnd : GameEvent
{
    public int winner;
}

public class OnGameEnd : GameEvent
{
    public Actor winner;
}

public class OnNextRound : GameEvent {}
public class OnBotPlay : GameEvent { }
public class OnTurnVisualsComplete : GameEvent { }

public class OnTurnResolveBegin : GameEvent
{
    public List<DieRoll> results;
    public Actor activeActor;
    public Actor passiveActor;
}