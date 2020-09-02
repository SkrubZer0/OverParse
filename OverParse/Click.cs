﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace OverParse
{
    public partial class MainWindow : Window
    {


        private void EndEncounter_Click(object sender, RoutedEventArgs e)
        {
            //Ending encounter
            bool temp = Properties.Settings.Default.AutoEndEncounters;
            Properties.Settings.Default.AutoEndEncounters = false;
            UpdateForm(null, null); // I'M FUCKING STUPID
            Properties.Settings.Default.AutoEndEncounters = temp;
            encounterlog.backupCombatants = encounterlog.combatants;


            List<Combatant> workingListCopy = new List<Combatant>();
            foreach (Combatant c in workingList)
            {
                Combatant temp2 = new Combatant(c.ID, c.Name, c.isTemporary);
                foreach (Attack a in c.Attacks)
                    temp2.Attacks.Add(new Attack(a.ID, a.Damage, a.JA, a.Cri));
                temp2.Damaged = c.Damaged;
                temp2.ActiveTime = c.ActiveTime;
                temp2.PercentReadDPS = c.PercentReadDPS;
                workingListCopy.Add(temp2);
            }
            //Saving last combatant list"
            lastCombatants = encounterlog.combatants;
            encounterlog.combatants = workingListCopy;
            string filename = encounterlog.WriteLog();
            if (filename != null)
            {
                if ((SessionLogs.Items[0] as MenuItem).Name == "SessionLogPlaceholder")
                    SessionLogs.Items.Clear();
                int items = SessionLogs.Items.Count;

                string prettyName = filename.Split('/').LastOrDefault();

                sessionLogFilenames.Add(filename);

                var menuItem = new MenuItem() { Name = "SessionLog_" + items.ToString(), Header = prettyName };
                menuItem.Click += OpenRecentLog_Click;
                SessionLogs.Items.Add(menuItem);
            }
            if (Properties.Settings.Default.LogToClipboard)
            {
                encounterlog.WriteClipboard();
            }

            encounterlog = new Log(Properties.Settings.Default.Path);
            UpdateForm(null, null);
        }


        private void EndEncounterNoLog_Click(object sender, RoutedEventArgs e)
        {
            //Ending encounter (no log)
            bool temp = Properties.Settings.Default.AutoEndEncounters;
            Properties.Settings.Default.AutoEndEncounters = false;
            UpdateForm(null, null);
            Properties.Settings.Default.AutoEndEncounters = temp;
            //Reinitializing log
            encounterlog = new Log(Properties.Settings.Default.Path);
            UpdateForm(null, null);
        }

        private void AutoEndEncounters_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoEndEncounters = AutoEndEncounters.IsChecked;
            SetEncounterTimeout.IsEnabled = AutoEndEncounters.IsChecked;
        }

        private void SetEncounterTimeout_Click(object sender, RoutedEventArgs e)
        {
            AlwaysOnTop.IsChecked = false;
            Inputbox input = new Inputbox("Encounter Timeout", "Please input the amount of seconds you would like for the Encounter Timeout", Properties.Settings.Default.EncounterTimeout.ToString()) { Owner = this };
            input.ShowDialog();
            if (Int32.TryParse(input.ResultText, out int x))
            {
                if (x > 0) { Properties.Settings.Default.EncounterTimeout = x; }
                else { MessageBox.Show("Error"); }
            }
            else
            {
                if (input.ResultText.Length > 0) { MessageBox.Show("Could not read your input. Please use numbers only."); }
            }

            AlwaysOnTop.IsChecked = Properties.Settings.Default.AlwaysOnTop;
        }

        private void LogToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LogToClipboard = LogToClipboard.IsChecked;
        }

        private void OpenLogsFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Directory.GetCurrentDirectory() + "\\Logs");
        }

        private void OpenRecentLog_Click(object sender, RoutedEventArgs e)
        {
            string filename = sessionLogFilenames[SessionLogs.Items.IndexOf((e.OriginalSource as MenuItem))];
            //attempting to open
            Process.Start(Directory.GetCurrentDirectory() + "\\" + filename);
        }

        private void FilterPlayers_Click(object sender, RoutedEventArgs e)
        {
            UpdateForm(null, null);
        }

        private void SeparateZanverse_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateZanverse = SeparateZanverse.IsChecked;
            UpdateForm(null, null);
        }

        private void SeparateFinish_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateFinish = SeparateFinish.IsChecked;
            UpdateForm(null, null);
        }

        private void SeparateStatus_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateStatus = SeparateStatus.IsChecked;
            UpdateForm(null, null);
        }

        private void SeparateAIS_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateAIS = SeparateAIS.IsChecked;
            HideAIS.IsEnabled = SeparateAIS.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparateDB_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateDB = SeparateDB.IsChecked;
            HideDB.IsEnabled = SeparateDB.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparateRide_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateRide = SeparateRide.IsChecked;
            HideRide.IsEnabled = SeparateRide.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparatePwp_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparatePwp = SeparatePwp.IsChecked;
            HidePwp.IsEnabled = SeparatePwp.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparateLsw_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateLsw = SeparateLsw.IsChecked;
            HideLsw.IsEnabled = SeparateLsw.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void HidePlayers_Click(object sender, RoutedEventArgs e)
        {
            if (HidePlayers.IsChecked)
            {
                HideAIS.IsChecked = false;
                HideDB.IsChecked = false;
                HideRide.IsChecked = false;
                HidePwp.IsChecked = false;
            }
            UpdateForm(null, null);
        }

        private void HideAIS_Click(object sender, RoutedEventArgs e)
        {
            if (HideAIS.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HideDB_Click(object sender, RoutedEventArgs e)
        {
            if (HideDB.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HideRide_Click(object sender, RoutedEventArgs e)
        {
            if (HideRide.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HidePwp_Click(object sender, RoutedEventArgs e)
        {
            if (HidePwp.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HideLsw_Click(object sender, RoutedEventArgs e)
        {
            if (HideLsw.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void AnonymizeNames_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AnonymizeNames = AnonymizeNames.IsChecked;
            UpdateForm(null, null);
        }

        private void Onlyme_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Onlyme = Onlyme.IsChecked;
            UpdateForm(null, null);
        }

        private void DPSFormat_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DPSformat = DPSFormat.IsChecked;
            UpdateForm(null, null);
        }

        private void Nodecimal_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Nodecimal = Nodecimal.IsChecked;
            UpdateForm(null, null);
        }

        private void DefaultWindowSize_Click(object sender, RoutedEventArgs e)
        {
            Height = 275;
            Width = 670;
        }

        private void ChangeFont_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new emanual.Wpf.Dialogs.FontDialogEx() { Owner = this };

            var font = new emanual.Wpf.Dialogs.ToolFont
            {
                FontFamily = CombatantData.FontFamily,
                FontSize = CombatantData.FontSize,
                FontWeight = CombatantData.FontWeight,
                FontStyle = CombatantData.FontStyle,
                FontStretch = CombatantData.FontStretch
            };

            // 言語情報を保存するため、textBox1 の Tag プロパティに設定した
            if (CombatantData.Tag == null)
                font.FontLanguage = "ja-jp";
            else
                font.FontLanguage = (string)CombatantData.Tag;

            dlg.Font = font;
            dlg.SampleText = "Sample サンプル\nPSO2\nOverParse";

            if (dlg.ShowDialog().Value)
            {
                dlg.SetPropertyToTargetObject(CombatantData);
                CombatantData.Tag = dlg.Font.FontLanguage;
            }
        }

        private void DamageTaken_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DTcfg = DTcfg.IsChecked;
            if (DTcfg.IsChecked)
            {
                DmgDHC.Width = new GridLength(0);

            }
            else
            {
                DmgDHC.Width = new GridLength(0.9, GridUnitType.Star);
            }
            UpdateForm(null, null);
        }

        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Pctcfg = Pctcfg.IsChecked;
            if (Pctcfg.IsChecked)
            {
                PercentHC.Width = new GridLength(0);

            }
            else
            {
                PercentHC.Width = new GridLength(52);
            }
            UpdateForm(null, null);
        }

        private void JA_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.JAcfg = JAcfg.IsChecked;
            if (JAcfg.IsChecked)
            {
                JAHC.Width = new GridLength(0);

            }
            else
            {
                JAHC.Width = new GridLength(52);
            }
            UpdateForm(null, null);
        }

        private void Critical_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Criticalcfg = Cricfg.IsChecked;
            if (Cricfg.IsChecked)
            {
                CriHC.Width = new GridLength(0);

            }
            else
            {
                CriHC.Width = new GridLength(52);
            }
            UpdateForm(null, null);
        }


        private void CompactMode_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CompactMode = CompactMode.IsChecked;
            if (CompactMode.IsChecked)
            {
                AtkHC.Width = new GridLength(0);
            }
            else
            {
                AtkHC.Width = new GridLength(1.7, GridUnitType.Star);
            }
            UpdateForm(null, null);
        }

        private void VariableColumn_Click(object sender, RoutedEventArgs e)
        {
            PercentHC.Width = new GridLength(0.4, GridUnitType.Star);
            DmgHC.Width = new GridLength(0.8, GridUnitType.Star);
            DPSHC.Width = new GridLength(0.6, GridUnitType.Star);
            JAHC.Width = new GridLength(0.4, GridUnitType.Star);
            CriHC.Width = new GridLength(0.4, GridUnitType.Star);
        }

        private void ShowDamageGraph_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowDamageGraph = ShowDamageGraph.IsChecked;
            UpdateForm(null, null);
        }

        private void HighlightYourDamage_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.HighlightYourDamage = HighlightYourDamage.IsChecked;
            UpdateForm(null, null);
        }

        private void WindowOpacity_0_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = 0;
            HandleWindowOpacity();
        }

        private void WindowOpacity_25_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = .25;
            HandleWindowOpacity();
        }

        private void WindowOpacity_50_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = .50;
            HandleWindowOpacity();
        }

        private void WindowOpacity_75_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = .75;
            HandleWindowOpacity();
        }

        private void WindowOpacity_100_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = 1;
            HandleWindowOpacity();
        }

        private void ListOpacity_0_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = 0;
            HandleListOpacity();
        }

        private void ListOpacity_25_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = .25;
            HandleListOpacity();
        }

        private void ListOpacity_50_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = .50;
            HandleListOpacity();
        }

        private void ListOpacity_75_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = .75;
            HandleListOpacity();
        }

        private void ListOpacity_100_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = 1;
            HandleListOpacity();
        }

        private void AlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AlwaysOnTop = AlwaysOnTop.IsChecked;
            OnActivated(e);
        }

        private void AutoHideWindow_Click(object sender, RoutedEventArgs e)
        {
            if (AutoHideWindow.IsChecked && Properties.Settings.Default.AutoHideWindowWarning)
            {
                MessageBox.Show("When PSO2 or OverParse is not in the foreground, OverParse will be hidden. To display OverParse, use Alt + Tab to select OverParse, or click the taskbar icon.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                Properties.Settings.Default.AutoHideWindowWarning = false;
            }
            Properties.Settings.Default.AutoHideWindow = AutoHideWindow.IsChecked;
        }

        private void ClickthroughToggle(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ClickthroughEnabled = ClickthroughMode.IsChecked;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show($"OverParse v3\nSelf Monitoring tool. \n\nShoutouts to Variant, AIDA, and everyone else who makes the Tweaker plugin possible.\n\nPlease use damage information responsibly.", "OverParse");
        }

        private void LowResources_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LowResources = LowResources.IsChecked;
            if (Properties.Settings.Default.LowResources)
            {
                thisProcess.PriorityClass = ProcessPriorityClass.Idle;
                MessageBox.Show("Process priority of OverParse is now set to low. \n\nThis setting is only recommended if your CPU is a potato. Please remember that OverParse may become unresponsive due to this setting.", "OverParse");
            } else {
                thisProcess.PriorityClass = ProcessPriorityClass.Normal;
            }
        }

        private void CPUdraw_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CPUdraw = CPUdraw.IsChecked;
            if (Properties.Settings.Default.CPUdraw)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                MessageBox.Show("OverParse is now using CPU rendering. \n\nThis setting is only recommended if you do not have a discrete GPU. \n Please remember that this setting may increase CPU load.", "OverParse");
            } else {
                RenderOptions.ProcessRenderMode = RenderMode.Default;
            }
        }

        private void Clock_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Clock = Clock.IsChecked;
            if (Properties.Settings.Default.Clock) { Datetime.Visibility = Visibility.Visible; }
            else { Datetime.Visibility = Visibility.Collapsed; }
        }

        private void Github_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/SkrubZer0/OverParse");

        private void SkipPlugin_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.InstalledPluginVersion = 5;

        private void ResetLogFolder_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Path = "A://BROKEN/FILE/PATH";
            EndEncounterNoLog_Click(this, null);
        }

        private void UpdatePlugin_Click(object sender, RoutedEventArgs e)
        {
            encounterlog.UpdatePlugin(Properties.Settings.Default.Path);
            EndEncounterNoLog_Click(this, null);
        }

        private void ResetOverParse(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to Reset OverParse? \n\nYour user settings will be deleted but your logs will remain in your log folder.", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result != MessageBoxResult.Yes)
                return;

            //Resetting
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.ResetInvoked = true;
            Properties.Settings.Default.Save();

            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void Updateskills_Click(object sender, RoutedEventArgs e)
        {
            string[] tmp;
            //skills.csv
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    Stream stream = client.OpenRead("https://raw.githubusercontent.com/VariantXYZ/PSO2ACT/master/PSO2ACT/skills.csv");
                    StreamReader webreader = new StreamReader(stream);
                    String content = webreader.ReadToEnd();

                    tmp = content.Split('\n');
                    File.WriteAllText("skills.csv", content);                    
                }

                skillDict.Clear();

                foreach (string s in tmp)
                {
                    string[] split = s.Split(',');
                    if (split.Length > 1)
                    {
                        skillDict.Add(split[1], split[0]);
                    }
                }

                MessageBox.Show("skills.csv has been successfully updated");
            }
            catch
            {
                MessageBox.Show("Failed to update skills.csv");
            }

            

            //ignoreskills.csv
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.DownloadFile("https://raw.githubusercontent.com/SkrubZer0/OverParse/master/OverParse/Other_Files/ignoreskills.csv", "ignoreskills.csv");
                }

                ignoreskill = File.ReadAllLines("ignoreskills.csv");

                MessageBox.Show("ignoreskills.csv has been successfully updated");
            }
            catch
            {
                MessageBox.Show("Failed to update ignoreskills.csv", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
