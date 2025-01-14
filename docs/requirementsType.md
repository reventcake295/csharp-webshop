## Application
### system background workings
- UI
  - Menu
    - display
    - input handling
  - input handling
  - output formatting and display
  - Change language
- user
  - login
  - user management (add,edit,remove)
- actions
  - triggering

### user requirements:
- As a user, I want to interact with the store through a command line interface.
- As a customer, I want to be able to add a product from the catalog to my shopping cart.
- As a customer, I want to be able to remove a product from my shopping cart.
- As a customer, I want to see all the items currently in my shopping cart.
- As a customer, I want my shopping cart to be empty when my order is successfully placed.

## Database -> Application
### system background workings:
- retrieve settings
  - ```sql
    SELECT DefaultLang, AvailableLangs, MaxInputLoop FROM capstoneStore.Settings;
    ```
- check if a user exists by returning the number of rows that has a given username 
  - ```sql
    SELECT count(*) AS userCount FROM users WHERE username=@username;
    ```
- check if a user exists with a username, then return the required information for processing
  - ```sql
    SELECT count(*) AS userCount, password, user_id, auth_id FROM users
    WHERE username=@username;
    ```
- load all users
  - ```sql
    SELECT user_id, username, email, adres_street, adres_number, adres_add, adres_postal, adres_city, auth_id
    FROM users;
    ```
- load the tax's data
  - ```sql
    SELECT * FROM Taxes;
    ```
- load the money data
  - ```sql
    SELECT * FROM Money;
    ```
### user requirements:
- As a user, I want to see the entire product catalog.
  - ```sql
    SELECT P.product_id, P.name, P.description, P.price, M.name, M.displayFormat, T.name, T.percent, P.money_id, P.taxes_id 
    FROM Products P 
    JOIN Taxes T ON T.taxes_id = P.taxes_id 
    JOIN Money M ON M.money_id = P.money_id;
    ```
- As a customer, I want to see the status of my order.
  - general overview of orders, of a specific user:
    ```sql
    SELECT O.order_id, O.statusId, O.order_date, O.orderTotal, M.displayFormat 
    FROM Orders O
    JOIN Money M ON M.money_id = O.money_id
    WHERE O.user_id=@userId;
    ```
    specific order selects:
    ```sql
    SELECT P.name, P.description, oP.pcsPrice, oP.count, oP.total, M.name, M.displayFormat, T.name, T.percent, P.money_id, P.taxes_id 
    FROM orderProducts oP 
    JOIN Taxes T ON T.taxes_id = oP.taxes_id 
    JOIN Money M ON M.money_id = oP.money_id
    JOIN Products P ON oP.product_id = P.product_id 
    WHERE oP.order_id = @orderId; 
    ```
- As an administrator, I want to see all incoming orders.
  - General overview of incoming orders
    ```sql
    SELECT O.statusId, O.order_date, O.orderTotal, M.displayFormat 
    FROM Orders O
    JOIN Money M ON M.money_id = O.money_id
    WHERE O.statusId=@statusId;
    ```
    specific order selects:
    ```sql
    SELECT P.name, P.description, oP.pcsPrice, oP.count, oP.total, M.name, M.displayFormat, T.name, T.percent, P.money_id, P.taxes_id 
    FROM orderProducts oP 
    JOIN Taxes T ON T.taxes_id = oP.taxes_id 
    JOIN Money M ON M.money_id = oP.money_id 
    JOIN Products P ON oP.product_id = P.product_id 
    WHERE oP.order_id = @orderId; 
    ```
## Application -> Database
### system background workings:
- update settings
  - ```sql
    UPDATE Settings SET maxInputLoop=@maxInputLoop, AvailableLAngs=@availableLangs, DefaultLang=@defaultLang WHERE settingsId = 1
    ```
- add a user to the database
  - ```sql
    INSERT INTO users (username, password, email, adres_street, adres_number, adres_add, adres_postal, adres_city, auth_id) 
    VALUES (@username, @password, @email, @adresStreet, @adresNumber, @adresAdd, @adresPostal, @adresCity, @authId)
    ```
- update user in the database
  - ```sql
    UPDATE users 
    SET username=@username, password=@password, email=@email, adres_street=@adresStreet, adres_number=@adresNumber, adres_add=@adresAdd, adres_postal=@adresPostal, adres_city=@adresCity, auth_id=@authId
    WHERE user_id = @userId;
    ```
- add a new taxes type
  - ```sql
    INSERT INTO Taxes (name, percent) 
    VALUES (@taxName, @taxPercent);
    ```
- update a tax's type
  - ```sql
    UPDATE Taxes 
    SET name=@taxName, percent = @taxPercent
    WHERE taxes_id = @taxesId;
    ```
- remove a tax's type
  - ```sql
    DELETE FROM Taxes WHERE taxes_id = @taxesId;
    ```
- add a money type
  - ```sql
    INSERT INTO Money (name, displayFormat) 
    VALUES (@moneyName, @moneyDisplayFormat);
    ```
- update a money type
  - ```sql
    UPDATE Money 
    SET name=@moneyName, displayFormat = @moneyDisplayFormat
    WHERE money_id = @moneyId;
    ```
- remove a money type
  - ```sql
    DELETE FROM Money WHERE money_id = @moneyId;
    ```
### User requirements:
- As an administrator, I want to add a product to my product catalog.
  - ```sql
    INSERT INTO Products (name, description, money_id, taxes_id, price) 
    VALUES (@productName, @productDescription, @productMoneyId, @productTaxesId, @productPrice);
    ```
- As an administrator, I want to edit a product in my product catalog.
  - ```sql
    UPDATE Products 
    SET money_id=@productMoneyId, taxes_id=@productTaxesId, name=@productName, description=@productDescription, price=@productPrice
    WHERE product_id = @productId;
    ```
- As an administrator, I want to delete a product in my product catalog.
  - ```sql
    DELETE FROM Products WHERE product_id = @productId 
    AND NOT EXISTS(
        SELECT COUNT(*) FROM orderProducts 
        WHERE product_id = @productId
    );
    ```
- As a customer, I want to be able to place an order containing all the items in my shopping cart.
  - First insert a header, then insert all products into the joining table along with the product info about it, 
    do note that the product insertion is done with one insertion, but the value rows are dynamically inserted 
    before the values are put in via the parameters; 
    this means that only the number belonging to the current inserted row is put in via a string builder
    
    ```sql
    INSERT INTO Orders (user_id, order_date, statusId, orderTotal, money_id)
    VALUES (@userId, @orderDate, @orderStatusId, @orderTotal, @orderMoneyId) RETURNING @_process_OrderId = order_id;
    INSERT INTO orderProducts (order_id, product_id, taxes_id, money_id, count, pcsPrice, total)
    VALUES
    (@_process_OrderId, @orderProductId1, @productTaxesId1, @productMoneyId1, @productCount1, @productPcsPrice1, @productTotal1),
    (@_process_OrderId, @orderProductId2, @productTaxesId2, @productMoneyId2, @productCount2, @productPcsPrice2, @productTotal2),
    (@_process_OrderId, @orderProductId3, @productTaxesId3, @productMoneyId3, @productCount3, @productPcsPrice3, @productTotal3);
    # ...;
    ```
- As an administrator, I want to complete an incoming order.
  - ```sql
    UPDATE Orders SET statusId=@newStatusId WHERE order_id = @orderId;
    ```
- As an administrator, I want to reject an incoming order.
  - ```sql
    UPDATE Orders SET statusId=@newStatusId WHERE order_id = @orderId;
    ```