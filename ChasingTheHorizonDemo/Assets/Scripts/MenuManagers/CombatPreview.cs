using UnityEngine;
using UnityEngine.UI;

//This was used to test highlighting buttons without selecting them
//All the Combat Preview data will be moved here in the future, instead of it being handled in the ActionMenuManager
public class CombatPreview : MonoBehaviour
{
    [Header("Attacker Preview")]
    public Image attackerPortrait = null;
    public Text attackerHP = null;
    public Text attackerDamage = null;
    public Text attackerHit = null;
    public Text attackerCrit = null;
    [Header("Defender Preview")]
    public Image defenderPortrait = null;
    public Text defenderHP = null;
    public Text defenderDamage = null;
    public Text defenderHit = null;
    public Text defenderCrit = null;

    [SerializeField] private CursorController cursor;
    [SerializeField] private TileMap map;

    private void OnEnable()
    {
        map = FindObjectOfType<TileMap>();
        Invoke("MenuBlorp", 0.1f);
        Invoke("FillPreview", 0.17f);
    }
    private void OnDisable()
    {
        attackerPortrait.sprite = null;
        attackerHP.text = null;
        attackerDamage.text = null;
        attackerHit.text = null;
        attackerCrit.text = null;

        defenderPortrait.sprite = null;
        defenderHP.text = null;
        defenderDamage.text = null;
        defenderHit.text = null;
        defenderCrit.text = null;
    }
    public void FillPreview()
    {
        attackerPortrait.sprite = map.selectedUnit.unit.portrait;
        attackerHP.text = map.selectedUnit.currentHealth.ToString();
        attackerDamage.text = HitPreview(map.selectedUnit, map.selectedUnit.target).ToString();
        attackerHit.text = (map.selectedUnit.CombatStatistics().hit - map.selectedUnit.target.CombatStatistics().avoid).ToString();
        attackerCrit.text = map.selectedUnit.CombatStatistics().crit.ToString();

        defenderPortrait.sprite = map.selectedUnit.target.unit.portrait;
        defenderHP.text = map.selectedUnit.target.currentHealth.ToString();

        if(Vector2.Distance(map.selectedUnit.transform.localPosition, map.selectedUnit.target.transform.localPosition) > map.selectedUnit.target.equippedWeapon.range)
            defenderDamage.text = "0";
        else
            defenderDamage.text = HitPreview(map.selectedUnit.target, map.selectedUnit).ToString();

        defenderHit.text = (map.selectedUnit.target.CombatStatistics().hit - map.selectedUnit.CombatStatistics().avoid).ToString();
        defenderCrit.text = map.selectedUnit.target.CombatStatistics().crit.ToString();
    }
    public void ClearPreview()
    {
        attackerPortrait.sprite = null;
        attackerHP.text = null;
        attackerDamage.text = null;
        attackerHit.text = null;
        attackerCrit.text = null;

        defenderPortrait.sprite = null;
        defenderHP.text = null;
        defenderDamage.text = null;
        defenderHit.text = null;
        defenderCrit.text = null;
    }

    private void MenuBlorp()
    {
        SoundManager.instance.PlayFX(11);
    }

    private int HitPreview(Unit attacker, Unit defender)
    {
        return attacker.CombatStatistics().attack - defender.CombatStatistics().protection;
    }
}
