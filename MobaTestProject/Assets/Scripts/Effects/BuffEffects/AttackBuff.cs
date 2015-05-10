using UnityEngine;
using System.Collections;

public class AttackBuff : Effect
{
	public override void Effects (BaseUnit info, Transform target)
	{
		info.Attack += 10;
	}
}
