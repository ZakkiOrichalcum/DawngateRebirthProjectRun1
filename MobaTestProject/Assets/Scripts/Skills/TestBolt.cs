using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestBolt : SkillData 
{
	public override string Name {get{return "Just a test bolt to test actives and effects.";}}
	
	public override AbilityType Type {get{return AbilityType.PASSIVEANDACTIVE;}}
	public override string Description {get{return "";}}
	
	public override string PassiveName {get{return "";}}

	public override PassiveTypes PTypes {get{return PassiveTypes.BUFF;}}
	public override void Passive(BaseUnit target, Transform unit)
	{
		target.buffEffects.Add(new AttackBuff());
	}
	
	public override string ActiveName {get{return "Test Bolt";}}
	public override ActiveTypes ATypes {get{return ActiveTypes.SKILLSHOT;}}
	public override float[] Cooldown {get{return new float[] {3f,2.5f,2f,1.5f,1f};}}
	public override float MissleSpeed {get{return 4f;}}
	public override float MissleLifetime {get{return 2f;}}
	public override void Active(Transform casterPoint, BaseUnit caster, Transform trans, Vector3 pos/*, Effect[] onHitEffects*/)
	{
		Vector3 dir = pos - casterPoint.position;
		dir.y = 0;
		Quaternion rot = Quaternion.LookRotation(dir);
		double damage = 100 + 20 /**PerLevel*/ + caster.Attack * 0.6;
		List<string> temp = new List<string> {"RedTeam", "Neutrals"};
		CurrentCooldown = Time.time + Cooldown[SkillLevel-1];
		GameObject missle = MonoBehaviour.Instantiate(Resources.Load ("TestBolt"), casterPoint.position, rot) as GameObject;
		SkillShotBehaviour skShot = missle.GetComponent<SkillShotBehaviour>();
		skShot.StatsSet(MissleSpeed, casterPoint, damage, MissleLifetime, "None", temp);
	}
}
