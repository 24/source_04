// $$info.test.regex
// $$info.VSProject

//*************************************************************************************************************************
//****                                                  anki.project.xml
//*************************************************************************************************************************

RunSourceCommand.SetProjectFromSource();
//RunSourceCommand.SetProject(@"$Root$\Apps\tools\tv\_Test\Test_TV.project.xml");
Trace.WriteLine("toto");

RunSourceCommand.CompileProject("AnkiJS.project.xml");

// scanned
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-01-acide aminé").SetParameters(new QuestionsParameters { PageRange = "14-26" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-01-acide aminé").GetParameters().zTraceJson();
//QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-01-acide aminé").ExtractImagesFromPdf();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-01-acide aminé").Scan(simulate: true);
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-01-acide aminé").Scan(imageScan: true);

QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-02-lipides").SetParameters(new QuestionsParameters { PageRange = "18-42" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-02-lipides").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-02-lipides").Scan(imageScan: true);

QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-04-enzymologie").SetParameters(new QuestionsParameters { PageRange = "13-39" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-04-enzymologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-04-enzymologie").Scan(imageScan: true);

// S1\UE1\UE1-05-glucides pas de questions

// scanned
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-07-bio-acides nucléiques").SetParameters(new QuestionsParameters { PageRange = "09-13" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-07-bio-acides nucléiques").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-07-bio-acides nucléiques").Scan();

// scanned
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-08-bio-réplication").SetParameters(new QuestionsParameters { PageRange = "07-11" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-08-bio-réplication").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-08-bio-réplication").Scan();

// scanned
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-09-bio-génome").SetParameters(new QuestionsParameters { PageRange = "12-17" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-09-bio-génome").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-09-bio-génome").Scan();

// scanned - UE1-10-bio-traduction-réparation
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-10-bio-traduction-réparation").SetParameters(new QuestionsParameters { PageRange = "09-19" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-10-bio-traduction-réparation").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-10-bio-traduction-réparation").Scan();

// UE1-11-glucides 2
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-11-glucides 2").SetParameters(new QuestionsParameters { PageRange = "06-31" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-11-glucides 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-11-glucides 2").Scan();

// scanned
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-13-lipides").SetParameters(new QuestionsParameters { PageRange = "09-12" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-13-lipides").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-13-lipides").Scan();

// scanned
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-14-glucides").SetParameters(new QuestionsParameters { PageRange = "10-14" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-14-glucides").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-14-glucides").Scan();



// scanned
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").SetParameters(new QuestionsParameters { PageRange = "15-21", PageColumn = 2, PageRotate = PageRotate.Rotate90, ImagesExtracted = true });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").GetParameters().zTraceJson();
//QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").ExtractImagesFromPdf();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").Scan(range: "15", simulate: true);
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").Scan(range: "15");
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").Scan(range: "16-21");

QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").SetParameters(new QuestionsParameters { PageRange = "15-20", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").Scan(range: "15-17");
















