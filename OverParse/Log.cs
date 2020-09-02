﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace OverParse
{
    public class Log
    {
        private long startTimestamp = 0;
        public long newTimestamp = 0;
        private string encounterData;
        private List<long> instances = new List<long>();
        public List<Combatant> combatants = new List<Combatant>();
        public List<Combatant> backupCombatants = new List<Combatant>();

        private const int pluginVersion = 5;
        public bool valid;
        public bool notEmpty;
        public bool running;
        public DirectoryInfo logDirectory;
        public string filename;
        private StreamReader logReader;


        public Log(string attemptDirectory)
        {
            valid = false;
            notEmpty = false;
            running = false;
            bool nagMe = false;

            if (Properties.Settings.Default.BanWarning)
            {
                MessageBoxResult panicResult = MessageBox.Show("OverParse is a 3rd party tool that does not lie within SEGA's ToS. \n\nSEGA has announced that they will ban players that have been publically found to be using parsers. \n\nTherefore using OverParse can risk your acount being banned if used irresponibly. \n\nDo you wish to continue through setup?", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (panicResult == MessageBoxResult.No)
                {
                    Environment.Exit(-1);
                }
                Properties.Settings.Default.BanWarning = false;
            }

            while (!File.Exists($"{attemptDirectory}\\pso2.exe"))
            {
                //Console.WriteLine("pso2_binディレクトリが無効です。");

                if (nagMe)
                {
                    MessageBox.Show("This is not a valid pso2_bin directory. \n\npso2.exe cannot be found.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Please select your pso2_bin directory.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    nagMe = true;
                }

                //WINAPI FILE DIALOGS DON'T SHOW UP FOR PEOPLE SOMETIMES AND I HAVE NO IDEA WHY, *** S I C K  M E M E ***
                //VistaFolderBrowserDialog oDialog = new VistaFolderBrowserDialog();
                //oDialog.Description = "Select your pso2_bin folder...";
                //oDialog.UseDescriptionForTitle = true;

                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "Please select your pso2_bin directory"
                };
                System.Windows.Forms.DialogResult picked = dialog.ShowDialog();
                if (picked == System.Windows.Forms.DialogResult.OK)
                {
                    attemptDirectory = dialog.SelectedPath;
                    //Console.WriteLine($"Testing {attemptDirectory} as pso2_bin directory...");
                    Properties.Settings.Default.Path = attemptDirectory;
                }
                else
                {
                    //Console.WriteLine("Canceled out of directory picker");
                    MessageBox.Show("Exiting Application", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    Environment.Exit(-1); // ABORT ABORT ABORT
                    break;
                }
            }

            if (!File.Exists($"{attemptDirectory}\\pso2.exe")) { return; }

            valid = true;

            //Console.WriteLine("Making sure pso2_bin\\damagelogs exists");
            logDirectory = new DirectoryInfo($"{attemptDirectory}\\damagelogs");

            if (Properties.Settings.Default.LaunchMethod == "Unknown")
            {
                MessageBoxResult tweakerResult = MessageBox.Show("Are you using PSO2 Tweaker?", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                Properties.Settings.Default.LaunchMethod = (tweakerResult == MessageBoxResult.Yes) ? "Tweaker" : "Manual";
            }

            if (Properties.Settings.Default.LaunchMethod == "Tweaker")
            {
                bool warn = true;
                if (logDirectory.Exists)
                {
                    if (logDirectory.GetFiles().Count() > 0)
                    {
                        warn = false;
                    }
                }

                if (warn && Hacks.DontAsk)
                {
                    MessageBox.Show("Your PSO2 folder does not contain damage logs, please turn on the PSO2DamageDump.dll plugin in the PSO2 Tweaker.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    Hacks.DontAsk = true;
                    Properties.Settings.Default.FirstRun = false;
                    Properties.Settings.Default.Save();
                    return;
                }
            }
            else if (Properties.Settings.Default.LaunchMethod == "Manual")
            {
                bool pluginsExist = File.Exists(attemptDirectory + "\\pso2h.dll") && File.Exists(attemptDirectory + "\\ddraw.dll") && File.Exists(attemptDirectory + "\\plugins" + "\\PSO2DamageDump.dll");
                if (!pluginsExist)
                    Properties.Settings.Default.InstalledPluginVersion = -1;

                //Console.WriteLine($"Installed: {Properties.Settings.Default.InstalledPluginVersion} / Current: {pluginVersion}");

                if (Properties.Settings.Default.InstalledPluginVersion < pluginVersion)
                {
                    MessageBoxResult selfdestructResult;

                    if (pluginsExist)
                    {
                        //Console.WriteLine("Prompting for plugin update");
                        selfdestructResult = MessageBox.Show("Do you want to update your PSO2DamageDump.dll now? ", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }
                    else
                    {
                        //Console.WriteLine("初期プラグインのinstall prompt");
                        selfdestructResult = MessageBox.Show("OverParse requires PSO2DamageDump.dll in order to recieve damage logs. It is recommended to turn it on in PSO2 Tweaker", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }

                    if (selfdestructResult == MessageBoxResult.No && !pluginsExist)
                    {
                        //Console.WriteLine("Denied plugin install");
                        MessageBox.Show("OverParse requires PSO2DamageDump.dll. Exiting Application", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                        Environment.Exit(-1);
                        return;
                    }
                    else if (selfdestructResult == MessageBoxResult.Yes)
                    {
                        //Console.WriteLine("Accepted plugin install");
                        bool success = UpdatePlugin(attemptDirectory);
                        if (!pluginsExist && !success)
                            Environment.Exit(-1);
                    }
                }
            }

            Properties.Settings.Default.FirstRun = false;

            if (!logDirectory.Exists)
                return;
            if (logDirectory.GetFiles().Count() == 0)
                return;

            notEmpty = true;

            FileInfo log = logDirectory.GetFiles().Where(f => Regex.IsMatch(f.Name, @"\d+\.")).OrderByDescending(f => f.Name).First();
            //Console.WriteLine($"Reading from {log.DirectoryName}\\{log.Name}");
            filename = log.Name;
            FileStream fileStream = File.Open(log.DirectoryName + "\\" + log.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(0, SeekOrigin.Begin);
            logReader = new StreamReader(fileStream);

                string existingLines = logReader.ReadToEnd(); // gotta get the dummy line for current player name
                string[] result = existingLines.Split('\n');
            foreach (string s in result)
            {
                if (s == "")
                    continue;
                string[] parts = s.Split(',');
                if (parts[0] == "0" && parts[3] == "YOU")
                {
                    Hacks.currentPlayerID = parts[2];
                    //Console.WriteLine("Found existing active player ID: " + parts[2]);
                }
            }
        }

        public bool UpdatePlugin(string attemptDirectory)
        {
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\pso2h.dll", attemptDirectory + "\\pso2h.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\ddraw.dll", attemptDirectory + "\\ddraw.dll", true);
                Directory.CreateDirectory(attemptDirectory + "\\plugins");
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\PSO2DamageDump.dll", attemptDirectory + "\\plugins" + "\\PSO2DamageDump.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\PSO2DamageDump.cfg", attemptDirectory + "\\plugins" + "\\PSO2DamageDump.cfg", true);
                Properties.Settings.Default.InstalledPluginVersion = pluginVersion;
                MessageBox.Show("Setup is complete. Please restart PSO2 if it is already running.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                //Console.WriteLine("Plugin install successful");
                return true;
            }
            catch
            {
                MessageBox.Show("An error has occured during setup. Please check that PSO2 is not running and that OverParse has read/write permissions.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                //Console.WriteLine($"PLUGIN INSTALL FAILED: {ex.ToString()}");
                return false;
            }
        }

        public void WriteClipboard()
        {
            string log = "";
            foreach (Combatant c in combatants)
            {
                if (c.IsAlly)
                {
                    string shortname = c.Name;
                    if (c.Name.Length > 6)
                    {
                        shortname = c.Name.Substring(0, 6);
                    }

                    log += $"{shortname} {(c.Damage).ToString("N0")} | ";
                }
            }

            if (log == "") { return; }
            log = log.Substring(0, log.Length - 2);

            try
            {
                Clipboard.SetText(log);
            }
            catch
            {
                //LMAO
            }
        }

        public string WriteLog()
        {

            if (combatants.Count != 0)
            {
                long elapsed = newTimestamp - startTimestamp;
                TimeSpan timespan = TimeSpan.FromSeconds(elapsed);
                long totalDamage = combatants.Where(c => c.IsAlly || c.IsZanverse || c.IsFinish || c.IsStatus).Sum(x => x.Damage);
                double totalDPS = combatants.Where(c => c.IsAlly || c.IsZanverse || c.IsFinish || c.IsStatus).Sum(x => x.DPS);
                string timer = timespan.ToString(@"mm\:ss");
                string log = DateTime.Now.ToString("F") + " | " + timer + " | " + "Total Damage Dealt: " + totalDamage.ToString("N0") + " | " + "Total DPS: " + totalDPS.ToString("N0") + Environment.NewLine + Environment.NewLine;

                foreach (Combatant c in combatants)
                {
                    if (c.IsAlly || c.IsZanverse || c.IsFinish || c.IsStatus)
                        log += $"{c.Name} | {c.PercentReadDPSReadout}% | {c.ReadDamage.ToString("N0")} Damage | {c.Damaged.ToString("N0")} Damage Taken | {c.DPS.ToString("N0")} DPS | JA : {c.WJAPercent}% | Critical : {c.WCRIPercent}% | Max : {c.MaxHit}" + Environment.NewLine;
                }

                log += Environment.NewLine + Environment.NewLine;

                foreach (Combatant c in combatants)
                {
                    if (c.IsAlly || c.IsZanverse || c.IsFinish || c.IsStatus)
                    {
                        string header = $"[ {c.Name} - {c.PercentReadDPSReadout}% - {c.ReadDamage.ToString("N0")} Damage ]";
                        log += header + Environment.NewLine + Environment.NewLine;

                        List<string> attackNames = new List<string>();
                        List<string> finishNames = new List<string>();
                        List<string> statusNames = new List<string>();
                        List<Tuple<string, List<long>, List<long>, List<long>>> attackData = new List<Tuple<string, List<long>, List<long>, List<long>>>();

                        if (c.IsZanverse && Properties.Settings.Default.SeparateZanverse)
                        {
                            foreach (Combatant c2 in backupCombatants)
                            {
                                if (c2.GetZanverseDamage > 0)
                                    attackNames.Add(c2.ID);
                            }

                            foreach (string s in attackNames)
                            {
                                Combatant targetCombatant = backupCombatants.First(x => x.ID == s);
                                List<long> matchingAttacks = targetCombatant.Attacks.Where(a => a.ID == "2106601422").Select(a => a.Damage).ToList();
                                List<long> jaPercents = targetCombatant.Attacks.Where(a => a.ID == "2106601422").Select(a => a.JA).ToList();
                                List<long> criPercents = targetCombatant.Attacks.Where(a => a.ID == "2106601422").Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<long>, List<long>, List<long>>(targetCombatant.Name, matchingAttacks, jaPercents, criPercents));
                            }
                        }

                        else if (c.IsFinish && Properties.Settings.Default.SeparateFinish)
                        {
                            foreach (Combatant c3 in backupCombatants)
                            {
                                if (c3.GetFinishDamage > 0)
                                    finishNames.Add(c3.ID);
                            }

                            foreach (string htf in finishNames)
                            {
                                Combatant tCombatant = backupCombatants.First(x => x.ID == htf);
                                List<long> fmatchingAttacks = tCombatant.Attacks.Where(a => Combatant.FinishAttackIDs.Contains(a.ID)).Select(a => a.Damage).ToList();
                                List<long> jaPercents = tCombatant.Attacks.Where(a => Combatant.FinishAttackIDs.Contains(a.ID)).Select(a => a.JA).ToList();
                                List<long> criPercents = tCombatant.Attacks.Where(a => Combatant.FinishAttackIDs.Contains(a.ID)).Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<long>, List<long>, List<long>>(tCombatant.Name, fmatchingAttacks, jaPercents, criPercents));
                            }

                        }

                        else if (c.IsStatus && Properties.Settings.Default.SeparateStatus)
                        {
                            foreach (Combatant c4 in backupCombatants)
                            {
                                if (c4.GetStatusDamage > 0)
                                    statusNames.Add(c4.ID);
                            }

                            foreach (string sta in statusNames)
                            {
                                Combatant tCombatant = backupCombatants.First(x => x.ID == sta);
                                List<long> fmatchingAttacks = tCombatant.Attacks.Where(a => Combatant.StatusEffectIDs.Contains(a.ID)).Select(a => a.Damage).ToList();
                                List<long> jaPercents = tCombatant.Attacks.Where(a => Combatant.StatusEffectIDs.Contains(a.ID)).Select(a => a.JA).ToList();
                                List<long> criPercents = tCombatant.Attacks.Where(a => Combatant.StatusEffectIDs.Contains(a.ID)).Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<long>, List<long>, List<long>>(tCombatant.Name, fmatchingAttacks, jaPercents, criPercents));
                            }
                        }

                        else
                        {
                            foreach (Attack a in c.Attacks)
                            {
                                if ((a.ID == "2106601422" && Properties.Settings.Default.SeparateZanverse) || (Combatant.FinishAttackIDs.Contains(a.ID) && Properties.Settings.Default.SeparateFinish) || (Combatant.StatusEffectIDs.Contains(a.ID) && Properties.Settings.Default.SeparateStatus))
                                    continue;
                                if (MainWindow.skillDict.ContainsKey(a.ID))
                                    a.ID = MainWindow.skillDict[a.ID]; // these are getting disposed anyway, no 1 cur
                                if (!attackNames.Contains(a.ID))
                                    attackNames.Add(a.ID);
                            }

                            foreach (string s in attackNames)
                            {
                                List<long> matchingAttacks = c.Attacks.Where(a => a.ID == s).Select(a => a.Damage).ToList();
                                List<long> jaPercents = c.Attacks.Where(a => a.ID == s).Select(a => a.JA).ToList();
                                List<long> criPercents = c.Attacks.Where(a => a.ID == s).Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<long>, List<long>, List<long>>(s, matchingAttacks, jaPercents, criPercents));
                            }
                        }

                        attackData = attackData.OrderByDescending(x => x.Item2.Sum()).ToList();

                        foreach (var i in attackData)
                        {
                            double percent = i.Item2.Sum() * 100d / c.ReadDamage;
                            string spacer = (percent >= 9) ? "" : " ";

                            string paddedPercent = percent.ToString("00.00");
                            string hits = i.Item2.Count().ToString("N0");
                            string sum = i.Item2.Sum().ToString("N0");
                            string min = i.Item2.Min().ToString("N0");
                            string max = i.Item2.Max().ToString("N0");
                            string avg = i.Item2.Average().ToString("N0");
                            string ja = (i.Item3.Average() * 100).ToString("N2") ?? "null";
                            string cri = (i.Item4.Average() * 100).ToString("N2") ?? "null";
                            log += $"{paddedPercent}%	| {i.Item1} ({sum}) Damage";
                            log += $" - JA : {ja}% - Critical : {cri}%" + Environment.NewLine;
                            log += $"      	|   {hits} hits - {min} min, {avg} avg, {max} max" + Environment.NewLine;
                        }

                        log += Environment.NewLine;
                    }
                }


                log += "Instance IDs: " + String.Join(", ", instances.ToArray());

                DateTime thisDate = DateTime.Now;
                string directory = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                Directory.CreateDirectory($"Logs/{directory}");
                string datetime = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                string filename = $"Logs/{directory}/OverParse - {datetime}.txt";
                File.WriteAllText(filename, log);

                return filename;
            }
            return null;
        }

        public string LogStatus()
        {
            if (!valid)
            {
                return "USER SHOULD PROBABLY NEVER SEE THIS";
            }

            if (!notEmpty)
            {
                return "Directory No logs: Enable plugin and check pso2_bin!";
            }

            if (!running)
            {
                return $"Waiting...";
            }

            return encounterData;
        }

        public void UpdateLog(object sender, EventArgs e)
        {
            if (!valid || !notEmpty)
            {
                return;
            }

            string newLines = logReader.ReadToEnd();
            if (newLines != "")
            {
                string[] result = newLines.Split('\n');
                foreach (string str in result)
                {
                    if (str != "")
                    {
                        string[] parts = str.Split(',');
                        long lineTimestamp = long.Parse(parts[0]);
                        long instanceID = long.Parse(parts[1]);
                        string sourceID = parts[2];
                        string sourceName = parts[3];
                        string targetID = parts[4];
                        string targetName = parts[5];
                        string attackID = parts[6];
                        long hitDamage = long.Parse(parts[7]);
                        long justAttack =long.Parse(parts[8]);
                        long critical = long.Parse(parts[9]);

                        int index = -1;

                        if (lineTimestamp == 0 && parts[3] == "YOU")
                        {
                            Hacks.currentPlayerID = parts[2];
                            continue;
                        }


                        if (sourceID != Hacks.currentPlayerID && Properties.Settings.Default.Onlyme)
                        {
                            continue;
                        }

                        if (!instances.Contains(instanceID))
                            instances.Add(instanceID);

                        if (hitDamage < 1)
                            continue;

                        if (sourceID == "0" || attackID == "0")
                            continue;

                        //Add different attacks if a new attack is found
                        if (10000000 < long.Parse(sourceID))
                        {
                            foreach (Combatant x in combatants)
                            {
                                if (x.ID == sourceID && x.isTemporary == "no")
                                {
                                    index = combatants.IndexOf(x);
                                }
                            }

                            if (index == -1)
                            {
                                combatants.Add(new Combatant(sourceID, sourceName));
                                index = combatants.Count - 1;
                            }

                            Combatant source = combatants[index];

                            newTimestamp = lineTimestamp;
                            if (startTimestamp == 0)
                            {
                                startTimestamp = newTimestamp;
                            }

                            source.Attacks.Add(new Attack(attackID, hitDamage, justAttack, critical));
                            running = true;
                        }
                        else //Add Damage Taken if combatant takes damage
                        {
                            foreach (Combatant x in combatants)
                            {
                                if (x.ID == targetID && x.isTemporary == "no")
                                {
                                    index = combatants.IndexOf(x);
                                }
                            }

                            if (index == -1)
                            {
                                combatants.Add(new Combatant(targetID, targetName));
                                index = combatants.Count - 1;
                            }

                            Combatant source = combatants[index];

                            newTimestamp = lineTimestamp;
                            if (startTimestamp == 0)
                            {
                                //Console.WriteLine($"FIRST ATTACK RECORDED: {hitDamage} dmg from {sourceID} ({sourceName}) with {attackID}, to {targetID} ({targetName})");
                                startTimestamp = newTimestamp;
                            }

                            source.Damaged += hitDamage;
                            running = true;
                        }
                    }
                }

                combatants.Sort((x, y) => y.ReadDamage.CompareTo(x.ReadDamage));

                if (startTimestamp != 0)
                {
                    encounterData = "0:00:00 - ∞ DPS";
                }

                if (startTimestamp != 0 && newTimestamp != startTimestamp)
                {
                    foreach (Combatant x in combatants)
                    {
                        if (x.IsAlly || x.IsZanverse)
                            x.ActiveTime = (newTimestamp - startTimestamp);
                    }
                }
            }
        }

    }
}
