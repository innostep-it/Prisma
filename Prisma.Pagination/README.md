# 📖 Prisma.Pagination

Ein Framework zur einfachen Implementierung von Paginierung in .NET-Anwendungen. Es bietet eine flexible Möglichkeit, Daten in Seiten aufzuteilen und zu sortieren, um eine effiziente Datenverarbeitung und -anzeige zu ermöglichen.

---

## 🔍 Übersicht

Das Projekt **Prisma.Pagination** stellt folgende Hauptkomponenten bereit:

- **`PaginatedRequest`**: Eine Klasse zur Definition von Paginierungs- und Sortierungsparametern.
- **`PaginatedResult<TEntity>`**: Eine Klasse, die das Ergebnis einer Paginierung enthält, einschließlich der Metadaten wie Gesamtanzahl der Elemente und Seiten.
- **`QueryableExtension.AsPaginatedResult`**: Eine Erweiterungsmethode, die auf einer `IQueryable`-Datenquelle angewendet wird, um die Paginierung und Sortierung zu ermöglichen.

---

## ⚙️ Verwendung der `AsPaginatedResult`-Methode

Die `AsPaginatedResult`-Methode ist eine Erweiterungsmethode, die auf einer `IQueryable`-Datenquelle aufgerufen wird. Sie ermöglicht die Paginierung, Sortierung und Transformation der Daten in ein paginiertes Ergebnis.

### 🧪 Beispiel

```csharp
using Prisma.Pagination.Entities;
using Prisma.Pagination.Extensions;

var query = dbContext.Users.AsQueryable();

var request = new PaginatedRequest
{
    Page = 1,
    PageSize = 10,
    OrderColumnName = "Name",
    OrderDirectionDesc = false
};

var result = query.AsPaginatedResult(request, user => new UserDto
{
    Id = user.Id,
    Name = user.Name
});

// Zugriff auf die Ergebnisse
Console.WriteLine($"Seite: {result.Page} von {result.TotalPages}");
foreach (var item in result.Items)
{
    Console.WriteLine($"User: {item.Name}");
}
```

### 🔧 Schritte

1. **Erstellen Sie eine `PaginatedRequest`-Instanz**:
   - Definieren Sie die gewünschte Seite (`Page`) und die Anzahl der Elemente pro Seite (`PageSize`).
   - Optional: Geben Sie die Spalte für die Sortierung (`OrderColumnName`) und die Sortierrichtung (`OrderDirectionDesc`) an.

2. **Rufen Sie die `AsPaginatedResult`-Methode auf**:
   - Übergeben Sie die Datenquelle (`IQueryable`), die `PaginatedRequest`-Instanz und eine Mapping-Funktion, um die Daten in das gewünschte Format zu transformieren.

3. **Verarbeiten Sie das Ergebnis**:
   - Greifen Sie auf die paginierten Elemente (`Items`) und die Metadaten wie `TotalCount`, `Page` und `TotalPages` zu.

---

## 🛠️ Klassenübersicht

### `PaginatedRequest`

Eine Klasse zur Definition der Paginierungs- und Sortierungsparameter.

```csharp
public class PaginatedRequest
{
    public uint Page { get; set; }
    public uint PageSize { get; set; }
    public string? OrderColumnName { get; set; }
    public bool OrderDirectionDesc { get; set; }
}
```

### `PaginatedResult<TEntity>`

Eine Klasse, die das Ergebnis der Paginierung enthält.

```csharp
public class PaginatedResult<TEntity>
{
    public List<TEntity> Items { get; set; }
    public uint TotalCount { get; set; }
    public uint Page { get; set; }
    public uint PageSize { get; set; }
    public uint TotalPages => (uint)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
```

---

## 🧾 Zusammenfassung

| Komponente                  | Beschreibung                                                                 |
|-----------------------------|-----------------------------------------------------------------------------|
| `PaginatedRequest`          | Definiert die Parameter für Paginierung und Sortierung.                     |
| `PaginatedResult<TEntity>`  | Enthält die Ergebnisse der Paginierung und zugehörige Metadaten.            |
| `AsPaginatedResult`         | Erweiterungsmethode zur Paginierung und Sortierung von `IQueryable`-Daten. |

Mit **Prisma.Pagination** können Sie Ihre Daten effizient paginieren und sortieren, um eine bessere Benutzererfahrung und Performance zu gewährleisten.
