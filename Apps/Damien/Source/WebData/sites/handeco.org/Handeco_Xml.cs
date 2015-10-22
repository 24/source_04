using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using pb;
using pb.Data.Xml;
using pb.IO;
using pb.Web.Data;

namespace hts.WebData
{
    public static class Handeco_Xml
    {
        public static void ExportXml(IEnumerable<HeaderDetail<Handeco_Header, Handeco_Detail>> headerDetails, string headersFile, string detailsFile)
        {
            Trace.WriteLine("export xml Handeco");
            Trace.WriteLine("   headers file  \"{0}\"", headersFile);
            Trace.WriteLine("   details file  \"{0}\"", detailsFile);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            zfile.CreateFileDirectory(headersFile);
            zfile.CreateFileDirectory(detailsFile);

            using (XmlWriter xwHeaders = XmlWriter.Create(headersFile, settings), xwDetails = XmlWriter.Create(detailsFile, settings))
            {
                xwHeaders.WriteStartElement("Handeco");
                xwDetails.WriteStartElement("Handeco");
                headerDetails.zForEach(headerDetail =>
                {
                    RemoveDuplicate(headerDetail);
                    ExportXml(xwHeaders, headerDetail, false);
                    ExportXml(xwDetails, headerDetail, true);
                });
                xwHeaders.WriteEndElement();
                xwDetails.WriteEndElement();
            }
        }

        private static void RemoveDuplicate(HeaderDetail<Handeco_Header, Handeco_Detail> headerDetail)
        {
            //   company.detail.raisonSociale       company.header.name
            if (headerDetail.Header.Name.Equals(headerDetail.Detail.RaisonSociale, StringComparison.InvariantCultureIgnoreCase))
                headerDetail.Header.Name = null;

            SortedSet<string> detailGroupes = new SortedSet<string>(headerDetail.Detail.Groupes);
            headerDetail.Detail.Groupes = detailGroupes.ToArray();
            List<string> headerGroupes = new List<string>();
            foreach (string groupe in headerDetail.Header.Groupes)
            {
                if (!detailGroupes.Contains(groupe))
                    headerGroupes.Add(groupe);
            }
            headerDetail.Header.Groupes = headerGroupes.ToArray();

            //   company.header.activités  company.detail.activités.type
            SortedSet<string> detailActivités = new SortedSet<string>(from activité in headerDetail.Detail.Activités select activité.Type);
            List<string> headerActivités = new List<string>();
            foreach (string activité in headerDetail.Header.Activités)
            {
                if (!detailActivités.Contains(activité))
                    headerActivités.Add(activité);
            }
            headerDetail.Header.Activités = headerActivités.ToArray();
        }

