using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Duranda
{
    /*
        Wrapper class because their console sucks
    */
    public class DurandaConsole
    {
        public delegate void ConsoleCommandCallback(List<string> args);
        public delegate void ConsoleConfigExecuted(string name, string path);
        public delegate void ConsoleCallbackInvoked(string command, string description, List<string> args);
        public event ConsoleCallbackInvoked OnCommandExecuted;
        public event ConsoleConfigExecuted OnConfigExecuted;

        private Console Console => Console.instance;
        private readonly Dictionary<string, ConsoleCommandCallback> Commands = new Dictionary<string, ConsoleCommandCallback>();
        private readonly Dictionary<string, string> Descriptions = new Dictionary<string, string>();
        private string TextInput;

        private readonly List<string> BuiltInCommands = new List<string> { "ping", "hidebetatext", "setkey", "resetenv", "fov", "genloc", "lodbias", "info", "kick", "ban", "unban", "banned", "help", "god", "ghost", "raiseskill", "resetskill", "resetcharacter", "heal", "puke", "hair", "beard", "model", "dpsdebug", "players", "freefly", "ffsmooth", "save", "exploremap", "resetmap", "pos", "goto", "location", "killall", "tame", "removedrops", "wind", "resetwind", "tod", "skiptime", "sleep", "event", "stopevent", "randomevent", "spawn", "debugmode", "imacheater" };

        public DurandaConsole()
        {
            Clear();
            SetFontColor(Color.cyan);
            SetFontSize(12);
            WriteLine("LOADED\nDevelopers: KillStr3aK\n\ntype \"commands\" - for commands");

            OnConfigExecuted += OnConfigExecutedCallback;
            OnCommandExecuted += OnCommandExecutedCallback;
        }

        public void LoadConfig(string path = Constans.AUTOEXEC_PATH)
        {
            if (!File.Exists(path))
            {
                WriteLine($"No configuration file found at path \"{path}\"");
                return;
            }

            FileStream fs = File.Open(path, FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs);

            try
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("//"))
                        continue;

                    List<string> @params = line.Split(' ').ToList();
                    string command = @params[0];
                    if(command == "await")
                    {
                        if (int.TryParse(@params[1], out int ms))
                        {
                            if (ms < 1)
                                continue;

                            System.Threading.Thread.Sleep(ms);
                        } else WriteLine("Invalid await input!");
                    } else
                    {
                        @params.RemoveAt(0);
                        ExecuteCommand(command, @params);
                    }
                }

                string name = GetConfigName(path);
                OnConfigExecuted?.Invoke(name, path);
            } catch (Exception ex)
            {
                Debug(ex);
            }

            sr.Close();
            fs.Close();
        }

        public void Update()
        {
            if (!Console.IsVisible())
                return;

            if (!string.IsNullOrEmpty(Console.instance.m_input.text) && Console.instance.m_input.text != TextInput)
            {
                TextInput = Console.instance.m_input.text;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (string.IsNullOrEmpty(TextInput))
                    return;

                string[] textInput = TextInput.Split(' ');
                if (BuiltInCommands.Contains(textInput[0]))
                    return;

                List<string> commandParams = new List<string>();
                for (int i = 1; i < textInput.Length; i++)
                {
                    commandParams.Add(textInput[i]);
                }

                ExecuteCommand(textInput[0], commandParams);
                TextInput = "";
            }
        }

        public void WriteLine(string msg)
        {
            List<string> m_chatBuffer = Reflection.GetPrivateFieldValue<List<string>, Console>(Console.instance, "m_chatBuffer");

            if(m_chatBuffer != null)
            {
                m_chatBuffer.Add("[DURANDA] " + msg);
                Reflection.CallPrivateVoidMethod<Console>(Console.instance, "UpdateChat");
            }
        }

        public void Debug(Exception ex)
        {
            WriteLine($"Error: {ex.Message} Stack Trace:{ex.StackTrace}");
        }

        public void SetFontColor(Color color)
        {
            Console.instance.m_output.color = color;
        }

        public Color GetFontColor()
        {
            return Console.instance.m_output.color;
        }

        public void SetFontSize(int size)
        {
            Console.instance.m_output.fontSize = size;
        }

        public int GetFontSize()
        {
            return Console.instance.m_output.fontSize;
        }

        public void RegisterCommand(string command, string description = "", ConsoleCommandCallback callback = null)
        {
            if (Commands == null)
                return;

            if (Commands.ContainsKey(command))
            {
                WriteLine($"Command {command} is already registered.");
                return;
            }

            Commands[command] = callback;
            Descriptions[command] = description;
        }

        public void ExecuteCommand(string command, List<string> args)
        {
            if (!Commands.ContainsKey(command))
            {
                WriteLine("Invalid command.");
                return;
            }

            Commands[command]?.Invoke(args);
            OnCommandExecuted?.Invoke(command, Descriptions[command], args);
        }

        public void ListCommands()
        {
            foreach (var i in Commands)
            {
                WriteLine($"{i.Key} => {Descriptions[i.Key]}");
            }
        }

        public bool IsCheatsEnabled()
        {
            return Console.instance.IsCheatsEnabled();
        }

        public void Open()
        {
            Console.instance.m_chatWindow.gameObject.SetActive(true);
        }

        public void Close()
        {
            Console.instance.m_chatWindow.gameObject.SetActive(false);
        }

        public void Clear()
        {
            List<string> m_chatBuffer = Reflection.GetPrivateFieldValue<List<string>, Console>(Console.instance, "m_chatBuffer");

            if (m_chatBuffer != null)
            {
                m_chatBuffer.Clear();
            }

            Console.instance.m_output.text = "";
        }

        private string GetConfigName(string path)
        {
            string[] configPath = path.Split('/');
            return configPath[configPath.Length - 1].Replace("/", "");
        }

        private void OnConfigExecutedCallback(string name, string path)
        {
            WriteLine($"Executed {name} configuration!");
        }

        private void OnCommandExecutedCallback(string command, string description, List<string> args)
        {
            string log = $"Executed command: {command}";
            if(args.Count > 0)
            {
                log += "\nParameters:";

                for(int i = 0; i < args.Count; i++)
                {
                    log +=  $"\n\t[{i}] {args[i]}";
                }
            }

            UnityEngine.Debug.Log(log);
        }
    }
}