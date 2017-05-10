// $$info.test.regex
// $$info.VSProject
// $$info.test.project

//*************************************************************************************************************************
//****                                                  anki.project.xml
//*************************************************************************************************************************

RunSourceCommand.SetProjectFromSource();
//RunSourceCommand.SetProject(@"$Root$\Apps\tools\tv\_Test\Test_TV.project.xml");
Trace.WriteLine("toto");

RunSourceCommand.CompileProject("AnkiJS.project.xml");

// S2-UE5                     - reste  69 pages
// S1-UE1                     - reste 165 pages
// S1-UE2                     - reste 315 pages
// S1-UE3                     - reste 105 pages
// S1-UE7                     - reste  86 pages
// total                      - reste 740 pages

// S1-UE1                     - reste 165 pages
// S1-UE1-bio                 - reste 107 pages
// S1-UE1-chimie              - reste  58 pages

// S1-UE2                     - reste 315 pages
// S1-UE2-bio repro           - reste  52 pages
// S1-UE2-bio cell            - reste 104 pages
// S1-UE2-histo               - reste  54 pages

// S1-UE3                     - reste 105 pages

// S1-UE7                     - reste  86 pages
// S1-UE7-droit               - reste  26 pages
// S1-UE7-économie            - reste  32 pages
// S1-UE7-psycho              - reste  12 pages
// S1-UE7-socio               - reste  16 pages

// S1-UE1-bio - ***********************************************************************************************************************************************

// S1-UE1-bio-01-acide aminé - scanned - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-01-acide aminé").SetParameters(new QuestionsParameters { PageRange = "14-26" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-01-acide aminé").GetParameters().zTraceJson();
//QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-01-acide aminé").ExtractImagesFromPdf();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-01-acide aminé").Scan(simulate: true);
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-01-acide aminé").Scan(imageScan: true);

// S1-UE1-bio-02-lipides - scanned - 25 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-02-lipides").SetParameters(new QuestionsParameters { PageRange = "18-42" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-02-lipides").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-02-lipides").Scan(range: "18-32", imageScan: true);
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-02-lipides").Scan(range: "33-42", imageScan: true);

// S1-UE1-bio-03-proteines - scanned - 20 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-03-proteines").SetParameters(new QuestionsParameters { PageRange = "14-33" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-03-proteines").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-03-proteines").Scan(range: "14-28", imageScan: true);
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-03-proteines").Scan(range: "29-33", imageScan: true);

// S1-UE1-bio-04-enzymologie - scanned - 27 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-04-enzymologie").SetParameters(new QuestionsParameters { PageRange = "13-39" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-04-enzymologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-04-enzymologie").Scan(range: "13-32", imageScan: true);
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-04-enzymologie").Scan(range: "33-39", imageScan: true);

// S1-UE1-05-glucides - pas de questions

// S1-UE1-bio-06-chaine oxydative - scanned - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-06-chaine oxydative").SetParameters(new QuestionsParameters { PageRange = "14-17" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-06-chaine oxydative").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-06-chaine oxydative").Scan();

// S1-UE1-bio-07-bio-acides nucléiques - scanned - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-07-bio-acides nucléiques").SetParameters(new QuestionsParameters { PageRange = "09-13" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-07-bio-acides nucléiques").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-07-bio-acides nucléiques").Scan();

// S1-UE1-bio-08-bio-réplication - scanned - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-08-bio-réplication").SetParameters(new QuestionsParameters { PageRange = "07-11" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-08-bio-réplication").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-08-bio-réplication").Scan();

// S1-UE1-bio-09-bio-génome - scanned - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-09-bio-génome").SetParameters(new QuestionsParameters { PageRange = "12-17" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-09-bio-génome").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-09-bio-génome").Scan();

// S1-UE1-bio-10-bio-traduction-réparation - scanned - 11 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-10-bio-traduction-réparation").SetParameters(new QuestionsParameters { PageRange = "09-19" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-10-bio-traduction-réparation").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-10-bio-traduction-réparation").Scan();

// S1-UE1-bio-11-glucides 2 - scanned - 26 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-11-glucides 2").SetParameters(new QuestionsParameters { PageRange = "06-31" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-11-glucides 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-11-glucides 2").Scan(range: "06-19", imageScan: true);
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-11-glucides 2").Scan(range: "20-20", imageScan: true);
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-11-glucides 2").Scan(range: "21-31", imageScan: true);

// S1-UE1-bio-12-métabolisme genéral - scanned - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-12-métabolisme genéral").SetParameters(new QuestionsParameters { PageRange = "11-15" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-12-métabolisme genéral").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-12-métabolisme genéral").Scan();

// S1-UE1-bio-13-lipides - scanned - scanned - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-13-lipides").SetParameters(new QuestionsParameters { PageRange = "09-12" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-13-lipides").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-13-lipides").Scan();

// S1-UE1-bio-14-glucides - scanned - scanned - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-14-glucides").SetParameters(new QuestionsParameters { PageRange = "10-14" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-14-glucides").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-14-glucides").Scan(range: "10-13");
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-14-glucides").Scan(range: "14-14");

// S1-UE1-bio-15-acides aminés-urée - scanned - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-15-acides aminés-urée").SetParameters(new QuestionsParameters { PageRange = "07-10" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-15-acides aminés-urée").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-bio\UE1-bio-15-acides aminés-urée").Scan();


// S1-UE1-chimie - ********************************************************************************************************************************************

// S1-UE1-chimie-01-atomistique - scanned - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-01-atomistique").SetParameters(new QuestionsParameters { PageRange = "10-18" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-01-atomistique").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-01-atomistique").Scan();

// S1-UE1-chimie-02-nomenclature - scanned - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-02-nomenclature").SetParameters(new QuestionsParameters { PageRange = "05-08" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-02-nomenclature").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-02-nomenclature").Scan();

