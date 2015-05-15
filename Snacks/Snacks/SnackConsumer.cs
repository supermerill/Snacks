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

namespace Snacks
{

	//Register BackgroundProcessing on Eva
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	class RegisterSnackEvaForBackgroundProcessing : MonoBehaviour
	{
		void Start()
		{
			print("[SNACKS] register eva snackConsumer");
			AvailablePart evaAPart = PartLoader.getPartInfoByName("kerbalEVA");
			try
			{
				evaAPart.partPrefab.AddModule("SnackConsumer");
			}
			catch (NullReferenceException e)
			{
				print("[SNACKS] register eva snackConsumer partPrefab =  " + evaAPart.partPrefab);
				print("[SNACKS] register eva snackConsumer Modules = " + evaAPart.partPrefab.Modules);
				print("[SNACKS] register eva snackConsumer Modules.Count = " + evaAPart.partPrefab.Modules.Count);
				//???
			}
		}
	}

	class SnackConsumer : PartModule
	{
		//private static System.Random random = new System.Random();

		//before next lunch inside this part
		[KSPField(isPersistant = true)]
		public double nextSnackTime = 0;

		//Are we in a critic state?
		[KSPField(isPersistant = true)]
		public int diet = 0;

		public override void OnStart(PartModule.StartState state)
		{
			base.OnStart(state);
			Debug.Log("[SNACK]Start on part " + part.name);
		}

		public override void OnLoad(ConfigNode node)
		{
			base.OnLoad(node);
			Debug.Log("[SNACK]OnLoad " + part.name + " : nextSnackTime=" + nextSnackTime);
		}

		public override void OnSave(ConfigNode node)
		{
			base.OnSave(node);
			Debug.Log("[SNACK]OnSave " + part.name + " : nextSnackTime=" + nextSnackTime);
		}

		public virtual void FixedUpdate()
		{
			double currentTime = Planetarium.GetUniversalTime();
			//Debug.Log("[SNACK]fixedUpdate! " + currentTime + " ? " + nextSnackTime);
			if (currentTime > nextSnackTime)
			{
				if (nextSnackTime == 0)
				{
					nextSnackTime = nextMeal(currentTime);
					Debug.Log("[SNACK]fixedUpdate! init => " + nextSnackTime);
				}
				else
				{
					nextSnackTime = nextMeal(nextSnackTime);
					Debug.Log("[SNACK]fixedUpdate! eat snack! => " + nextSnackTime);
					EatSnacks();
					SnackSnapshot.Instance().SetRebuildSnapshot();
				}
			}
		}


		//public void EatSnacks()
		//{
		//	//consume snacks?
		//	float snacksRequired = (float)CalculateSnacksRequired();
		//	Debug.Log("[SNACK]eat snacksRequired: " + snacksRequired);
		//	if (snacksRequired == 0) return;

		//	float snacksGet = part.RequestResource(SnackConfiguration.Instance().SnackResourceId, snacksRequired);
		//	float snacksMissed = snacksRequired - snacksGet;
		//	Debug.Log("[SNACK]eat snacksEated: " + snacksGet);

		//	//if not engough snacks to eat
		//	if (snacksMissed > 0)
		//	{
		//		//let a chance to player to reactivate a locked container, to redock
		//		if (!diet)
		//		{
		//			diet = true;
		//			ScreenMessages.PostScreenMessage(
		//					 " Kerbals are starving "
		//				 + " inside " + vessel.name
		//				 + "!! They need snacks!"
		//					 , 14, ScreenMessageStyle.UPPER_CENTER);
		//			//TODO: stop the timeWarp
		//		}
		//		else
		//		{

