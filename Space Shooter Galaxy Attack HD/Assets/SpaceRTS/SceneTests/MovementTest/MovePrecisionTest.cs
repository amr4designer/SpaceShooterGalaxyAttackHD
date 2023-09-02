using NullPointerCore.Backend.Commands;
using NullPointerCore.Extras;
using SpaceRTSKit.Commands;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceRTSKit.Tests
{
	public class MovePrecisionTest : MonoBehaviour
	{
		public List<CommandController> testSubjects = new List<CommandController>();
		public List<Transform> waypoints = new List<Transform>();

		public void OnDrawGizmos()
		{
			foreach(Transform wp in waypoints)
				GizmosExt.DrawWireCircle(wp.position, 1);
		}

		public void OnGUI()
		{
			int i=2;
			foreach(CommandController nav in testSubjects)
			{
				if( GUI.Button(new Rect(10, Screen.height-(i*26), 220, 22), "Run " + nav.gameObject.name) )
				{
					foreach(Transform wp in waypoints)
					{
						nav.Append(new CmdMove(wp.position, Vector3.zero));
					}
				}
				i++;
			}
		}
	}
}