// S1-UE1-chimie-03-stéreochimie - 10 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-03-stéreochimie").SetParameters(new QuestionsParameters { PageRange = "08-17" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-03-stéreochimie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-03-stéreochimie").Scan(range: "08-14");
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-03-stéreochimie").Scan(range: "05-17");

// S1-UE1-chimie-04-effets electroniques - 8 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-04-effets electroniques").SetParameters(new QuestionsParameters { PageRange = "06-13" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-04-effets electroniques").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-04-effets electroniques").Scan();

// S1-UE1-chimie-05-mécanismes réactionnels - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-05-mécanismes réactionnels").SetParameters(new QuestionsParameters { PageRange = "07-10" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-05-mécanismes réactionnels").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-05-mécanismes réactionnels").Scan();

// S1-UE1-chimie-06-chaines hydrocarbonnées - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-06-chaines hydrocarbonnées").SetParameters(new QuestionsParameters { PageRange = "21-26" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-06-chaines hydrocarbonnées").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-06-chaines hydrocarbonnées").Scan();

// S1-UE1-chimie-07-thermodynamie - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-07-thermodynamie").SetParameters(new QuestionsParameters { PageRange = "07-10" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-07-thermodynamie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-07-thermodynamie").Scan();

// S1-UE1-chimie-08-fonctions monovalentes - 14 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-08-fonctions monovalentes").SetParameters(new QuestionsParameters { PageRange = "21-34" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-08-fonctions monovalentes").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-08-fonctions monovalentes").Scan();

// S1-UE1-chimie-09-fonctions divalentes - 7 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-09-fonctions divalentes").SetParameters(new QuestionsParameters { PageRange = "12-18" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-09-fonctions divalentes").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-09-fonctions divalentes").Scan();

// S1-UE1-chimie-10-fonctions trivalentes - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-10-fonctions trivalentes").SetParameters(new QuestionsParameters { PageRange = "09-14" });
QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-10-fonctions trivalentes").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE1\UE1-chimie\UE1-chimie-10-fonctions trivalentes").Scan();


// S1-UE2-bio repro - *****************************************************************************************************************************************

// S1-UE2-bio repro-01-méiose gamatogénèse - 7 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-01-méiose gamatogénèse").SetParameters(new QuestionsParameters { PageRange = "04-10" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-01-méiose gamatogénèse").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-01-méiose gamatogénèse").Scan();

// S1-UE2-bio repro-01-méiose gamatogénèse 2 - 11 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-01-méiose gamatogénèse 2").SetParameters(new QuestionsParameters { PageRange = "08-18" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-01-méiose gamatogénèse 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-01-méiose gamatogénèse 2").Scan();

// S1-UE2-bio repro-02-fécondation - 15 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-02-fécondation").SetParameters(new QuestionsParameters { PageRange = "09-23" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-02-fécondation").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-02-fécondation").Scan();

// S1-UE2-bio repro-03-dev embryonnaire - 14 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-03-dev embryonnaire").SetParameters(new QuestionsParameters { PageRange = "07-20" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-03-dev embryonnaire").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-03-dev embryonnaire").Scan();

// S1-UE2-bio repro-04-gastrulation - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-04-gastrulation").SetParameters(new QuestionsParameters { PageRange = "05-09" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-04-gastrulation").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-04-gastrulation").Scan();

// S1-UE2-bio repro-05-3ème 4ème sem dev - 11 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-05-3ème 4ème sem dev").SetParameters(new QuestionsParameters { PageRange = "11-21" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-05-3ème 4ème sem dev").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-05-3ème 4ème sem dev").Scan();

// S1-UE2-bio repro-06-embryologie - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-06-embryologie").SetParameters(new QuestionsParameters { PageRange = "06-10" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-06-embryologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio repro\UE2-bio repro-06-embryologie").Scan();


// S1-UE2-bio cell - ******************************************************************************************************************************************

// S1-UE2-bio cell-01-cellule - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-01-cellule").SetParameters(new QuestionsParameters { PageRange = "05-09" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-01-cellule").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-01-cellule").Scan();


// S1-UE2-bio cell-02-compartimentation - 13 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-02-compartimentation").SetParameters(new QuestionsParameters { PageRange = "09-21" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-02-compartimentation").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-02-compartimentation").Scan();

// S1-UE2-bio cell-03-membrane trafic - 24 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-03-membrane trafic").SetParameters(new QuestionsParameters { PageRange = "10-33" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-03-membrane trafic").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-03-membrane trafic").Scan();

// S1-UE2-bio cell-04-chromosomes caryotype - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-04-chromosomes caryotype").SetParameters(new QuestionsParameters { PageRange = "06-09" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-04-chromosomes caryotype").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-04-chromosomes caryotype").Scan();

// S1-UE2-bio cell-05-cytosquelette - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-05-cytosquelette").SetParameters(new QuestionsParameters { PageRange = "08-16" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-05-cytosquelette").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-05-cytosquelette").Scan();

// S1-UE2-bio cell-06-énergie - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-06-énergie").SetParameters(new QuestionsParameters { PageRange = "08-16" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-06-énergie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-06-énergie").Scan();

// S1-UE2-bio cell-07-cytosol - 10 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-07-cytosol").SetParameters(new QuestionsParameters { PageRange = "06-15" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-07-cytosol").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-07-cytosol").Scan();

// S1-UE2-bio cell-08-noyau - 15 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-08-noyau").SetParameters(new QuestionsParameters { PageRange = "10-24" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-08-noyau").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-08-noyau").Scan();

// S1-UE2-bio cell-09-surface adhérence - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-09-surface adhérence").SetParameters(new QuestionsParameters { PageRange = "08-16" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-09-surface adhérence").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-09-surface adhérence").Scan();

// S1-UE2-bio cell-10-membrane plasmique - 10 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-10-membrane plasmique").SetParameters(new QuestionsParameters { PageRange = "07-16" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-10-membrane plasmique").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-10-membrane plasmique").Scan();

