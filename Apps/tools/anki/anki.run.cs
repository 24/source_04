// $$info.test.regex

//*************************************************************************************************************************
//****                                                  anki.project.xml
//*************************************************************************************************************************

RunSourceCommand.SetProjectFromSource();
//RunSourceCommand.SetProject(@"$Root$\Apps\tools\tv\_Test\Test_TV.project.xml");
Trace.WriteLine("toto");

RunSourceCommand.CompileProject("AnkiJS.project.xml");



QuestionRun.CreateQuestionsManager(@"UE8-med\").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\").CreateAllQuestionResponseFiles();

//QuestionRun.WriteAnkiCard("UE3 - Activité électrique cardiaque");
//QuestionRun.WriteAnkiCard("UE3 - Equilibre acido-basique");
//QuestionRun.WriteAnkiCard("UE3 - Etat de la matiere et leur caracterisaion Etat d'agregation");
//QuestionRun.WriteAnkiCard("UE3 - Etat de la matiere et leur caracterisation");
//QuestionRun.WriteAnkiCard("UE3 - Etat de la matiere et leur caracterisation Proprietes colligatives");
//QuestionRun.WriteAnkiCard("UE3 - Gaz, eau et solutions aqueuses");
//QuestionRun.WriteAnkiCard("UE3 - pH et equilibre acido-basique");

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-01-Physio du muscle strié squelettique").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-01-Physio du muscle strié squelettique").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-02-Physiologie du muscle lisse").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-02-Physiologie du muscle lisse").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-03-Physiologie cardiovasculaire").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-03-Physiologie cardiovasculaire").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-04-Systeme a haute pression et regulation de la PA").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-04-Systeme a haute pression et regulation de la PA").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-05-Transport de l'O2 par le sang").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-05-Transport de l'O2 par le sang").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-06-La sensibilite").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-06-La sensibilite").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-07-La motricite").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-07-La motricite").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-08-Transport de l'O2 et du CO2 par le sang").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-08-Transport de l'O2 et du CO2 par le sang").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-09-Equilibration").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-09-Equilibration").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-10-Introduction à la physiologie endocrinienne").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\UE8-med-10-Introduction à la physiologie endocrinienne").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-01-Fiche - Le vieillissement cellulaire").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-01-Fiche - Le vieillissement cellulaire").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-02-Les cellules souches").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-02-Les cellules souches").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-03-Modes de transmission et maladies genetiques").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-03-Modes de transmission et maladies genetiques").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-04-Principales anomalies chromosomiques").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-04-Principales anomalies chromosomiques").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-05-Indication du diagnostic prenatal chromosomique").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-05-Indication du diagnostic prenatal chromosomique").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-06-Information consentement et secret medical").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-06-Information consentement et secret medical").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-07-Communication cellulaire").RenameAndCopyScanFiles(simulate: true);
QuestionRun.CreateQuestionsManager(@"UE8\UE8-07-Communication cellulaire").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-07-Communication cellulaire").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-08-Pathologie moleculaires et genome techniques pour la recherche de mutations").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-08-Pathologie moleculaires et genome techniques pour la recherche de mutations").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-09-Pathologies moleculaires et genome techniques d'etude du genome").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-09-Pathologies moleculaires et genome techniques d'etude du genome").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-10-Pathologie moleculaire et genome stabilité et evolution du genome").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-10-Pathologie moleculaire et genome stabilité et evolution du genome").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-11-Pathologie moleculaire et genome mutations").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-11-Pathologie moleculaire et genome mutations").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-12-Ethique medecine legale leglislation 1").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-12-Ethique medecine legale leglislation 1").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-13-Ethique medecine legale leglislation 2").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-13-Ethique medecine legale leglislation 2").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-14-La relation soignant-soigné").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-14-La relation soignant-soigné").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE8\UE8-15-Ethique medecine et santé des humains").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-15-Ethique medecine et santé des humains").CreateAllQuestionResponseFiles();


QuestionRun.CreateQuestionsManager(@"UE8\UE8-16-Pathologie moleculaire et genome les hemoglobinopathies").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8\UE8-16-Pathologie moleculaire et genome les hemoglobinopathies").CreateAllQuestionResponseFiles();



