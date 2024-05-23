#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Dynamics.PhysBone.Components;

public class FixPhysboneEditor : MonoBehaviour
{
	[MenuItem("Tools/Star Utils/Fix Physbones")]
	static void RemoveDuplicatePhysBones() 
	{
		foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
		{
			VRCPhysBone[] physBones = go.GetComponents<VRCPhysBone>();

			int numPhysBones = physBones.Length;

			if (numPhysBones > 1)
			{
				Debug.Log("Found " + numPhysBones + " VRCPhysBones on " + go.name);
				for (int i = 0; i < numPhysBones - 1; i++)
				{
					VRCPhysBone physBone = physBones[i];
					DestroyImmediate(physBone);
				}
			}
		}
		Debug.Log("Removed duplicate VRCPhysBones");
	}
}
#endif