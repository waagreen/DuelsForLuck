using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BattleVisualDirector : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private BattleDisplay p1Display, p2Display;

    private static readonly WaitForSeconds _waitForSeconds0_15 = new(0.15f);
    private static readonly WaitForSeconds _waitForSeconds0_8 = new(0.8f);
    private static readonly WaitForSeconds _waitForSeconds1_5 = new(1.5f);

    private void Awake()
    {
        EventsManager.AddSubscriber<OnTurnStart>(HandleTurnStartVisuals);
        EventsManager.AddSubscriber<OnTurnResolveBegin>(HandleTurnResolveVisuals);
        EventsManager.AddSubscriber<OnRoundEnd>(HandleRoundEndVisuals);
    }

    private void OnDestroy()
    {
        EventsManager.AddSubscriber<OnTurnStart>(HandleTurnStartVisuals);
        EventsManager.AddSubscriber<OnTurnResolveBegin>(HandleTurnResolveVisuals);
        EventsManager.AddSubscriber<OnRoundEnd>(HandleRoundEndVisuals);
    }

    private void HandleTurnStartVisuals(OnTurnStart evt)
    {
        StartCoroutine(RunTurnStartSequence(evt.actor.Order));
    } 

    private IEnumerator RunTurnStartSequence(int turnIndex)
    {
        // Camera moves to the perspective of next player
        float orbitAngle = (turnIndex == 0) ? 180f : 0f;

        Sequence turnSeq = DOTween.Sequence();
        turnSeq.AppendInterval(0.35f);
        turnSeq.Append(orbitCamera.OrbitToAngle(orbitAngle, 1f).SetEase(Ease.InOutSine));
        turnSeq.AppendInterval(0.2f);
        turnSeq.Append(orbitCamera.AdjustViewAngle(45f, 0.6f).SetEase(Ease.InBack));
        turnSeq.AppendInterval(0.35f);

        // Waits for the camera sequence to finish
        yield return turnSeq.WaitForCompletion();
        
        EventsManager.Broadcast(new OnTurnVisualsComplete());
    }

    private void HandleTurnResolveVisuals(OnTurnResolveBegin evt)
    {
        StartCoroutine(RunTurnResolveVisuals(evt));
    }

    private IEnumerator RunTurnResolveVisuals(OnTurnResolveBegin evt)
    {
        // Grab correct references
        BattleDisplay activeDisplay = (evt.activeActor.Order == 0) ? p2Display : p1Display;
        ActorModel damagedModel = activeDisplay.GetModel();
        int passiveActorIndex = evt.passiveActor.Order;

        // Turn camera to face the opponent
        Sequence camSeq = DOTween.Sequence();
        camSeq.Append(orbitCamera.AdjustViewAngle(0f, 0.6f).SetEase(Ease.OutBack));
        camSeq.AppendInterval(0.5f);

        // Wait for the camera finish turning
        yield return camSeq.WaitForCompletion();

        // Loop through dice results
        for (int i = 0; i < evt.results.Count; i++)
        {
            DieRoll roll = evt.results[i];

            // 3.1. Show die face
            yield return activeDisplay.ShowDieFace(i, roll.value - 1, 0.2f).WaitForCompletion();

            if (roll.value > 1) // Hit
            {
                EventsManager.Broadcast(new OnActorHealthChange { turnIndex = passiveActorIndex, dealtaHealth = -roll.damage });
                damagedModel.Hurt(); // broadcast an event?
                BroadcastCameraShake(0.1f, roll.damage);

                yield return _waitForSeconds0_15;
            }
            else // Miss
            {
                yield return activeDisplay.DisableDie(i, 0.3f).WaitForCompletion();
            }

            // Pauses for legibility
            yield return _waitForSeconds0_8;
        }
        
        // Pauses for legibility
        yield return _waitForSeconds1_5;

        // Hide all dice
        yield return activeDisplay.HideAllDice(0.2f).WaitForCompletion();

        // Resets all die representations
        activeDisplay.ResetRepresentations();

        EventsManager.Broadcast(new OnTurnVisualsComplete());
    }

    private void HandleRoundEndVisuals(OnRoundEnd evt)
    {

    }

    private IEnumerator RunRoundEndVisuals(OnRoundEnd evt)
    {
        yield return null;
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
}
