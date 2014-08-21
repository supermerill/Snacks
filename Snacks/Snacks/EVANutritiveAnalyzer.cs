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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Snacks
{
    class EVANutritiveAnalyzer : PartModule
    {

        [KSPField(isPersistant = true, guiActive = true, guiName = "Nutritive value:")]
        public double sampleAnalysis = 0;

        private double latitude = 0;
        private double longitude = 0;

        [KSPEvent(guiActive = true, guiName = "Perform Nutritive Analysis")]
        public void NutritiveAnalysis()
        {
            ScreenMessages.PostScreenMessage("Performing Nutritive Analysis", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            Vessel eva = FlightGlobals.ActiveVessel;
            if (!eva.Landed)
            {
                sampleAnalysis = 0;
                return;
            }
            if (latitude == 0 && longitude == 0)
            {
                latitude = eva.latitude;
                longitude = eva.longitude;
                System.Random rand = new System.Random();
                sampleAnalysis = rand.NextDouble() * 200;
                return;
            }
            Debug.Log("lat:" + eva.latitude + " long:" + eva.longitude + " slat:" + latitude + "slong:" + longitude);
            double diff1 = Math.Abs(eva.latitude - latitude);
            double diff2 = Math.Abs(eva.longitude - longitude);
            Debug.Log("diff1:" + diff1 + " diff2:" + diff2);
            if (diff1 < .01 && diff2 < .01)
            {
                ScreenMessages.PostScreenMessage("The soil hasn't changed much, better look somewhere else.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            else
            {
                //ScreenMessages.PostScreenMessage("The soil changed.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                latitude = eva.latitude;
                longitude = eva.longitude;
                System.Random rand = new System.Random();
                sampleAnalysis = rand.NextDouble() * 200 + 50;
            }

            // This will hide the Activate event, and show the Deactivate event.
            //Events["NutritiveAnalysis"].active = false;
            //Events["NutritiveAnalysis"].active = true;
        }

        [KSPEvent(guiActive = true, guiName = "Deactivate Snack Factory", active = false)]
        public void DeactivateEvent()
        {
            ScreenMessages.PostScreenMessage("Clicked Deactivate", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            // This will hide the Deactivate event, and show the Activate event.
           // Events["ActivateEvent"].active = true;
           // Events["DeactivateEvent"].active = false;
        }

        /*
         * Called after the scene is loaded.
         */
        public override void OnAwake()
        {
            Debug.Log("Nutritive Analyzer OnAwake()");
        }

        /*
         * Called when the part is activated/enabled. This usually occurs either when the craft
         * is launched or when the stage containing the part is activated.
         * You can activate your part manually by calling part.force_activate().
         */
        public override void OnActive()
        {
            Debug.Log("Snacks - Nutritive Analyzere");
        }

        /*
         * Called after OnAwake.
         */
        public override void OnStart(PartModule.StartState state)
        {
        }


        /*
         * KSP adds the return value to the information box in the VAB/SPH.
         */
        public override string GetInfo()
        {
            return "Generates snacks";
        }

        /*
         * Called when the part is deactivated. Usually because it was destroyed.
         */
        public override void OnInactive()
        {
            Debug.Log("Snacks - Start FlightController Destroyed");
        }

        /*
         * Called when the game is loading the part information. It comes from: the part's cfg file,
         * the .craft file, the persistence file, or the quicksave file.
         */
        public override void OnLoad(ConfigNode node)
        {

        }

        /*
         * Called when the game is saving the part information.
         */
        public override void OnSave(ConfigNode node)
        {

        }
    }
}