using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBoostManager : MonoBehaviour
{
    private Plant p;
    private List<ClickBoostScriptable> currentBoosts = new List<ClickBoostScriptable>();

    private void Start()
    {
        p = GetComponent<Plant>();
    }

    public void ApplyBoosts(List<ClickBoostScriptable> clickBoostsSettings)
    {
        if (clickBoostsSettings.Count > 0)
        {
            foreach (var boost in clickBoostsSettings)
            {
                currentBoosts.Add(boost);
                switch (boost.typeBooster)
                {
                    case ClickBooster.Damage:
                        p.PlantDamage += Mathf.RoundToInt(boost.amount);
                        break;
                    case ClickBooster.IntervalSpeed:
                        p.ActionInterval += boost.amount;
                        break;
                    case ClickBooster.Defense:
                        p.DamageReceivedModifier -= boost.amount;
                        break;
                    case ClickBooster.Range:
                        p.DettectRange += new Vector2(boost.amount, boost.amount);
                        if (p.plantRangeArea != null)
                            p.plantRangeArea.RedrawRangeArea();
                        break;
                }
            }
        }
        StartCoroutine(WaitBoostTime());
    }

    public void RemoveBoost()
    {
        foreach (var boost in currentBoosts)
        {
            switch (boost.typeBooster)
            {
                case ClickBooster.Damage:
                    p.PlantDamage -= Mathf.RoundToInt(boost.amount);
                    break;
                case ClickBooster.IntervalSpeed:
                    p.ActionInterval -= boost.amount;
                    break;
                case ClickBooster.Defense:
                    p.DamageReceivedModifier += boost.amount;
                    break;
                case ClickBooster.Range:
                    p.DettectRange -= new Vector2(boost.amount, boost.amount);
                    if (p.plantRangeArea != null)
                        p.plantRangeArea.RedrawRangeArea();
                    break;
            }
        }
        currentBoosts.Clear();
    }

    public IEnumerator WaitBoostTime()
    {
        yield return new WaitForSeconds(p.BoostsDuration);
        if(currentBoosts.Count > 0)
        {
            RemoveBoost();
        }
        p.EndBoost();
    }
}