// S1-UE2-bio cell-11-matrice extracellulaire - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-11-matrice extracellulaire").SetParameters(new QuestionsParameters { PageRange = "06-14" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-11-matrice extracellulaire").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-11-matrice extracellulaire").Scan();

// S1-UE2-bio cell-12-comunication intercellulaire - 13 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-12-comunication intercellulaire").SetParameters(new QuestionsParameters { PageRange = "10-22" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-12-comunication intercellulaire").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-12-comunication intercellulaire").Scan();

// S1-UE2-bio cell-13-apoptose - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-13-apoptose").SetParameters(new QuestionsParameters { PageRange = "04-08" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-13-apoptose").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-13-apoptose").Scan();

// S1-UE2-bio cell-14-cellules tissus - 7 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-14-cellules tissus").SetParameters(new QuestionsParameters { PageRange = "06-12" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-14-cellules tissus").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-14-cellules tissus").Scan();

// S1-UE2-bio cell-15-mitose - 17 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-15-mitose").SetParameters(new QuestionsParameters { PageRange = "08-24" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-15-mitose").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-15-mitose").Scan();

// S1-UE2-bio cell-16-différenciation cellulaire - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-16-différenciation cellulaire").SetParameters(new QuestionsParameters { PageRange = "05-08" });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-16-différenciation cellulaire").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-bio cell\UE2-bio cell-16-différenciation cellulaire").Scan();


// S1-UE2-histo - *********************************************************************************************************************************************

// S1-UE2-histo-01-épithéliums - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-01-épithéliums").SetParameters(new QuestionsParameters { PageRange = "23-28", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-01-épithéliums").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-01-épithéliums").Scan();

// S1-UE2-histo-02-tissus conjonctifs - pas de question

// S1-UE2-histo-02-tissus conjonctifs 2 - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-02-tissus conjonctifs 2").SetParameters(new QuestionsParameters { PageRange = "09-12", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-02-tissus conjonctifs 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-02-histo-tissus conjonctifs 2").Scan();

// S1-UE2-histo-03-tissu nerveux - 7 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-03-tissu nerveux").SetParameters(new QuestionsParameters { PageRange = "22-28", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-03-tissu nerveux").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-03-tissu nerveux").Scan();

// S1-UE2-histo-04-tissu musculaire - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-04-tissu musculaire").SetParameters(new QuestionsParameters { PageRange = "18-22", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-04-tissu musculaire").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-04-tissu musculaire").Scan();

// S1-UE2-histo-05-tissu osseux - 3 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-05-tissu osseux").SetParameters(new QuestionsParameters { PageRange = "15-17", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-05-tissu osseux").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-05-tissu osseux").Scan();

// S1-UE2-histo-06-histologie embryologie - 19 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-06-histologie embryologie").SetParameters(new QuestionsParameters { PageRange = "26-44", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-06-histologie embryologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-06-histologie embryologie").Scan();

// S1-UE2-histo-07-histologie - 10 pages
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-07-histologie").SetParameters(new QuestionsParameters { PageRange = "15-24", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-histo-07-histologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE2\UE2-histo\UE2-07-histologie").Scan();


// S1-UE3 - ***************************************************************************************************************************************************

// S1-UE3-01-force énergie - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-01-force énergie").SetParameters(new QuestionsParameters { PageRange = "09-14" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-01-force énergie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-01-force énergie").Scan();

// S1-UE3-02-électricité - 7 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-02-électricité").SetParameters(new QuestionsParameters { PageRange = "11-17" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-02-électricité").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-02-électricité").Scan();

// S1-UE3-03-ondes optique - 8 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-03-ondes optique").SetParameters(new QuestionsParameters { PageRange = "07-14" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-03-ondes optique").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-03-ondes optique").Scan();

// S1-UE3-04-imagerie radioprotection - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-04-imagerie radioprotection").SetParameters(new QuestionsParameters { PageRange = "10-15" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-04-imagerie radioprotection").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-04-imagerie radioprotection").Scan();

// S1-UE3-05-ondes optique - 7 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-05-ondes optique").SetParameters(new QuestionsParameters { PageRange = "09-15" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-05-ondes optique").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-05-ondes optique").Scan();

// S1-UE3-06-radioactivité - 8 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-06-radioactivité").SetParameters(new QuestionsParameters { PageRange = "07-14" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-06-radioactivité").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-06-radioactivité").Scan();

// S1-UE3-07-rayonnements - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-07-rayonnements").SetParameters(new QuestionsParameters { PageRange = "09-17" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-07-rayonnements").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-07-rayonnements").Scan();

// S1-UE3-08-radiobiologie - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-08-radiobiologie").SetParameters(new QuestionsParameters { PageRange = "07-11" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-08-radiobiologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-08-radiobiologie").Scan();

// S1-UE3-09-fluides - 10 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-09-fluides").SetParameters(new QuestionsParameters { PageRange = "07-16" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-09-fluides").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-09-fluides").Scan();

// S1-UE3-10-rmn irm - 10 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-10-rmn irm").SetParameters(new QuestionsParameters { PageRange = "10-19" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-10-rmn irm").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-10-rmn irm").Scan();

// S1-UE3-11-homéostasie - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-11-homéostasie").SetParameters(new QuestionsParameters { PageRange = "05-09" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-11-homéostasie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-11-homéostasie").Scan();

// S1-UE3-12-neurophysiologie - 3 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-12-neurophysiologie").SetParameters(new QuestionsParameters { PageRange = "08-10" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-12-neurophysiologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-12-neurophysiologie").Scan();

// S1-UE3-13-homéostasie - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-13-homéostasie").SetParameters(new QuestionsParameters { PageRange = "09-13" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-13-homéostasie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-13-homéostasie").Scan();

// S1-UE3-14-physiologie cardiovasculaire - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-14-physiologie cardiovasculaire").SetParameters(new QuestionsParameters { PageRange = "07-11" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-14-physiologie cardiovasculaire").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-14-physiologie cardiovasculaire").Scan();

