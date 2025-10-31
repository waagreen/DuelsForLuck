using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleVisualDirector : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private BattleDisplay p1Display, p2Display;

    private List<OnCardEffect> effectsToDisplay;

    private void Awake()
    {
        effectsToDisplay = new();
        
        EventsManager.AddSubscriber<OnTurnStart>(HandleTurnStartVisuals);
        EventsManager.AddSubscriber<OnAllEffectsApplied>(HandleTurnResolveVisuals);
        EventsManager.AddSubscriber<OnRoundEnd>(HandleRoundEndVisuals);
        EventsManager.AddSubscriber<OnCardEffect>(RegisterEffect);
    }

    private void OnDestroy()
    {
        EventsManager.AddSubscriber<OnTurnStart>(HandleTurnStartVisuals);
        EventsManager.AddSubscriber<OnAllEffectsApplied>(HandleTurnResolveVisuals);
        EventsManager.AddSubscriber<OnRoundEnd>(HandleRoundEndVisuals);
        EventsManager.RemoveSubscriber<OnCardEffect>(RegisterEffect);
    }

    private void RegisterEffect(OnCardEffect evt)
    {
        effectsToDisplay.Add(evt);
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

    private void HandleTurnResolveVisuals(OnAllEffectsApplied evt)
    {
        StartCoroutine(RunTurnResolveVisuals(evt));
    }

    private IEnumerator RunTurnResolveVisuals(OnAllEffectsApplied evt)
    {
        foreach (OnCardEffect trigger in effectsToDisplay)
        {
            bool isOtherTarget = trigger.targetType != CardEffectTarget.Self;
            BattleDisplay battleDisplay = (trigger.target.Order == 0) ? p1Display : p2Display;
            Debug.Log($"APPLYING EFFECT ON {trigger.targetType}");
            if (isOtherTarget)
            {
                // Turn camera to face the opponent
                Sequence camSeq = DOTween.Sequence();
                camSeq.AppendInterval(0.5f);
                camSeq.Append(orbitCamera.AdjustViewAngle(0f, 0.6f).SetEase(Ease.OutBack));
                camSeq.AppendInterval(0.5f);

                // Wait for the camera finish turning
                yield return camSeq.WaitForCompletion();
            }

            Sequence effectSeq = DOTween.Sequence();
            effectSeq.Append(battleDisplay.GetEffectSequence(trigger.effectType));
            effectSeq.AppendInterval(0.5f);

            yield return effectSeq.WaitForCompletion();

            if (isOtherTarget)
            {
                // Turn camera to face the opponent
                Sequence camSeq = DOTween.Sequence();
                camSeq.Append(orbitCamera.AdjustViewAngle(45f, 0.6f).SetEase(Ease.OutBack));
                camSeq.AppendInterval(0.5f);

                // Wait for the camera finish turning
                yield return camSeq.WaitForCompletion();
            }
        }

        effectsToDisplay.Clear();
        
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
