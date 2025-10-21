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

    public Tween ShowDieFace(int dieIndex, int faceValue, float duration)
    {
        if (dieIndex < 0 || dieIndex >= diceRepresentation.Count) return null;
        return diceRepresentation[dieIndex].ShowFace(faceValue, duration);
    }

    public Tween DisableDie(int dieIndex, float duration)
    {
        if (dieIndex < 0 || dieIndex >= diceRepresentation.Count) return null;
        return diceRepresentation[dieIndex].Disable(duration);
    }

    public Sequence HideAllDice(float duration)
    {
        Sequence hideSeq = DOTween.Sequence();
        foreach (var die in diceRepresentation)
        {
            hideSeq.Join(die.Hide(duration));
        }
        return hideSeq;
    }

    public void ResetRepresentations()
    {
        foreach (DieDisplay die in diceRepresentation)
        {
            die.Reset();
        }
    }

    public ActorModel GetModel() => model;
}
