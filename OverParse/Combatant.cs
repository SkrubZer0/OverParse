using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace OverParse
{
    public class Combatant
    {
        private const float maxBGopacity = 0.6f;
        public string ID;
        public string Name { get; set; }
        public static float maxShare = 0;
        public static string Log;
        public string isTemporary;
        public long ActiveTime;
        public float PercentDPS, PercentReadDPS;
        
        public List<Attack> Attacks;
        
        public static string[] FinishAttackIDs = new string[] {   "2268332858"  , // Hero Time Sword slashes
                                                                  "170999070"   , // Hero Time Sword finish
                                                                  "2268332813"  , // Hero Time Sword finish hard hit
                                                                  "1266101764"  , // Hero Time Talis pull-in
                                                                  "11556353"    , // Hero Time Talis slashes
                                                                  "1233721870"  , // Hero Time Talis slashes while switched to Sword
                                                                  "1233722348"  , // Hero Time Talis slashes while switched to TMG
                                                                  "3480338695"  , // Hero Time TMG burst
                                                                  "3695463316"  , // Phantom Time (Katana)
                                                                  "975125104"   , // Phantom Time (Katana) final
                                                                  "2243620231"  , // Phantom Time (Assault Rifle)
                                                                  "1412080400"  , // Phantom Time (Assault Rifle) final
                                                                  "2995855500"  , // Phantom Time (Rod)
                                                                  "2682739872"  , // Phantom Time (Rod) 2nd part
                                                                  "2682739887"  , // Phantom Time (Rod) 3rd part
                                                                  "2682739886"  , // Phantom Time (Rod) final lasers
                                                                  "156270665"   , // Katana Combat finish slash
                                                                  "4259442013"    // Katana Combat finish AOE
                                                                  }; 

        public static string[] PhotonAttackIDs = new string[] {   "2414748436"  , // Facility Cannon
                                                                  "1954812953"  , // Photon Cannon (Uncharged)
                                                                  "2822784832"  , // Photon Cannon (Charged)
                                                                  "3339644659"  , // Photon Particle Turret
                                                                  "2676260123"  , // Photon Laser Cannon
                                                                  "224805109"     // Photon Punisher
                                                                  }; 

        public static string[] AISAttackIDs = new string[] {      "119505187"   , // A.I.S rifle (Solid Vulcan)
                                                                  "79965782"    , // A.I.S melee first attack (Photon Saber)
                                                                  "79965783"    , // A.I.S melee second attack (Photon Saber)
                                                                  "79965784"    , // A.I.S melee third attack (Photon Saber)
                                                                  "80047171"    , // A.I.S dash melee (Photon Saber)
                                                                  "434705298"   , // A.I.S rockets (Photon Grenade)
                                                                  "79964675"    , // A.I.S gap closer PA attack (Photon Rush)
                                                                  "1460054769"  , // A.I.S cannon (Photon Blaster)
                                                                  "4081218683"  , // A.I.S mob freezing attack (Photon Blizzard)
                                                                  "3298256598"  , // A.I.S Weak Bullet
                                                                  "2826401717"  , // A.I.S Area Heal
                                                                  "1164312683"  , // A.I.S. Vega Solid Vulcan
                                                                  "858127000"   , // A.I.S. Vega Blade Rush first
                                                                  "503978418"   , // A.I.S. Vega Blade Rush second
                                                                  "3724116814"  , // A.I.S. Vega Blade Rush third
                                                                  "2655531208"  , // A.I.S. Vega Step Attack
                                                                  "383210274"   , // A.I.S. Vega Multi-Lock Missiles 
                                                                  "2640906853"  , // A.I.S. Vega Counter Shield
                                                                  "3968047485"  , // A.I.S. Vega Counter Shield big
                                                                  "1628689645"    // A.I.S. Vega Photon Blaster
                                                                  }; 

        public static string[] RideAttackIDs = new string[] {     "3491866260"  , // Rideroid throw
                                                                  "2056025809"  , // Rideroid hit forward slow
                                                                  "2534881408"  , // Rideroid hit forward stop
                                                                  "2600476838"  , // Rideroid hit dodge
                                                                  "1247666429"  , // Rideroid hit forward fast
                                                                  "3750571080"  , // Big UFO outer control unit hit?
                                                                  "3642240295"  , // Big UFO Core hit?
                                                                  "651750924"   , // Big UFO hit?
                                                                  "2452463220"  , // Something relating to big ufo and rideroid
                                                                  "1732461796"  , // Something relating to big ufo and rideroid
                                                                  "3809261131"  , // Something relating to big ufo and rideroid
                                                                  "1876785244"  , // Rideroid auto-attack (Mother phase 1)
                                                                  "3765765641"  , // Rideroid rockets (Mother phase 1)
                                                                  "3642969286"  , // Rideroid barrel roll (Mother phase 1)
                                                                  "1258041436"    // Rideroid Mother's wall spun back (Mother phase 1)
                                                                  }; 

        public static string[] DBAttackIDs = new string[] {       "267911699"   , // Dark Blast (Elder) first hit
                                                                  "262346668"   , // Dark Blast (Elder) second
                                                                  "265285249"   , // Dark Blast (Elder) third
                                                                  "264996390"   , // Dark Blast (Elder) fourth (kick)
                                                                  "311089933"   , // Dark Blast (Elder) fifth (launcher)
                                                                  "3988916155"  , // Dark Blast (Elder) sixth (pummel)
                                                                  "265781051"   , // Dark Blast (Elder) seventh (pummel pt2)
                                                                  "3141577094"  , // Dark Blast (Elder) Step Attack
                                                                  "2289473436"  , // Dark Blast (Elder) Violence Step
                                                                  "517914866"   , // Physical Dash melee
                                                                  "517914869"   , // Physical Dash melee wide range
                                                                  "1117313539"  , // Punishment Knuckle (uncharged)
                                                                  "1611279117"  , // Punishment Knuckle (charged)
                                                                  "3283361988"  , // Ultimate Impact
                                                                  "1117313602"  , // Infinity Rush hits (uncharged)
                                                                  "395090797"   , // Infinity Rush finish (uncharged)
                                                                  "2429416220"  , // Infinity Rush hits charged
                                                                  "1697271546"  , // Infinity Rush finish charged
                                                                  "1117313924"  , // Tyrant Strike
                                                                  "2743071591"  , // Dark Blast (Loser) hit
                                                                  "1783571383"  , // Ortho Sabarta (1-4?)
                                                                  "2928504078"  , // Ortho Sabarta (1-4?)
                                                                  "1783571188"  , // Convergent Ray uncharged
                                                                  "2849190450"  , // Convergent Ray charged
                                                                  "1223455602"  , // Gamma Burst
                                                                  "651603449"   , // Wisdom Force
                                                                  "2970658149"  , // Dive Assault
                                                                  "2191939386"  , // Counter Step
                                                                  "2091027507"  , // Special,Diffusion Ray
                                                                  "4078260742"  , // Sharp Glide
                                                                  "2743062721"  , // Attack Advance (Loser)
                                                                  "3379639420"  , // Dark Blast (Apprentice) first hit
                                                                  "3380458763"  , // Dark Blast (Apprentice) second hit
                                                                  "3380192966"  , // Dark Blast (Apprentice) third hit
                                                                  "3380628902"  , // Dark Blast (Apprentice) fourth hit
                                                                  "3377229307"  , // Dark Blast (Apprentice) fifth hit
                                                                  "3376960044"  , // Dark Blast (Apprentice) sixth hit
                                                                  "3377051585"  , // Dark Blast (Apprentice) seventh hit
                                                                  "3377849861"  , // Dark Blast (Apprentice) eighth hit
                                                                  "855002982"   , // Dark Blast (Apprentice) lots'o'slashes (ninth attack)
                                                                  "2326333456"  , // Dark Blast (Apprentice) ninth hit final
                                                                  "3725887474"  , // Dark Blast (Apprentice) Step Attack
                                                                  "361825851"   , // Graceful Dance projectile
                                                                  "3535795759"  , // Graceful Dance spin
                                                                  "781100939"   , // Graceful Dance (Royal Scorpion) projectile
                                                                  "793625150"   , // Royal Scorpion
                                                                  "1764406382"  , // Servant Hornet
                                                                  "3891439877"  , // Servant Hornet
                                                                  "2295506478"  , // Fortissimo Kick
                                                                  "1738105582"  , // Fortissimo Kick AOE
                                                                  "37504833"    , // Fortissimo Kick (Royal Scorpion)
                                                                  "1891210633"  , // Black Queen Arrival
                                                                  "3617357696"  , // Sensational Speed Attack
                                                                  "452272060"   , // Ravishing Step
                                                                  "2002943320"  , // Dark Blast (Double) first hit
                                                                  "2000047869"  , // Dark Blast (Double) second hit
                                                                  "2002496834"  , // Dark Blast (Double) third hit
                                                                  "1957174279"  , // Dark Blast (Double) fourth hit
                                                                  "1955884339"  , // Dark Blast (Double) fifth hit
                                                                  "4271466373"  , // Surprise Hammer
                                                                  "305729398"   , // Surprise Hammer second hit
                                                                  "682884756"   , // Waku Waku Go-Kart physical hit
                                                                  "4271465479"  , // Waku Waku Go-Kart physical hit "2-wheelie mode"
                                                                  "3983075073"  , // Waku Waku Go-Kart projectile
                                                                  "4271465542"  , // Happy Bazooka
                                                                  "3593316716"  , // Flower Carnival attacks
                                                                  "483639921"   , // Flower Carnival finish
                                                                  "472092093"     // Clutch Step             
                                                                  };

        public static string[] LaconiumAttackIDs = new string[] { "1913897098"  , // Rapid-Fire Mana Gun
                                                                  "2235773608"  , // Laconium Sword air second normal attack 
                                                                  "2235773610"  , // Laconium Sword air first normal attack 
                                                                  "2235773611"  , // Laconium Sword air third normal attack
                                                                  "2235773818"  , // Buster Divide (Laconium Sword uncharged)
                                                                  "2235773926"  , // Laconium Sword second normal attack
                                                                  "2235773927"  , // Laconium Sword first normal attack
                                                                  "2235773944"  , // Laconium Sword third normal attack
                                                                  "2618804663"  , // Buster Divide (Laconium Sword charged)
                                                                  "2619614461"  , // Laconium Sword Step Attack
                                                                  "3607718359"    // Laconium Sword slash
                                                                  }; 

        public long Damage => GetDamage();
        public long ReadDamage => GetMPADamage();
        public long DBDamage => Attacks.Where(a => DBAttackIDs.Contains(a.ID)).Sum(x => x.Damage);   
        public long LswDamage => Attacks.Where(a => LaconiumAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        public long PwpDamage => Attacks.Where(a => PhotonAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        public long AisDamage => Attacks.Where(a => AISAttackIDs.Contains(a.ID)).Sum(x => x.Damage);
        public long RideDamage => Attacks.Where(a => RideAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public long Damaged;
        public string ReadDamaged => Damaged.ToString("N0");

        public long GetZanverseDamage => Attacks.Where(a => a.ID == "2106601422").Sum(x => x.Damage);
        public long GetFinishDamage => Attacks.Where(a => FinishAttackIDs.Contains(a.ID)).Sum(x => x.Damage);

        public string JAPercent => GetJAPercent();
        public string CRIPercent => GetCRIPercent();
        public string WJAPercent => GetWJAPercent();
        public string WCRIPercent => GetWCRIPercent();

        public bool IsYou => (ID == Hacks.currentPlayerID);
        public bool IsAlly => (long.Parse(ID) >= 10000000) && !IsZanverse && !IsFinish;
        public bool IsAIS => (isTemporary == "AIS");
        public bool IsRide => (isTemporary == "Ride");
        public bool IsZanverse => (isTemporary == "Zanverse");
        public bool IsPwp => (isTemporary == "Pwp");
        public bool IsFinish => (isTemporary == "Finish Attacks");
        public bool IsDB => (isTemporary == "DB");
        public bool IsLsw => (isTemporary == "Lsw");

        public string MaxHit => GetMaxHit();
        public string MaxHitdmg => MaxHitAttack.Damage.ToString("N0");
        public string MaxHitID => MaxHitAttack.ID;
        public long MaxHitNum => MaxHitAttack.Damage;

        public string PercentReadDPSReadout => GetPercentReadDPSReadout();
        public string DamageReadout => ReadDamage.ToString("N0");

        public double DPS => GetDPS();
        public double ReadDPS => GetMPADPS();
        public string StringDPS => ReadDPS.ToString("N0");

        public string DisplayName
        {
            get
            {
                if (Properties.Settings.Default.AnonymizeNames && IsAlly)
                {
                    if (IsYou)
                    {
                        return Name;
                    }
                    else
                    {
                        return "----";
                    }
                }
                return Name;
            }
        }

        public string FDPSReadout
        {
            get
            {
                if (Properties.Settings.Default.DPSformat)
                {
                    return FormatNumber(ReadDPS);
                } else {
                    return StringDPS;
                }
            }
        }

        private long GetDamage()
        {
            return Attacks.Sum(x => x.Damage);
        }

        private string GetMaxHit()
        {
            if (MaxHitAttack == null)
                return "--";
            string attack = "Unknown";
            if (MainWindow.skillDict.ContainsKey(MaxHitID))
            {
                attack = MainWindow.skillDict[MaxHitID];
            }
            return MaxHitAttack.Damage.ToString("N0") + $" ({attack})";
        }

        private string GetJAPercent()
        {
            try
            {
                IEnumerable<Attack> JAs = Attacks.Where(a => !MainWindow.ignoreskill.Contains(a.ID));

                if (JAs.Any())
                {
                    Double JAAverage = JAs.Average(x => x.JA) * 100;

                    if (Properties.Settings.Default.Nodecimal)
                    {
                        return JAAverage.ToString("N0");
                    }
                    else
                    {
                        return JAAverage.ToString("N2");
                    }
                }
                else
                {
                    if (Properties.Settings.Default.Nodecimal)
                    {
                        return "0";
                    }
                    else
                    {
                        return "0.00";
                    }
                }
            }
            catch { return "Error"; }
        }

        private string GetWJAPercent()
        {
            IEnumerable<Attack> JAs = Attacks.Where(a => !MainWindow.ignoreskill.Contains(a.ID));

            if (JAs.Any())
            {
                Double JAAverage = JAs.Average(x => x.JA) * 100;
                return JAAverage.ToString("00.00");
            }
            else
            {
                return "0.00";
            }
        }

        private string GetCRIPercent()
        {
            try
            {
                IEnumerable<Attack> CRIs = Attacks;

                if (CRIs.Any())
                {
                    Double CRIAverage = CRIs.Average(x => x.Cri) * 100;

                    if (Properties.Settings.Default.Nodecimal)
                    {
                        return CRIAverage.ToString("N0");
                    }
                    else
                    {
                        return CRIAverage.ToString("N2");
                    }
                }
                else
                {
                    if (Properties.Settings.Default.Nodecimal)
                    {
                        return "0";
                    }
                    else
                    {
                        return "0.00";
                    }
                }
            }
            catch { return "Error"; }
        }

        private string GetWCRIPercent()
        {
            IEnumerable<Attack> CRIs = Attacks;

            if (CRIs.Any())
            {
                Double CRIAverage = CRIs.Average(x => x.Cri) * 100;

                return CRIAverage.ToString("00.00");
            }
            else
            {
                return "00.00";
            }
        }

        private double GetDPS()
        {
            if (ActiveTime == 0)
            {
                return Damage;
            }
            else
            {
                return Damage / ActiveTime;
            }
        }

        private double GetMPADPS()
        {
            if (ActiveTime == 0)
            {
                return ReadDamage;
            }
            else
            {
                return Math.Round(ReadDamage / (double)ActiveTime);
            }
        }

        private long GetMPADamage()
        {
            if (IsZanverse || IsFinish || IsAIS || IsPwp || IsDB || IsRide)
                return Damage;

            long temp = Damage;
            if (Properties.Settings.Default.SeparateZanverse)
                temp -= GetZanverseDamage;
            if (Properties.Settings.Default.SeparateFinish)
                temp -= GetFinishDamage;
            if (Properties.Settings.Default.SeparatePwp)
                temp -= PwpDamage;
            if (Properties.Settings.Default.SeparateAIS)
                temp -= AisDamage;
            if (Properties.Settings.Default.SeparateDB)
                temp -= DBDamage;
            if (Properties.Settings.Default.SeparateRide)
                temp -= RideDamage;
            return temp;
        }

        private String FormatNumber(double value)
        {
            long num = (long)Math.Round(value);

            if (value >= 100000000)
                return (value / 1000000).ToString("#,0") + "M";
            if (value >= 1000000)
                return (value / 1000000D).ToString("0.0") + "M";
            if (value >= 100000)
                return (value / 1000).ToString("#,0") + "K";
            if (value >= 1000)
                return (value / 1000D).ToString("0.0") + "K";
            return value.ToString("#,0");
        }

        public string AnonymousName()
        {
            if (IsYou)
                return Name;
            else
                return "----";
        }

        public Brush Brush
        {
            get
            {
                if (Properties.Settings.Default.ShowDamageGraph && (IsAlly))
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 128, 128), Color.FromArgb(128, 30, 30, 30));
                } else {
                    if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                        return new SolidColorBrush(Color.FromArgb(128, 0, 255, 255));
                    return new SolidColorBrush(Color.FromArgb(127, 30, 30, 30));
                }
            }
        }

        public Brush Brush2
        {
            get
            {
                if (Properties.Settings.Default.ShowDamageGraph && (IsAlly && !IsZanverse))
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 64, 64), Color.FromArgb(0, 0, 0, 0));
                } else {
                    if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                        return new SolidColorBrush(Color.FromArgb(128, 0, 64,64));
                    return new SolidColorBrush(new Color());
                }
            }
        }

        LinearGradientBrush GenerateBarBrush(Color c, Color c2)
        {
            if (!Properties.Settings.Default.ShowDamageGraph)
                c = new Color();

            if (IsYou && Properties.Settings.Default.HighlightYourDamage)
                c = Color.FromArgb(128, 0, 255, 255);

            LinearGradientBrush lgb = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(1, 0)
            };
            lgb.GradientStops.Add(new GradientStop(c, 0));
            lgb.GradientStops.Add(new GradientStop(c, ReadDamage / maxShare));
            lgb.GradientStops.Add(new GradientStop(c2, ReadDamage / maxShare));
            lgb.GradientStops.Add(new GradientStop(c2, 1));
            lgb.SpreadMethod = GradientSpreadMethod.Repeat;
            return lgb;
        }

        public Attack MaxHitAttack
        {
            get
            {
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                return Attacks.FirstOrDefault();
            }
        }

        private string GetPercentReadDPSReadout()
        {
            if (PercentReadDPS < -.5)
            {
                return "--";
            }
            else
            {
                return $"{PercentReadDPS:0.00}";
            }
        }

        //Constructors
        public Combatant(string id, string name)
        {
            ID = id;
            Name = name;
            PercentDPS = -1;
            Attacks = new List<Attack>();
            isTemporary = "no";
            PercentReadDPS = 0;
            ActiveTime = 0;
            Damaged = 0;
        }

        public Combatant(string id, string name, string temp)
        {
            ID = id;
            Name = name;
            PercentDPS = -1;
            Attacks = new List<Attack>();
            isTemporary = temp;
            PercentReadDPS = 0;
            ActiveTime = 0;
            Damaged = 0;
        }
    }

    static class Hacks
    {
        public static string currentPlayerID;
        public static bool DontAsk = false;
        public static string targetID = "";
    }

    public class Attack
    {
        public string ID;
        public long Damage;
        public long JA;
        public long Cri;

        public Attack(string initID, long initDamage, long justAttack, long critical)
        {
            ID = initID;
            Damage = initDamage;
            JA = justAttack;
            Cri = critical;
        }
    }


}