// S1-UE3-15-rein - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-15-rein").SetParameters(new QuestionsParameters { PageRange = "08-13" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-15-rein").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-15-rein").Scan();

// S1-UE3-16-neurophysiologie 2 - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-16-neurophysiologie 2").SetParameters(new QuestionsParameters { PageRange = "09-13" });
QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-16-neurophysiologie 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE3\UE3-16-neurophysiologie 2").Scan();

// S1-UE3-17-rayonnements - pas de question


// S1-UE7-droit - *********************************************************************************************************************************************

// S1-UE7-droit-01-droit - 2 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-01-droit").SetParameters(new QuestionsParameters { PageRange = "08-09", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-01-droit").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-01-droit").Scan();

// S1-UE7-droit-02-justice - 9 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-02-justice").SetParameters(new QuestionsParameters { PageRange = "14-22" });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-02-justice").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-02-justice").Scan();

// S1-UE7-droit-03-justice normes - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-03-justice normes").SetParameters(new QuestionsParameters { PageRange = "13-16", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-03-justice normes").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-03-justice normes").Scan();

// S1-UE7-droit-04-normes malade - 6 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-04-normes malade").SetParameters(new QuestionsParameters { PageRange = "14-19" });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-04-normes malade").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-04-normes malade").Scan();

// S1-UE7-droit-05-malade - 5 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-05-malade").SetParameters(new QuestionsParameters { PageRange = "18-22", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-05-malade").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-droit\UE7-droit-05-malade").Scan();


// S1-UE7-économie - ******************************************************************************************************************************************

// S1-UE7-économie-01-croissance - 3 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-01-croissance").SetParameters(new QuestionsParameters { PageRange = "13-15", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-01-croissance").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-01-croissance").Scan();

// S1-UE7-économie-02-croissance 2 - 12 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-02-croissance 2").SetParameters(new QuestionsParameters { PageRange = "14-25" });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-02-croissance 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-02-croissance 2").Scan();

// S1-UE7-économie-03-croissance 3 - 13 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-03-croissance 3").SetParameters(new QuestionsParameters { PageRange = "17-29" });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-03-croissance 3").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-03-croissance 3").Scan();

// S1-UE7-économie-04-santé - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-04-santé").SetParameters(new QuestionsParameters { PageRange = "13-16" });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-04-santé").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-économie\UE7-économie-04-santé").Scan();


// S1-UE7-psycho - ********************************************************************************************************************************************

// S1-UE7-psycho-01-médecin malade - 2 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-01-médecin malade").SetParameters(new QuestionsParameters { PageRange = "10-11", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-01-médecin malade").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-01-médecin malade").Scan();

// S1-UE7-psycho-02-dev psychomoteur - pas de question

// S1-UE7-psycho-03-dev psychosexuel - 3 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-03-dev psychosexuel").SetParameters(new QuestionsParameters { PageRange = "11-13", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-03-dev psychosexuel").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-03-dev psychosexuel").Scan();

// S1-UE7-psycho-04-dev enfant - 3 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-04-dev enfant").SetParameters(new QuestionsParameters { PageRange = "12-14", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-04-dev enfant").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-04-dev enfant").Scan();

// S1-UE7-psycho-05-dev affectif - 2 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-05-dev affectif").SetParameters(new QuestionsParameters { PageRange = "11-12", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-05-dev affectif").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-05-dev affectif").Scan();

// S1-UE7-psycho-06-dev enfant migrant - pas de question

// S1-UE7-psycho-07-bébé - 2 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-07-bébé").SetParameters(new QuestionsParameters { PageRange = "08-09", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-07-bébé").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-psycho\UE7-psycho-07-bébé").Scan();


// S1-UE7-socio - *********************************************************************************************************************************************

// S1-UE7-socio-01-sciences sociales - pas de question

// S1-UE7-socio-02-sciences sociales 2 - 4 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-02-sciences sociales 2").SetParameters(new QuestionsParameters { PageRange = "11-14", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-02-sciences sociales 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-02-sciences sociales 2").Scan();

// S1-UE7-socio-03-familles - 2 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-03-familles").SetParameters(new QuestionsParameters { PageRange = "10-11", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-03-familles").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-03-familles").Scan();

// S1-UE7-socio-04-alliances - 3 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-04-alliances").SetParameters(new QuestionsParameters { PageRange = "11-13", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-04-alliances").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-04-alliances").Scan();

// S1-UE7-socio-05-inégalités sociales - 3 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-05-inégalités sociales").SetParameters(new QuestionsParameters { PageRange = "11-13", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-05-inégalités sociales").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-05-inégalités sociales").Scan();

// S1-UE7-socio-06-genre - 2 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-06-genre").SetParameters(new QuestionsParameters { PageRange = "11-12", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-06-genre").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-06-genre").Scan();

// S1-UE7-socio-07-déviance - 2 pages
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-07-déviance").SetParameters(new QuestionsParameters { PageRange = "10-11", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-07-déviance").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S1\UE7\UE7-socio\UE7-socio-07-déviance").Scan();


// S2-UE5 - ***************************************************************************************************************************************************

// S2-UE5-01-anatomie - scanned - 7 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").SetParameters(new QuestionsParameters { PageRange = "15-21", PageColumn = 2, PageRotate = PageRotate.Rotate90, ImagesExtracted = true });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").GetParameters().zTraceJson();
//QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").ExtractImagesFromPdf();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").Scan(range: "15", simulate: true);
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").Scan(range: "15");
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-01-anatomie").Scan(range: "16-21");

// S2-UE5-02-ostéologie - scanned - 6 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").SetParameters(new QuestionsParameters { PageRange = "15-20", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").Scan(range: "15-17");
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").Scan(range: "18");
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-02-ostéologie").Scan(range: "19-20");

// S2-UE5-03-arthrologie - scanned - 4 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-03-arthrologie").SetParameters(new QuestionsParameters { PageRange = "10-13", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-03-arthrologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-03-arthrologie").Scan();

