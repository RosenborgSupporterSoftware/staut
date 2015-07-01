Readme for .net-delen av STAut
==============================

Den delen av STAut-koden som kjører på .Net ligger altså her.

Følgende prosjekter eksisterer i løsningen:

- Teller.Core: Kjernen i prosjektet. Inneholder entities og logikk for håndtering av datafiler fra BillettService, data persistance o.l.

- Teller.Persistance: Alt som rører databasen skal ligge her, for å tvinge oss til å holde hoveddomenet fri for persistance-bekymringer.

- Teller: Kommandolinje-applikasjon laget for å kunne dra ut data i forskjellige formater mens selve STAut-web-funksjonaliteten utvikles.

- Ingest: Kommandolinje-applikasjon som skal lese xml-filer fra BillettService og pumpe nye data inn i databasen.

- Teller.Tests: Enhetstester for Teller.Core. Skrevet med XUnit.net.

