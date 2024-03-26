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
					//TODO : rawStat�� ���̺����������� �߰�
					for (int i = 0; i < Level; i++)
					{

					}
				}
				level = value;
			}
		}
		public float atk;
		//���� �������ݷ�
		public float matk;
		//�������ݷ�
		public float hit;
		//���߷� �� ���Ÿ� ���ݷ�
		public float criticalChance;
		//ũ��Ȯ��
		public float deffence;
		//����
		public float mDeffence;
		//��������
		public float flee;
		//ȸ����
		public float aspd;
		//���ݼӵ�
		public float castingTime;
		//ĳ���ýð�
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
		//��
		public int agi;
		//��ø
		public int vit;
		//ü��
		public int intelligence;
        //����
		public int dex;
		//������
		public int luk;
		//��

    }
}