QuestionRun.CreateQuestionsManager(@"UE6\UE6-01-Fiche - Histoire du medicament").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-01-Fiche - Histoire du medicament").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-02-Statut des medicaments et autres produits de santé 1").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-02-Statut des medicaments et autres produits de santé 1").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-02-Statut des medicaments et autres produits de santé 2").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-02-Statut des medicaments et autres produits de santé 2").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-03-Description des medicaments").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-03-Description des medicaments").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-04-Developpement des medicaments").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-04-Developpement des medicaments").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-05-Production des medicaments").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-05-Production des medicaments").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-06-Cibles mecanismes d'action 1").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-06-Cibles mecanismes d'action 1").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-06-Cibles mécanismes d'action 2").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-06-Cibles mécanismes d'action 2").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-07-Les aspects socioeconomiques du medicament 1").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-07-Les aspects socioeconomiques du medicament 1").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-07-Les aspects socioeconomiques du medicament 2").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-07-Les aspects socioeconomiques du medicament 2").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-08-Evaluation des medicaments et agrement pour leur commercialisation 1").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-08-Evaluation des medicaments et agrement pour leur commercialisation 1").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-09-Regle de prescription l'ordonnance").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-09-Regle de prescription l'ordonnance").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-10-Apercu de la pharmacodynamie").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-10-Apercu de la pharmacodynamie").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-11-Apercu de la pharmacocinetique").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-11-Apercu de la pharmacocinetique").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE6\UE6-12-Rapport benefice risque decision therapeutique et effets indesirables").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE6\UE6-12-Rapport benefice risque decision therapeutique et effets indesirables").CreateAllQuestionResponseFiles();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé 2").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé 2").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé 2").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé 2").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Statut des medicaments et autres produits de santé").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Regle de prescription L'ordonnance").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Regle de prescription L'ordonnance").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Regle de prescription L'ordonnance").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Regle de prescription L'ordonnance").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Rapport benefice_risque Decision therapeutique et effets indesirables").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Rapport benefice_risque Decision therapeutique et effets indesirables").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Rapport benefice_risque Decision therapeutique et effets indesirables").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Rapport benefice_risque Decision therapeutique et effets indesirables").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Production des medicaments").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Production des medicaments").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Production des medicaments").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Production des medicaments").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Les aspects socioeconomiques du medicament 2").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Les aspects socioeconomiques du medicament 2").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Les aspects socioeconomiques du medicament 2").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Les aspects socioeconomiques du medicament 2").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Fiche - Histoire du medicament").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Fiche - Histoire du medicament").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Fiche - Histoire du medicament").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Fiche - Histoire du medicament").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Evaluation des medicaments et agrement pour leur commercialisation 1").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Evaluation des medicaments et agrement pour leur commercialisation 1").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Evaluation des medicaments et agrement pour leur commercialisation 1").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Evaluation des medicaments et agrement pour leur commercialisation 1").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Developpement des medicaments").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Developpement des medicaments").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Developpement des medicaments").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Developpement des medicaments").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Description des medicaments").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Description des medicaments").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Description des medicaments").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Description des medicaments").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mécanismes d'action 2").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mécanismes d'action 2").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mécanismes d'action 2").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mécanismes d'action 2").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mecanismes d'action 1").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mecanismes d'action 1").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mecanismes d'action 1").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Cibles, mecanismes d'action 1").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Aspect socioeconomiques du medicament 1").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Aspect socioeconomiques du medicament 1").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Aspect socioeconomiques du medicament 1").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Aspect socioeconomiques du medicament 1").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacodynamie").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacodynamie").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacodynamie").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacodynamie").ExportResponse();

//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacocinetique").CreateQuestionResponseFiles();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacocinetique").CreateQuestionsFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacocinetique").CreateResponseFile();
//QuestionRun.CreateQuestionsManager(@"UE6\UE6 - Apercu de la pharmacocinetique").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE5\").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE5\").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE5\").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE5\").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE5\UE5 - Appareil Cardiovasculaire 2").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE5\UE5 - Appareil Cardiovasculaire 2").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE5\UE5 - Appareil Cardiovasculaire 2").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE5\UE5 - Appareil Cardiovasculaire 2").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE5\").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE5\").CreateAnkiFileFromQuestionFiles();

QuestionRun.CreateQuestionsManager(@"UE3\UE3-05-Propriete colligative et transports membranaires").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-05-Propriete colligative et transports membranaires").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE3\UE3-06-Transports electriques et transmembranaires").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-06-Transports electriques et transmembranaires").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE3\UE3-09-Regulations des espaces hydriques").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-09-Regulations des espaces hydriques").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE3\UE3-10-Regulation du bilan hydro-sodé").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-10-Regulation du bilan hydro-sodé").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE3\UE3-12-Regulation acido-basique").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-12-Regulation acido-basique").CreateAllQuestionResponseFiles();

QuestionRun.CreateQuestionsManager(@"UE3\UE3-14-Thermoregulation").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-14-Thermoregulation").CreateAllQuestionResponseFiles();










