Übersicht
Dieses Projekt implementiert ein Event-Management-System, das speziell für die Verwendung mit dem Azure Service Bus ausgelegt ist. Es bietet eine flexible Möglichkeit, Events zu definieren, zu veröffentlichen und zu empfangen. Die Hauptkomponenten sind die BaseEvent-Basisklasse und das IEventHandler-Interface, die zusammen mit der AddEventHandling-Methode in der ServiceCollectionExtension verwendet werden, um Event-Handler automatisch in die Dependency Injection (DI) aufzunehmen.

<hr></hr>
Verwendung der BaseEvent-Klasse
Die BaseEvent-Klasse dient als Basisklasse für alle Events, die im System definiert werden. Sie enthält grundlegende Eigenschaften, die von allen Events geerbt werden können.
Beispiel:

```csharp
public class UserCreatedEvent : BaseEvent
{
    public string UserId { get; set; }
    public string UserName { get; set; }
}

Schritte:
Erstellen Sie eine neue Klasse, die von BaseEvent erbt.
Fügen Sie spezifische Eigenschaften hinzu, die für das Event benötigt werden.
<hr></hr>
Verwendung des IEventHandler-Interfaces
Das IEventHandler-Interface wird verwendet, um Handler-Klassen für spezifische Events zu definieren. Diese Handler-Klassen enthalten die Logik, die ausgeführt wird, wenn ein Event empfangen wird.
Beispiel:

```csharp
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    public async Task ExecuteAsync(UserCreatedEvent eventData)
    {
        // Logik zur Verarbeitung des Events
        Console.WriteLine($"User erstellt: {eventData.UserName}");
    }
}

Schritte:
Implementieren Sie das IEventHandler<TEvent>-Interface, wobei TEvent der Typ des Events ist.
Definieren Sie die Logik in der Methode ExecuteAsync.
<hr></hr>
Automatische Registrierung mit AddEventHandling
Die Methode AddEventHandling in der ServiceCollectionExtension registriert alle Event-Handler automatisch in der Dependency Injection (DI). Dadurch werden die Handler automatisch erkannt und können verwendet werden.
Beispiel:

```csharp
public static void ConfigureServices(IServiceCollection services)
{
    services.AddEventHandling();
}

Schritte:
Rufen Sie AddEventHandling in der ConfigureServices-Methode auf.
Alle Klassen, die das IEventHandler-Interface implementieren, werden automatisch registriert.
<hr></hr>
Azure Service Bus Integration
Dieses Framework ist speziell für die Integration mit dem Azure Service Bus ausgelegt. Es unterstützt die Erstellung von Queues und Topics, das Senden von Nachrichten sowie das Empfangen und Verarbeiten von Events. Die Konfiguration erfolgt über die EventManagementConfiguration, die Verbindungszeichenfolgen und andere Einstellungen für den Azure Service Bus enthält.

<hr></hr>
Zusammenfassung
BaseEvent: Basisklasse für alle Events.
IEventHandler: Interface zur Definition von Event-Handlern.
AddEventHandling: Automatische Registrierung von Event-Handlern in der DI.
Azure Service Bus: Vollständige Unterstützung für Queues, Topics und Nachrichtenverarbeitung.
Mit diesen Komponenten können Sie ein flexibles und erweiterbares Event-Management-System aufbauen, das optimal für den Einsatz mit dem Azure Service Bus geeignet ist.