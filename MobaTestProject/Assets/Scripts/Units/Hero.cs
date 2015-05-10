using UnityEngine;
using System.Collections;

public class Hero : BaseUnit 	
{
	private int resource;
	public bool hasResource;
	public int Resource
	{
		get{return resource;}
		set{resource = value;}
	}

	public Hero (string n, Transform own, string t, double hp, int hpPer, double hpRegen, int a, int aPer, float atkspd, float atkspdPer, int arm, int armPer, 
	             int mr, int mrPer, int atkrnge, int ms, float pwrRatio, float cdRatio, float asRatio, float msRatio) : base( n, own, t, hp, hpPer, hpRegen, a, aPer, atkspd, atkspdPer, arm, armPer, 
	                                                                                                       mr, mrPer, atkrnge, ms, pwrRatio, cdRatio, asRatio, msRatio )
	{
		CurrentGold = 450;
		CurrentXP = 0;
		XpReward = 150;
		MoneyReward = 300;
		StatisticCalculation();
	}

	public override void StatisticCalculation( )
	{
		int temp = (int)(CurrentHealth/MaxHealth);
		MaxHealth = BaseHealth + HealthPerLevel*Level + AdditionalHealth;
		CurrentHealth = temp*MaxHealth;
		Attack = (int)(BaseAttack + AttackPerLevel*Level + Power*PowerRatio);
		Armor = BaseArmor + ArmorPerLevel*Level + AdditionalArmor;
		MagicResist = BaseMagicResist + MagicResistPerLevel*Level + AdditionalMagicResist;
		Cooldown = (float)((Haste * CooldownRatio)/100);
		AttackSpeed = BaseAttackSpeed + AttackSpeedPerLevel*Level + Haste*AttackspeedRatio;
		MovementSpeed = (int)(BaseMovementSpeed + Haste*MovementspeedRatio + AdditionalMovementSpeed);

		foreach( Effect e in buffEffects )
		{
			e.Effects(this, Owner.transform);
		}
	}

	public override void RegenAndPassives( )
	{
		if(CurrentHealth < MaxHealth)
		{
			CurrentHealth += HealthRegen;
			if(CurrentHealth > MaxHealth)
				CurrentHealth = MaxHealth;
		}
	}
}
