using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefineClasses 
{
	[System.Serializable]
	public class PlayerRawStats
	{
		private int level;
		public int Level 
		{ 
			get 
			{ 
				
				return level;
			}
			set 
			{ 
				int BeforeLevel = level;
				if (BeforeLevel != 0)
				{
					//TODO : rawStat을 테이블증가값으로 추가
					for (int i = 0; i < Level; i++)
					{

					}
				}
				level = value;
			}
		}
		public float atk;
		//근접 물리공격력
		public float matk;
		//마법공격력
		public float hit;
		//명중률 및 원거리 공격력
		public float criticalChance;
		//크리확률
		public float deffence;
		//방어력
		public float mDeffence;
		//마법방어력
		public float flee;
		//회피율
		public float aspd;
		//공격속도
		public float castingTime;
		//캐스팅시간
	}
}
namespace Scriptables
{
	using DefineClasses;
	[CreateAssetMenu(fileName = "New PlayerStat",menuName ="playerStables/playerStat",order = 50)]
	public class PlayerStat : ScriptableObject
	{
		public PlayerRawStats rawStat = new PlayerRawStats();
		public int str;
		//힘
		public int agi;
		//민첩
		public int vit;
		//체력
		public int intelligence;
        //지능
		public int dex;
		//손재주
		public int luk;
		//운

    }
}