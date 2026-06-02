# Food Delivery System
# Мюмюн Муса Муса
## Факултетен номер: 2401321002

## Описание

Система за онлайн поръчка и доставка на храна. Клиентите могат да разглеждат ресторанти и менюта и да правят поръчки. Администраторът управлява ресторантите, менютата, потребителите и техните роли. Собствениците на ресторанти потвърждават поръчките, а куриерите ги маркират като доставени.

## Технологии

- **Backend:** ASP.NET Core 8, Entity Framework Core, SQL Server
- **Frontend:** SPA — HTML, CSS, Vanilla JavaScript
- **Сигурност:** JWT Bearer токени

## Роли

| Роля | Права |
|---|---|
| Admin | Пълен контрол |
| RestaurantOwner | Управлява своя ресторант и меню |
| DeliveryDriver | Маркира поръчки като доставени |
| Customer | Разглежда ресторанти и поръчва |

## Инсталация и стартиране

1. Отворете `FoodDeliverySolution.sln` във Visual Studio 2022
2. Десен бутон на Solution → Set Startup Projects → задайте `FoodDelivery.API` и `FoodDelivery.Web` на Start
3. Натиснете F5 — базата данни се създава автоматично

**Frontend:** `https://localhost:7002`  
**Swagger:** `https://localhost:7001/swagger`

### Admin акаунт по подразбиране
- **Email:** admin@fooddelivery.com
- **Парола:** Admin123!