QuestionRun.CreateQuestionsManager(@"UE8-med\").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE8-med\").CreateAllQuestionResponseFiles();

QuestionTest.Test_ColumnText_01(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test");


QuestionRun.CreateQuestionsManager(@"UE5\UE5-01-anatomie").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"UE5\UE5-01-anatomie").CreateAllQuestionResponseFiles();

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
// $$info.VSProject

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
RunSourceVSProjectCommand.UpdateVSProject(new string[] { @"$Root$\Apps\tools\anki\anki.project.xml", @"$Root$\Apps\tools\anki\AnkiJS.project.xml" },
  options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
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



//*************************************************************************************************************************
//****                                   QuestionTest
//*************************************************************************************************************************

QuestionTest.Test_ColumnText_01(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test");
QuestionTest.Test_ColumnText_01(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test", limit: 1);
new ColumnTextManager(minSpaceBeforeColumn: 4).Test_GetColumnInfos(zFile.ReadAllLines(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test\UE5 - Fiche - Introduction à l'anatomie-page-015.txt")).zView();
new ColumnTextManager(minSpaceBeforeColumn: 4).Test_GetColumnInfos(zFile.ReadAllLines(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test\UE5 - Fiche - Introduction à l'anatomie-page-019.txt")).zView();

new ColumnTextManager(minSpaceBeforeColumn: 4).Test_GetColumnInfos(zFile.ReadAllLines(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test\UE5 - Fiche - Introduction à l'anatomie-page-015.txt"))
  .Where(columnInfo => columnInfo.ColumnPosition != -1).GroupBy(columnInfo => columnInfo.ColumnPosition).Select(group => new { ColumnPosition = group.Key, Count = group.Count() }).zTraceJson();

new ColumnTextManager(minSpaceBeforeColumn: 4).Test_GetColumnInfos(zFile.ReadAllLines(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test\UE5 - Fiche - Introduction à l'anatomie-page-015.txt"))
  .Where(columnInfo => columnInfo.ColumnPosition != -1).GroupBy(columnInfo => columnInfo.ColumnPosition).Select(group => new { ColumnPosition = group.Key, Count = group.Count() }).zTraceJson();

new int[] { 11, 5, 15 }.Min().zTrace();
new int[] { }.Min().zTrace();




//*************************************************************************************************************************
//****                                   Test_HttpClient
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Web\Http\_Test\Test_Http.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Web\Http\_Test\Test_HttpClient.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Web\Http\_Test\Test_HttpManager_v5.project.xml");

Test_HttpClient.Test_01("https://www.google.fr");
Test_HttpClient.Test_HttpClient_01("https://www.google.fr");
Test_HttpClient.Test_HttpClient_01("https://www.google.fr",
  userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0",
  accept: "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
Test_HttpClient.Test_HttpClient_01("https://www.google.fr",
  userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
Test_HttpClient.Test_HttpClient_01("https://www.google.fr",
    accept: "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
Test_HttpClient.Test_HttpClient_01("http://localhost:8080/test_utf8_01.html");
Test_HttpClient.Test_HttpClient_01("http://localhost:8080/test_ansi_01.html");
// http status : 200 OK, 400 Bad Request, 500 Internal Server Error
Test_HttpClient.Test_03("https://httpbin.org/status/400");
Test_HttpClient.Test_03("http://toto.toto");
//Test_HttpClient.Test_04("http://toto.toto");
Test_HttpClient.Test_LoadText_01("http://toto.toto");
Test_HttpClient._Test_LoadText_01("http://toto.toto");
Test_HttpClient.Test_LoadText_02("http://toto.toto");
Test_HttpClient.Test_LoadToFile_01("https://www.google.fr", @"c:\pib\_dl\_aaaaaaaa\cache.new\www.google.fr_01.html");
Test_HttpClient.Test_LoadToFile_01("https://www.google.fr", @"c:\pib\_dl\_aaaaaaaa\cache.new\www.google.fr_02.html", userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
Test_HttpClient.Test_LoadToFile_01("http://localhost:8080/test_utf8_01.html", @"c:\pib\_dl\_aaaaaaaa\cache.new\test_utf8_01.html");
Test_HttpClient.Test_LoadToFile_01("http://localhost:8080/test_utf8_bad_01.html", @"c:\pib\_dl\_aaaaaaaa\cache.new\test_utf8_bad_01.html");
Test_HttpClient.Test_LoadToFile_01("http://localhost:8080/test_ansi_01.html", @"c:\pib\_dl\_aaaaaaaa\cache.new\test_ansi_01.html");
Test_HttpClient.Test_LoadToFile_01("http://localhost:8080/test_ansi_bad_01.html", @"c:\pib\_dl\_aaaaaaaa\cache.new\test_ansi_bad_01.html");

Test_HttpClient.Test_HttpLog("https://www.google.fr/");
Test_HttpClient.Test_HttpLog("https://www.google.fr/", userAgent: "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0", accept: "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

Test_HttpManager_v5.Test_LoadText_01("https://www.google.fr/", @"c:\pib\_dl\_aaaaaaaa\cache");
Test_HttpManager_v5.Test_LoadText_01("https://www.google.fr", @"c:\pib\_dl\_aaaaaaaa\cache");
Test_HttpClient.Test_LoadToFile_01("https://www.google.fr", @"c:\pib\_dl\_aaaaaaaa\cache\www.google.fr_01.html");
Test_HttpClient.Test_Upload_02("http://localhost:8080/upload", "avatar", @"c:\pib\_dl\Document-page-015_01.jpg", "someParam", "someValue");

Test_HttpClient.Test_LoadText_02(HttpMethod.Get, "http://toto.toto");
Test_HttpClient.Test_LoadText_02(HttpMethod.Get, "https://httpbin.org/get");
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post");
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", new StringContent("toto,tata"));
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", new StringContent("toto,tata", Encoding.Default, "text/plain"));
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", Test_HttpClient.CreateFormUrlEncodedContent("toto", "tata", "zozo", "zaza"));
//Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", Test_HttpClient.CreateMultipartFormDataContent("data", "toto", "tata", "zozo", "zaza"));
//Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", Test_HttpClient.CreateMultipartFormDataContentFile("data", "image", @"c:\pib\_dl\Document-page-015_01.jpg", "toto", "tata", "zozo", "zaza"));
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", Test_HttpClient.CreateMultipartFormDataContent("data").zAddStringContent("toto", "tata", "zozo", "zaza"));
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", Test_HttpClient.CreateMultipartFormDataContent("data").zAddStringContent("toto", "tata", "zozo", "zaza").zAddUploadFile("image", @"c:\pib\_dl\Document-page-015_01.jpg"));
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", Test_HttpClient.CreateMultipartFormDataContent("data").zAddStringContent("toto", "tata", "zozo", "zaza").zAddUploadFile("image1", @"c:\pib\_dl\Document-page-015_01.jpg").zAddUploadFile("image2", @"c:\pib\_dl\Document-page-015_02.jpg"));

Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "http://localhost:8080/post", Test_HttpClient.CreateMultipartFormDataContent("data").zAddStringContent("toto", "tata", "zozo", "zaza"));
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "http://localhost:8080/post", Test_HttpClient.CreateMultipartFormDataContent("data").zAddStringContent("toto", "tata", "zozo", "zaza").zAddUploadFile("image", @"c:\pib\_dl\Document-page-015_01.jpg"));
Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "http://localhost:8080/post", Test_HttpClient.CreateMultipartFormDataContent("data").zAddStringContent("toto", "tata", "zozo", "zaza").zAddUploadFile("image1", @"c:\pib\_dl\Document-page-015_01.jpg").zAddUploadFile("image2", @"c:\pib\_dl\Document-page-015_02.jpg"));

Test_HttpManager_v5.Test_LoadText_01(new HttpRequest_v3 { Url = "https://httpbin.org/get" }, @"c:\pib\_dl\_aaaaaaaa\cache");
Test_HttpManager_v5.Test_LoadText_01(new HttpRequest_v3 { Method = HttpRequestMethod.Post, Url = "https://httpbin.org/post",
  Content = new TextContent("toto,tata,zozo,zaza") }, @"c:\pib\_dl\_aaaaaaaa\cache");
Test_HttpManager_v5.Test_LoadText_01(new HttpRequest_v3 { Method = HttpRequestMethod.Post, Url = "https://httpbin.org/post",
  Content = new UrlEncodedContent("toto", "tata", "zozo", "zaza") }, @"c:\pib\_dl\_aaaaaaaa\cache");
Test_HttpManager_v5.Test_LoadText_01(new HttpRequest_v3 { Method = HttpRequestMethod.Post, Url = "https://httpbin.org/post",
  Content = new MultipartContents("data", new TextContent("tata") { Name = "toto" }, new TextContent("zaza") { Name = "zozo" }) }, @"c:\pib\_dl\_aaaaaaaa\cache");
Test_HttpManager_v5.Test_LoadText_01(new HttpRequest_v3 { Method = HttpRequestMethod.Post, Url = "https://httpbin.org/post",
  Content = new MultipartContents("data", new TextContent("tata") { Name = "toto" }, new TextContent("zaza") { Name = "zozo" },
  new FileContent(@"c:\pib\_dl\Document-page-015_01.jpg") { Name = "image1" }) }, @"c:\pib\_dl\_aaaaaaaa\cache");
Test_HttpManager_v5.Test_LoadText_01(new HttpRequest_v3 { Method = HttpRequestMethod.Post, Url = "https://httpbin.org/post",
  Content = new MultipartContents("data", new TextContent("tata") { Name = "toto" }, new TextContent("zaza") { Name = "zozo" },
  new FileContent(@"c:\pib\_dl\Document-page-015_01.jpg") { Name = "image1" }, new FileContent(@"c:\pib\_dl\Document-page-015_02.jpg") { Name = "image2" }) },
  @"c:\pib\_dl\_aaaaaaaa\cache");


Test_HttpClient.Test_LoadText_02(HttpMethod.Post, "https://httpbin.org/post", Test_HttpClient.CreateMultipartFormDataContent("data", "toto", "tata", "zozo", "zaza"));


HttpResult<string> result = Test_Http.CreateHttpManager(@"c:\pib\_dl\_aaaaaaaa\cache").LoadText(new HttpRequest { Url = "https://www.google.fr/" });
Trace.WriteLine($"Success {result.Success} LoadFromWeb {result.LoadFromWeb} LoadFromCache {result.LoadFromCache}");
Trace.WriteLine(result.Data.Substring(0, 100));
Test_Http.Test_LoadText("https://www.google.fr/", cacheDirectory: @"c:\pib\_dl\_aaaaaaaa\cache");
Test_Http.Test_LoadText("http://localhost:8080/test_utf8_01.html", cacheDirectory: @"c:\pib\_dl\_aaaaaaaa\cache");
Test_Http.Test_LoadText("http://localhost:8080/test_utf8_bad_01.html", cacheDirectory: @"c:\pib\_dl\_aaaaaaaa\cache");
Test_Http.Test_LoadText("http://localhost:8080/test_ansi_01.html", cacheDirectory: @"c:\pib\_dl\_aaaaaaaa\cache");
Test_Http.Test_LoadText("http://localhost:8080/test_ansi_bad_01.html", cacheDirectory: @"c:\pib\_dl\_aaaaaaaa\cache");

HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://www.google.fr/");
Trace.WriteLine($"{(request.Headers.Accept == null ? "null" : "not null")} \"{request.Headers.Accept}\" {request.Headers.Accept.Count}");

//*************************************************************************************************************************
//****                                   Test_Async
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\_pb\_Test\Test_01.project.xml");

Test_Async.Test_Exception();



//*************************************************************************************************************************
//****                                   Test_HttpRun.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_HttpRun.project.xml");


List<string> list = new List<string> { "toto", "tata", "tutu" };
list.zToStringValues().zTrace();


//*************************************************************************************************************************
//****                                   Test_Reflection.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Reflection\_Test\Test_Reflection.project.xml");

Test_Reflection.Test_Reflection_04();


//*************************************************************************************************************************
//****                                   Test.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\_pb\_Test\Test.project.xml");

Test_Async.Test_Exception();         // exception is not catched
await Test_Async.Test_Exception();   // exception is catched
Test_Async.Test_CatchException();    // exception is catched
Test_Async.Test_Try_01();            // exception is catched
Test_Async.Test_Try_02();            // exception is catched


//*************************************************************************************************************************
//****                                   Test_OcrWebService.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Web\Data\Ocr\_Test\Test_OcrWebService.project.xml");

// c:\pib\_dl\_aaaaaaaa\ocr_cache
var response = await Test_OcrWebService.CreateOcrWebService().AccountInfo();
response.zTraceJson();

OcrRequest request = new OcrRequest { DocumentFile = @"c:\pib\_dl\Document-page-015_01.jpg", Language = "french,english", OutputFormat = "txt", GetText = true, GetWords = true };
var response = await Test_OcrWebService.CreateOcrWebService().ProcessDocument(request);
response.zTraceJson();

Test_OcrWebService2.Test_AccountInformation();
Test_OcrWebService2.Test_ProcessDocument(@"c:\pib\_dl\Document-page-015_01.jpg");

Test_OcrWebService2.Test_Authorization();
Test_OcrWebService.CreateOcrWebService().Test_Authorization();

//*************************************************************************************************************************
//****                                   Test_Ghostscript.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Data\Pdf\_Test\Test_Ghostscript.project.xml");


Test_Ghostscript.Test_PdfToImages(@"c:\pib\drive\google\valentin\S1\UE1\UE1-02-lipides\UE1-02-lipides.pdf", @"c:\pib\drive\google\valentin\S1\UE1\UE1-02-lipides\data\images");
Test_Ghostscript.Test_PdfToImages(@"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\UE1-01-acide aminé.pdf", @"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\data\images");



//*************************************************************************************************************************
//****                                   Test_Text.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Text\_Test\Test_Text.project.xml");

Test_Regex.Test(@"(?<=^|,)\s*([0-9]+)(?:\s*-\s*([0-9]+))?\s*(?=,|$)", "1,3,5-12");
Test_Regex.Test(@"(?<=^|,)\s*([0-9]+)(?:\s*-\s*([0-9]+))?\s*(?=,|$)", " 1 , 3 , 5 - 12 ");

//*************************************************************************************************************************
//****                                   Test_iText.project.xml
//****                                   Test_iText7.project.xml
//****                                   Test_PdfScript.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_iText.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_iText7.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_PdfScript.project.xml");


string file = @"c:\pib\drive\google\valentin\S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation\data\images\UE3 - Etat de la matiere et leur caracterisaion Etat d agregation-page-010.jpg";
string file = @"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\data\images2\page-001.jpeg";
string file = @"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\data\images2\01\page-001.jpeg";
Image image = zimg.LoadImageFromFile(file);
Trace.WriteLine($"width {image.Width} height {image.Height} PixelFormat {image.PixelFormat} x dpi {image.HorizontalResolution} y dpi {image.VerticalResolution}");

//*************************************************************************************************************************
//****                                   Test_iTextSharp.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Data\Pdf\_Test\Test_iTextSharp.project.xml");

Test_iTextSharp.Test_TraceObjects(@"c:\pib\drive\google\valentin\S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation.pdf");
Test_iTextSharp.Test_TraceInfos(@"c:\pib\drive\google\valentin\S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation.pdf");
Test_iTextSharp.Test_TraceImages(@"c:\pib\drive\google\valentin\S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation.pdf");
Test_iTextSharp.Test_ExtractImages(@"c:\pib\drive\google\valentin\S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation.pdf", @"data\images2");

Test_iTextSharp.Test_TraceImages(@"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\UE1-01-acide aminé.pdf");
Test_iTextSharp.ExtractImagesFromPDF(@"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\UE1-01-acide aminé.pdf", @"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\data\images2");

Test_iTextSharp.Test_ExtractImages_v2(@"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\UE1-01-acide aminé.pdf", @"data\images2");
Test_iTextSharp.Test_ExtractImages_v2(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\UE5-01-anatomie.pdf", @"data\images2");
Test_iTextSharp.Test_ExtractImages_v2(@"c:\pib\drive\google\valentin\S2\UE5\UE5-02-app. locomoteur osteologie\UE5-02-app. locomoteur osteologie.pdf", @"data\images2");

Test_iTextSharp.Test_ExtractImage(@"c:\pib\_dl\dev\test_pdf\UE5-01-anatomie.pdf", 7, "image-008.bin");


//*************************************************************************************************************************
//****                                   Test_iText7.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Data\Pdf\_Test\Test_iText7.project.xml");

Test_iText7.Test_01(@"c:\pib\_dl\dev\test_pdf\UE5-01-anatomie.pdf", "objects");
Test_iText7.Test_TraceObjects(@"c:\pib\_dl\dev\test_pdf\UE5-01-anatomie.pdf");
Test_iText7.Test_ExtractImage(@"c:\pib\_dl\dev\test_pdf\UE5-01-anatomie.pdf", 7, "image-008");
Test_iText7.Test_ExtractImages(@"c:\pib\_dl\dev\test_pdf\UE5-01-anatomie.pdf", "UE5-01-anatomie");
Test_iText7.Test_ExtractImages(@"c:\pib\_dl\dev\test_pdf\UE1-01-acide aminé.pdf", "UE1-01-acide aminé");

//*************************************************************************************************************************
//****                                   Test_Image.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Data\_Test\Test_Image.project.xml");

Test_Image_01.Test_01(@"c:\pib\_dl\test.png");
Test_Image_01.Test_02(@"c:\pib\_dl\dev\test_image\test_02.bmp");
// 319 x 312 - 361 x 364
Test_Image_01.CopyBytes(@"c:\pib\_dl\dev\test_image\page-001.jpeg.bin", 2514, 317, 310, 363, 366, @"c:\pib\_dl\dev\test_image\page-001_01.bin");
Test_Image_01.CreateBitmap(@"c:\pib\_dl\dev\test_image\page-001_01.bin", 47, 57, @"c:\pib\_dl\dev\test_image\page-001_01.bmp");
Test_Image_01.CopyBytes(@"c:\pib\_dl\dev\test_image\page-001.jpeg.bin", 2514, 260, 260, 420, 420, @"c:\pib\_dl\dev\test_image\page-001_02.bin");
Test_Image_01.CreateBitmap(@"c:\pib\_dl\dev\test_image\page-001_02.bin", 161, 161, @"c:\pib\_dl\dev\test_image\page-001_02.bmp");
Test_Image_01.SaveBitmapData(@"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.png", @"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.bin");
Test_Image_01.ConvertToGif(@"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.png", @"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif");
Test_Image_01.BitmapInfo(@"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.png");
Test_Image_01.BitmapInfo(@"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif");
Test_Image_01.SaveBitmapData(@"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif", @"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif.bin");
Test_Image_01.ReplaceData(@"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif.bin", @"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif_02.bin");
Test_Image_01.CreateBitmap(@"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif_02.bin", 2514, 3474, @"c:\pib\_dl\dev\test_image\UE5-01-anatomie-000.gif_02.bmp");

Test_Image_01.Test_03(@"c:\pib\_dl\dev\test_image\page-001.jpeg.bin", 2514, 317, 310, 363, 366, @"c:\pib\_dl\dev\test_image\page-001_01.bmp");


Image image = Image.FromFile(@"c:\pib\drive\google\valentin\S1\UE1\UE1-01-acide aminé\data\images\page-014.jpg");
Image image = Image.FromFile(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015.png");
Image image = Image.FromFile(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015_01.png");
Image image = Image.FromFile(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015_02.png");
Trace.WriteLine($"Width {image.Width} Height {image.Height} PixelFormat {image.PixelFormat} HorizontalResolution {image.HorizontalResolution} VerticalResolution {image.VerticalResolution}");

Test_Image_01.Crop_v3(Image.FromFile(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015.png"), new Rectangle(0, 0, 2514, 3474 / 2))
  .Save(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015_01.png", ImageFormat.Png);
Test_Image_01.Crop_v3(Image.FromFile(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015.png"), new Rectangle(0, 3474 / 2, 2514, 3474 / 2))
  .Save(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015_02.png", ImageFormat.Png);

string inputFile = @"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015.png";
//string outputFile = @"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015_01.png";
string outputFile = @"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015_02.png";
//Rectangle rectangle = new Rectangle(0, 0, 2514, 3474 / 2);
Rectangle rectangle = new Rectangle(0, 3474 / 2, 2514, 3474 / 2);
using (Image image = Image.FromFile(inputFile))
{
  Bitmap bitmap = zimg.Crop(image, rectangle);
  bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
  bitmap.Save(outputFile, ImageFormat.Png);
}


//*************************************************************************************************************************
//****                                   Test_Image_01
//*************************************************************************************************************************

System.Drawing.Image img = System.Drawing.Image.FromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg");
Trace.WriteLine($"Type {img.GetType()} PixelFormat {img.PixelFormat} width {img.Width} height {img.Height}");  // Type System.Drawing.Bitmap PixelFormat Format24bppRgb width 1240 height 1754
System.Drawing.Image img = System.Drawing.Image.FromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015_01.jpg");
Trace.WriteLine($"Type {img.GetType()} PixelFormat {img.PixelFormat} width {img.Width} height {img.Height}");  // Type System.Drawing.Bitmap PixelFormat Format24bppRgb width 1240 height 1754

System.Drawing.Image img = zimg.LoadImageFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg");
Trace.WriteLine($"Type {img.GetType()} PixelFormat {img.PixelFormat} width {img.Width} height {img.Height}");  // Type System.Drawing.Bitmap PixelFormat Format24bppRgb width 1240 height 1754

System.Drawing.Image img = zimg.LoadBitmapFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg");
Trace.WriteLine($"Type {img.GetType()} PixelFormat {img.PixelFormat} width {img.Width} height {img.Height}");  // Type System.Drawing.Bitmap PixelFormat Format24bppRgb width 1240 height 1754

pb.Data.Test.Test_Image_01.Crop_v1((System.Drawing.Bitmap)zimg.LoadImageFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg"), 0, 0, 1240, 1754 / 2)
  .Save(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015_01.jpg");
pb.Data.Test.Test_Image_01.Crop_v1((System.Drawing.Bitmap)zimg.LoadImageFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg"), 10, 10, 10, 10)
  .Save(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015_01.jpg");

// from   : Type System.Drawing.Bitmap PixelFormat Format24bppRgb width 1240 height 1754
//          Format24bppRgb  24 bits per pixel; 8 bits each are used for the red, green, and blue 
//          Format32bppArgb 32 bits per pixel; 8 bits each are used for the alpha, red, green, and blue
// result : Type System.Drawing.Bitmap PixelFormat Format32bppArgb width 1240 height 877
Test_Image_01.Crop_v2((Bitmap)zimg.LoadImageFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg"),
  new Rectangle(0, 0, 1240, 1754 / 2))
  .Save(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015_01.jpg", ImageFormat.Jpeg);
  
var img = Test_Image_01.Crop_v2((Bitmap)zimg.LoadImageFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg"),
  new Rectangle(0, 0, 1240, 1754 / 2));
img.RotateFlip(RotateFlipType.Rotate90FlipNone);
img.Save(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015_01.jpg", ImageFormat.Jpeg);
var img = Test_Image_01.Crop_v2((Bitmap)zimg.LoadImageFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg"),
  new Rectangle(0, 1754 / 2, 1240, 1754 / 2));
img.RotateFlip(RotateFlipType.Rotate90FlipNone);
img.Save(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015_02.jpg", ImageFormat.Jpeg);

pb.Data.Test.Test_Image_01.Crop_v3((System.Drawing.Bitmap)zimg.LoadImageFromFile(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015.jpg"),
  new System.Drawing.Rectangle(0, 0, 1500, 1754))
  .Save(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images\Document-page-015_test_02.jpg", ImageFormat.Jpeg);

















Trace.WriteLine($@"{TimeSpan.FromMilliseconds(10129):hh\:mm\:ss\.fff}");
Trace.WriteLine($@"{TimeSpan.FromMilliseconds(10000):hh\:mm\:ss}");
Trace.WriteLine(@"{0:hh\:mm\:ss\:ff}", TimeSpan.FromMilliseconds(10127));


//*************************************************************************************************************************
//****                                                  RunSourceCommand.UpdateVSProject
//*************************************************************************************************************************

// simulate runsource.irunsource.project.xml
RunSourceCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);
// simulate runsource.dll.project.xml
RunSourceCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);
// simulate runsource.command.project.xml
RunSourceCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);
// simulate runsource.runsource.project.xml
RunSourceCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.runsource\runsource.runsource.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);
// simulate runsource.launch.project.xml
RunSourceCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.launch\runsource.launch.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);

// simulate runsource.launch.project.xml
RunSourceCommand.UpdateVSProject(@"$Root$\Apps\pbc\pbc.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);

RunSourceCommand.UpdateVSProject(@"$Root$\Apps\tools\test_vs_project\anki.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);
RunSourceCommand.UpdateVSProject(@"$Root$\Apps\tools\test_vs_project\anki.project.xml", options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference);