		//			Debug.Log("No snacks for: " + vessel.name);
		//			int fastingKerbals = Convert.ToInt32(snacksMissed / SnackConfiguration.Instance().SnacksPerMeal);
		//			if (SnackConfiguration.Instance().KerbalDeath)
		//			{
		//				//if it's the first depletion, don't kill the cre instantly : 
		//				// give the player a little room for re-docking or unlocking the canister
		//				if (snacksRequired > snacksMissed)
		//				{
		//					ScreenMessages.PostScreenMessage(
		//						fastingKerbals + " Kerbals are starving "
		//					+ " inside " + vessel.name
		//					+ "!! They need snacks!"
		//						, 14, ScreenMessageStyle.UPPER_CENTER);
		//				}
		//				else
		//				{
		//					//real Starve to dead (you had the meal interval to correct that!)
		//					killKerbals();
		//				}
		//			}
		//			else
		//			{
		//				if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
		//				{
		//					double repLoss;
		//					if (Reputation.CurrentRep > 0)
		//						repLoss = fastingKerbals * SnackConfiguration.Instance().LossPerDayPerKerbal * Reputation.Instance.reputation;
		//					else
		//						repLoss = fastingKerbals;

		//					Reputation.Instance.AddReputation(Convert.ToSingle(-1 * repLoss), TransactionReasons.Any);
		//					ScreenMessages.PostScreenMessage(fastingKerbals + " Kerbals didn't have any snacks(reputation decreased by " + Convert.ToInt32(repLoss) + ")", 5, ScreenMessageStyle.UPPER_LEFT);
		//				}
		//				else
		//				{
		//					ScreenMessages.PostScreenMessage(fastingKerbals + " Kerbals didn't have any snacks.", 5, ScreenMessageStyle.UPPER_LEFT);
		//				}
		//			}
		//		}
		//	}
		//}

		public void EatSnacks()
		{
			Debug.Log("[SNACK]MODULE EatSnacks ");
			//consume snacks?
			float snacksRequired = (float)CalculateSnacksRequired();
			diet = EatSnacks(vessel, snacksRequired, diet, myResourceRequest, killKerbals);
		}
		public float myResourceRequest(Vessel v, float amountRequired, string resourceName)
		{
			return part.RequestResource(resourceName, amountRequired);
		}



		private double CalculateSnacksRequired()
		{
			double snacksExtraPer = SnackConfiguration.Instance().BonusSnacksPerMeal;
			double demand = part.protoModuleCrew.Count * SnackConfiguration.Instance().SnacksPerMeal;
			double extra = 0;
			foreach (ProtoCrewMember crew in part.protoModuleCrew)
			{
				if (GetRandomChance(crew.courage / 2.0))
					extra += snacksExtraPer;
				if (GetRandomChance(crew.stupidity / 2.0))
					extra -= snacksExtraPer;
				if (crew.isBadass && GetRandomChance(.2))
					extra -= snacksExtraPer;
			}
			return extra + demand;
		}


		private void killKerbals()
		{
			Debug.Log("[SNACK]MOD killKerbals! " + part.name);
			if (part.name.Contains("kerbalEVA"))
			{
				ScreenMessages.PostScreenMessage(part.protoModuleCrew.Count + " has died from straving"
					+ " (including " + (part.protoModuleCrew.Count == 0 ? "?" : part.protoModuleCrew[0].name) + ")"
					+ " in EVA."
					, 10F, ScreenMessageStyle.UPPER_LEFT);
				Debug.Log("[SNACK]EVA DIE " + " on part " + part.name);
				part.explode();
			}
			else
			{
				ScreenMessages.PostScreenMessage(part.protoModuleCrew.Count + " kerbals has died from straving"
					+ " (including " + (part.protoModuleCrew.Count == 0 ? "?" : part.protoModuleCrew[0].name) + ")"
					+ " inside " + vessel.name
					, 10F, ScreenMessageStyle.UPPER_LEFT);
				List<ProtoCrewMember> killingList = new List<ProtoCrewMember>();
				Debug.Log("[SNACK]killingList = " + killingList.Count + " on part " + part.name);
				killingList.AddRange(part.protoModuleCrew);
				foreach (ProtoCrewMember tempCrewMember in killingList)
				{
					part.RemoveCrewmember(tempCrewMember);
					tempCrewMember.Die();
				}
			}
		}


