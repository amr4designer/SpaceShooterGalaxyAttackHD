using GameBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SpaceRTSKit.Core;

namespace SpaceRTSKit.Tests
{
	public class SceneTestCommon : MonoBehaviour
	{
		private SelectionSystem selectionSystem = null;
		private HumanPlayerHandler rtsScene = null;

		private GameEntity selectedUnit;
		private bool mouseOverMenu = false;

		protected SelectionSystem SelectionSystem { get { return selectionSystem; } }
		protected GameEntity SelectedUnit { get { return selectedUnit; } }
		protected HumanPlayerHandler RTSScene { get { return rtsScene; } }
		protected virtual void Start()
		{
			GameScene gs = GetComponent<GameScene>();
			selectionSystem = gs.Get<SelectionSystem>();
			rtsScene = FindObjectOfType<HumanPlayerHandler>();
			if(selectionSystem)
			{
				selectionSystem.selectionAddedEntity += OnUnitSelected;
				selectionSystem.selectionRemovedEntity += OnUnitUnselected;
			}
		}

		protected virtual void OnDestroy()
		{
			if(selectionSystem)
			{
				selectionSystem.selectionAddedEntity -= OnUnitSelected;
				selectionSystem.selectionRemovedEntity -= OnUnitUnselected;
			}
		}

		protected virtual void OnUnitSelected(GameEntity obj)
		{
			selectedUnit = obj;	
		}

		protected virtual void OnUnitUnselected(GameEntity obj)
		{
			selectedUnit = null;
			if(mouseOverMenu && rtsScene)
			{
				mouseOverMenu = false;
				rtsScene.OnPointerEnterGameArea();
			}
		}

		protected virtual void OnGUI()
		{
			if (selectedUnit != null)
			{
				Rect menuRect = new Rect(15, Screen.height - 15 - 200, 220, 200);
				Vector2 mousePos = new Vector3(Input.mousePosition.x, Screen.height-Input.mousePosition.y);
				bool newMouseOverMenu = menuRect.Contains(mousePos);
				if(mouseOverMenu && !newMouseOverMenu && rtsScene)
					rtsScene.OnPointerEnterGameArea();
				if(!mouseOverMenu && newMouseOverMenu && rtsScene)
					rtsScene.OnPointerExitGameArea();
				mouseOverMenu = newMouseOverMenu;

				GUI.Box(menuRect, selectedUnit.name);
				menuRect.xMin += 5;
				menuRect.xMax -= 5;
				menuRect.yMin += 22;
				menuRect.yMax -= 5;
				GUILayout.BeginArea(menuRect, GUI.skin.box);
				OnSelectedUnitDrawMenu(selectedUnit, menuRect);
				GUILayout.EndArea();
			}
		}

		protected virtual void OnSelectedUnitDrawMenu(GameEntity selectedUnit, Rect contentRect)
		{
		}
	}
}
