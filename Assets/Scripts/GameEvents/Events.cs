using System.Collections.Generic;

#region BASE EVENTS
public class GameEvent { }

public class ActorEvent : GameEvent
{
    public Actor actor;
}

public class CardsEvent : GameEvent
{
    public List<CardRuntime> cards;
    public int ownerOrder;
}
#endregion

#region PLAY
public class OnBotPlay : GameEvent { }

public class OnDrawCard : CardsEvent { }
public class OnPlayCard : CardsEvent { }
public class OnDiscard : CardsEvent { }

public class OnPlayIsAviable : ActorEvent { }

public class OnDieResult : GameEvent
{
    public int result;
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
public class OnCreateActor : ActorEvent { }
public class OnGameEnd : ActorEvent {}
public class OnNextRound : GameEvent { }

public class OnRoundEnd : GameEvent
{
    public int winner;
}
#endregion

#region TURN
public class OnTurnStart : ActorEvent { }
public class OnTurnVisualsComplete : GameEvent { }

public class OnTurnResolveBegin : GameEvent
{
    public List<DieRoll> results;
    public Actor activeActor;
    public Actor passiveActor;
}
#endregion
