﻿/**
The MIT License (MIT)
Copyright (c) 2014 Troy Gruetzmacher

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 * 
 * 
 * */

using KSP.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Snacks
{
	[KSPAddon(KSPAddon.Startup.Flight | KSPAddon.Startup.EveryScene, false)]
	class SupplyWindow : KSPPluginFramework.MonoBehaviourWindow
	{

		private Texture2D texture;
		private static ApplicationLauncherButton button;
		private Vector2 scrollPos = new Vector2();
		private GUIStyle redStyle;
		private GUIStyle regStyle;
		private GUIStyle yelStyle;
		private GUIStyle hedStyle;

		internal override void Awake()
		{
			WindowCaption = "Snack Supply";
			WindowRect = new Rect(0, 0, 300, 300);
			Visible = false;
			string textureName = "Snacks/Textures/snacks";
			texture = GameDatabase.Instance.GetTexture(textureName, false);
			GameEvents.onGUIApplicationLauncherReady.Add(SetupGUI);
		}

		private void SetupGUI()
		{
			if (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER)
			{
				if (button == null)
					button = ApplicationLauncher.Instance.AddModApplication(ShowGUI, HideGUI, null, null, null, null, ApplicationLauncher.AppScenes.ALWAYS, texture);
			}
			else if (button != null)
				ApplicationLauncher.Instance.RemoveModApplication(button);
		}


		private void ShowGUI()
		{
			Visible = true;
		}

		private void HideGUI()
		{
			Visible = false;
		}

		private void SetupStyles()
		{

			hedStyle = new GUIStyle(GUI.skin.label);
			//hedStyle.fontSize = hedStyle.fontSize + 2;
			regStyle = new GUIStyle(GUI.skin.label);
			regStyle.margin = new RectOffset(25, 0, 0, 0);
			redStyle = new GUIStyle(regStyle);
			redStyle.normal.textColor = Color.red;
			yelStyle = new GUIStyle(regStyle);
			yelStyle.normal.textColor = Color.yellow;
		}

		internal override void DrawWindow(int id)
		{
			if (hedStyle == null)
				SetupStyles();
			DragEnabled = true;
			TooltipsEnabled = true;
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300), GUILayout.Width(300));

			Dictionary<int, List<ShipSupply>> snapshot = SnackSnapshot.Instance().Vessels();
			var keys = snapshot.Keys.ToList();
			keys.Sort();
			foreach (int planet in keys)
			{

				List<ShipSupply> supplies;
				snapshot.TryGetValue(planet, out supplies);
				//supplies.Sort();
				GUILayout.Label(supplies.First().BodyName + ":", hedStyle);
				foreach (ShipSupply supply in supplies)
				{
					if (supply.Percent > 50)
						GUILayout.Label(new GUIContent(supply.VesselName + ": " + supply.SnackAmount + "/" + supply.SnackMaxAmount, "Crew: " + supply.CrewCount + "  Duration*: " + supply.DayEstimate + " days"), regStyle);
					else if (supply.Percent > 25)
						GUILayout.Label(new GUIContent(supply.VesselName + ": " + supply.SnackAmount + "/" + supply.SnackMaxAmount, "Crew: " + supply.CrewCount + "  Duration*: " + supply.DayEstimate + " days"), yelStyle);
					else
						GUILayout.Label(new GUIContent(supply.VesselName + ": " + supply.SnackAmount + "/" + supply.SnackMaxAmount, "Crew: " + supply.CrewCount + "  Duration*: " + supply.DayEstimate + " days"), redStyle);
				}
			}

			GUILayout.EndScrollView();

		}
		private void onDestroy()
		{
			Debug.Log("SupplyWindow destroyed");
			if (button != null)
				ApplicationLauncher.Instance.RemoveModApplication(button);
		}
	}


}
