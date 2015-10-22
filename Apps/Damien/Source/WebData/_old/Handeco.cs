using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using pb.Data.Xml;
using PB_Util;

// liste de valeurs dans Adhésion à des groupements et fédérations professionnels  http://www.handeco.org/fournisseur/consulter/id/967
// tel sur 2 lignes :
//   APAJH Association départementale 11 - EA       http://www.handeco.org/fournisseur/consulter/id/5279
//   APAJH Association départementale 11 - ESAT     http://www.handeco.org/fournisseur/consulter/id/5280
// supprimer doublon :
//   company.detail.adhésionGroupement  company.header.groupes
//   company.detail.raisonSociale       company.header.name
//   company.header.activités
// logo :
//   ESAT DE LA FAYE http://www.handeco.org/fournisseur/consulter/id/214


namespace Download.Handeco
{
    public class Handeco_Company
    {
        public Handeco_HeaderCompany header;
        public Handeco_DetailCompany detail;
    }

    public static class Handeco
    {
        private static string __xmlCompanyListFile;
        private static string __xmlDetailCompanyListFile;

        public static void Init()
        {
            Handeco_LoadHeaderFromWeb.ClassInit(XmlConfig.CurrentConfig.GetElement("Handeco/Header"));
            Handeco_LoadHeader.ClassInit(XmlConfig.CurrentConfig.GetElement("Handeco/Header"));
            Handeco_LoadDetailCompanyFromWeb.ClassInit(XmlConfig.CurrentConfig.GetElement("Handeco/Detail"));
            Handeco_LoadDetailCompany.ClassInit(XmlConfig.CurrentConfig.GetElement("Handeco/Detail"));
            __xmlCompanyListFile = XmlConfig.CurrentConfig.GetExplicit("Handeco/Xml/XmlCompanyListFile");
            __xmlDetailCompanyListFile = XmlConfig.CurrentConfig.GetExplicit("Handeco/Xml/XmlDetailCompanyListFile");
        }

        public static void ExportXmlCompanyList(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            Init();

            Trace.WriteLine("export Handeco");
            Trace.WriteLine("   file        \"{0}\"", __xmlCompanyListFile);
            Trace.WriteLine("   file detail \"{0}\"", __xmlDetailCompanyListFile);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(__xmlCompanyListFile, settings), xwDetail = XmlWriter.Create(__xmlDetailCompanyListFile, settings))
            {
                xw.WriteStartElement("Handeco");
                xwDetail.WriteStartElement("Handeco");
                Handeco.LoadDetailCompanyList(startPage, maxPage, reload, loadImage).zForEach(company =>
                {
                    RemoveDuplicate(company);
                    ExportXml_Company(xw, company, false);
                    ExportXml_Company(xwDetail, company, true);
                });
                xw.WriteEndElement();
                xwDetail.WriteEndElement();
            }
        }

        public static void RemoveDuplicate(Handeco_Company company)
        {
            //   company.detail.raisonSociale       company.header.name
            if (company.header.name.Equals(company.detail.raisonSociale, StringComparison.InvariantCultureIgnoreCase))
                company.header.name = null;
            List<string> groupes = new List<string>();

            //   company.detail.adhésionGroupement  company.header.groupes
            //string detailGroupe = company.detail.adhésionGroupement;
            SortedSet<string> detailGroupes = new SortedSet<string>(company.detail.groupes);
            company.detail.groupes = detailGroupes.ToArray();

            foreach (string groupe in company.header.groupes)
            {
                //if (!groupe.Equals(detailGroupe, StringComparison.InvariantCultureIgnoreCase))
                if (!detailGroupes.Contains(groupe))
                    groupes.Add(groupe);
            }
            company.header.groupes = groupes.ToArray();

            //   company.header.activités  company.detail.activités.type
            //company.header.activités
            //SortedList<string, string> headerActivités = new SortedList<string, string>();
            //SortedSet<string> headerActivités = new SortedSet<string>(company.header.activités);
            SortedSet<string> detailActivités = new SortedSet<string>(from activité in company.detail.activités select activité.type);
            List<string> headerActivités = new List<string>();
            foreach (string activité in company.header.activités)
            {
                if (!detailActivités.Contains(activité))
                    headerActivités.Add(activité);
            }
            company.header.activités = headerActivités.ToArray();
        }

