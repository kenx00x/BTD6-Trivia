using Assets.Scripts.Simulation;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.UI_New.Main;
using Harmony;
using MelonLoader;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using UnityEngine;
[assembly: MelonInfo(typeof(Trivia.Class1), "Trivia", "1.0.0", "kenx00x")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace Trivia
{
    public class Class1 : MelonMod
    {
        public static string question = "";
        public static string[] answers = new string[4];
        public static string correctAnswer = "";
        public static int staticRound;
        public override void OnApplicationStart()
        {
            MelonLogger.Log("Trivia loaded");
        }
        private static string ReplaceText(string input)
        {
            input = input.Replace("&quot;", "\"");
            input = input.Replace("&#039;", "'");
            input = input.Replace("&ldquo;", "\"");
            input = input.Replace("&rdquo;", "\"");
            input = input.Replace("&eacute;", "é");
            return input;
        }
        public override void OnGUI()
        {
            if (question != "")
            {
                GUIStyle questionStyle = new GUIStyle();
                GUIStyle buttonStyle = new GUIStyle();
                questionStyle.alignment = TextAnchor.MiddleCenter;
                questionStyle.normal.textColor = Color.white;
                Texture2D blackTexture = new Texture2D(1, 1);
                blackTexture.SetPixel(0, 0, Color.black);
                blackTexture.Apply();
                questionStyle.normal.background = blackTexture;
                Texture2D grayTexture = new Texture2D(1, 1);
                grayTexture.SetPixel(0, 0, Color.gray);
                grayTexture.Apply();
                buttonStyle = questionStyle;
                buttonStyle.hover.background = grayTexture;
                GUI.Label(new Rect(Screen.width / 2 - 400, 40, 800, 50f), question, questionStyle);
                if (GUI.Button(new Rect(Screen.width / 2 - 400, 90, 400, 30), answers[0], buttonStyle))
                {
                    checkAnswer(answers[0]);
                }
                if (GUI.Button(new Rect(Screen.width / 2, 90, 400, 30), answers[1], buttonStyle)) 
                {
                    checkAnswer(answers[1]);
                }
                if (GUI.Button(new Rect(Screen.width / 2 - 400, 120, 400, 30), answers[2], buttonStyle)) 
                {
                    checkAnswer(answers[2]);
                }
                if (GUI.Button(new Rect(Screen.width / 2, 120, 400, 30), answers[3], buttonStyle))
                {
                    checkAnswer(answers[3]);
                }
            }
        }
        private void checkAnswer(string answer)
        {
            if (correctAnswer == answer)
            {
                InGame.Bridge.simulation.cashManagers.entries[0].value.cash.Value += staticRound*25;
            }
            for (int i = 0; i < answers.Length - 1; i++)
            {
                answers[i] = "";
            }
            question = "";
        }
        [HarmonyPatch(typeof(Simulation), "OnRoundEnd")]
        public class Simulation_Patch
        {
            [HarmonyPostfix]
            public static void Postfix(int round)
            {
                if (question == "")
                {
                    staticRound = round;
                    System.Random rand = new System.Random();
                    string json = new WebClient().DownloadString("https://opentdb.com/api.php?amount=1");
                    JObject rss = JObject.Parse(json);
                    question = ReplaceText((string)rss.SelectToken("results[0].question"));
                    correctAnswer = ReplaceText((string)rss.SelectToken("results[0].correct_answer"));
                    answers[3] = correctAnswer;
                    JArray wrongAnswers = (JArray)rss.SelectToken("results[0].incorrect_answers");
                    for (int i = 0; i < wrongAnswers.Count; i++)
                    {
                        wrongAnswers[i] = ReplaceText(wrongAnswers[i].ToString());
                        answers[i] = wrongAnswers[i].ToString();
                    }
                    for (int i = 0; i < answers.Length - 1; i++)
                    {
                        int random = rand.Next(i, answers.Length);
                        string temp = answers[i];
                        answers[i] = answers[random];
                        answers[random] = temp;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(MainMenu), "Open")]
        public class MainMenu_Patch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                question = "";
            }
        }
    }
}