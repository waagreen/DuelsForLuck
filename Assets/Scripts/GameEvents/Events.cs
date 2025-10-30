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
public class CardEvent : GameEvent
{
    public CardRuntime card;
}
#endregion

#region PLAY
public class OnSendCardsToDeck : CardsEvent { } 
public class OnSendCardsToHand : CardsEvent { }
public class OnSendCardsToDiscard : CardsEvent { }
public class OnSendCardsToDraw : CardsEvent { }
public class OnPlayCard : CardEvent { }
public class OnPickDrawCard : CardEvent { }
public class OnPlayIsAviable : ActorEvent { }
public class OnSelectDie : GameEvent
{
    public int value;
    public int id;
}
public class OnDisselectDie : GameEvent { }
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
public class OnEffectVisualsTrigger : GameEvent
{
    public CardEffectType effectType;
    public int value;
    public Actor target;
}
#endregion

#region GAME STATE
public class OnCreateActor : ActorEvent { }
public class OnGameEnd : ActorEvent {}
public class OnRoundEnd : ActorEvent { }
public class OnNextRound : GameEvent { }
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
