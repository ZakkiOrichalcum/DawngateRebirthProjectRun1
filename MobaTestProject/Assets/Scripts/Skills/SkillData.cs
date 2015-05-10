using UnityEngine;
using System.Collections;

public abstract class SkillData
{
	public abstract string Name {get;}
	private int skillLevel = 0;
	public int SkillLevel
	{
		get{return skillLevel;}
		set{skillLevel = value;}
	}
	public enum AbilityType
	{
		PASSIVE,
		ACTIVE,
		PASSIVEANDACTIVE
	}

	public abstract AbilityType Type {get;}
	public abstract string Description {get;}

	public abstract string PassiveName{get;}
	public enum PassiveTypes
	{
		NONE,
		BUFF,
		TRIGGEREDBUFF,
		ONSPELLCAST,
		ONDAMAGE
	}
	public abstract PassiveTypes PTypes {get;}
	public abstract void Passive(BaseUnit target, Transform unit);

	public abstract string ActiveName{get;}
	public enum ActiveTypes
	{
		NONE      = 1 << 0,
		SELF      = 1 << 1,
		TARGET    = 1 << 2,
		SKILLSHOT = 1 << 3,
		AOE       = 1 << 4
	}
	public abstract ActiveTypes ATypes {get;}
	public abstract float[] Cooldown {get;}
	private float currentCooldown;
	public float CurrentCooldown 
	{
		get{return currentCooldown;} 
		set{currentCooldown = value;}
	}
	public abstract float MissleSpeed {get;}
	public abstract float MissleLifetime {get;}
	public abstract void Active(Transform casterT, BaseUnit caster, Transform trans, Vector3 position/*, Effect[] onHiteffects*/);

	public void LevelUp()
	{
		if(skillLevel >= 5)
			Debug.Log ("Skill Levelup error!");
		else
			skillLevel++;
	}
}
