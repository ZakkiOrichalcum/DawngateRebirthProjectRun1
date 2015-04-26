using UnityEngine;
using System.Collections;

public class BaseUnit
{
	private GameObject owner;
	private string name;
	private string type;
	private float nextAttackTime;

	//Stats
	private double currentHealth;
	private double maxHealth;
	private int healthPer;
	private int baseAttack;
	private int attack;
	private int attackPer;
	private float baseAttackSpeed;
	private float attackSpeed;
	private float attackSpeedPer;
	private int baseArmor;
	private int armor;
	private int armorPer;
	private int baseMagicResist;
	private int magicResist;
	private int magicResistPer;
	private int attackRange;
	private int baseMovementSpeed;
	private int movementSpeed;

	//bools
	private bool dead;
	private bool hasHPBar;

	public BaseUnit( GameObject own, string n, string t, double hp, double hpPer, int a, int aPer, float atkspd, float atkspdPer, int arm, int armPer, 
	                int mr, int mrPer, int atkrnge, int ms, bool hpbar )
	{
		owner = own;
		name = n;
		type = t;
		currentHealth = maxHealth = hp;
		baseAttack = attack = a;
		attackPer = aPer;
		baseAttackSpeed = attackSpeed = atkspd;
		attackSpeedPer = atkspdPer;
		baseArmor = armor = arm;
		armorPer = armPer;
		baseMagicResist = magicResist = mr;
		magicResistPer = mrPer;
		attackRange = atkrnge;
		baseMovementSpeed = movementSpeed = ms;
		dead = false;
		hasHPBar = hpbar;
	}

	//Getter/Setter Methods
	public string Name
	{
		get{return name;}
		set{name = value;}
	}

	public string Type
	{
		get{return type;}
		set{type = value;}
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

	public double HealthPer
	{
		get{return healthPer;}
		set{healthPer = value;}
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

	public int AttackPer
	{
		get{return attackPer;}
		set{attackPer = value;}
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

	public int ArmorPer
	{
		get{return armorPer;}
		set{armorPer = value;}
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

	public int MagicResistPer
	{
		get{return magicResistPer;}
		set{magicResistPer = value;}
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

	public float AttackSpeedPer
	{
		get{return attackSpeedPer;}
		set{attackSpeedPer = value;}
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
		get{return attackRange;}
		set{attackRange = value;}
	}
	
	public bool Dead
	{
		get{return dead;}
		set{dead = value;}
	}

	public void TakeDamage( double n )
	{
		CurrentHealth -= (int)n;
		Debug.Log(Name + " took " + n + " damage. (" + CurrentHealth + "/" + MaxHealth + ")");
		if( CurrentHealth <= 0 )
		{
			Dead = true;
			Debug.Log(Name + " is dead.");
		}
		if( hasHPBar )
		{
			HealthBar hpbar = owner.transform.FindChild("Canvas").FindChild("HealthBar").GetComponent<HealthBar>();
			float normalizedHealth = (float)(CurrentHealth/MaxHealth);
			hpbar.SetHealthVisual(normalizedHealth);
		}
	}

	public void TakeHeal( double n )
	{
		CurrentHealth += (int)n;
		if(CurrentHealth >= MaxHealth)
		{
			CurrentHealth = MaxHealth;
			Debug.Log(Name + " was full healed. (" + CurrentHealth + "/" + MaxHealth + ")");
		}
		else
			Debug.Log(Name + " was healed " + (int)n + " damage. (" + CurrentHealth + "/" + MaxHealth + ")");
	}
}
