# Інструкція з тестування мікросервісу

## Крок 1: Налаштування API ключа

✅ **Вже виконано!** API ключ налаштовано: `test-api-key-123`

Якщо потрібно змінити, відредагуйте `Zoho API/appsettings.json`:
```json
"ApiKey": "ваш-ключ-тут"
```

## Крок 2: Запуск мікросервісу

### Варіант A: Через командний рядок

```powershell
cd "Zoho API"
dotnet run
```

### Варіант B: Через IDE (Rider/Visual Studio)

Просто натисніть F5 або "Run"

**Мікросервіс буде доступний на:** `http://localhost:5042`

## Крок 3: Тестування

### Спосіб 1: Використання .http файлу (Rider/Visual Studio)

1. Відкрийте файл `Zoho API/Zoho API.http`
2. Замініть `{{ApiKey}}` на `test-api-key-123` (або налаштуйте змінну)
3. Натисніть "Run" біля кожного запиту

### Спосіб 2: Використання cURL

Відкрийте **новий термінал** (мікросервіс має працювати в першому):

#### Тест 1: Webhook (найпростіший - не потребує Syrve API)

```powershell
curl.exe -X POST http://localhost:5042/api/syrve/webhook `
  -H "X-API-Key: test-api-key-123" `
  -H "Content-Type: application/json" `
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

**Очікуваний результат:** `200 OK` з повідомленням про успіх

#### Тест 2: Призначення офіціанта (потребує Syrve API)

```powershell
curl.exe -X POST http://localhost:5042/api/assign-waiter `
  -H "X-API-Key: test-api-key-123" `
  -H "Content-Type: application/json" `
  -d '{
    "organizationId": "bebb377a-f070-42b0-a68c-28ade4db6e28",
    "reserveId": "95472c6c-ecbd-49b4-8f7f-afea1eb0360d",
    "employeeId": "df103a36-9b52-45b9-b911-b8821e5b24d3"
  }'
```

**Примітка:** Може повернути помилку, якщо Syrve API не доступний або endpoint невірний.

#### Тест 3: Помилка валідації

```powershell
curl.exe -X POST http://localhost:5042/api/assign-waiter `
  -H "X-API-Key: test-api-key-123" `
  -H "Content-Type: application/json" `
  -d '{
    "organizationId": "",
    "reserveId": "",
    "employeeId": ""
  }'
```

**Очікуваний результат:** `400 Bad Request` з описом помилок валідації

#### Тест 4: Відсутній API ключ

```powershell
curl.exe -X POST http://localhost:5042/api/assign-waiter `
  -H "Content-Type: application/json" `
  -d '{
    "organizationId": "test",
    "reserveId": "test",
    "employeeId": "test"
  }'
```

**Очікуваний результат:** `401 Unauthorized`

#### Тест 5: Невірний API ключ

```powershell
curl.exe -X POST http://localhost:5042/api/assign-waiter `
  -H "X-API-Key: invalid-key" `
  -H "Content-Type: application/json" `
  -d '{
    "organizationId": "test",
    "reserveId": "test",
    "employeeId": "test"
  }'
```

**Очікуваний результат:** `403 Forbidden`

### Спосіб 3: Використання Postman

1. Відкрийте Postman
2. Імпортуйте файл `Zoho API.postman_collection.json`
3. Налаштуйте змінні:
   - `BaseUrl`: `http://localhost:5042`
   - `ApiKey`: `test-api-key-123`
4. Запускайте запити з колекції

## Крок 4: Перевірка логів

Після виконання тестів перевірте файл логів:

```
Zoho API/logs/syrve-sync.log
```

Там будуть записані всі запити, відповіді та помилки.

## Швидкий старт (все в одному)

1. **Термінал 1** - Запуск мікросервісу:
```powershell
cd "Zoho API"
dotnet run
```

2. **Термінал 2** - Тест webhook:
```powershell
curl.exe -X POST http://localhost:5042/api/syrve/webhook -H "X-API-Key: test-api-key-123" -H "Content-Type: application/json" -d "{\"eventType\":\"ReserveUpdate\",\"organizationId\":\"bebb377a-f070-42b0-a68c-28ade4db6e28\",\"eventInfo\":{\"id\":\"95472c6c-ecbd-49b4-8f7f-afea1eb0360d\",\"reserve\":{\"id\":\"95472c6c-ecbd-49b4-8f7f-afea1eb0360d\",\"employeeId\":\"df103a36-9b52-45b9-b911-b8821e5b24d3\",\"organizationId\":\"bebb377a-f070-42b0-a68c-28ade4db6e28\"}}}"
```

## Очікувані результати

✅ **Успішний запит:** `200 OK` з даними  
❌ **Помилка валідації:** `400 Bad Request`  
❌ **Відсутній API ключ:** `401 Unauthorized`  
❌ **Невірний API ключ:** `403 Forbidden`  
❌ **Помилка Syrve API:** `502 Bad Gateway`

