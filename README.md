# WordPress XML Tool

Das **WordPress XML Tool** ist eine Windows-Anwendung, um Blogartikel aus WordPress-XML-Sicherungen komfortabel zu durchsuchen, zu filtern, zu bearbeiten und in verschiedene Formate zu exportieren. Das Tool richtet sich an Blogger:innen, die ihre Inhalte aus WordPress-Backups weiterverwenden oder sichern möchten.

## Features

- **XML-Import:** Lade eine WordPress-XML-Sicherung und zeige alle Blogartikel übersichtlich an.
- **Filter:** Suche und filtere Artikel nach Titel, Jahr und Autor.
- **Artikel bearbeiten:** Bearbeite Titel, Autor, Datum und HTML-Inhalt eines Artikels direkt im Editor (mit Monaco Editor).
- **Vorschau:** Zeige den HTML-Inhalt eines Artikels direkt in der Anwendung an.
- **Export:** Exportiere einzelne oder alle gefilterten Artikel als Markdown, TXT, JSON oder XML.
- **Modularer Aufbau:** Die Anwendung ist in logische Module (Model, Filter, Export, UI) gegliedert und leicht erweiterbar.

## Installation

1. **Voraussetzungen:**
   - Windows 10/11
   - .NET 6.0 oder .NET 8.0 Desktop Runtime (je nach Build)
2. **Download:**
   - Kompiliere das Projekt mit Visual Studio (Projektdatei: `WordPress_XML_Tool.csproj`)
   - Oder verwende die bereitgestellte `WordPress_XML_Tool.exe` im `bin/Release/net8.0-windows/`-Ordner
3. **Start:**
   - Starte die Anwendung mit `start.bat` oder direkt per Doppelklick auf die EXE-Datei.

## Nutzung

1. **XML laden:**
   - Über das Menü `Datei > XML öffnen` eine WordPress-XML-Sicherung auswählen.
2. **Artikel filtern:**
   - Filterleiste oben nutzen (Titel, Jahr, Autor).
3. **Artikel anzeigen & bearbeiten:**
   - Artikel in der Liste links auswählen, rechts wird der HTML-Inhalt angezeigt.
   - Über das Menü `Edit > Artikel bearbeiten...` öffnet sich ein Editor mit HTML-Bearbeitung.
4. **Export:**
   - Über das Menü `Export` können einzelne oder alle gefilterten Artikel in verschiedenen Formaten gespeichert werden.

## Technische Hinweise

- **Editor:** Für die HTML-Bearbeitung wird der Monaco Editor (wie in VS Code) im eingebetteten Browser verwendet (`editor.html`).
- **Dateiformate:**
  - **Markdown:** Titel, Metadaten und HTML-Inhalt als Markdown-Datei
  - **TXT:** Reiner Text mit Metadaten
  - **JSON:** Strukturierte Artikeldaten
  - **XML:** Original-XML-Element des Artikels
- **Modularität:**
  - `ArticleModel.cs`: Datenmodell & XML-Parser
  - `ArticleFilter.cs`: Filterlogik
  - `ArticleExporter.cs`: Exportfunktionen
  - `EditArticleDialog.cs`: Editor-Dialog
  - `editor.html`: HTML-Editor (Monaco)

## Lizenz & Autor

(c) 2026 Manfred Zainhofer

Dieses Tool ist Open Source und darf frei verwendet und angepasst werden.

---

**Hinweis:**
Dieses Tool ist ein privates Hilfsprogramm und wird ohne Gewähr bereitgestellt. Für Feedback oder Erweiterungswünsche bitte ein Issue eröffnen.