// S2-UE5-05-myologie - scanned - 6 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-05-myologie").SetParameters(new QuestionsParameters { PageRange = "15-20", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-05-myologie").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-05-myologie").Scan();

// S2-UE5-06-colonne vertébrale - scanned - 11 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-06-colonne vertébrale").SetParameters(new QuestionsParameters { PageRange = "24-34", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-06-colonne vertébrale").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-06-colonne vertébrale").Scan();

// S2-UE5-07-région scapulo-humérale - scanned - 9 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-07-région scapulo-humérale").SetParameters(new QuestionsParameters { PageRange = "23-31", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-07-région scapulo-humérale").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-07-région scapulo-humérale").Scan();

// S2-UE5-08-avant-bras et carpe - scanned - 8 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-08-avant-bras et carpe").SetParameters(new QuestionsParameters { PageRange = "26-33", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-08-avant-bras et carpe").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-08-avant-bras et carpe").Scan();

// S2-UE5-09-os coxal fémur - scanned - 10 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-09-os coxal fémur").SetParameters(new QuestionsParameters { PageRange = "23-32", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-09-os coxal fémur").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-09-os coxal fémur").Scan(range: "23-30");
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-09-os coxal fémur").Scan(range: "31-32");

// S2-UE5-10-os jambe pied - scanned - 6 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-10-os jambe pied").SetParameters(new QuestionsParameters { PageRange = "17-22", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-10-os jambe pied").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-10-os jambe pied").Scan();

// S2-UE5-12-app. cardiovasculaire 2 - scanned - 7 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-12-app. cardiovasculaire 2").SetParameters(new QuestionsParameters { PageRange = "18-24", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-12-app. cardiovasculaire 2").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-12-app. cardiovasculaire 2").Scan();

// S2-UE5-13-app. respiratoire - scanned - 12 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-13-app. respiratoire").SetParameters(new QuestionsParameters { PageRange = "27-38", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-13-app. respiratoire").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-13-app. respiratoire").Scan(range: "27-36");
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-13-app. respiratoire").Scan(range: "37-38");

// S2-UE5-14-app. digestif 1 - scanned - 14 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-14-app. digestif 1").SetParameters(new QuestionsParameters { PageRange = "41-54", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-14-app. digestif 1").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-14-app. digestif 1").Scan();

// S2-UE5-16-app. uro-génital - scanned - 9 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-16-app. uro-génital").SetParameters(new QuestionsParameters { PageRange = "27-35", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-16-app. uro-génital").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-16-app. uro-génital").Scan();

// S2-UE5-17-région cervicale - scanned - 7 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-17-région cervicale").SetParameters(new QuestionsParameters { PageRange = "18-24", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-17-région cervicale").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-17-région cervicale").Scan();

// S2-UE5-18-système nerveux - scanned - 3 pages
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-18-système nerveux").SetParameters(new QuestionsParameters { PageRange = "12-14", PageColumn = 2, PageRotate = PageRotate.Rotate90 });
QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-18-système nerveux").GetParameters().zTraceJson();
await QuestionRun.CreateQuestionsManager(@"S2\UE5\UE5-18-système nerveux").Scan();








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
//****                                   $$info.test.project
//*************************************************************************************************************************

int i = 1; i++;
Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_Basic.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RegexValues.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RegexValuesList.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RunSourceExtension.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_MongoExtension.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_MongoSerializer.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_DocumentComparator.project.xml");
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
Test_Regex.Test(@"^\s*(?:correction|annales?\s+classees?\s+corrigees?)\s*$", "  ANNALES CLASSEES CORRIGEES");
Test_Regex.Test(@"^\s*(?:annales?\s+classees?\s+corrigees?)\s*$", "  ANNALES CLASSEES CORRIGEE5");


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

Test_iText7.Test_ExtractText(@"c:\pib\_dl\_dl\_pib\dl\print\.01_quotidien\Journaux\Journaux - 2017-03-26\Le Monde + 2 Suppléments Du Dimanche 26 & Lundi 27 Mars 2017.pdf", 3);

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

string inputFile1 = @"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015-01.png";
string inputFile2 = @"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015-02.png";
string outputFile1 = @"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015-03.png";
string outputFile2 = @"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\images\page-015-04.png";
using (Image image1 = Image.FromFile(inputFile1))
using (Image image2 = Image.FromFile(inputFile2))
{
    zimg.JoinRight(image1, image2).Save(outputFile1, ImageFormat.Png);
    zimg.JoinBottom(image1, image2).Save(outputFile2, ImageFormat.Png);
}



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






//*************************************************************************************************************************
//****                                   Test_TextDataReader.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Data\Text\_Test\Test_TextDataReader.project.xml");

