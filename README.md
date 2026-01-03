# Virtuele Dierentuin

**Projectnaam:** Dierentuin  
- Esad Hamza Caliskan — 1206800  
- Brandon Hiraki — 1200837

---

## Inleiding

De Virtuele Dierentuin is een webapplicatie waarmee gebruikers een digitale dierentuin kunnen beheren.  
Gebruikers kunnen dieren, verblijven en categorieën aanmaken, bewerken en verwijderen, en daarnaast acties uitvoeren zoals zonsopgang, zonsondergang en etenstijd.

De applicatie is gebouwd met **ASP.NET Core MVC** en **Entity Framework Core**, en laat zien hoe een complete CRUD-webapplicatie wordt opgezet volgens het MVC‑patroon.

---

## Doel van de applicatie

Het doel van de applicatie is om:
- Dieren, verblijven en categorieën overzichtelijk te beheren
- Gedrag van dieren te simuleren via acties (Sunrise, Sunset, Feeding Time)
- Te controleren of de dierentuin logisch en veilig is ingericht (CheckConstraints)
- Een schaalbare en uitbreidbare architectuur te demonstreren

---

## Functionaliteiten

### Dieren
- CRUD (Create, Read, Update, Delete)
- Filteren op naam, soort, grootte, dieet, activiteit, categorie en verblijf
- Acties:
  - Sunrise
  - Sunset
  - Feeding Time
  - CheckConstraints

---

### Verblijven
- CRUD op verblijven
- Eigenschappen: naam, klimaat, habitat type, beveiligingsniveau, grootte
- Acties per verblijf:
  - Sunrise
  - Sunset
  - Feeding Time
  - CheckConstraints

---

### Categorieën
- CRUD op categorieën
- Dieren koppelen aan categorieën
- Filteren op categorie

---

### Dierentuin (globaal)
- Sunrise (alle verblijven)
- Sunset (alle verblijven)
- Feeding Time (alle verblijven)
- CheckConstraints (globale controle)
- AutoAssign (automatisch dieren indelen)

---

## Technische specificaties

- **Taal:** C#  
- **Framework:** ASP.NET Core MVC  
- **ORM:** Entity Framework Core  
- **Database:** SQLite  
- **Frontend:** Razor Views  
- **Architectuur:** MVC (Model – View – Controller)

---

## Testen

De applicatie is getest op:

- Dierenbeheer (CRUD en filtering)
- Verblijvenbeheer (CRUD en acties)
- Categoriebeheer (CRUD en koppeling)
- Database migraties en seed data

Alle kernfunctionaliteiten functioneren zoals verwacht.