        public static void ExportXml_Company(XmlWriter xw, Handeco_Company company, bool detail)
        {
            IEnumerator<string> detailGroupes = null;
            IEnumerator<string> headerGroupes = null;
            IEnumerator<Activity> detailActivités = null;
            IEnumerator<string> headerActivités = null;
            IEnumerator<Contact> detailContacts = null;
            IEnumerator<string> detailUnknowInfos = null;

            if (detail)
            {
                detailGroupes = ((IEnumerable<string>)company.detail.groupes).GetEnumerator();
                headerGroupes = ((IEnumerable<string>)company.header.groupes).GetEnumerator();
                detailActivités = ((IEnumerable<Activity>)company.detail.activités).GetEnumerator();
                headerActivités = ((IEnumerable<string>)company.header.activités).GetEnumerator();
                detailContacts = ((IEnumerable<Contact>)company.detail.contacts).GetEnumerator();
                detailUnknowInfos = ((IEnumerable<string>)company.detail.unknowInfos).GetEnumerator();
            }

            xw.WriteStartElement("Company");
            xw.zWriteElementText("société", company.detail.raisonSociale);
            xw.zWriteElementText("société2", company.header.name);
            if (!detail)
            {
                xw.zWriteElementText("groupe", (from groupe in company.detail.groupes select groupe).FirstOrDefault());
                xw.zWriteElementText("groupe2", (from groupe in company.header.groupes select groupe).FirstOrDefault());
            }
            if (detail)
            {
                xw.zWriteElementText("groupe", detailGroupes.MoveNext() ? detailGroupes.Current : null);
                xw.zWriteElementText("groupe2", headerGroupes.MoveNext() ? headerGroupes.Current : null);

                Activity activity = null;
                if (detailActivités.MoveNext())
                    activity = detailActivités.Current;
                xw.zWriteElementText("activité", activity != null ? activity.type : null);
                xw.zWriteElementText("activité2", headerActivités.MoveNext() ? headerActivités.Current : null);
                xw.zWriteElementText("activité_description", activity != null ? activity.description : null);
                xw.zWriteElementText("moyens_activité", activity != null ? activity.moyensTechniquesDisponibles : null);
                xw.zWriteElementText("effectif_activité", activity != null ? activity.effectifTotalMobilisable : null);
                xw.zWriteElementText("modalités_activité", activity != null ? activity.modalitésPratiques : null);
                xw.zWriteElementText("couverture_activité", activity != null ? activity.couvertureGéographique : null);

                Contact contact = null;
                if (detailContacts.MoveNext())
                    contact = detailContacts.Current;
                xw.zWriteElementText("contact", contact != null ? contact.description : null);
                xw.zWriteElementText("nom_contact", contact != null ? contact.nom : null);
                xw.zWriteElementText("fonction_contact", contact != null ? contact.fonction : null);
                xw.zWriteElementText("tel_contact", contact != null ? contact.tel : null);
                xw.zWriteElementText("mobile_contact", contact != null ? contact.mobile : null);
                xw.zWriteElementText("email_contact", contact != null ? contact.email : null);
                xw.zWriteElementText("unknow", detailUnknowInfos.MoveNext() ? detailUnknowInfos.Current : null);
            }

            xw.zWriteElementText("emplacement", company.detail.localisation);
            xw.zWriteElementText("code_postal", company.header.postalCode);
            xw.zWriteElementText("dernière_miseàjour", company.detail.dernièreMiseàjour != null ? ((DateTime)company.detail.dernièreMiseàjour).ToString("yyyy-MM-dd") : null);
            xw.zWriteElementText("création", company.detail.dateCréation);
            xw.zWriteElementText("statut", company.detail.statutJuridique);
            xw.zWriteElementText("type", company.detail.typeStructure);
            xw.zWriteElementText("siteWeb", company.detail.siteWeb);
            xw.zWriteElementText("siret", company.detail.siret);
            xw.zWriteElementText("normes", company.detail.normes);
            xw.zWriteElementText("chiffre_affaires", company.detail.chiffreAffairesAnnuel);
            xw.zWriteElementText("effectif_total", company.detail.effectifTotal);
            xw.zWriteElementText("effectif_production", company.detail.effectifProduction);
            xw.zWriteElementText("effectif_encadrement", company.detail.effectifEncadrement);
            xw.zWriteElementText("nombre_travailleurs_handicapés", company.detail.nombreTravailleursHandicapés);
            xw.zWriteElementText("nombre_handicapé_accompagné", company.detail.nombreHandicapéAccompagné);

            xw.zWriteElementText("appartenance_groupe", company.detail.appartenanceGroupe);
            xw.zWriteElementText("présentation_groupe", company.detail.présentationGroupe);
            xw.zWriteElementText("siteweb_groupe", company.detail.siteWebGroupe);
            xw.zWriteElementText("adhésion_réseaux", company.detail.adhésionRéseauxHandicap);
            //xw.zWriteElementText("adhésion_groupement", company.detail.adhésionGroupement);
            xw.zWriteElementText("cotraitance", company.detail.cotraitance);

            xw.zWriteElementText("adresse_principale", company.detail.adressePrincipale);
            xw.zWriteElementText("adresse_siège", company.detail.adresseSiège);
            xw.zWriteElementText("adresse_antennes", company.detail.adresseAntennes);
            xw.zWriteElementText("email", company.detail.email);
            xw.zWriteElementText("tel", company.detail.tel);
            xw.zWriteElementText("fax", company.detail.fax);
            xw.zWriteElementText("code_ape", company.detail.codeApe);
            xw.zWriteElementText("numero_finess", company.detail.numeroFiness);

            xw.zWriteElementText("logo", company.detail.logo);
            xw.zWriteElementText("url", company.detail.sourceUrl);

            xw.WriteEndElement();

            while (detail)
            {
                bool detailGroupe = detailGroupes.MoveNext();
                bool headerGroupe = headerGroupes.MoveNext();
                bool headerActivity = headerActivités.MoveNext();
                bool detailActivity = detailActivités.MoveNext();
                bool detailContact = detailContacts.MoveNext();
                bool detailUnknowInfo = detailUnknowInfos.MoveNext();

                if (!detailGroupe && !headerGroupe && !headerActivity && !detailActivity && !detailContact && !detailUnknowInfo)
                    break;

                xw.WriteStartElement("Company");

                if (detailUnknowInfo)
                    xw.zWriteElementText("Unknow", detailUnknowInfos.Current);
                if (detailGroupe)
                    xw.zWriteElementText("groupe", detailGroupes.Current);
                if (headerGroupe)
                    xw.zWriteElementText("groupe2", headerGroupes.Current);

                Activity activity = null;
                if (detailActivity)
                    activity = detailActivités.Current;
                if (activity != null)
                    xw.zWriteElementText("activité", activity.type);
                if (headerActivity)
                    xw.zWriteElementText("activité2", headerActivités.Current);
                if (activity != null)
                {
                    xw.zWriteElementText("activité_description", activity.description);
                    xw.zWriteElementText("moyens_activité", activity.moyensTechniquesDisponibles);
                    xw.zWriteElementText("effectif_activité", activity.effectifTotalMobilisable);
                    xw.zWriteElementText("modalités_activité", activity.modalitésPratiques);
                    xw.zWriteElementText("couverture_activité", activity.couvertureGéographique);
                }

                if (detailContact)
                {
                    Contact contact = detailContacts.Current;
                    xw.zWriteElementText("contact", contact.description);
                    xw.zWriteElementText("nom_contact", contact.nom);
                    xw.zWriteElementText("fonction_contact", contact.fonction);
                    xw.zWriteElementText("tel_contact", contact.tel);
                    xw.zWriteElementText("mobile_contact", contact.mobile);
                    xw.zWriteElementText("email_contact", contact.email);
                }

                xw.WriteEndElement();
            }
        }

        public static IEnumerable<Handeco_Company> LoadDetailCompanyList(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            return from header in new Handeco_LoadHeaderPages(startPage, maxPage, reload, loadImage) select Handeco.LoadDetailCompany(header, reload, loadImage);
        }

        public static Handeco_Company LoadDetailCompany(Handeco_HeaderCompany header, bool reload = false, bool loadImage = false)
        {
            Handeco_DetailCompany detail = Handeco_LoadDetailCompany.LoadCompany(header.urlDetail, reload, loadImage);
            return new Handeco_Company { header = header, detail = detail };
        }
    }
}