		//same function as BackgroundProcessing.resourceRequest()
		//public static double GetSnackResource(List<ProtoPartSnapshot> protoPartSnapshots, double demand, string resourceName)
		//{
		//	double supplied = 0;
		//	bool resFound = false;
		//	foreach (ProtoPartSnapshot pps in protoPartSnapshots)
		//	{
		//		var res = from r in pps.resources
		//				  where r.resourceName == resourceName
		//				  select r;
		//		if (res.Count() > 0)
		//		{
		//			Debug.Log("[SNACK]BP find snacks closet");
		//			resFound = true;
		//			ConfigNode node = res.First().resourceValues;
		//			double amount = Double.Parse(node.GetValue("amount"));
		//			Debug.Log("[SNACK]BP find " + amount + " " + resourceName);
		//			if (amount >= demand)
		//			{
		//				node.SetValue("amount", (amount -= demand).ToString());
		//				supplied += demand;
		//				Debug.Log("[SNACK]BP get " + demand + ", now has " + Double.Parse(node.GetValue("amount")) + " " + resourceName);
		//				return supplied;
		//			}
		//			else
		//			{
		//				node.SetValue("amount", "0");
		//				supplied += amount;
		//				Debug.Log("[SNACK]BP get " + demand + ", now has " + Double.Parse(node.GetValue("amount")) + " " + resourceName);
		//				demand -= amount;
		//			}
		//			Debug.Log("[SNACK]BP has in my hand " + supplied);
		//		}
		//	}
		//	if (!resFound)
		//		return demand;//if no snack resources were found, this vessel has not been loaded.  Feed them from the magic bucket.
		//	return supplied;
		//}

		private static bool GetRandomChance(double prob)
		{
			if (new System.Random().NextDouble() < prob)
				return true;
			return false;
		}

		// ---- BACKGROUND PROCESSING part --------------------------------------------------------------------

		public class BackgroundData
		{
			public List<float> kerbalStupidty;
			public List<float> kerbalCourage;
			public List<bool> kerbalIsBadass;
			//TODO: get nextsnacktime from node
			public double nextSnackTime = 0;
			public int diet = 0;

			private Vessel vessel;
			private uint partId;

			public BackgroundData(Vessel v, uint partFlightId)
			{
				vessel = v;
				partId = partFlightId;
				ProtoPartSnapshot p = getProtoPart(vessel, partId);

				kerbalStupidty = new List<float>();
				kerbalCourage = new List<float>();
				kerbalIsBadass = new List<bool>();
				nextSnackTime = 0;

				foreach (ProtoCrewMember pc in p.protoModuleCrew)
				{
					kerbalCourage.Add(pc.courage);
					kerbalStupidty.Add(pc.stupidity);
					kerbalIsBadass.Add(pc.isBadass);
				}
				//get our module
				foreach (ProtoPartModuleSnapshot module in p.modules)
				{
					if (module.moduleName.Equals("SnackConsumer"))
					{
						Debug.Log("[SNACK]load our module " + p.partName + " : " + module.moduleValues.GetValue("nextSnackTime"));
						if (module.moduleValues != null && module.moduleValues.GetValue("nextSnackTime") != null)
						{
							nextSnackTime = double.Parse(module.moduleValues.GetValue("nextSnackTime"));
							diet = int.Parse(module.moduleValues.GetValue("diet"));
						}
					}
				}
			}

