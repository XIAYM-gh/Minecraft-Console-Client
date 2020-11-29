using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftClient.ChatBots
{
    /// <summary>
    /// In-Chat Hangman game
    /// </summary>
    
    public class HangmanGame : ChatBot
    {
        private int vie = 0;
        private int vie_param = 10;
        private int compteur = 0;
        private int compteur_param = 3000; //5 minutes
        private bool running = false;
        private bool[] discovered;
        private string word = "";
        private string letters = "";
        private bool English;

        /// <summary>
        /// Le jeu du Pendu / Hangman Game
        /// </summary>
        /// <param name="english">if true, the game will be in english. If false, the game will be in french.</param>

        public HangmanGame(bool english)
        {
            English = english;
            //XIAYM:管那么多干嘛，是个英文字符串就给你汉化了!
        }

        public override void Update()
        {
            if (running)
            {
                if (compteur > 0)
                {
                    compteur--;
                }
                else
                {
                    SendText(English ? "您输入这个单词花了太长的时间!" : "Temps imparti écoulé !");
                    SendText(English ? "游戏已终止!" : "Partie annulée.");
                    running = false;
                }
            }
        }

        public override void GetText(string text)
        {
            string message = "";
            string username = "";
            text = GetVerbatim(text);

            if (IsPrivateMessage(text, ref message, ref username))
            {
                if (Settings.Bots_Owners.Contains(username.ToLower()))
                {
                    switch (message)
                    {
                        case "start":
                            start();
                            break;
                        case "stop":
                            running = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                if (running && IsChatMessage(text, ref message, ref username))
                {
                    if (message.Length == 1)
                    {
                        char letter = message.ToUpper()[0];
                        if (letter >= 'A' && letter <= 'Z')
                        {
                            if (letters.Contains(letter))
                            {
                                SendText(English ? ("单词 " + letter + " 已被尝试.") : ("Le " + letter + " a déjà été proposé."));
                            }
                            else
                            {
                                letters += letter;
                                compteur = compteur_param;

                                if (word.Contains(letter))
                                {
                                    for (int i = 0; i < word.Length; i++) { if (word[i] == letter) { discovered[i] = true; } }
                                    SendText(English ? ("答对了!单词是 " + letter + '!') : ("Le " + letter + " figurait bien dans le mot :)"));
                                }
                                else
                                {
                                    vie--;
                                    if (vie == 0)
                                    {
                                        SendText(English ? "游戏结束! :]" : "Perdu ! Partie terminée :]");
                                        SendText(English ? ("正确的单词是: " + word) : ("Le mot était : " + word));
                                        running = false;
                                    }
                                    else SendText(English ? ("并不是 " + letter + ".") : ("Le " + letter + " ? Non."));
                                }

                                if (running)
                                {
                                    SendText(English ? ("单词提示: " + word_cached + " (剩余次数 : " + vie + ")")
                                    : ("Mot mystère : " + word_cached + " (vie : " + vie + ")"));
                                }

                                if (winner)
                                {
                                    SendText(English ? ("Congrats, " + username + '!') : ("Félicitations, " + username + " !"));
                                    running = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void start()
        {
            vie = vie_param;
            running = true;
            letters = "";
            word = chooseword();
            compteur = compteur_param;
            discovered = new bool[word.Length];

            SendText(English ? "猜单词游戏 v1.0 - By ORelio" : "Pendu v1.0 - Par ORelio");
            SendText(English ? ("单词提示: " + word_cached + " (剩余次数 : " + vie + ")")
            : ("Mot mystère : " + word_cached + " (vie : " + vie + ")"));
            SendText(English ? ("尝试一些单词.. :)") : ("Proposez une lettre ... :)"));
        }

        private string chooseword()
        {
            if (System.IO.File.Exists(English ? Settings.Hangman_FileWords_EN : Settings.Hangman_FileWords_FR))
            {
                string[] dico = System.IO.File.ReadAllLines(English ? Settings.Hangman_FileWords_EN : Settings.Hangman_FileWords_FR, Encoding.UTF8);
                return dico[new Random().Next(dico.Length)];
            }
            else
            {
                LogToConsole(English ? "文件未找到: " + Settings.Hangman_FileWords_EN : "Fichier introuvable : " + Settings.Hangman_FileWords_FR);
                return English ? "缺少词意" : "DICOMANQUANT";
            }
        }

        private string word_cached
        {
            get
            {
                string printed = "";
                for (int i = 0; i < word.Length; i++)
                {
                    if (discovered[i])
                    {
                        printed += word[i];
                    }
                    else printed += '_';
                }
                return printed;
            }
        }

        private bool winner
        {
            get
            {
                for (int i = 0; i < discovered.Length; i++)
                {
                    if (!discovered[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