QuestionRun.CreateQuestionsManager(@"UE3\UE3-08-Potentiel membranaire de repos potentiel local potentiel d'action").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-08-Potentiel membranaire de repos potentiel local potentiel d'action").CreateAllQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-08-Potentiel membranaire de repos potentiel local potentiel d'action").CreateAnkiFileFromQuestionFiles();


QuestionRun.CreateQuestionsManager(@"UE3\UE3-07-Activité électrique cardiaque").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-07-Activité électrique cardiaque").CreateAllQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-07-Activité électrique cardiaque").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3-07-Activité électrique cardiaque").CreateAnkiFileFromScanFiles();

QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Transports electriques et transmembranaires").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Transports electriques et transmembranaires").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Transports electriques et transmembranaires").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Transports electriques et transmembranaires").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Transports electriques et transmembranaires").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Transports electriques et transmembranaires").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Thermoregulation").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Thermoregulation").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Thermoregulation").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Thermoregulation").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Thermoregulation").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Thermoregulation").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulations des espaces hydriques").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulations des espaces hydriques").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulations des espaces hydriques").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulations des espaces hydriques").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulations des espaces hydriques").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulations des espaces hydriques").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation du bilan hydro-sodé").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation du bilan hydro-sodé").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation du bilan hydro-sodé").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation du bilan hydro-sodé").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation du bilan hydro-sodé").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation du bilan hydro-sodé").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation acido-basique").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation acido-basique").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation acido-basique").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation acido-basique").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation acido-basique").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Regulation acido-basique").ExportResponse();

QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Propriete colligative et Transports membranaires").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Propriete colligative et Transports membranaires").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Propriete colligative et Transports membranaires").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Propriete colligative et Transports membranaires").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Propriete colligative et Transports membranaires").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Propriete colligative et Transports membranaires").ExportResponse();


QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Potentiel membranaire de repos, potentiel local, potentiel d'action").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Potentiel membranaire de repos, potentiel local, potentiel d'action").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Potentiel membranaire de repos, potentiel local, potentiel d'action").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Potentiel membranaire de repos, potentiel local, potentiel d'action").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Potentiel membranaire de repos, potentiel local, potentiel d'action").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - Potentiel membranaire de repos, potentiel local, potentiel d'action").ExportResponse();



//QuestionRun.Exec(@"UE3\UE3 - pH et equilibre acido-basique");
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateQuestionsFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").ExportResponse();

QuestionRun.CreateQuestionsManager(@"test\UE3 - pH et equilibre acido-basique").CreateQuestionFiles();


QuestionRun.ReadResponse("UE3 - Etat de la matiere et leur caracterisaion Etat d'agregation");
QuestionRun.ReadResponse("UE3 - Etat de la matiere et leur caracterisation");
QuestionRun.ReadResponse("UE3 - Etat de la matiere et leur caracterisation Proprietes colligatives");
QuestionRun.ReadResponse("UE3 - Gaz, eau et solutions aqueuses");
QuestionRun.ReadResponse("UE3 - pH et equilibre acido-basique");
QuestionRun.ExportResponse(@"");