			public void killKerbals()
			{
				ProtoPartSnapshot partSnapshot = getProtoPart(vessel, partId);
				Debug.Log("[SNACK]BP killKerbals! " + partSnapshot.partName);
				if (partSnapshot.partName.Contains("kerbalEVA"))
				{
					ScreenMessages.PostScreenMessage(
						(partSnapshot.protoModuleCrew.Count == 0 ? "A Kerbal" : partSnapshot.protoModuleCrew[0].name)
						+ " has died from straving in EVA."
						, 10F, ScreenMessageStyle.UPPER_LEFT);
					Debug.Log("[SNACK]EVA DIE " + " on part " + partSnapshot.partName);
					HighLogic.CurrentGame.DestroyVessel(vessel);
					vessel.DestroyVesselComponents();
					FlightGlobals.DestroyObject(vessel.gameObject);
					//UnityEngine.GameObject.Destroy(vessel.gameObject);
				}
				else
				{
					ScreenMessages.PostScreenMessage(partSnapshot.protoModuleCrew.Count + " kerbals has died from straving"
						+ " (including "
						+ (partSnapshot.protoModuleCrew.Count == 0 ? "?" : partSnapshot.protoModuleCrew[0].name) + ")"
						+ " inside " + vessel.name
						, 10F, ScreenMessageStyle.UPPER_LEFT);
				List<ProtoCrewMember> killingList = new List<ProtoCrewMember>();
				Debug.Log("[SNACK]killingList = " + killingList.Count + " on part " + partSnapshot.partName);
				killingList.AddRange(partSnapshot.protoModuleCrew);
				foreach (ProtoCrewMember tempCrewMember in killingList)
				{
					partSnapshot.RemoveCrew(tempCrewMember);
					tempCrewMember.Die();
				}
				}
			}
		}

		public static void BackgroundLoad(Vessel v, uint partFlightId, ref System.Object data)
		{
			Debug.Log("[SNACK]BP BackgroundLoad");
			//need to load data for MY part (unless that, multiple cockpit consume multiple times.
			//data = new BackgroundData(v);
			data = new BackgroundData(v, partFlightId);
		}

		public static void BackgroundSave(Vessel v, uint partFlightId, System.Object data)
		{
			if (!(data is BackgroundData))
			{
				return;
			}
			Debug.Log("[SNACK]BP BackgroundSave " + getProtoPart(v, partFlightId).partName + " nextSnackTime " + ((BackgroundData)data).nextSnackTime);
			//get our module
			foreach (ProtoPartModuleSnapshot module in getProtoPart(v, partFlightId).modules)
			{
				if (module.moduleName.Equals("SnackConsumer"))
				{
					module.moduleValues.SetValue("nextSnackTime", ((BackgroundData)data).nextSnackTime.ToString());
				}
			}
		}

		public static ProtoPartSnapshot getProtoPart(Vessel v, uint partFlightId)
		{
			foreach (ProtoPartSnapshot tempPartSnapshot in v.protoVessel.protoPartSnapshots)
			{
				if (tempPartSnapshot.flightID == partFlightId)
				{
					return tempPartSnapshot;
				}
			}
			return null;
		}

		public static double nextMeal(double lastMealTime)
		{
			double snackFrequency = 6 * 60 * 60 / SnackConfiguration.Instance().MealsPerDay;
			Debug.Log("[SNACK]BP snackFrequency =  6 * 60 * 60 / " + SnackConfiguration.Instance().MealsPerDay);
			Debug.Log("[SNACK]BP snackFrequency =  " + snackFrequency);
			Debug.Log("[SNACK]BP snackFrequency =  6 * 60 * 60 / " + SnackConfiguration.Instance().MealsPerDay);
			double newObj = (new System.Random().NextDouble() + 0.5) * snackFrequency + lastMealTime;
			Debug.Log("[SNACK]BP snackTiemAdd = " + (newObj - lastMealTime) + " to " + lastMealTime + " => " + newObj);
			return newObj;
		}

