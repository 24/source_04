using System;

namespace pb
{
    [Flags]
    public enum SpecialDay
    {
        NoDay                 = 0x00000000,
        Monday                = 0x00000001,
        Tuesday               = 0x00000002,
        Wednesday             = 0x00000004,
        Thursday              = 0x00000008,
        Friday                = 0x00000010,
        Saturday              = 0x00000020,
        Sunday                = 0x00000040,
        NewYearDay            = 0x00000080,   // 1er janvier
        MayFirst              = 0x00000100,   // 1er mai
        VictoryInEuropeDay    = 0x00000200,   // 8 mai
        MardiGras             = 0x00000400,   // mardi gras
        PalmSunday            = 0x00000800,   // dimanche des Rameaux
        EasterSunday          = 0x00001000,   // dimanche de Pâques
        EasterMonday          = 0x00002000,   // lundi de Pâques
        AscensionThursday     = 0x00004000,   // jeudi de l'Ascension
        PentecostSunday       = 0x00008000,   // dimanche de Pentecôte
        PentecostMonday       = 0x00010000,   // lundi de Pentecôte
        FrenchRevolutionDay   = 0x00020000,   // 14 juillet
        AssumptionDay         = 0x00040000,   // 15 aout Assomption
        AllSaintsDay          = 0x00080000,   // 1 novembre Toussaint
        ArmisticeDay          = 0x00100000,   // 11 novembre Armistice
        ChristmasDay          = 0x00200000    // 25 décembre jour de Noël
    }

    public class SpecialDayTools
    {
        public static SpecialDay GetSpecialDays(string[] text)
        {
            SpecialDay sd = 0;
            foreach (string t in text)
                sd |= GetSpecialDay(t);
            return sd;
        }

        public static SpecialDay GetSpecialDay(string text)
        {
            switch (text.ToLower())
            {
                case "monday":
                    return SpecialDay.Monday;
                case "tuesday":
                    return SpecialDay.Tuesday;
                case "wednesday":
                    return SpecialDay.Wednesday;
                case "thursday":
                    return SpecialDay.Thursday;
                case "friday":
                    return SpecialDay.Friday;
                case "saturday":
                    return SpecialDay.Saturday;
                case "sunday":
                    return SpecialDay.Sunday;
                case "mayfirst":
                    return SpecialDay.MayFirst;
                case "newyearday":
                    return SpecialDay.NewYearDay;
                case "victoryineuropeday":
                    return SpecialDay.VictoryInEuropeDay;
                case "mardigras":
                    return SpecialDay.MardiGras;
                case "palmsunday":
                    return SpecialDay.PalmSunday;
                case "eastersunday":
                    return SpecialDay.EasterSunday;
                case "eastermonday":
                    return SpecialDay.EasterMonday;
                case "ascensionthursday":
                    return SpecialDay.AscensionThursday;
                case "pentecostsunday":
                    return SpecialDay.PentecostSunday;
                case "pentecostmonday":
                    return SpecialDay.PentecostMonday;
                case "frenchrevolutionday":
                    return SpecialDay.FrenchRevolutionDay;
                case "assumptionday":
                    return SpecialDay.AssumptionDay;
                case "allsaintsday":
                    return SpecialDay.AllSaintsDay;
                case "armisticeday":
                    return SpecialDay.ArmisticeDay;
                case "christmasday":
                    return SpecialDay.ChristmasDay;
                default:
                    //throw new PBException("error unknow special day \"{0}\"", text);
                    return SpecialDay.NoDay;
            }
        }

