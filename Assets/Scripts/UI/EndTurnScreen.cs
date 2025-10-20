using DG.Tweening;
using TMPro;
using UnityEngine;

public class EndTurnScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text message;
    private Sequence messageSeq;

    private void UpdateMessage(OnRoundEnd evt)
    {
        string winner = evt.winner == Order.First ? "<color=#3B8ACC>BLUE</color>" : "<color=#CC3B8A>RED</color>";
        message.SetText($"{winner} Win! \n\nFirst to wins two rounds \ntakes the game");

        messageSeq?.Kill();
        messageSeq = DOTween.Sequence();

        messageSeq.Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InExpo));
        messageSeq.AppendInterval(2f);
        messageSeq.Append(transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutCubic));
        messageSeq.AppendCallback(() =>
        {
            EventsManager.Broadcast(new OnResetTurn());
        });
        messageSeq.Play();
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnRoundEnd>(UpdateMessage);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnRoundEnd>(UpdateMessage);
    }
}
