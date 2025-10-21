using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BattleVisualDirector : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private BattleDisplay p1Display, p2Display;

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
        StartCoroutine(RunTurnStartSequence(evt.currentActor.Order));
    } 

    private IEnumerator RunTurnStartSequence(int turnIndex)
    {
        // Camera moves to the perspective of next player
        float orbitAngle = (turnIndex == 0) ? 180f : 0f;

        Sequence turnSeq = DOTween.Sequence();
        turnSeq.Append(orbitCamera.OrbitToAngle(orbitAngle, 1f).SetEase(Ease.InOutSine));
        turnSeq.AppendInterval(0.2f);
        turnSeq.Append(orbitCamera.AdjustViewAngle(45f, 0.6f).SetEase(Ease.InBack));

        // Waits for the camera sequence to finish
        yield return turnSeq.WaitForCompletion();
        
        // (Optional) If the logic needs to wait for it (ie: bot play)
        // We can send a "OnTurnVisualsReady" here
    }

    private void HandleTurnResolveVisuals(OnTurnResolveBegin evt)
    {
        StartCoroutine(RunTurnResolveVisuals(evt));
    }

    private IEnumerator RunTurnResolveVisuals(OnTurnResolveBegin evt)
    {
        // Grab correct references
        BattleDisplay activeDisplay = (evt.activeActor.Order == 0) ? p1Display : p2Display;
        ActorModel damagedModel = ((evt.passiveActor.Order == 0) ? p1Display : p2Display).GetModel();
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

                yield return new WaitForSeconds(0.15f);
            }
            else // Miss
            {
                yield return activeDisplay.DisableDie(i, 0.3f).WaitForCompletion();
            }

            // Pauses for legibility
            yield return new WaitForSeconds(0.8f);
        }
        
        // Pauses for legibility
        yield return new WaitForSeconds(1.5f);

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