        public static bool IsSpecialDay(SpecialDay specialDay, Date date)
        {
            if ((specialDay & SpecialDay.Monday) == SpecialDay.Monday && date.DayOfWeek == DayOfWeek.Monday)
                return true;
            if ((specialDay & SpecialDay.Tuesday) == SpecialDay.Tuesday && date.DayOfWeek == DayOfWeek.Tuesday)
                return true;
            if ((specialDay & SpecialDay.Wednesday) == SpecialDay.Wednesday && date.DayOfWeek == DayOfWeek.Wednesday)
                return true;
            if ((specialDay & SpecialDay.Thursday) == SpecialDay.Thursday && date.DayOfWeek == DayOfWeek.Thursday)
                return true;
            if ((specialDay & SpecialDay.Friday) == SpecialDay.Friday && date.DayOfWeek == DayOfWeek.Friday)
                return true;
            if ((specialDay & SpecialDay.Saturday) == SpecialDay.Saturday && date.DayOfWeek == DayOfWeek.Saturday)
                return true;
            if ((specialDay & SpecialDay.Sunday) == SpecialDay.Sunday && date.DayOfWeek == DayOfWeek.Sunday)
                return true;
            if ((specialDay & SpecialDay.NewYearDay) == SpecialDay.NewYearDay && date == GetNewYearDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.MayFirst) == SpecialDay.MayFirst && date == GetMayFirstDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.VictoryInEuropeDay) == SpecialDay.VictoryInEuropeDay && date == GetVictoryInEuropeDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.MardiGras) == SpecialDay.MardiGras && date == GetMardiGrasDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.PalmSunday) == SpecialDay.PalmSunday && date == GetPalmSundayDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.EasterSunday) == SpecialDay.EasterSunday && date == GetEasterSundayDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.EasterMonday) == SpecialDay.EasterMonday && date == GetEasterMondayDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.AscensionThursday) == SpecialDay.AscensionThursday && date == GetAscensionThursdayDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.PentecostSunday) == SpecialDay.PentecostSunday && date == GetPentecostSundayDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.PentecostMonday) == SpecialDay.PentecostMonday && date == GetPentecostMondayDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.FrenchRevolutionDay) == SpecialDay.FrenchRevolutionDay && date == GetFrenchRevolutionDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.AssumptionDay) == SpecialDay.AssumptionDay && date == GetAssumptionDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.AllSaintsDay) == SpecialDay.AllSaintsDay && date == GetAllSaintsDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.ArmisticeDay) == SpecialDay.ArmisticeDay && date == GetArmisticeDate(date.Year))
                return true;
            if ((specialDay & SpecialDay.ChristmasDay) == SpecialDay.ChristmasDay && date == GetChristmasDate(date.Year))
                return true;
            return false;
        }

        public static Date GetNewYearDate(int year)
        {
            return new Date(year, 1, 1);
        }

        public static Date GetMayFirstDate(int year)
        {
            return new Date(year, 5, 1);
        }

        public static Date GetVictoryInEuropeDate(int year)
        {
            return new Date(year, 5, 8);
        }

        public static Date GetMardiGrasDate(int year)
        {
            // Mardi gras http://fr.wikipedia.org/wiki/Mardi_gras
            // 47 jours avant Pâques
            return GetEasterSundayDate(year).AddDays(-47);
        }

        public static Date GetPalmSundayDate(int year)
        {
            // dimanche des Rameaux http://fr.wikipedia.org/wiki/Dimanche_des_Rameaux
            // Le dimanche des Rameaux est le dimanche qui précède Pâques
            return GetEasterSundayDate(year).AddDays(-7);
        }

        public static Date GetEasterSundayDate(int year)
        {
            // Dimanche de Pâques (Easter Sunday)
            // from JOURS FÉRIÉS ET DIMANCHE http://www.csharpfr.com/codes/JOURS-FERIES-DIMANCHE_26760.aspx
            // Calcul de la date des Pâques http://fr.wikipedia.org/wiki/Calcul_de_la_date_des_P%C3%A2ques#Calendrier_lunaire_perp.C3.A9tuel_julien
            // Le calcul des dates de paques lesdatesdepaques.pdf http://www.imcce.fr/~procher/Presentations/lesdatesdepaques.pdf
            // dates des fêtes de Pâques http://www.lexilogos.com/fetes_date.htm


            // Calcul du jour de pâques (algorithme de Oudin (1940))
            // Nombre d'or (astronomie) http://fr.wikipedia.org/wiki/Nombre_d%27or_(astronomie)
            // on appelle Nombre d’or, le rang d’une année dans le cycle de Méton qui comporte 19 années
            // et permet de faire coïncider, à quelques heures près, cycles lunaires et cycles solaires
            // Nombre d'or = (année modulo 19) + 1
            // Calcul du nombre d'or - 1
            int goldNumber = year % 19;
            // Année divisé par cent
            int yearDiv100 = year / 100;
            // intEpacte est = 23 - Epacte (modulo 30)
            int epacte = (int)((yearDiv100 - yearDiv100 / 4 - (8 * yearDiv100 + 13) / 25 + (19 * goldNumber) + 15) % 30);
            //Le nombre de jours à partir du 21 mars pour atteindre la pleine lune Pascale
            int daysEquinoxeToMoonFull = (int)(epacte - (epacte / 28) * (1 - (epacte / 28) * (29 / (epacte + 1)) * ((21 - goldNumber) / 11)));
            //Jour de la semaine pour la pleine lune Pascale (0=dimanche)
            int weekDayMoonFull = (int)((year + year / 4 + daysEquinoxeToMoonFull + 2 - yearDiv100 + yearDiv100 / 4) % 7);
            // Nombre de jours du 21 mars jusqu'au dimanche de ou 
            // avant la pleine lune Pascale (un nombre entre -6 et 28)
            int daysEquinoxeBeforeFullMoon = daysEquinoxeToMoonFull - weekDayMoonFull;
            // mois de pâques
            int monthPaques = (int)(3 + (daysEquinoxeBeforeFullMoon + 40) / 44);
            // jour de pâques
            int dayPaques = (int)(daysEquinoxeBeforeFullMoon + 28 - 31 * (monthPaques / 4));
            return new Date(year, monthPaques, dayPaques);
        }

        public static Date GetEasterMondayDate(int year)
        {
            // Lundi de Pâques (Easter Monday)
            return GetEasterSundayDate(year).AddDays(1);
        }

        public static Date GetAscensionThursdayDate(int year)
        {
            // Ascension (fête) http://fr.wikipedia.org/wiki/Ascension_(f%C3%AAte)
            // L’Ascension est une fête chrétienne célébrée quarante jours après Pâques (en comptant le dimanche de Pâques).
            return GetEasterSundayDate(year).AddDays(39);
        }

        public static Date GetPentecostSundayDate(int year)
        {
            // Pentecôte http://fr.wikipedia.org/wiki/Dimanche_de_Pentec%C3%B4te
            // La Pentecôte est célébrée le septième dimanche, soit quarante-neuf jours après le dimanche de Pâques
            return GetEasterSundayDate(year).AddDays(49);
        }

        public static Date GetPentecostMondayDate(int year)
        {
            // Pentecôte http://fr.wikipedia.org/wiki/Dimanche_de_Pentec%C3%B4te
            // La Pentecôte est célébrée le septième dimanche, soit quarante-neuf jours après le dimanche de Pâques
            return GetEasterSundayDate(year).AddDays(50);
        }

        public static Date GetFrenchRevolutionDate(int year)
        {
            return new Date(year, 7, 14);
        }

        public static Date GetAssumptionDate(int year)
        {
            return new Date(year, 8, 15);
        }

        public static Date GetAllSaintsDate(int year)
        {
            return new Date(year, 11, 1);
        }

        public static Date GetArmisticeDate(int year)
        {
            return new Date(year, 11, 11);
        }

        public static Date GetChristmasDate(int year)
        {
            return new Date(year, 12, 25);
        }
    }
}
