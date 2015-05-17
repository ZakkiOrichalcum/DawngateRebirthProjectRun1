using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	private string unitName;
	private string type;
	private int charID;
	private string team;
	private float nextAttackTime;
	
	//Stats
	private int level;
	private int baseHealth;
	private double currentHealth;
	private double maxHealth;
	private int healthPerLevel;
	private double healthRegen;
	private int baseAttack;
	private int attack;
	private double attackPerLevel;
	private float baseAttackSpeed;
	private float attackSpeed;
	private float attackSpeedPerLevel;
	private int baseArmor;
	private int armor;
	private double armorPerLevel;
	private int baseMagicResist;
	private int magicResist;
	private double magicResistPerLevel;
	private int attackRange;
	private int baseMovementSpeed;
	private int movementSpeed;
	private int power;
	private float powerRatio;
	private int haste;
	private int additionalHealth;
	private int additionalArmor;
	private int additionalMagicResist;
	private int additionalMovementSpeed;
	private float cooldown;
	private float cooldownRatio;
	private float attackspeedRatio;
	private float movementspeedRatio;
	private int currentXP;
	private int currentGold;
	private double basexp;
	private double xpPerLevel;
	private double xpReward;
	private double baseMoney;
	private double moneyPerLevel;
	private double moneyReward;
	
	//Abilities and Skills
	//private SkillData[] abilities;
	
	//Effects
	//	public List<Effect> buffEffects;
	//	public List<Effect> onHitEffects;
	//	public List<Effect> timedEffects;
	//	public List<Effect> onSpellDamageEffects;
	
	//bools
	private bool dead;
	
	public void Init( string n, string t, string tm, int l, double hp, int hpPer, double hpRegen, int a, double aPer, float atkspd, float atkspdPer, int arm, double armPer, 
	                 int mr, double mrPer, int atkrnge, int ms, float pwrRatio, float cdRatio, float asRatio, float msRatio, double x, double xPer, double m, double mPer )
	{
		unitName = n;
		type = t;
		team = tm;
		level = l;
		baseHealth = (int)hp;
		currentHealth = maxHealth = hp;
		healthPerLevel = hpPer;
		healthRegen = hpRegen;
		baseAttack = attack = a;
		attackPerLevel = aPer;
		baseAttackSpeed = attackSpeed = atkspd;
		attackSpeedPerLevel = atkspdPer;
		baseArmor = armor = arm;
		armorPerLevel = armPer;
		baseMagicResist = magicResist = mr;
		magicResistPerLevel = mrPer;
		attackRange = atkrnge/100;
		baseMovementSpeed = movementSpeed = ms;
		dead = false;
		Power = Haste = AdditionalArmor = AdditionalHealth = AdditionalMagicResist = AdditionalMovementSpeed = 0;
		cooldown = 0f;
		powerRatio = pwrRatio;
		cooldownRatio = cdRatio;
		attackspeedRatio = asRatio;
		movementspeedRatio = msRatio;
		basexp = xpReward = x;
		xpPerLevel = xPer;
		baseMoney = moneyReward = m;
		moneyPerLevel = mPer;
		//		buffEffects = new List<Effect>();
		//		onHitEffects = new List<Effect>();
		//		timedEffects = new List<Effect>();
		//		onSpellDamageEffects = new List<Effect>();
		StatisticCalculation();
	}
	
	//Getter/Setter Methods
	public string UnitName
	{
		get{return unitName;}
		set{unitName = value;}
	}
	
	public string Type
	{
		get{return type;}
		set{type = value;}
	}

	public string Team
	{
		get{return team;}
		set{team = value;}
	}

	public int CharID
	{
		get{return charID;}
		set{charID = value;}
	}
	
	public int Level
	{
		get{return level;}
		set{level = value;}
	}
	
	public int BaseHealth
	{
		get{return baseHealth;}
		set{baseHealth = value;}
	}
	
	public double MaxHealth
	{
		get{return maxHealth;}
		set{maxHealth = value;}
	}
	
	public double CurrentHealth
	{
		get{return currentHealth;}
		set{currentHealth = value;}
	}
	
	public int HealthPerLevel
	{
		get{return healthPerLevel;}
		set{healthPerLevel = value;}
	}
	
	public double HealthRegen
	{
		get{return healthRegen;}
		set{healthRegen = value;}
	}
	
	public int BaseAttack
	{
		get{return baseAttack;}
		set{baseAttack = value;}
	}
	
	public int Attack
	{
		get{return attack;}
		set{attack = value;}
	}
	
	public double AttackPerLevel
	{
		get{return attackPerLevel;}
		set{attackPerLevel = value;}
	}
	
	public int BaseArmor
	{
		get{return baseArmor;}
		set{baseArmor = value;}
	}
	
	public int Armor
	{
		get{return armor;}
		set{armor = value;}
	}
	
	public double ArmorPerLevel
	{
		get{return armorPerLevel;}
		set{armorPerLevel = value;}
	}
	
	public int BaseMagicResist
	{
		get{return baseMagicResist;}
		set{baseMagicResist = value;}
	}
	
	public int MagicResist
	{
		get{return magicResist;}
		set{magicResist = value;}
	}
	
	public double MagicResistPerLevel
	{
		get{return magicResistPerLevel;}
		set{magicResistPerLevel = value;}
	}
	
	public float BaseAttackSpeed
	{
		get{return baseAttackSpeed;}
		set{baseAttackSpeed = value;}
	}
	
	public float AttackSpeed
	{
		get{return attackSpeed;}
		set{attackSpeed = value;}
	}
	
	public float AttackSpeedPerLevel
	{
		get{return attackSpeedPerLevel;}
		set{attackSpeedPerLevel = value;}
	}
	
	public float NextAttackTime
	{
		get{return nextAttackTime;}
		set{nextAttackTime = value;}
	}
	
	public int BaseMovementSpeed
	{
		get{return baseMovementSpeed;}
		set{baseMovementSpeed = value;}
	}
	
	public int MovementSpeed
	{
		get{return movementSpeed;}
		set{movementSpeed = value;}
	}
	
	public int AttackRange
	{
		get{return attackRange * 100;}
		set{attackRange = value/100;}
	}
	
	public int Power
	{
		get{return power;}
		set{power = value;}
	}
	public float PowerRatio
	{
		get{return powerRatio;}
		set{powerRatio = value;}
	}
	public int Haste
	{
		get{return haste;}
		set{haste = value;}
	}
	public int AdditionalHealth
	{
		get{return additionalHealth;}
		set{additionalHealth = value;}
	}
	public int AdditionalArmor
	{
		get{return additionalArmor;}
		set{additionalArmor = value;}
	}
	public int AdditionalMagicResist
	{
		get{return additionalMagicResist;}
		set{additionalMagicResist = value;}
	}
	public int AdditionalMovementSpeed
	{
		get{return additionalMovementSpeed;}
		set{additionalMovementSpeed = value;}
	}
	public float Cooldown
	{
		get{return cooldown;}
		set{cooldown = value;}
	}
	public float CooldownRatio
	{
		get{return cooldownRatio;}
		set{cooldownRatio = value;}
	}
	public float AttackspeedRatio
	{
		get{return attackspeedRatio;}
		set{attackspeedRatio = value;}
	}
	public float MovementspeedRatio
	{
		get{return movementspeedRatio;}
		set{movementspeedRatio = value;}
	}
	
	public int CurrentXP
	{
		get{return currentXP;}
		set{currentXP = value;}
	}
	
	public int CurrentGold
	{
		get{return currentGold;}
		set{currentGold = value;}
	}
	
	public bool Dead
	{
		get{return dead;}
		set{dead = value;}
	}
	
	public double BaseXp
	{
		get{return basexp;}
		set{basexp = value;}
	}
	
	public double XpPerLevel
	{
		get{return xpPerLevel;}
		set{xpPerLevel = value;}
	}
	
	public double XpReward
	{
		get{return xpReward;}
		set{xpReward = value;}
	}
	
	public double BaseMoney
	{
		get{return baseMoney;}
		set{baseMoney = value;}
	}
	
	public double MoneyPerLevel
	{
		get{return moneyPerLevel;}
		set{moneyPerLevel = value;}
	}
	
	public double MoneyReward
	{
		get{return moneyReward;}
		set{moneyReward = value;}
	}
	
	//	public SkillData[] Abilities
	//	{
	//		get{return abilities;}
	//		set{abilities = value;}
	//	}
	
	public void StatisticCalculation( ) 
	{
		int temp = (int)(CurrentHealth/MaxHealth);
		MaxHealth = BaseHealth + HealthPerLevel*Level + AdditionalHealth;
		CurrentHealth = temp*MaxHealth;
		Attack = (int)(BaseAttack + AttackPerLevel*Level + Power*PowerRatio);
		Armor = (int)(BaseArmor + ArmorPerLevel*Level + AdditionalArmor);
		MagicResist = (int)(BaseMagicResist + MagicResistPerLevel*Level + AdditionalMagicResist);
		Cooldown = (float)((Haste * CooldownRatio)/100);
		AttackSpeed = BaseAttackSpeed + AttackSpeedPerLevel*Level + Haste*AttackspeedRatio;
		MovementSpeed = (int)(BaseMovementSpeed + Haste*MovementspeedRatio + AdditionalMovementSpeed);
		
//		foreach( Effect e in buffEffects )
//		{
//			e.Effects(this, Owner.transform);
//		}

		xpReward = (int)(basexp + xpPerLevel*level);
		moneyReward = (int)(baseMoney + moneyPerLevel*level);

	}
	public void RegenAndPassives( ) 
	{
		if(CurrentHealth < MaxHealth)
		{
			CurrentHealth += HealthRegen;
			if(CurrentHealth > MaxHealth)
				CurrentHealth = MaxHealth;
		}
	}

	public void Respawn( )
	{
		currentHealth = maxHealth;
		StatisticCalculation();
	}
	
	public void TakeDamage( Unit source )
	{
		if(!Dead)
		{
			CurrentHealth -= (int)source.Attack;
			Debug.Log("[" + Time.time + "] " + unitName + " took " + source.Attack + " damage from " + source.UnitName + ". (" + CurrentHealth + "/" + MaxHealth + ")");
			if( CurrentHealth <= 0 )
			{
				Dead = true;
				Debug.Log("[" + Time.time + "] " + unitName + " is dead.");
			}
		}
	}
	
	public void TakeHeal( double n )
	{
		CurrentHealth += (int)n;
		if(CurrentHealth >= MaxHealth)
		{
			CurrentHealth = MaxHealth;
			Debug.Log(unitName + " was full healed. (" + CurrentHealth + "/" + MaxHealth + ")");
		}
		else
			Debug.Log(unitName + " was healed " + (int)n + " damage. (" + CurrentHealth + "/" + MaxHealth + ")");
	}
}