QuestionTest.Test_Find_01("2016", QuestionRun.GetQuestionRegexValuesList()).zTraceJson();
QuestionTest.Test_Find_02("Q 24 : Q51 :  Q51 :  Q 60 : Q51 :", QuestionRun.GetResponseRegexValuesList());
QuestionTest.Test_FindInFile_01(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-012.txt", QuestionRun.GetQuestionRegexValuesList());
QuestionTest.Test_FindInFile_01(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-022.txt", QuestionRun.GetResponseRegexValuesList());

QuestionReader.Read(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-009.txt", QuestionRun.GetQuestionRegexValuesList()).zTraceJson();
QuestionReader.Read(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-012_02.txt", QuestionRun.GetQuestionRegexValuesList()).zTraceJson();
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Activité électrique cardiaque\scan\UE3 - Activité électrique cardiaque-page-022_02.txt", QuestionRun.GetResponseRegexValuesList()).zTraceJson();
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Activité électrique cardiaque\scan\UE3 - Activité électrique cardiaque-page-022_02.txt", QuestionRun.GetResponseRegexValuesList()).zSave(@"c:\pib\drive\google\valentin\UE3\UE3 - Activité électrique cardiaque\responses.json");
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Equilibre acido-basique\scan\UE3 - Equilibre acido-basique-page-016_02.txt", QuestionRun.GetResponseRegexValuesList()).zTraceJson();
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Equilibre acido-basique\scan\UE3 - Equilibre acido-basique-page-016_02.txt", QuestionRun.GetResponseRegexValuesList()).zSave(@"c:\pib\drive\google\valentin\UE3\UE3 - Equilibre acido-basique\responses.json");
QuestionRun.WriteAnkiFile(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-009.anki.txt", @"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-009.txt", @"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-022_02.txt");

QuestionRun.Test_01();
QuestionRun.Test_02();

//*************************************************************************************************************************
//****                                   VSProject
//*************************************************************************************************************************

// VSProjectUpdateOptions.AddSource | VSProjectUpdateOptions.RemoveSource | VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
// VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference
// VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate

// simulate no remove
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.Simulate);
// save vs project no remove
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.AddAssemblyReference);
// simulate
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);
// save vs project
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference);

// simulate - runsource.irunsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.irunsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml", options: VSProjectUpdateOptions.All);
// simulate - runsource.dll.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.dll.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
RunSourceVSProjectCommand.TraceVSProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.csproj");
RunSourceVSProjectCommand.TraceRunSourceProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml");
// simulate - runsource.command.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.command.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.project.xml", options: VSProjectUpdateOptions.All);
RunSourceVSProjectCommand.TraceVSProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.csproj");
RunSourceVSProjectCommand.TraceRunSourceProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.project.xml");
// simulate - runsource.launch.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.launch\runsource.launch.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.launch.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.launch\runsource.launch.project.xml", options: VSProjectUpdateOptions.All);
// simulate - runsource.runsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.runsource\runsource.runsource.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.runsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.runsource\runsource.runsource.project.xml", options: VSProjectUpdateOptions.All);


RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Lib\pb\Source\Project\XmlConfig.project.xml", VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.Simulate);
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Lib\pb\Source\Project\XmlConfig.project.xml", VSProjectUpdateOptions.AddSourceLink);
RunSourceVSProjectCommand.TraceVSProject();
RunSourceVSProjectCommand.TraceRunSourceProject();
RunSourceVSProjectCommand.Test_BackupVSProject();

VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts_test_08.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\WebData.project.xml".zGetRunSourceProjectPath());

VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\Basic.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\RunSourceExtension.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\MongoExtension.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\MongoDefaultSerializer.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\MongoWebHeaderSerializer.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\HtmlRun.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\WebData.project.xml".zGetRunSourceProjectPath());

FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata.txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata(1).txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata[1].txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata_1.txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata.txt").GetPath(2).zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata(1).txt").GetPath(2).zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata[1].txt").GetPath(2).zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata_1.txt").GetPath(2).zTraceJson();

//*************************************************************************************************************************
//****                                   Test Project
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_Basic.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RegexValues.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RegexValuesList.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RunSourceExtension.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_MongoExtension.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_MongoSerializer.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_HttpRun.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_HtmlRun.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_Extension_01.dll.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_VSProject.project.xml");



using(FileStream fs = zFile.Open(@"c:\pib\_dl\test_01.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
{
    fs.Write(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }, 0, 5);
}

//*************************************************************************************************************************
//****                                   $$info.test.regex
//*************************************************************************************************************************

Trace.WriteLine("toto");
Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Text\_Test\Test_Text.project.xml");

Test_Regex.Test(@"(?<=(?:^|\s))([a-e]{1,4})(?=(?:\s|$))", " abc ");

("aa" + ((char)0x2072)).zTrace();
("aa" + ((char)0x2082)).zTrace();
("aa" + ((char)0x207A)).zTrace();
("aa" + ((char)0x208A)).zTrace();


Test_RegexValues.Test2(QuestionRun.GetResponseRegexValuesList(), "Q 37 : D Q 26 : D Q 25 : E  Q 24 : C", contiguous: true);
Test_Regex.Test(@"\s*([a-e]{1,4})", "  AB ");


RunSourceCommand.CompileProject(@"$Root$\Lib\pb\Source\pb\NodeJS\_Test\Test_NodeJS_01.project.xml");
RunSourceCommand.CompileProject(@"$Root$\Lib\pb\Source\Project\NodeJSLib.project.xml");


QuestionReader.Read(new string[] { @"c:\pib\drive\google\valentin\_test\test_question_association_01.txt" }, QuestionRun.GetQuestionRegexValuesList()).zSave(@"c:\pib\drive\google\valentin\_test\test_question_association_01.json", jsonIndent: true);


new System.Web.HtmlString("&amp;").ToHtmlString().zTrace();
System.Web.HttpUtility.HtmlEncode("&").zTrace();