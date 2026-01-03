Virtuele Dierentuin – Designdocument
Projectnaam: Dierentuin
Studenten:
Esad Hamza Caliskan – 1206800
Brandon Hiraki – 1200837
Inleiding
Voor deze eindopdracht wordt een virtuele dierentuin ontwikkeld. Dit project maakt gebruik van de programmeertaal C# in combinatie met ASP.NET Core MVC om een webapplicatie te bouwen die dieren, verblijven en categorieën beheert.

Het doel van de applicatie is om gebruikers in staat te stellen een dierentuin te organiseren en te beheren. Daarnaast worden acties zoals zonsopgang, zonsondergang en etenstijd gesimuleerd.

Deze opdracht is bedoeld om aan te tonen dat de basisprincipes van programmeren in C# beheerst worden, evenals het toepassen van frameworks en tools zoals Entity Framework Core en Razor Views.

Doel van de Applicatie
De Virtuele Dierentuin biedt een intuïtieve manier om dieren, verblijven en categorieën te beheren. Gebruikers kunnen:

Dieren, verblijven en categorieën toevoegen, bewerken en verwijderen

Dieren koppelen aan categorieën en verblijven

Acties uitvoeren zoals zonsopgang, zonsondergang en etenstijd

Controleren of verblijven voldoen aan de eisen van de dieren

Gegevens zoeken en filteren

De applicatie is modulair en schaalbaar opgezet, zodat toekomstige uitbreidingen eenvoudig mogelijk zijn.

Functionaliteiten
Dierenbeheer
In de applicatie is het mogelijk om dieren toe te voegen, te bewerken en te verwijderen. Elk dier heeft eigenschappen zoals:

Naam (bijvoorbeeld "Tijger")

Soort (bijvoorbeeld Panthera tigris)

Grootte

Voedingsklasse (bijvoorbeeld Carnivoor)

Activiteitspatroon (Dag / Nacht / Altijd)

Dieren kunnen worden gefilterd op naam, soort of categorie.

Verblijvenbeheer
Gebruikers kunnen verblijven beheren met eigenschappen zoals:

Naam

Klimaat (bijvoorbeeld Tropisch)

Habitat type (bijvoorbeeld Bos of Water)

Beveiligingsniveau

Grootte

Per verblijf kunnen de volgende acties uitgevoerd worden:

Zonsopgang: toont welke dieren wakker worden of gaan slapen

Zonsondergang: toont welke dieren actief worden of gaan slapen

Etenstijd: toont wat elk dier eet

Categorieënbeheer
Categorieën worden gebruikt om dieren te groeperen, zoals:

Herbivoren

Carnivoren

Gebruikers kunnen categorieën beheren en dieren aan categorieën koppelen. Daarnaast kunnen dieren worden gefilterd op categorie.

Check Constraints
De applicatie controleert automatisch:

Of een verblijf groot genoeg is voor alle dieren erin

Of conflicterende diëten niet samen in één verblijf zitten (bijvoorbeeld roofdieren met prooidieren)

Dit voorkomt fouten in de indeling van de dierentuin.

Technische Specificaties
De applicatie is gebouwd met:

C#

ASP.NET Core MVC

Entity Framework Core

SQLite

Razor Views

Architectuur:

Model: gegevensstructuren (Animal, Enclosure, Category)

View: presentatie in de browser

Controller: afhandeling van gebruikersacties

De database wordt opgezet via migraties en gevuld met seed data.

Testen
De volgende onderdelen zijn getest:

Dierenbeheer: toevoegen, bewerken, verwijderen en filteren

Verblijvenbeheer: beheer en acties functioneren correct

Categorieënbeheer: koppelen en beheren werkt correct

Database: migraties en seed data functioneren zoals verwacht

Conclusie
De Virtuele Dierentuin is een volledige webapplicatie waarin gebruikers eenvoudig dieren, verblijven en categorieën kunnen beheren. Door de combinatie van ASP.NET Core MVC en Entity Framework is de applicatie overzichtelijk, uitbreidbaar en onderhoudsvriendelijk.

Het project toont aan dat de studenten in staat zijn om een gestructureerde webapplicatie te ontwikkelen volgens professionele standaarden.
