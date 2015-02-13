using System;

using ScrollsModLoader.Interfaces;
using UnityEngine;
using Mono.Cecil;
//using Mono.Cecil;
//using ScrollsModLoader.Interfaces;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Reflection;
using JsonFx.Json;
using System.Text.RegularExpressions;


namespace battlepaint.mod
{



    /*struct playerinfo 
    {
        public string ID;
        public string name;
    }*/

    

    public class battlepaint : BaseMod
	{



        Color dblack = new Color(1f, 1f, 1f, 0.5f);

        bool pushtopaint = true;

        

		//initialize everything here, Game is loaded at this point4
        public battlepaint()
        {

        }




        public static string GetName()
		{
            return "battlepaint";
		}

		public static int GetVersion ()
		{
			return 3;
		}

		//only return MethodDefinitions you obtained through the scrollsTypes object
		//safety first! surround with try/catch and return an empty array in case it fails
		public static MethodDefinition[] GetHooks (TypeDefinitionCollection scrollsTypes, int version)
		{
            try
            {
                return new MethodDefinition[] {
                 scrollsTypes["BattleMode"].Methods.GetMethod("OnGUI")[0],
             };
            }
            catch
            {
                return new MethodDefinition[] { };
            }
		}


        public override bool WantsToReplace(InvocationInfo info)
        {
            return false;
        }


        public override void ReplaceMethod(InvocationInfo info, out object returnValue)
        {

            returnValue = null;
        }

        public override void BeforeInvoke(InvocationInfo info)
        {

            return;

        }


        Screendrawer scdraw = new Screendrawer();

        public override void AfterInvoke (InvocationInfo info, ref object returnValue)
        //public override bool BeforeInvoke(InvocationInfo info, out object returnValue)
        {

            if (info.target is BattleMode && info.targetMethod.Equals("OnGUI"))
            {

                //todo: paint fancy buttons!

                float num2 = (float)Screen.height * 0.03f;
                float num3 = num2 * 1.39f;
                //menu button is:
                Rect position3 = new Rect(-num3, -num2, num3 + num3, num2 + num2);

                GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/CardListPopupGradient");

                GUI.color = Color.white;

                Rect onOff = new Rect(position3.xMax + 5, 2f, num3+num3, num2);
                string button = "ON";
                if (this.pushtopaint) button = "PTP";
                if (GUI.Button(onOff, button))
                {
                    this.pushtopaint = !this.pushtopaint;
                }

                GUI.color = Color.white;
                if (scdraw.currentColor != Color.green) GUI.color = dblack;

                Rect greenb = new Rect(onOff.xMax + 5, 2f, num3, num2);

                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;

                if (GUI.Button(greenb, ""))
                {
                    scdraw.currentColor = Color.green;
                }

                GUI.color = Color.white;
                if (scdraw.currentColor != Color.red) GUI.color = dblack;

                Rect redb = new Rect(greenb.xMax + 5, 2f, num3, num2);
                GUI.backgroundColor = Color.red;
                if (GUI.Button(redb,""))
                {
                    scdraw.currentColor = Color.red;
                }

                GUI.color = Color.white;
                if (scdraw.currentColor != Color.yellow) GUI.color = dblack;

                Rect yellowb = new Rect(redb.xMax + 5, 2f, num3, num2);
                GUI.backgroundColor = Color.yellow;
                if (GUI.Button(yellowb, ""))
                {
                    scdraw.currentColor = Color.yellow;
                }


                GUI.color = Color.white;
                if (scdraw.currentColor != Color.blue) GUI.color = dblack;

                Rect blueb = new Rect(yellowb.xMax + 5, 2f, num3, num2);
                GUI.backgroundColor = Color.blue;
                if (GUI.Button(blueb, ""))
                {
                    scdraw.currentColor = Color.blue;
                }


                GUI.color = Color.white;
                GUI.backgroundColor = oldColor;
                scdraw.Update(this.pushtopaint);
            }

            return;
        }

        
	}
}

