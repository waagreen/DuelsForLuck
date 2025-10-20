using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas display;
    [SerializeField] private RectTransform diceHolder;
    [SerializeField] private DieDisplay diePrefab;
    [SerializeField] private ActorModel model;
    [SerializeField][Range(0, 1)] private int damageFrom;

    private List<DieDisplay> diceRepresentation;
    private Sequence damageSequence;

    private void Start()
    {
        diceRepresentation = new();

        for (int i = 0; i < 5; i++)
        {
            DieDisplay die = Instantiate(diePrefab, diceHolder);
            die.VisualSetup((damageFrom == 0) ? Actor.PColor : Actor.BotColor);
            diceRepresentation.Add(die);
        }
    }

    private void ResetRepresentations()
    {
        foreach (DieDisplay die in diceRepresentation)
        {
            die.Reset();
        }
    }

    private void BroadcastHealth(int damage, int damagedActor)
    {
        // Since this is called after changing the turn order, we pass the active actors's health, since they received damage last turn
        EventsManager.Broadcast(new OnActorHealthChange() { turnIndex = damagedActor, dealtaHealth = -damage });
    }

    private void BroadcastCameraShake(float duration, int damage)
    {
        bool isCrit = damage > 1;
        OnCameraShake shake = new()
        {
            duration = duration * (isCrit ? 1.2f : 1f),
            strength = isCrit ? 2f : 0.5f,
            vibrato = isCrit ? 6 : 2,
            randomness = isCrit ? 60f : 30f,
        };

        EventsManager.Broadcast(shake);
    }

    public Sequence DamageFeedbackSequence(List<DieRoll> turnResults, int damagedActor)
    {
        damageSequence?.Kill();
        damageSequence = DOTween.Sequence();

        // Display each die result sequentialy 
        for (int i = 0; i < turnResults.Count; i++)
        {
            DieRoll roll = turnResults[i];
            DieDisplay die = diceRepresentation[i];

            damageSequence.Append(die.ShowFace(roll.value - 1, 0.2f));

            // Face value of 1 represents zero damage
            if (roll.value > 1)
            {
                damageSequence.JoinCallback(() => BroadcastHealth(roll.damage, damagedActor));
                damageSequence.Join(model.Hurt());
                damageSequence.JoinCallback(() => BroadcastCameraShake(0.1f, roll.damage));
            }
            else
            {
                damageSequence.AppendInterval(0.25f);
                damageSequence.Append(die.Disable(0.3f));
            }

            damageSequence.AppendInterval(0.8f);
        }

        // Wait for readability
        damageSequence.AppendInterval(1.5f);

        // Hide all dice simultaneously
        for (int i = 0; i < turnResults.Count; i++)
        {
            damageSequence.Join(diceRepresentation[i].Hide(0.2f));
        }

        damageSequence.OnComplete(ResetRepresentations);
        return damageSequence;
    }
}
