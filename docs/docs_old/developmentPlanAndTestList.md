# Development plan Capstone Store
- create the classes and function signatures as stated in csb_capstone_classDiagram.drawio
- database tables have already been created during the requirements development work due to the sql query's that needed to be written
- fill in the sqlBuilder and related classes(DatabaseConn, SqlHelper, MappingAttribute)
- construct the Settings class in full and bind it to the sql table in the database
- fully test the database connection and ensure that there are no problems with it, use the settings class where needed

- create the language functionality almost completely, only thing left is the switch languages function, lang text will be added over the course of the development process
- construct the base UI functionality
- complete the menu map in full

-- Base system done now constructing the specific functionality

- exit function
- back function
- implement user handling
    - add user
    - view users
    - edit user
    - remove user
- login / logout function
- implement product workings
    - base product handling
    - add product
    - list / read product
    - edit product
    - remove product
- implement Order base handling
- implement shopping cart workings
    - add product to shopping cart
    - view shopping cart and specific cart product
    - remove product from shopping cart
    - create order based on shopping cart
- further implement Order handling
    - view orders based on status and user id
    - accept and reject orders
- change languages function
