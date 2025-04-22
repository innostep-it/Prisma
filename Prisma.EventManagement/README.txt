# 📬 Azure Event Management System

Ein modulares Event-Management-Framework für .NET, optimiert für die Integration mit **Azure Service Bus**.  
Definiere, veröffentliche und empfange Events mit minimalem Setup – vollständig DI-kompatibel.

---

## 🔍 Übersicht

Dieses Projekt implementiert ein Event-System mit folgenden Kernkomponenten:

- `BaseEvent` – Basisklasse für alle Events
- `IEventHandler<T>` – Interface für Event-Handler
- `AddEventHandling()` – Automatische Registrierung der Handler in DI
- **Azure Service Bus Integration** – Unterstützung für Queues, Topics & Messaging

---

## 🧱 Verwendung der `BaseEvent`-Klasse

Die `BaseEvent`-Klasse dient als Grundlage für alle Events im System.

### 🧪 Beispiel

```csharp
public class UserCreatedEvent : BaseEvent
{
    public string UserId { get; set; }
    public string UserName { get; set; }
}
```

### 🔧 Schritte

1. Erstellen Sie eine neue Klasse, die von `BaseEvent` erbt.
2. Fügen Sie spezifische Eigenschaften hinzu, die für das Event benötigt werden.

---

## ⚙️ Verwendung des `IEventHandler`-Interfaces

Das `IEventHandler<T>`-Interface definiert die Logik für den Umgang mit empfangenen Events.

### 🧪 Beispiel

```csharp
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public async Task ExecuteAsync(UserCreatedEvent eventData)
    {
        // Logik zur Verarbeitung des Events
        Console.WriteLine($"User erstellt: {eventData.UserName}");
    }
}
```

### 🔧 Schritte

1. Implementieren Sie `IEventHandler<TEvent>`, wobei `TEvent` der Typ des Events ist.
2. Definieren Sie die Logik in der Methode `ExecuteAsync`.

---

## 🤖 Automatische Registrierung mit `AddEventHandling()`

Die Methode `AddEventHandling()` in der `ServiceCollectionExtension` registriert automatisch alle Event-Handler in der Dependency Injection.

### 🧪 Beispiel

```csharp
public static void ConfigureServices(IServiceCollection services)
{
    services.AddEventHandling();
}
```

### 🔧 Schritte

1. Rufen Sie `AddEventHandling()` in der `ConfigureServices`-Methode auf.
2. Alle Klassen, die `IEventHandler<T>` implementieren, werden automatisch registriert.

---

## ☁️ Azure Service Bus Integration

Dieses Framework ist speziell für Azure Service Bus konzipiert:

- Erstellung von Queues und Topics
- Senden und Empfangen von Nachrichten
- Konfigurierbar über `EventManagementConfiguration`  
  (z. B. Connection Strings, Queue-/Topic-Namen)

---

## 🧾 Zusammenfassung

| Komponente         | Beschreibung                                                  |
|--------------------|---------------------------------------------------------------|
| `BaseEvent`        | Basisklasse für alle Events                                   |
| `IEventHandler<T>` | Interface zur Definition von Event-Handlern                   |
| `AddEventHandling` | Automatische DI-Registrierung aller Event-Handler             |
| Azure Integration  | Unterstützt Queues, Topics & Messaging via Azure Service Bus  |

---

## 🛠️ Nächste Schritte

- [ ] Beispielprojekt hinzufügen
- [ ] Unit-Tests erweitern
- [ ] Erweiterte Konfigurationsmöglichkeiten dokumentieren
