/**
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace Snacks
{
    class SnackConfiguration
    {

		private ConfigNode node;
		public double SnacksPerMeal { get; private set; }
		public int NumberMealDiet { get; private set; }
		public double ElectricChargePerSnack { get; private set; }
		public double LossPerDayPerKerbal { get; private set; }
		public int SnackResourceId { get; private set; }
		public int MealsPerDay { get; private set; }
		public double DelayedReaction { get; private set; }
		public bool KerbalDeath { get; private set; }
		public double EvaSnack { get; private set; }
		public int NumberMealDietOnEva { get; private set; }
		public string SnacksResourceName { get; private set; }
		public double StupididyForgotChance { get; private set; }
		public double CouragousDoubleChance { get; private set; }
		public double BadassDietChance { get; private set; }
		public int BonusSnacksPerMeal { get; private set; }


        private SnackConfiguration()
		{
			string file = IOUtils.GetFilePathFor(this.GetType(), "snacks.cfg");
			Debug.Log("loading file:" + file);
			node = ConfigNode.Load(file).GetNode("SNACKS");

			Debug.Log("[SNACK]conf init");
			SnacksResourceName = node.GetValue("snacksResourceName");
			Debug.Log("[SNACK]conf snackResourceName=" + SnacksResourceName);
			PartResourceDefinition snacksResource = PartResourceLibrary.Instance.GetDefinition(SnacksResourceName);
			Debug.Log("[SNACK]conf snacksResource=" + snacksResource);
			SnackResourceId = snacksResource.id;
			Debug.Log("[SNACK]conf snackResourceId=" + SnackResourceId);
            //PartResourceDefinition soilResource = PartResourceLibrary.Instance.GetDefinition("Soil");
            //soilResourceId = soilResource.id;
			SnacksPerMeal = double.Parse(node.GetValue("snacksPerMeal"));
			Debug.Log("[SNACK]conf snacksPerMeal=" + SnacksPerMeal);
			ElectricChargePerSnack = double.Parse(node.GetValue("electricChargePerSnack"));
			Debug.Log("[SNACK]conf electricChargePerMeal=" + ElectricChargePerSnack);
			LossPerDayPerKerbal = double.Parse(node.GetValue("repLossPercent"));
			Debug.Log("[SNACK]conf lossPerDayPerKerbal=" + LossPerDayPerKerbal);
			MealsPerDay = int.Parse(node.GetValue("mealsPerDay"));
			Debug.Log("[SNACK]conf mealsPerDay=" + MealsPerDay);
			DelayedReaction = double.Parse(node.GetValue("delayedReaction"));
			Debug.Log("[SNACK]conf delayedReaction=" + DelayedReaction);
			KerbalDeath = bool.Parse(node.GetValue("kerbalDeath"));
			Debug.Log("[SNACK]conf kerbalDeath=" + KerbalDeath);
			EvaSnack = double.Parse(node.GetValue("evaSnack"));
			Debug.Log("[SNACK]conf evaSnack=" + EvaSnack);
			StupididyForgotChance = double.Parse(node.GetValue("stupididyForgotChance"));
			Debug.Log("[SNACK]conf StupididyForgotChance=" + StupididyForgotChance);
			CouragousDoubleChance = double.Parse(node.GetValue("couragousDoubleChance"));
			Debug.Log("[SNACK]conf CouragousDoubleChance=" + CouragousDoubleChance);
			BadassDietChance = double.Parse(node.GetValue("badassDietChance"));
			Debug.Log("[SNACK]conf BadassDietChance=" + BadassDietChance);
			NumberMealDiet = int.Parse(node.GetValue("numberMealDiet"));
			Debug.Log("[SNACK]conf BadassDietChance=" + BadassDietChance);
			NumberMealDietOnEva = int.Parse(node.GetValue("numberMealDietOnEva"));
			Debug.Log("[SNACK]conf NumberMealDietOnEva=" + NumberMealDietOnEva);
			BonusSnacksPerMeal = int.Parse(node.GetValue("bonusSnacksPerMeal"));
			Debug.Log("[SNACK]conf BonusSnacksPerMeal=" + BonusSnacksPerMeal);
			
			
        
        }

        private static SnackConfiguration snackConfig;

        public static SnackConfiguration Instance()
        {
            if(snackConfig == null)
                snackConfig = new SnackConfiguration();
            return snackConfig;
        }
    }
}