Test_TextDataReader.Test_Anki_01(@"c:\pib\drive\google\valentin\S2\UE5\UE5-01-anatomie\data\scan", limit: 1).zTraceJson();
Test_TextDataReader.Test_Anki_01(@"c:\pib\drive\google\valentin\S2\UE6\UE6-01-Histoire du medicament\data\scan", limit: 1).zTraceJson();
Test_TextDataReader.Test_Anki_01(@"c:\pib\drive\google\valentin\S2\UE6\UE6-01-Histoire du medicament\data\scan", limit: 1).zView();
QuestionTest.Test_ReadQuestionFiles(@"S2\UE6\UE6-01-Histoire du medicament", limit: 1).zView();
QuestionTest.Test_ReadQuestionFiles(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation", limit: 1).zView();
QuestionTest.Test_ReadQuestionFiles(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation").zView();
QuestionTest.Test_ReadQuestionFiles(@"S2\UE8-med\UE8-med-10-physio endocrinienne").zView();
QuestionTest.Test_ReadQuestionFilesAsQuestionData(@"S2\UE6\UE6-01-Histoire du medicament", limit: 1).zView();
QuestionTest.Test_ReadQuestionFilesAsQuestionData_v2(@"S2\UE6\UE6-01-Histoire du medicament").zView();
QuestionTest.Test_ReadQuestionFilesAsQuestionData_v2(@"S2\UE6\UE6-02-Statut des medicaments et autres produits de santé 1").zView();
QuestionTest.Test_ReadQuestionFilesAsQuestionData_v2(@"S2\UE6\UE6-02-Statut des medicaments et autres produits de santé 2").zView();
QuestionTest.Test_ReadQuestionFilesAsQuestionData_v2(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation").zView();

QuestionTest.Test_ReadQuestionFiles(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation").zView();
QuestionTest.Test_ReadQuestionFilesAsQuestionData_v2(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation").zView();
QuestionTest.Test_ReadQuestionFilesAsQuestionData_v2(@"S2\UE8-med\UE8-med-08-transport O2 et CO2").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE6\UE6-01-Histoire du medicament").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE6\UE6-02-Statut des medicaments et autres produits de santé 2").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE6\UE6-05-Production des medicaments").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE3\UE3-06-Transports electriques et transmembranaires").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE8-med\UE8-med-08-transport O2 et CO2").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE8-med\UE8-med-10-physio endocrinienne").zView();
QuestionTest.Test_QuestionReader_v1(@"S2\UE8-med\UE8-med-08-transport O2 et CO2").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE8-med\UE8-med-08-transport O2 et CO2").zView();
QuestionTest.Test_QuestionReader_v2(@"S2\UE3\UE3-05-Propriete colligative et transports membranaires").zView();


QuestionTest.TestUnit_QuestionReader_v1(@"S2\UE8-med\UE8-med-08-transport O2 et CO2", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);
QuestionTest.TestUnit_QuestionReader_v2(@"S2\UE8-med\UE8-med-08-transport O2 et CO2", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);

QuestionTest.TestUnit_QuestionReader_v1(@"S2\UE8\UE8-10-Pathologie moleculaire et genome stabilité et evolution du genome", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);
QuestionTest.TestUnit_QuestionReader_v2(@"S2\UE8\UE8-10-Pathologie moleculaire et genome stabilité et evolution du genome", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);

QuestionTest.TestUnit_QuestionReader_v1(@"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);
QuestionTest.TestUnit_QuestionReader_v2(@"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);


QuestionTest.TestUnit_QuestionReader_v1(@"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);
QuestionTest.TestUnit_QuestionReader_v2(@"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true);
QuestionTest.Test_ValidateFiles(@"c:\pib\dev_data\exe\runsource\valentin\test\questions", "*.questions.json");

QuestionTest.CompareJsonFiles(@"c:\pib\dev_data\exe\runsource\valentin\test\questions", "questions.json",
  elementsToCompare: new string[] { "Year", "Type", "Number", "QuestionText", "Choices", "SourceLine" },
  documentReference: new string[] { "document1.Year", "document1.Number", "document1.SourceFile", "document1.SourceLine", "document2.SourceFile", "document2.SourceLine" });



QuestionTest.TestUnit_ResponseReader_v1(@"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true);
QuestionTest.TestUnit_ResponseReader_v2(@"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true);

QuestionTest.TestUnit_ResponseReader_v1(@"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true);
QuestionTest.TestUnit_ResponseReader_v2(@"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true);
QuestionTest.Test_ValidateFiles(@"c:\pib\dev_data\exe\runsource\valentin\test\responses", "*.responses.json");


TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadQuestions, @"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true, v2: false);
TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadQuestions, @"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true, v2: true);
TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadResponses, @"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true, v2: false);
TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadResponses, @"S2\UE3\UE3-05-Propriete colligative et transports membranaires", @"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true, v2: true);

TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadQuestions, @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true, v2: false);
TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadQuestions, @"c:\pib\dev_data\exe\runsource\valentin\test\questions", traceUnknowValue: true, v2: true);
TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadResponses, @"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true, v2: false);
TestUnit.TestUnit.TestUnit_Reader(TestUnitOpe.ReadResponses, @"c:\pib\dev_data\exe\runsource\valentin\test\responses", traceUnknowValue: true, v2: true);

QuestionTest.CompareJsonFiles(@"c:\pib\dev_data\exe\runsource\valentin\test\responses", "responses.json",
  elementsToCompare: new string[] { "Year", "QuestionNumber", "Responses" },
  documentReference: new string[] { "document1.Year", "document1.QuestionNumber", "document1.SourceFile", "document1.SourceLine" });


QuestionTest.Test_QuestionReader_v2_ToFile(@"S2\UE6\UE6-01-Histoire du medicament");
//QuestionRun.CreateQuestionsManager(@"UE6\UE6-01-Fiche - Histoire du medicament").CreateAllQuestionResponseFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE6\UE6-01-Histoire du medicament").CreateQuestionsFile();


QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation").RenameAndCopyScanFiles(simulate: true);
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-02-Etat de la matiere et leur caracterisation").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-03-Etat de la matiere et leur caracterisation proprietes colligatives").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-04-Gaz eau et solutions aqueuses").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-05-Propriete colligative et transports membranaires").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-06-Transports electriques et transmembranaires").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-07-Activité électrique cardiaque").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-08-Potentiel membranaire de repos potentiel local potentiel d'action").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-09-Regulations des espaces hydriques").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-10-Regulation du bilan hydro-sodé").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-11-Equilibre acido-basique").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-12-Regulation acido-basique").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-13-pH et equilibre acido-basique").RenameAndCopyScanFiles();
QuestionRun.CreateQuestionsManager(@"S2\UE3\UE3-14-Thermoregulation").RenameAndCopyScanFiles();

string[] directories = new string[] {
  @"S2\UE5\UE5-01-anatomie",
  @"S2\UE5\UE5-02-ostéologie",
  @"S2\UE5\UE5-03-arthrologie",
  @"S2\UE5\UE5-05-myologie",
  @"S2\UE5\UE5-06-colonne vertébrale",
  @"S2\UE5\UE5-07-région scapulo-humérale",
  @"S2\UE5\UE5-08-avant-bras et carpe",
  @"S2\UE5\UE5-09-os coxal fémur",
  @"S2\UE5\UE5-10-os jambe pied",
  @"S2\UE5\UE5-12-app. cardiovasculaire 2",
  @"S2\UE5\UE5-13-app. respiratoire",
  @"S2\UE5\UE5-14-app. digestif 1",
  @"S2\UE5\UE5-15-app. digestif 2",
  @"S2\UE5\UE5-16-app. uro-génital",
  @"S2\UE5\UE5-17-région cervicale",
  @"S2\UE5\UE5-18-système nerveux"
  };
string[] directories = new string[] {
  @"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation",
  @"S2\UE3\UE3-02-Etat de la matiere et leur caracterisation",
  @"S2\UE3\UE3-03-Etat de la matiere et leur caracterisation proprietes colligatives",
  @"S2\UE3\UE3-04-Gaz eau et solutions aqueuses",
  @"S2\UE3\UE3-05-Propriete colligative et transports membranaires",
  @"S2\UE3\UE3-06-Transports electriques et transmembranaires",
  @"S2\UE3\UE3-07-Activité électrique cardiaque",
  @"S2\UE3\UE3-08-Potentiel membranaire de repos potentiel local potentiel d'action",
  @"S2\UE3\UE3-09-Regulations des espaces hydriques",
  @"S2\UE3\UE3-10-Regulation du bilan hydro-sodé",
  @"S2\UE3\UE3-11-Equilibre acido-basique",
  @"S2\UE3\UE3-12-Regulation acido-basique",
  @"S2\UE3\UE3-13-pH et equilibre acido-basique",
  @"S2\UE3\UE3-14-Thermoregulation"
  };
QuestionTest.Test_QuestionReader_Comparev1v2(directories, @"c:\pib\drive\google\valentin\_test\compare_v1_v2");
string[] directories = new string[] {
  @"S2\UE6\UE6-01-Histoire du medicament",
  @"S2\UE6\UE6-02-Statut des medicaments et autres produits de santé 1",
  @"S2\UE6\UE6-02-Statut des medicaments et autres produits de santé 2",
  @"S2\UE6\UE6-03-Description des medicaments",
  @"S2\UE6\UE6-04-Developpement des medicaments",
  @"S2\UE6\UE6-05-Production des medicaments",
  @"S2\UE6\UE6-06-Cibles mecanismes d'action 1",
  @"S2\UE6\UE6-06-Cibles mécanismes d'action 2",
  @"S2\UE6\UE6-07-Les aspects socioeconomiques du medicament 1",
  @"S2\UE6\UE6-07-Les aspects socioeconomiques du medicament 2",
  @"S2\UE6\UE6-08-Evaluation des medicaments et agrement pour leur commercialisation 1",
  @"S2\UE6\UE6-09-Regle de prescription l'ordonnance",
  @"S2\UE6\UE6-10-Apercu de la pharmacodynamie",
  @"S2\UE6\UE6-11-Apercu de la pharmacocinetique",
  @"S2\UE6\UE6-12-Rapport benefice risque decision therapeutique et effets indesirables"
  };
QuestionTest.Test_QuestionReader_Comparev1v2(directories, @"c:\pib\drive\google\valentin\_test\compare_v1_v2");
string[] directories = new string[] {
  @"S2\UE8\UE8-01-Le vieillissement cellulaire",
  @"S2\UE8\UE8-02-Les cellules souches",
  @"S2\UE8\UE8-03-Modes de transmission et maladies genetiques",
  @"S2\UE8\UE8-04-Principales anomalies chromosomiques",
  @"S2\UE8\UE8-05-Indication du diagnostic prenatal chromosomique",
  @"S2\UE8\UE8-06-Information consentement et secret medical",
  @"S2\UE8\UE8-07-Communication cellulaire",
  @"S2\UE8\UE8-08-Pathologie moleculaires et genome techniques pour la recherche de mutations",
  @"S2\UE8\UE8-09-Pathologies moleculaires et genome techniques d'etude du genome",
  @"S2\UE8\UE8-10-Pathologie moleculaire et genome stabilité et evolution du genome",
  @"S2\UE8\UE8-11-Pathologie moleculaire et genome mutations",
  @"S2\UE8\UE8-12-Ethique medecine legale leglislation 1",
  @"S2\UE8\UE8-13-Ethique medecine legale leglislation 2",
  @"S2\UE8\UE8-14-La relation soignant-soigné",
  @"S2\UE8\UE8-15-Ethique medecine et santé des humains",
  @"S2\UE8\UE8-16-Pathologie moleculaire et genome les hemoglobinopathies"
  };
QuestionTest.Test_QuestionReader_Comparev1v2(directories, @"c:\pib\drive\google\valentin\_test\compare_v1_v2");


string[] directories = new string[] {
  @"S2\UE3\UE3-01-Etat de la matiere et leur caracterisaion etat d'agregation",
  @"S2\UE3\UE3-02-Etat de la matiere et leur caracterisation",
  @"S2\UE3\UE3-03-Etat de la matiere et leur caracterisation proprietes colligatives",
  @"S2\UE3\UE3-04-Gaz eau et solutions aqueuses",
  @"S2\UE3\UE3-05-Propriete colligative et transports membranaires",
  @"S2\UE3\UE3-06-Transports electriques et transmembranaires",
  @"S2\UE3\UE3-07-Activité électrique cardiaque",
  @"S2\UE3\UE3-08-Potentiel membranaire de repos potentiel local potentiel d'action",
  @"S2\UE3\UE3-09-Regulations des espaces hydriques",
  @"S2\UE3\UE3-10-Regulation du bilan hydro-sodé",
  @"S2\UE3\UE3-11-Equilibre acido-basique",
  @"S2\UE3\UE3-12-Regulation acido-basique",
  @"S2\UE3\UE3-13-pH et equilibre acido-basique",
  @"S2\UE3\UE3-14-Thermoregulation",

  @"S2\UE6\UE6-01-Histoire du medicament",
  @"S2\UE6\UE6-02-Statut des medicaments et autres produits de santé 1",
  @"S2\UE6\UE6-02-Statut des medicaments et autres produits de santé 2",
  @"S2\UE6\UE6-03-Description des medicaments",
  @"S2\UE6\UE6-04-Developpement des medicaments",
  @"S2\UE6\UE6-05-Production des medicaments",
  @"S2\UE6\UE6-06-Cibles mecanismes d'action 1",
  @"S2\UE6\UE6-06-Cibles mécanismes d'action 2",
  @"S2\UE6\UE6-07-Les aspects socioeconomiques du medicament 1",
  @"S2\UE6\UE6-07-Les aspects socioeconomiques du medicament 2",
  @"S2\UE6\UE6-08-Evaluation des medicaments et agrement pour leur commercialisation 1",
  @"S2\UE6\UE6-09-Regle de prescription l'ordonnance",
  @"S2\UE6\UE6-10-Apercu de la pharmacodynamie",
  @"S2\UE6\UE6-11-Apercu de la pharmacocinetique",
  @"S2\UE6\UE6-12-Rapport benefice risque decision therapeutique et effets indesirables",

  @"S2\UE8\UE8-01-Le vieillissement cellulaire",
  @"S2\UE8\UE8-02-Les cellules souches",
  @"S2\UE8\UE8-03-Modes de transmission et maladies genetiques",
  @"S2\UE8\UE8-04-Principales anomalies chromosomiques",
  @"S2\UE8\UE8-05-Indication du diagnostic prenatal chromosomique",
  @"S2\UE8\UE8-06-Information consentement et secret medical",
  @"S2\UE8\UE8-07-Communication cellulaire",
  @"S2\UE8\UE8-08-Pathologie moleculaires et genome techniques pour la recherche de mutations",
  @"S2\UE8\UE8-09-Pathologies moleculaires et genome techniques d'etude du genome",
  @"S2\UE8\UE8-10-Pathologie moleculaire et genome stabilité et evolution du genome",
  @"S2\UE8\UE8-11-Pathologie moleculaire et genome mutations",
  @"S2\UE8\UE8-12-Ethique medecine legale leglislation 1",
  @"S2\UE8\UE8-13-Ethique medecine legale leglislation 2",
  @"S2\UE8\UE8-14-La relation soignant-soigné",
  @"S2\UE8\UE8-15-Ethique medecine et santé des humains",
  @"S2\UE8\UE8-16-Pathologie moleculaire et genome les hemoglobinopathies",

  @"S2\UE8-med\UE8-med-01-muscle strié",
  @"S2\UE8-med\UE8-med-02-muscle lisse",
  @"S2\UE8-med\UE8-med-03-physio cardiovasculaire",
  @"S2\UE8-med\UE8-med-04-regulation PA",
  @"S2\UE8-med\UE8-med-05-transport O2",
  @"S2\UE8-med\UE8-med-06-sensibilité",
  @"S2\UE8-med\UE8-med-07-motricité",
  @"S2\UE8-med\UE8-med-08-transport O2 et CO2",
  @"S2\UE8-med\UE8-med-09-équilibration",
  @"S2\UE8-med\UE8-med-10-physio endocrinienne"
  };
QuestionTest.Test_QuestionReader_v1(directories, @"c:\pib\dev_data\exe\runsource\valentin\test");

QuestionTest.GetQuestionReaderFilesTest(@"c:\pib\dev_data\exe\runsource\valentin\test").zView();
zdir.EnumerateDirectoriesInfo(@"c:\pib\dev_data\exe\runsource\valentin\test").zView();



//*************************************************************************************************************************
//****                                   Test_TextDataReader.project.xml
//*************************************************************************************************************************


DocumentComparator.CompareBsonDocumentFiles(@"c:\pib\drive\google\valentin\S2\UE3\UE3-05-Propriete colligative et transports membranaires\data\questions_02.json",
  @"c:\pib\dev_data\exe\runsource\valentin\test\S2\UE3\UE3-05-Propriete colligative et transports membranaires.questions.json").zGetResults().zView();
  
//S2\UE6\UE6-10-Apercu de la pharmacodynamie\data\questions_04.json
DocumentComparator.CompareBsonDocumentFiles(
  //@"c:\pib\drive\google\valentin\S2\UE3\UE3-05-Propriete colligative et transports membranaires\data\questions_02.json",
  @"c:\pib\drive\google\valentin\S2\UE6\UE6-10-Apercu de la pharmacodynamie\data\questions_04.json",
  //@"c:\pib\dev_data\exe\runsource\valentin\test\S2\UE3\UE3-05-Propriete colligative et transports membranaires.questions.json",
  @"c:\pib\dev_data\exe\runsource\valentin\test\S2\UE6\UE6-10-Apercu de la pharmacodynamie.questions.json",
  comparatorOptions: DocumentComparatorOptions.ReturnNotEqualDocuments | DocumentComparatorOptions.ResultNotEqualElements,
  elementsToCompare: new string[] { "Year", "Type", "Number", "QuestionText", "Choices", "SourceLine" },
  documentReference: new string[] { "document1.Year", "document1.Number", "document1.SourceFile", "document1.SourceLine", "document2.SourceFile", "document2.SourceLine" })
  .GetResultDocuments(CompareDocumentsResultOptions.Result | CompareDocumentsResultOptions.DocumentsResult).zSave(@"c:\pib\dev_data\exe\runsource\valentin\test\S2\compare.json", jsonIndent: true);
ResultDocumentsSource
DontSetDocumentReference

//*************************************************************************************************************************
//****                                   Test_Diff.project.xml
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Text\Diff\_Test\Test_Diff.project.xml");

Test_Diff.Test_01("aa bb cc", "aa bb");
Test_Diff.Test_01("aa", "bb");
Test_Diff.Test_01("aa", "aa");
