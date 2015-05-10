using UnityEngine;
using System.Collections;

public class Minion : BaseUnit 
{
	public Minion (string n, Transform own, string t, int level, double hp, double xpPer, double m, double mPer, int hpPer, int a, int aPer, float atkspd, float atkspdPer, int arm, int armPer, 
	               int mr, int mrPer, int atkrnge, int ms, bool hpbar, double x) : base( n, own, t, hp, hpPer, 0, a, aPer, atkspd, atkspdPer, arm, armPer, 
	                                                                       mr, mrPer, atkrnge, ms, 0, 0, 0, 0 )
	{
		Level = level;
		BaseXp = x;
		XpPerLevel = xpPer;
		BaseMoney = m;
		MoneyPerLevel = mPer;
		StatisticCalculation();
	}

	public override void StatisticCalculation( )
	{
		XpReward = BaseXp + XpPerLevel*Level;
		MoneyReward = BaseMoney + MoneyPerLevel*Level;
		int temp = (int)(CurrentHealth/MaxHealth);
		MaxHealth = BaseHealth + HealthPerLevel*Level;
		CurrentHealth = temp*MaxHealth;
		Attack = BaseAttack + AttackPerLevel*Level;
		Armor = BaseArmor + ArmorPerLevel*Level;
		MagicResist = BaseMagicResist + MagicResistPerLevel*Level;
	}

}