		public static void FixedBackgroundUpdate(Vessel v, uint partFlightID,
			Func<Vessel, float, string, float> resourceRequest, ref System.Object objectData)
		{

			//Debug.Log("[SNACK]BP FixedBackgroundUpdate");
			try
			{
				BackgroundData data = (BackgroundData)objectData;
				//Debug.Log("[SNACK]BP FixedBackgroundUpdate " + data.kerbalStupidty.Count);
				//has Kerbals?
				if (data.kerbalStupidty.Count <= 0)
				{
					return;
				}

				//Debug.Log("[SNACK]BP FixedBackgroundUpdate time test " + Planetarium.GetUniversalTime() + ", "
				//	+ Planetarium.fetch.time + ", "
				//	+ Planetarium.fetch.timeScale + ", "
				//	+ Planetarium.fetch.fixedDeltaTime );
				//how many meals has passed?
				//TODO: one multi-meal instead of multi one-meal
				double currentTime = Planetarium.GetUniversalTime();
				//Debug.Log("[SNACK]BP FixedBackgroundUpdate time test "
				//	+ (int)((currentTime / (3600*24))) + "d"
				//	+ (int)((currentTime / 3600) % 6) + "h"
				//	+ (int)((currentTime / 60) % 60) + "m"
				//	+ (int)(currentTime % 60) + "s");
				currentTime = Planetarium.GetUniversalTime();

				//Debug.Log("[SNACK]BP FixedBackgroundUpdate " + currentTime + " ? " + data.nextSnackTime);
				while (currentTime > data.nextSnackTime)
				{
					if (data.nextSnackTime == 0)
					{
						Debug.Log("[SNACK]BP FixedBackgroundUpdate Init");
						data.nextSnackTime = nextMeal(currentTime);
					}
					else
					{
						Debug.Log("[SNACK]BP FixedBackgroundUpdate eat on part " + getProtoPart(v, partFlightID).partName);
						data.nextSnackTime = nextMeal(data.nextSnackTime);
						EatSnacks(v, partFlightID, resourceRequest, data);
						//maybe a bit too much?
						//i didn't know if this method is not too cpu-intensive...
						SnackSnapshot.Instance().SetRebuildSnapshot();
					}
				}

			}
			catch (Exception ex)
			{
				Debug.Log("Snacks - EatSnacks: " + ex.Message + ex.StackTrace);
			}
		}

		public static void EatSnacks(Vessel v, uint partFlightID, Func<Vessel, float, string, float> resourceRequest, BackgroundData data)
		{
			Debug.Log("[SNACK]BP EatSnacks ");
			//consume snacks?
			float snacksRequired = (float)CalculateSnacksRequired(data);
			data.diet = EatSnacks(v, snacksRequired, data.diet, resourceRequest, data.killKerbals);
		}


		//public static void EatSnacks(Vessel v, uint partFlightID, Func<Vessel, float, string, float> resourceRequest, BackgroundData data)
		//{
		//	Debug.Log("[SNACK]BP EatSnacks ");
		//	//consume snacks?
		//	float snacksRequired = (float)CalculateSnacksRequired(data);
		//	Debug.Log("[SNACK]BP EatSnacks snacksRequired = " + snacksRequired);
		//	if (snacksRequired == 0) return;

		//	float electricityRequired = (float) SnackConfiguration.Instance().ElectricChargePerSnack * snacksRequired;
		//	Debug.Log("[SNACK]BP EatSnacks vessel = " + v + ", " + v.Parts.Count);
		//	Debug.Log("[SNACK]BP EatSnacks ctrlState = " + v.protoVessel.ctrlState);
		//	Debug.Log("[SNACK]BP EatSnacks discoveryInfo = " + v.protoVessel.discoveryInfo);
		//	float snacksGet = resourceRequest(v, snacksRequired, SnackConfiguration.Instance().SnacksResourceName);
		//	//float snacksGet = (float)GetSnackResource(v.protoVessel.protoPartSnapshots, snacksRequired, "Snacks");
		//	float electricityGet = 0;
		//	if (electricityRequired > 0) {
		//		electricityGet = resourceRequest(v, electricityRequired, "ElectricCharge");
		//	}
		//	float snacksMissed = snacksRequired - snacksGet;
		//	float electricityMissed = electricityRequired - electricityGet;
		//	Debug.Log("[SNACK]BP EatSnacks snacksEated = " + snacksGet);

