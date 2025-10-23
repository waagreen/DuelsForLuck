using System.Collections.Generic;

public class GameEvent { }

public class OnCreateActor : GameEvent
{
    public Actor newActor;
}

public class OnCreateDeck : GameEvent
{
    public List<CardRuntime> deck;
}

#region PLAY

public class OnDieResult : GameEvent
{
    public int result;
}

public class OnBotPlay : GameEvent { }

public class OnPlayIsAviable : GameEvent
{
    public Actor currentActor;
}

#endregion

#region VISUAL FEEDBACK

public class OnActorHealthChange : GameEvent
{
    public int dealtaHealth;
    public int? turnIndex;
}

public class OnCameraShake : GameEvent
{
    public float duration;
    public float strength = 3;
    public int vibrato = 10;
    public float randomness = 90;
}

#endregion

#region GAME STATE

public class OnRoundEnd : GameEvent
{
    public int winner;
}

public class OnGameEnd : GameEvent
{
    public Actor winner;
}

public class OnNextRound : GameEvent { }

#endregion

#region TURN

public class OnTurnStart : GameEvent
{
    public Actor currentActor;
}

public class OnTurnResolveBegin : GameEvent
{
    public List<DieRoll> results;
    public Actor activeActor;
    public Actor passiveActor;
}

public class OnTurnVisualsComplete : GameEvent { }

#endregion
