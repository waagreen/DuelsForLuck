using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleVisualDirector : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private BattleDisplay p1BattleDisplay, p2BattleDisplay;
    [SerializeField] private ActorStatsDisplay p1StatsDisplay, p2StatsDisplay;

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
            bool isTargetFirst = trigger.target.Order == 0;
            BattleDisplay battleDisplay = isTargetFirst ? p1BattleDisplay : p2BattleDisplay;
            ActorStatsDisplay statsDisplay = isTargetFirst ? p1StatsDisplay : p2StatsDisplay;

            if (isOtherTarget)
            {
                // Pitch camera to face the opponent
                yield return PitchCamera(0f, 0.6f, 0.5f).WaitForCompletion();
            }

            Sequence effectSeq = DOTween.Sequence();
            effectSeq.Append(battleDisplay.GetEffectSequence(trigger.effectType));
            effectSeq.Append(statsDisplay.GetEffectSequence(trigger.effectType, trigger.value));
            effectSeq.AppendInterval(0.5f);
            yield return effectSeq.WaitForCompletion();

            if (isOtherTarget)
            {
                // Pitch camera to face the table
                yield return PitchCamera(45f, 0.6f).WaitForCompletion();
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

    private Sequence PitchCamera(float angle, float duration, float intervalPre = 0f, float intervalPost = 0f)
    { 
        Sequence camSeq = DOTween.Sequence();
        camSeq.AppendInterval(intervalPre);
        camSeq.Append(orbitCamera.AdjustViewAngle(angle, duration).SetEase(Ease.OutBack));
        camSeq.AppendInterval(intervalPost);
        return camSeq;
    }
}
