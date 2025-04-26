# 📊 Prisma Metrics Framework

Ein Framework zur Integration von **Grafana**, **Prometheus** und einem **Pushgateway** für die Erfassung und das Monitoring von Metriken in .NET-Anwendungen.

---

## 🔍 Übersicht

Dieses Framework ermöglicht die einfache Konfiguration und Nutzung eines **Metric Pushers**, der Metriken an ein Prometheus Pushgateway sendet. Es ist speziell für die Integration mit **Grafana** und **Prometheus** ausgelegt, um eine umfassende Visualisierung und Überwachung zu ermöglichen.

---

## ⚙️ Verwendung der `AddMetricsPusher`-Methode

Die `AddMetricsPusher`-Methode in der `ServiceCollectionExtension` registriert die notwendigen Dienste, um Metriken zu sammeln und an das Pushgateway zu senden.

### 🧪 Beispiel

```csharp
public static void ConfigureServices(IServiceCollection services)
{
    services.AddMetricsPusher();
}
```

### 🔧 Schritte

1. Rufen Sie `AddMetricsPusher()` in der `ConfigureServices`-Methode Ihrer Anwendung auf.
2. Stellen Sie sicher, dass die Konfiguration für den `MetricsPusherConfiguration`-Abschnitt in Ihrer `appsettings.json` korrekt definiert ist.

---

## 🛠️ Konfiguration

Die Konfiguration erfolgt über die `MetricsPusherConfiguration`-Klasse. Diese enthält die notwendigen Einstellungen für die Verbindung zum Pushgateway.

### Beispiel `appsettings.json`:

```json
{
  "MetricsPusherConfiguration": {
    "Endpoint": "http://<pushgateway-url>:9091/metrics",
    "Username": "your-username",
    "Password": "your-password",
    "JobName": "your-job-name",
    "PushCustomMetricsIntervalInSeconds": 15
  }
}
```

### Felder:

- **Endpoint**: URL des Prometheus Pushgateways.
- **Username**: Benutzername für die Authentifizierung.
- **Password**: Passwort für die Authentifizierung.
- **JobName**: Name des Jobs, der im Pushgateway angezeigt wird.
- **PushCustomMetricsIntervalInSeconds**: Intervall in Sekunden, in dem Metriken gepusht werden.

---

## 🚀 Funktionsweise des `MetricPusherService`

Der `MetricPusherService` ist ein `BackgroundService`, der Metriken in regelmäßigen Abständen an das Pushgateway sendet. 

### Ablauf:

1. Initialisierung eines HTTP-Clients mit Basic-Authentifizierung.
2. Start des `MetricPusher` mit den konfigurierten Optionen.
3. Sammeln und Senden von Metriken in einem definierten Intervall.

---

## 🧾 Zusammenfassung

| Komponente                  | Beschreibung                                                               |
|-----------------------------|----------------------------------------------------------------------------|
| `AddMetricsPusher`          | Registriert die notwendigen Dienste für den Metric Pusher.                 |
| `MetricsPusherService`      | Hintergrunddienst, der Metriken an das Pushgateway sendet.                 |
| `MetricsPusherConfiguration`| Konfigurationsklasse für die Verbindung zum Pushgateway.                   |

Mit diesem Framework können Sie Metriken effizient sammeln und in **Grafana** visualisieren, indem Sie **Prometheus** und ein **Pushgateway** nutzen.