		//	//if not engough snacks to eat
		//	if (snacksMissed > 0 || electricityMissed>0 )
		//	{
		//		Debug.Log("[SNACK]BP EatSnacks snacksMissed !! = " + snacksMissed + ", " + data.diet + ", " + electricityMissed);
		//		//if it's the first depletion, don't kill the cre instantly : 
		//		// give the player a little room for re-docking or unlocking the canister
		//		if (!data.diet)
		//		{
		//			data.diet = true;
		//			ScreenMessages.PostScreenMessage(
		//					 " Kerbals are starving "
		//				 + " inside " + v.name
		//				 + "!! They need some "+(snacksMissed>0?"Snacks! ":"")
		//				 + (electricityMissed > 0 ? "Electricty!" : "")
		//					 , 14, ScreenMessageStyle.UPPER_CENTER);
		//			//TODO: stop the timeWarp
		//		}
		//		else
		//		{

		//			Debug.Log("No snacks/elec for: " + v.vesselName);
		//			int fastingKerbals = Convert.ToInt32(snacksMissed / SnackConfiguration.Instance().SnacksPerMeal);
		//			int freezingKerbals = Convert.ToInt32(electricityMissed / 
		//				(SnackConfiguration.Instance().SnacksPerMeal*SnackConfiguration.Instance().ElectricChargePerSnack));
		//			if (SnackConfiguration.Instance().KerbalDeath)
		//			{
		//				killKerbals(v, partFlightID);
		//			}
		//			else
		//			{
		//				if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
		//				{
		//					double repLoss;
		//					int maxKerbalsMiss = Math.Max(freezingKerbals, fastingKerbals); 
		//					if (Reputation.CurrentRep > 0)
		//						repLoss = maxKerbalsMiss * SnackConfiguration.Instance().LossPerDayPerKerbal * Reputation.Instance.reputation;
		//					else
		//						repLoss = maxKerbalsMiss;

		//					Reputation.Instance.AddReputation(Convert.ToSingle(-1 * repLoss), TransactionReasons.Any);
		//					ScreenMessages.PostScreenMessage(maxKerbalsMiss + " Kerbals didn't have any "
		//					+ (snacksMissed>0?"Snacks ":"")
		//					+ (electricityMissed > 0 ? "Electricty" : "")
		//					+ "(reputation decreased by " + Convert.ToInt32(repLoss) + ")", 5, ScreenMessageStyle.UPPER_LEFT);
		//				}
		//				else
		//				{
		//					ScreenMessages.PostScreenMessage(fastingKerbals + " Kerbals didn't have any snacks.", 5, ScreenMessageStyle.UPPER_LEFT);
		//				}
		//			}
		//		}
		//	}
		//}

