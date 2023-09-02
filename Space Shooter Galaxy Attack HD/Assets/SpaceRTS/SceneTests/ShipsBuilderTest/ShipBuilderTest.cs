using GameBase;
using NullPointerGame.BuildSystem;
using UnityEngine;

namespace SpaceRTSKit.Tests
{
	public class ShipBuilderTest : SceneTestCommon
	{
		protected override void OnSelectedUnitDrawMenu(GameEntity selectedUnit, Rect contentRect)
		{
			Builder builder = selectedUnit.GetComponent<Builder>();
			if (builder != null)
			{
				
				foreach (UnitConfig toBuild in builder.buildables)
				{
					if (GUILayout.Button(toBuild.name))
						builder.Build(toBuild);
				}
			}
		}
	}
}