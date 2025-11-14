# Zoho CRM - Syrve Front API Integration Microservice

Мікросервіс для двосторонньої синхронізації офіціантів (EmployeeId) між Zoho CRM та Syrve Front API для замовлень і резервів.

## 📋 Зміст

- [Опис](#опис)
- [Вимоги](#вимоги)
- [Встановлення та запуск](#встановлення-та-запуск)
- [Налаштування](#налаштування)
- [API Endpoints](#api-endpoints)
- [Структура логів](#структура-логів)
- [Тестування](#тестування)
- [Архітектура](#архітектура)

## 📖 Опис

Мікросервіс виступає посередником між Zoho CRM та Syrve Front API, забезпечуючи:

- **Призначення офіціанта**: Приймає запити з Zoho CRM для призначення офіціанта до замовлення/резерву в Syrve
- **Синхронізація подій**: Приймає webhook події з Syrve Front API та передає їх у Zoho CRM
- **Логування**: Всі запити та відповіді логуються у файл для моніторингу та діагностики

## 🔧 Вимоги

- .NET 10.0 SDK або вище
- Windows / Linux / macOS
- Доступ до Syrve API Cloud ([https://api-eu.syrve.live/](https://api-eu.syrve.live/))
- API ключ Syrve (тестове або бойове середовище)
- Доступ до Zoho CRM webhook

## 🚀 Встановлення та запуск

### 1. Клонування репозиторію

```bash
git clone <repository-url>
cd zoho/Zoho\ API
```

### 2. Встановлення залежностей

```bash
dotnet restore
```

### 3. Налаштування конфігурації

Скопіюйте `appsettings.example.json` в `appsettings.json` та відредагуйте з вашими налаштуваннями:

```bash
cp "Zoho API/appsettings.example.json" "Zoho API/appsettings.json"
```

Потім відредагуйте `appsettings.json` (див. розділ [Налаштування](#налаштування))

### 4. Запуск мікросервісу

#### Development режим:

```bash
dotnet run
```

Або через Visual Studio / Rider - просто запустіть проект.

#### Production режим:

```bash
dotnet build -c Release
dotnet run -c Release
```

Мікросервіс буде доступний за адресою:
- HTTP: `http://localhost:5042`
- HTTPS: `https://localhost:7230`

### 5. Перевірка роботи

Відкрийте в браузері:
- OpenAPI документація: `http://localhost:5042/openapi/v1.json` (тільки в Development режимі)

## ⚙️ Налаштування

### appsettings.json

**Важливо:** Файл `appsettings.json` не включено в Git для безпеки. Використовуйте `appsettings.example.json` як шаблон.

Створіть файл `appsettings.json` на основі `appsettings.example.json` та відредагуйте з наступними параметрами:

```json
{
  "ApiKey": "YOUR_MICROSERVICE_API_KEY_HERE",
  "SyrveApi": {
    "BaseUrl": "https://api-eu.syrve.live",
    "ApiKey": "e27425e5f526464d88f2dda91080fac6",
    "AssignWaiterEndpoint": "api/1/reserve/assign-waiter",
    "ReserveStatusEndpoint": "api/1/reserve/status_by_id",
    "RestaurantSectionsWorkloadEndpoint": "api/1/reserve/restaurant_sections_workload"
  },
  "ZohoWebhook": {
    "Url": "https://www.zohoapis.eu/crm/v7/functions/handlesyrvewaiterupdate/actions/execute?auth_type=apikey&zapikey=YOUR_ZOHO_API_KEY"
  }
}
```

**Примітка:** 
- Для **тестового середовища** використовуйте API ключ: `e27425e5f526464d88f2dda91080fac6`
- Для **бойового середовища** використовуйте API ключ: `c64e0f8a-ac7`
- Документація Syrve API Cloud: [https://api-eu.syrve.live/](https://api-eu.syrve.live/)

#### Параметри конфігурації:

| Параметр | Опис | Обов'язковий |
|----------|------|--------------|
| `ApiKey` | API ключ для авторизації запитів до мікросервісу (використовується в заголовку `X-API-Key`) | Так |
| `SyrveApi:BaseUrl` | Базовий URL Syrve API Cloud (https://api-eu.syrve.live) | Так |
| `SyrveApi:ApiKey` | API ключ для авторизації до Syrve API Cloud (тестове/бойове середовище) | Так |
| `SyrveApi:AssignWaiterEndpoint` | Endpoint для призначення офіціанта | Так |
| `SyrveApi:ReserveStatusEndpoint` | Endpoint для отримання статусу резерву за ID | Ні |
| `SyrveApi:RestaurantSectionsWorkloadEndpoint` | Endpoint для отримання завантаженості секцій ресторану | Ні |
| `ZohoWebhook:Url` | URL webhook Zoho CRM для відправки подій | Так |

### appsettings.Development.json

Для розробки використовується окремий файл конфігурації з більш детальним логуванням.

## 🌐 API Endpoints

### 1. POST /api/assign-waiter

Призначає офіціанта до резерву/замовлення в Syrve.

#### Запит

**Headers:**
```
X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE
Content-Type: application/json
```

**Body:**
```json
{
  "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
  "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
  "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
}
```

#### Успішна відповідь (200 OK)

```json
{
  "creationStatus": "Success",
  "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
  "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
}
```

#### Помилка валідації (400 Bad Request)

```json
{
  "status": "error",
  "message": "OrganizationId is required and cannot be empty; ReserveId must be a valid GUID format",
  "errorType": "Validation",
  "timestamp": "2025-01-15 10:30:25"
}
```

#### Помилка зовнішнього сервісу (502 Bad Gateway)

```json
{
  "status": "error",
  "message": "Failed to communicate with external service: Syrve API returned 404: Not Found",
  "errorType": "HTTP",
  "timestamp": "2025-01-15 10:30:25"
}
```

#### Внутрішня помилка (500 Internal Server Error)

```json
{
  "status": "error",
  "message": "An internal error occurred: ...",
  "errorType": "Exception",
  "timestamp": "2025-01-15 10:30:25"
}
```

#### Приклад використання (cURL)

```bash
curl -X POST http://localhost:5042/api/assign-waiter \
  -H "X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
    "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
  }'
```

---

### 2. POST /api/syrve/webhook

Приймає webhook події з Syrve Front API та передає їх у Zoho CRM.

#### Запит

**Headers:**
```
X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE
Content-Type: application/json
```

**Body:**
```json
{
  "eventType": "ReserveUpdate",
  "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
  "eventInfo": {
    "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
    "reserve": {
      "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
      "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3",
      "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28"
    }
  }
}
```

#### Успішна відповідь (200 OK)

```json
{
  "status": "success",
  "message": "Event received and forwarded to Zoho",
  "timestamp": "2025-01-15 10:30:25"
}
```

#### Помилка обробки (500 Internal Server Error)

```json
{
  "status": "error",
  "message": "An internal error occurred: Failed to process webhook: ...",
  "errorType": "Exception",
  "timestamp": "2025-01-15 10:30:25"
}
```

**Примітка:** Навіть якщо відправка до Zoho CRM не вдалася, мікросервіс поверне 200 OK (якщо помилка оброблена) або 500 (якщо сталася необроблена помилка). Помилки логуються у файл логів.

#### Приклад використання (cURL)

```bash
curl -X POST http://localhost:5042/api/syrve/webhook \
  -H "X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "eventType": "ReserveUpdate",
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "eventInfo": {
      "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
      "reserve": {
        "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
        "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3",
        "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28"
      }
    }
  }'
```

---

### Авторизація

Всі endpoints вимагають заголовок `X-API-Key` з валідним API ключем, налаштованим в `appsettings.json`.

**Помилка авторизації (401 Unauthorized):**
```json
{
  "status": "error",
  "message": "API Key is required. Please provide X-API-Key header.",
  "errorType": "Authentication"
}
```

**Помилка авторизації (403 Forbidden):**
```json
{
  "status": "error",
  "message": "Invalid API Key",
  "errorType": "Authentication"
}
```

## 📝 Структура логів

Всі події логуються у файл `logs/syrve-sync.log` з ротацією по днях.

### Формат логів

```
[2025-01-15 10:30:25] [INF] [2025-01-15 10:30:25] [INFO] /api/assign-waiter called
Request: {"organizationId":"bebb377a-f070-42b0-a68c-28ade4db6e28","reserveId":"95472c6c-ecbd-49b4-8f7f-afea1eb0360d","employeeId":"df103a36-9b52-45b9-b911-b8821e5b24d3"}
Response: {"creationStatus":"Success","reserveId":"95472c6c-ecbd-49b4-8f7f-afea1eb0360d","employeeId":"df103a36-9b52-45b9-b911-b8821e5b24d3"}
Status: Success
```

### Рівні логування

- **INFO** - Успішні запити та відповіді
- **WARN** - Помилки валідації, попередження
- **ERROR** - Помилки виконання, винятки

### Приклади записів у логах

#### Успішний запит:
```
[2025-01-15 10:30:25] [INF] [2025-01-15 10:30:25] [INFO] /api/assign-waiter called
[2025-01-15 10:30:25] [INF] Request: {"organizationId":"...","reserveId":"...","employeeId":"..."}
[2025-01-15 10:30:25] [INF] Calling Syrve API: https://api-eu.syrve.live/api/1/reserve/assign-waiter
[2025-01-15 10:30:25] [INF] Syrve API response status: OK
[2025-01-15 10:30:25] [INF] Response: {"creationStatus":"Success",...}
[2025-01-15 10:30:25] [INF] Status: Success
```

#### Помилка валідації:
```
[2025-01-15 10:30:25] [INF] [2025-01-15 10:30:25] [INFO] /api/assign-waiter called
[2025-01-15 10:30:25] [INF] Request: {"organizationId":"","reserveId":"","employeeId":""}
[2025-01-15 10:30:25] [WRN] Validation failed: OrganizationId is required and cannot be empty
[2025-01-15 10:30:25] [INF] Response: {"status":"error","message":"...","errorType":"Validation"}
[2025-01-15 10:30:25] [INF] Status: Error
```

#### Помилка HTTP:
```
[2025-01-15 10:30:25] [ERR] HTTP error occurred while calling Syrve API
System.Net.Http.HttpRequestException: Syrve API returned 404: Not Found
   at Zoho_API.Services.SyrveApiService.AssignWaiterToReserveAsync(...)
[2025-01-15 10:30:25] [INF] Response: {"status":"error","message":"Failed to communicate with external service: ...","errorType":"HTTP"}
[2025-01-15 10:30:25] [INF] Status: Error
```

## 🧪 Тестування

### Тестові кроки

#### 1. Перевірка запуску мікросервісу

1. Запустіть мікросервіс: `dotnet run`
2. Перевірте, що сервіс запустився без помилок
3. Перевірте наявність файлу `logs/syrve-sync.log`

#### 2. Тестування `/api/assign-waiter`

**Тест 1: Успішне призначення офіціанта**

```bash
curl -X POST http://localhost:5042/api/assign-waiter \
  -H "X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
    "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
  }'
```

**Очікуваний результат:** 200 OK з відповіддю від Syrve API

**Тест 2: Помилка валідації (порожні поля)**

```bash
curl -X POST http://localhost:5042/api/assign-waiter \
  -H "X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "organizationId": "",
    "reserveId": "",
    "employeeId": ""
  }'
```

**Очікуваний результат:** 400 Bad Request з повідомленням про помилки валідації

**Тест 3: Помилка валідації (невалідний GUID)**

```bash
curl -X POST http://localhost:5042/api/assign-waiter \
  -H "X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "organizationId": "invalid-guid",
    "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
    "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
  }'
```

**Очікуваний результат:** 400 Bad Request з повідомленням про невалідний формат GUID

**Тест 4: Відсутній API ключ**

```bash
curl -X POST http://localhost:5042/api/assign-waiter \
  -H "Content-Type: application/json" \
  -d '{
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
    "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
  }'
```

**Очікуваний результат:** 401 Unauthorized

**Тест 5: Невірний API ключ**

```bash
curl -X POST http://localhost:5042/api/assign-waiter \
  -H "X-API-Key: invalid-key" \
  -H "Content-Type: application/json" \
  -d '{
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
    "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
  }'
```

**Очікуваний результат:** 403 Forbidden

#### 3. Тестування `/api/syrve/webhook`

**Тест 1: Успішна обробка webhook**

```bash
curl -X POST http://localhost:5042/api/syrve/webhook \
  -H "X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "eventType": "ReserveUpdate",
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "eventInfo": {
      "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
      "reserve": {
        "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
        "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3",
        "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28"
      }
    }
  }'
```

**Очікуваний результат:** 200 OK з повідомленням про успішну обробку

**Перевірка:**
- Перевірте лог-файл на наявність записів про відправку до Zoho CRM
- Перевірте, що подія була отримана в Zoho CRM

**Тест 2: Webhook з іншим типом події**

```bash
curl -X POST http://localhost:5042/api/syrve/webhook \
  -H "X-API-Key: YOUR_MICROSERVICE_API_KEY_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "eventType": "OrderUpdate",
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "eventInfo": {
      "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
      "order": {
        "id": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
        "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
      }
    }
  }'
```

**Очікуваний результат:** 200 OK (мікросервіс передає сирі дані без змін)

#### 4. Перевірка логів

1. Відкрийте файл `logs/syrve-sync.log`
2. Перевірте, що всі запити логуються
3. Перевірте формат логів відповідно до вимог

### Використання Postman

Див. файл `Zoho_API.postman_collection.json` для імпорту колекції з готовими запитами.

## 🏗️ Архітектура

```
Zoho CRM
   │
   │ (1) POST /api/assign-waiter   ← виклик Deluge з CRM
   ▼
[ Microservice (.NET 10) ]
   │
   ├── Виклик Syrve Front API → оновлення офіціанта
   │
   └── Webhook /api/syrve/webhook ← події з Syrve
       │
       └── Відправка даних у CRM (через наданий webhook)
```

### Компоненти

- **Controllers**: Обробка HTTP запитів
  - `WaiterController`: Обробка призначення офіціанта
  - `WebhookController`: Обробка webhook подій з Syrve
- **Services**: Бізнес-логіка та інтеграції
  - `SyrveApiService`: Виклики до Syrve Front API
  - `ZohoWebhookService`: Відправка подій до Zoho CRM
- **Validators**: Валідація вхідних даних
- **Middleware**: Авторизація та обробка помилок
- **Builders**: Побудова відповідей

## 📦 Залежності

- `Microsoft.AspNetCore.OpenApi` (10.0.0) - OpenAPI підтримка
- `Serilog.AspNetCore` (9.0.0) - Логування
- `Serilog.Sinks.File` (7.0.0) - Логування у файл

## 🔒 Безпека

- Всі endpoints вимагають API ключ у заголовку `X-API-Key`
- HTTPS підтримка (налаштовується через `UseHttpsRedirection`)
- Валідація всіх вхідних даних
- Обробка помилок без витоку конфіденційної інформації

## 📞 Підтримка

Для питань та проблем звертайтеся до розробника проекту.

## 📄 Ліцензія

[Вказати ліцензію, якщо потрібно]