		public static int EatSnacks(Vessel v, float snacksRequired, int diet,
			Func<Vessel, float, string, float> resourceRequest, Action killKerbals)
		{
			Debug.Log("[SNACK]ES EatSnacks ");
			//consume snacks?
			//float snacksRequired = (float)CalculateSnacksRequired(data);
			Debug.Log("[SNACK]ES EatSnacks snacksRequired = " + snacksRequired);
			if (snacksRequired == 0) return diet;

			float electricityRequired = (float)SnackConfiguration.Instance().ElectricChargePerSnack * snacksRequired;
			Debug.Log("[SNACK]ES EatSnacks vessel = " + v + ", " + v.Parts.Count);
			Debug.Log("[SNACK]ES EatSnacks ctrlState = " + v.protoVessel.ctrlState);
			Debug.Log("[SNACK]ES EatSnacks discoveryInfo = " + v.protoVessel.discoveryInfo);
			float snacksGet = resourceRequest(v, snacksRequired, SnackConfiguration.Instance().SnacksResourceName);
			//float snacksGet = (float)GetSnackResource(v.protoVessel.protoPartSnapshots, snacksRequired, "Snacks");
			float electricityGet = 0;
			if (electricityRequired > 0)
			{
				electricityGet = resourceRequest(v, electricityRequired, "ElectricCharge");
			}
			float snacksMissed = snacksRequired - snacksGet;
			float electricityMissed = electricityRequired - electricityGet;
			Debug.Log("[SNACK]ES EatSnacks snacksEated = " + snacksGet);

			//if not engough snacks to eat
			if (snacksMissed > 0 || electricityMissed > 0)
			{
				Debug.Log("[SNACK]ES EatSnacks snacksMissed !! = " + snacksMissed + ", " + diet + ", " + electricityMissed);
				//if it's the first depletion, don't kill the cre instantly : 
				// give the player a little room for re-docking or unlocking the canister
				if (diet < SnackConfiguration.Instance().NumberMealDiet)
				{
					diet++;
					ScreenMessages.PostScreenMessage(
							 " Kerbals are starving "
						 + " inside " + v.name
						 + "!! They need some " + (snacksMissed > 0 ? "Snacks! " : "")
						 + (electricityMissed > 0 ? "Electricty!" : "")
							 , 10, ScreenMessageStyle.UPPER_CENTER);
					//TODO: stop the timeWarp on the first died alert
					if (diet == 1)
					{
						//to set the warp rate:
						if (TimeWarp.CurrentRateIndex > 8)
						{
							//reduce it QUICK!
							TimeWarp.SetRate(7, true);
							TimeWarp.SetRate(0, false);
						}
						else
						{ 
							TimeWarp.SetRate(0, false);
						}

						//so call it once and store the reference
					}
				}
				else
				{

					Debug.Log("No snacks/elec for: " + v.vesselName);
					int fastingKerbals = Convert.ToInt32(snacksMissed / SnackConfiguration.Instance().SnacksPerMeal);
					int freezingKerbals = (electricityMissed == 0 ? 0 : Convert.ToInt32(electricityMissed /
						(SnackConfiguration.Instance().SnacksPerMeal * SnackConfiguration.Instance().ElectricChargePerSnack)));
					if (SnackConfiguration.Instance().KerbalDeath)
					{
						killKerbals();
					}
					else
					{
						if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
						{
							double repLoss;
							int maxKerbalsMiss = Math.Max(freezingKerbals, fastingKerbals);
							if (Reputation.CurrentRep > 0)
								repLoss = maxKerbalsMiss * SnackConfiguration.Instance().LossPerDayPerKerbal * Reputation.Instance.reputation;
							else
								repLoss = maxKerbalsMiss;

							Reputation.Instance.AddReputation(Convert.ToSingle(-1 * repLoss), TransactionReasons.Any);
							ScreenMessages.PostScreenMessage(maxKerbalsMiss + " Kerbals didn't have any "
							+ (snacksMissed > 0 ? "Snacks " : "")
							+ (electricityMissed > 0 ? "Electricty" : "")
							+ "(reputation decreased by " + Convert.ToInt32(repLoss) + ")", 5, ScreenMessageStyle.UPPER_LEFT);
						}
						else
						{
							ScreenMessages.PostScreenMessage(fastingKerbals + " Kerbals didn't have any snacks.", 5, ScreenMessageStyle.UPPER_LEFT);
						}
					}
				}
			}
			return diet;
		}

		private static double CalculateSnacksRequired(BackgroundData data)
		{
			double snacksExtraPer = SnackConfiguration.Instance().BonusSnacksPerMeal;
			double demand = data.kerbalCourage.Count * SnackConfiguration.Instance().SnacksPerMeal;
			Debug.Log("[SNACK]BP EatSnacks demand = " + demand);
			double extra = 0;
			for (int i = 0; i < data.kerbalCourage.Count; i++)
			{
				if (GetRandomChance(data.kerbalCourage[i] * SnackConfiguration.Instance().CouragousDoubleChance))
					extra += snacksExtraPer;
				if (GetRandomChance(data.kerbalStupidty[i] * SnackConfiguration.Instance().StupididyForgotChance))
					extra -= snacksExtraPer;
				if (data.kerbalIsBadass[i] && GetRandomChance(SnackConfiguration.Instance().BadassDietChance))
					extra -= snacksExtraPer;
			}
			Debug.Log("[SNACK]BP EatSnacks extra + demand = " + (extra + demand));
			return extra + demand;
		}

	}


}