        private static void ExportXml(XmlWriter xw, HeaderDetail<Handeco_Header, Handeco_Detail> headerDetail, bool detail)
        {
            IEnumerator<string> detailGroupes = null;
            IEnumerator<string> headerGroupes = null;
            IEnumerator<Activity> detailActivités = null;
            IEnumerator<string> headerActivités = null;
            IEnumerator<Contact> detailContacts = null;
            IEnumerator<string> detailUnknowInfos = null;

            if (detail)
            {
                detailGroupes = ((IEnumerable<string>)headerDetail.Detail.Groupes).GetEnumerator();
                headerGroupes = ((IEnumerable<string>)headerDetail.Header.Groupes).GetEnumerator();
                detailActivités = ((IEnumerable<Activity>)headerDetail.Detail.Activités).GetEnumerator();
                headerActivités = ((IEnumerable<string>)headerDetail.Header.Activités).GetEnumerator();
                detailContacts = ((IEnumerable<Contact>)headerDetail.Detail.Contacts).GetEnumerator();
                detailUnknowInfos = ((IEnumerable<string>)headerDetail.Detail.UnknowInfos).GetEnumerator();
            }

            xw.WriteStartElement("Company");
            xw.zWriteElementText("société", headerDetail.Detail.RaisonSociale);
            xw.zWriteElementText("société2", headerDetail.Header.Name);
            if (!detail)
            {
                xw.zWriteElementText("groupe", (from groupe in headerDetail.Detail.Groupes select groupe).FirstOrDefault());
                xw.zWriteElementText("groupe2", (from groupe in headerDetail.Header.Groupes select groupe).FirstOrDefault());
            }
            if (detail)
            {
                xw.zWriteElementText("groupe", detailGroupes.MoveNext() ? detailGroupes.Current : null);
                xw.zWriteElementText("groupe2", headerGroupes.MoveNext() ? headerGroupes.Current : null);

                Activity activity = null;
                if (detailActivités.MoveNext())
                    activity = detailActivités.Current;
                xw.zWriteElementText("activité", activity != null ? activity.Type : null);
                xw.zWriteElementText("activité2", headerActivités.MoveNext() ? headerActivités.Current : null);
                xw.zWriteElementText("activité_description", activity != null ? activity.Description : null);
                xw.zWriteElementText("moyens_activité", activity != null ? activity.MoyensTechniquesDisponibles : null);
                xw.zWriteElementText("effectif_activité", activity != null ? activity.EffectifTotalMobilisable : null);
                xw.zWriteElementText("modalités_activité", activity != null ? activity.ModalitésPratiques : null);
                xw.zWriteElementText("couverture_activité", activity != null ? activity.CouvertureGéographique : null);

                Contact contact = null;
                if (detailContacts.MoveNext())
                    contact = detailContacts.Current;
                xw.zWriteElementText("contact", contact != null ? contact.Description : null);
                xw.zWriteElementText("nom_contact", contact != null ? contact.Nom : null);
                xw.zWriteElementText("fonction_contact", contact != null ? contact.Fonction : null);
                xw.zWriteElementText("tel_contact", contact != null ? contact.Tel : null);
                xw.zWriteElementText("mobile_contact", contact != null ? contact.Mobile : null);
                xw.zWriteElementText("email_contact", contact != null ? contact.Email : null);
                xw.zWriteElementText("unknow", detailUnknowInfos.MoveNext() ? detailUnknowInfos.Current : null);
            }

            xw.zWriteElementText("emplacement", headerDetail.Detail.Localisation);
            xw.zWriteElementText("code_postal", headerDetail.Header.PostalCode);
            xw.zWriteElementText("dernière_miseàjour", headerDetail.Detail.DernièreMiseàjour != null ? ((DateTime)headerDetail.Detail.DernièreMiseàjour).ToString("yyyy-MM-dd") : null);
            xw.zWriteElementText("création", headerDetail.Detail.DateCréation);
            xw.zWriteElementText("statut", headerDetail.Detail.StatutJuridique);
            xw.zWriteElementText("type", headerDetail.Detail.TypeStructure);
            xw.zWriteElementText("siteWeb", headerDetail.Detail.SiteWeb);
            xw.zWriteElementText("siret", headerDetail.Detail.Siret);
            xw.zWriteElementText("normes", headerDetail.Detail.Normes);
            xw.zWriteElementText("chiffre_affaires", headerDetail.Detail.ChiffreAffairesAnnuel);
            xw.zWriteElementText("effectif_total", headerDetail.Detail.EffectifTotal);
            xw.zWriteElementText("effectif_production", headerDetail.Detail.EffectifProduction);
            xw.zWriteElementText("effectif_encadrement", headerDetail.Detail.EffectifEncadrement);
            xw.zWriteElementText("nombre_travailleurs_handicapés", headerDetail.Detail.NombreTravailleursHandicapés);
            xw.zWriteElementText("nombre_handicapé_accompagné", headerDetail.Detail.NombreHandicapéAccompagné);

            xw.zWriteElementText("appartenance_groupe", headerDetail.Detail.AppartenanceGroupe);
            xw.zWriteElementText("présentation_groupe", headerDetail.Detail.PrésentationGroupe);
            xw.zWriteElementText("siteweb_groupe", headerDetail.Detail.SiteWebGroupe);
            xw.zWriteElementText("adhésion_réseaux", headerDetail.Detail.AdhésionRéseauxHandicap);
            xw.zWriteElementText("cotraitance", headerDetail.Detail.Cotraitance);

            xw.zWriteElementText("adresse_principale", headerDetail.Detail.AdressePrincipale);
            xw.zWriteElementText("adresse_siège", headerDetail.Detail.AdresseSiège);
            xw.zWriteElementText("adresse_antennes", headerDetail.Detail.AdresseAntennes);
            xw.zWriteElementText("email", headerDetail.Detail.Email);
            xw.zWriteElementText("tel", headerDetail.Detail.Tel);
            xw.zWriteElementText("fax", headerDetail.Detail.Fax);
            xw.zWriteElementText("code_ape", headerDetail.Detail.CodeApe);
            xw.zWriteElementText("numero_finess", headerDetail.Detail.NumeroFiness);

            xw.zWriteElementText("logo", headerDetail.Detail.Logo);
            xw.zWriteElementText("url", headerDetail.Detail.SourceUrl);

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
                    xw.zWriteElementText("activité", activity.Type);
                if (headerActivity)
                    xw.zWriteElementText("activité2", headerActivités.Current);
                if (activity != null)
                {
                    xw.zWriteElementText("activité_description", activity.Description);
                    xw.zWriteElementText("moyens_activité", activity.MoyensTechniquesDisponibles);
                    xw.zWriteElementText("effectif_activité", activity.EffectifTotalMobilisable);
                    xw.zWriteElementText("modalités_activité", activity.ModalitésPratiques);
                    xw.zWriteElementText("couverture_activité", activity.CouvertureGéographique);
                }

                if (detailContact)
                {
                    Contact contact = detailContacts.Current;
                    xw.zWriteElementText("contact", contact.Description);
                    xw.zWriteElementText("nom_contact", contact.Nom);
                    xw.zWriteElementText("fonction_contact", contact.Fonction);
                    xw.zWriteElementText("tel_contact", contact.Tel);
                    xw.zWriteElementText("mobile_contact", contact.Mobile);
                    xw.zWriteElementText("email_contact", contact.Email);
                }

                xw.WriteEndElement();
            }
        }
    }
}
