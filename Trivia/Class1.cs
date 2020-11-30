using Assets.Scripts.Simulation;
using Assets.Scripts.Unity.UI_New;
using Assets.Scripts.Unity.UI_New.InGame;
using Harmony;
using MelonLoader;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(Trivia.Class1), "Trivia", "1.0.0", "kenx00x")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace Trivia
{
    public class Class1 : MelonMod
    {
        
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
            return input;
        }
        [HarmonyPatch(typeof(Simulation), "OnRoundEnd")]
        public class SimulatornOnRoundEnd_Patch
        {
            // Token: 0x06000003 RID: 3 RVA: 0x0000206A File Offset: 0x0000026A
            [HarmonyPostfix]
            public static void Postfix()
            {
                System.Random rand = new System.Random();
                string[] answers = new string[4];
                string json = new WebClient().DownloadString("https://opentdb.com/api.php?amount=1");
                JObject rss = JObject.Parse(json);
                string question = ReplaceText((string)rss.SelectToken("results[0].question"));
                string correctAnswer = ReplaceText((string)rss.SelectToken("results[0].correct_answer"));
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
                GUI.Label(new Rect(100f, 160f, 100f, 50f), question);
                /*
                MelonLogger.Log("1");
                GameObject test = new GameObject();
                NK_TextMeshProUGUI questionText = test.AddComponent<NK_TextMeshProUGUI>();
                
                MelonLogger.Log("3");

                questionText = UnityEngine.Object.Instantiate(questionText);
                MelonLogger.Log("3");
                RectTransform questionTransform = test.GetComponent<RectTransform>();
                CanvasRenderer questionCanvas = test.GetComponent<CanvasRenderer>();
                MelonLogger.Log("5");
                SceneManager.MoveGameObjectToScene(test,SceneManager.GetSceneByName("InGameUi"));
                MelonLogger.Log("7");
                //questionCanvas.absoluteDepth = 3;
                questionTransform.position = new Vector3(960, 540);
                MelonLogger.Log("8");
                //questionText.m_text = question;
                MelonLogger.Log("9");
                MelonLogger.Log("10");
                */
            }
        }
    }
}