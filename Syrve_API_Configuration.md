# Конфігурація Syrve API

## Оновлена інформація про Syrve API

На основі наданої документації, конфігурація Syrve API була оновлена.

### API Cloud

**Документація:** [https://api-eu.syrve.live/](https://api-eu.syrve.live/)

**Базовий URL:** `https://api-eu.syrve.live`

**API ключі:**
- **Тестове середовище:** `e27425e5f526464d88f2dda91080fac6`
- **Бойове середовище:** `c64e0f8a-ac7`

### Endpoints

#### 1. Призначення офіціанта
- **Endpoint:** `api/1/reserve/assign-waiter`
- **Метод:** POST
- **Призначення:** Призначення офіціанта до резерву/замовлення

#### 2. Статус резерву за ID
- **Endpoint:** `api/1/reserve/status_by_id`
- **Метод:** GET/POST (перевірити в документації)
- **Призначення:** Отримання детальної інформації про резерв/банкет

#### 3. Завантаженість секцій ресторану
- **Endpoint:** `api/1/reserve/restaurant_sections_workload`
- **Метод:** GET/POST (перевірити в документації)
- **Призначення:** Отримання інформації про завантаженість секцій ресторану
- **Примітка:** Потрібно викликати для кожного залу кожного закладу окремо

### Webhook для отримання подій

Для отримання повідомлень про створення/оновлення банкетів/резервів на касі:

1. Надати Syrve URL webhook мікросервісу: `https://your-server.com/api/syrve/webhook`
2. Syrve буде надсилати події на цей endpoint
3. Після отримання події можна викликати `api/1/reserve/status_by_id` для отримання детальної інформації

### API Web

**Тестове середовище:**
- URL: `https://372-414-171.syrve.app/`
- Login: `CRMiUM`
- Password: `CRMiUM`

**Бойове середовище:**
- URL: `https://krana-mry-cloud-chain.syrve.app/`
- Login: `CRMiUM`
- Password: `CRMiUM`

### OLAP Reports (Pivot-table)

Для отримання закритих замовлень за попередній період використовується розділ "OLAP reports (pivot-table)" в API Web.

## Зміни в конфігурації

### До оновлення:
```json
{
  "SyrveApi": {
    "BaseUrl": "https://api.syrve.com",
    "ApiKey": "YOUR_SYRVE_API_KEY_HERE",
    "AssignWaiterEndpoint": "api/v1/reserves/assign-waiter"
  }
}
```

### Після оновлення:
```json
{
  "SyrveApi": {
    "BaseUrl": "https://api-eu.syrve.live",
    "ApiKey": "e27425e5f526464d88f2dda91080fac6",
    "AssignWaiterEndpoint": "api/1/reserve/assign-waiter",
    "ReserveStatusEndpoint": "api/1/reserve/status_by_id",
    "RestaurantSectionsWorkloadEndpoint": "api/1/reserve/restaurant_sections_workload"
  }
}
```

## Важливі примітки

1. **Endpoint для призначення офіціанта** може відрізнятися від наданого. Потрібно перевірити в документації [https://api-eu.syrve.live/](https://api-eu.syrve.live/) точний endpoint.

2. **Webhook URL** потрібно надати Syrve для отримання подій про створення/оновлення резервів.

3. Для **бойового середовища** замініть API ключ на `c64e0f8a-ac7`.

4. **API Web** використовується для отримання звітів та закритих замовлень через OLAP reports.

