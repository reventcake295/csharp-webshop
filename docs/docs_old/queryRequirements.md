## Settings
### Query's
- SELECT *
### Commands
- UPDATE maxInputLoop=@maxInputLoop, AvailableLAngs=@availableLangs, DefaultLang=@defaultLang where settingsId = 1

## Orders
### Query's:
* SELECT * FROM Orders WHERE status=incoming JOIN user ON Orders.user_id = user.user_id
* SELECT * WHERE user_id=@userId

### Commands:
* UPDATE 1.status=(completed|rejected) WHERE order_id = @orderId
* INSERT 1 OUTPUT INTO @insertOrderId;

### Joins:
- ?JOIN user ON user.user_id = Orders.user_id

## orderProducts
### Query's:
* SELECT * WHERE order_id = @orderId JOIN Taxes ON taxes_id JOIN Money ON money_id JOIN Product ON product_id (name, description)

### Commands:
* INSERT AFTER INSERT Orders multiple(order_id=@insertOrderId)

### Joins:
- JOIN Product ON product_id (name, description)
- JOIN Taxes ON taxes_id
- JOIN Money ON money_id

## Product
### Query's:
* SELECT * JOIN Taxes ON taxes_id JOIN Money ON money_id

### Commands:
* INSERT 1
* UPDATE 1 WHERE product_id = @productId
* delete 1 WHERE product_id = @productId

### Joins:
- JOIN Taxes ON taxes_id
- JOIN Money ON money_id

## Taxes
### Query's:
* SELECT *

### Commands:
* UPDATE 1 ON taxes_id
* INSERT 1
* DELETE 1

### Joins:
- product JOIN ON taxes_id
- orderProducts JOIN ON taxes_id


## Money
### Query's:
* SELECT *

### Commands:
* UPDATE 1 ON money_id
* INSERT 1
* DELETE 1

### Joins:
- product JOIN ON money_id
- orderProducts JOIN ON money_id


## User
### Query's:
* SELECT 1 WHERE user_id=@userId JOIN Auth ON auth_id
* SELECT count(*) WHERE username=@username
* SELECT *(-password) JOIN Auth ON auth_id

### Commands:
* INSERT single
* UPDATE single

### Joins:
- JOIN Auth ON auth_id


## Auth
### Query's:
* SELECT *

### Commands:
* INSERT single
* UPDATE single ON money_id
* DELETE 1
* 
### Joins:
- User JOIN ON auth_id
