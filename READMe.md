### Capstone Store
This project is the Capstone project for Galileo Academy C# traject.

The Mysql database dump stored in the `/docs` folder is to be placed in a MariaDB database server and the account to be used by the application has to al least the following rights:
- delete
- insert
- select
- update

On the Schema, if these rights are not given, then the application will either not work or some parts of the application will not work.

The name of the schema does not matter as long
as it matches the one that is placed in `appsettings.json` which can be edited.

Here is a statement
that can be used for such assuming that the user account is called StoreManager accessed via localhost
and the schema is called capstoneStore:
- ```sql
  grant delete, insert, select, update on capstoneStore.* to StoreManager@localhost;
  ```

When the Schema is placed in the database, 
the `appsettings.json` file will need to be updated with the required access information of the user account.


The following accounts are usable for full access to the application:
- Admin:
  - username: reventcake
  - password: password
- Customer:
  - username cake
  - password: cake
