# Real Estate Property Management System

## **Introduction:**

The Real Estate Property Management System is a comprehensive database solution designed for property owners and managers. It handles tasks like managing owners, properties, tenants, rental contracts, maintenance requests, and payments.

## **Objective:**

The objective was to create a centralized, efficient, and easy-to-manage system that can cater to all aspects of real estate property management.

## **Technologies Used:**

SQL Server, .NET, Entity Framework

## **Database Design and Implementation:**

- **Database Schema:** The database for this system was designed to support multiple entities including Owners, Properties, Tenants, Rental Contracts, Maintenance Requests, and Payments. Each of these entities is represented as a table in the database. An Entity Relationship (ER) diagram was used to visually represent the structure of the database.

![Untitled](Real%20Estate%20Property%20Management%20System%20460c370ec9da405598dc35eb8147251e/Untitled.png)

- **Tables:** Several tables were created to store different types of data, for example:
    - **`Owners`**: This table holds information about property owners including their name, email, and contact number.
    - **`Properties`**: This table holds information about each property including the address, status, and the owner of the property.
    
    **Relationships:** Relationships between tables were established to maintain data integrity and provide structure to the database. For example, each property in the **`Properties`** table is associated with an owner in the **`Owners`** table.
    
    ![Untitled](Real%20Estate%20Property%20Management%20System%20460c370ec9da405598dc35eb8147251e/Untitled%201.png)
    

- **Constraints:** Constraints were used to maintain data integrity. For example, the **`Status`** column in the **`Properties`** table has a CHECK constraint to ensure that it only takes 'Available' or 'Not Available' as values.
    
    
    ![Untitled](Real%20Estate%20Property%20Management%20System%20460c370ec9da405598dc35eb8147251e/Untitled%202.png)
    

- S**tored Procedures and Triggers:** Stored procedures were used to encapsulate logic for data manipulation and ensure data consistency. Triggers were created to automatically update the **`Status`** of a property in the **`Properties`** table whenever a new **`RentalContract`** is added.
    
    
    ![Untitled](Real%20Estate%20Property%20Management%20System%20460c370ec9da405598dc35eb8147251e/Untitled%203.png)
    

## **Entity Framework Integration:**

Entity Framework (EF) was used to create a bridge between our Real Estate Property Management System's database and the .NET application. 

- **Database-First Approach:** The Database-First approach was utilized, which automatically creates model classes based on the database schema. This ensured that the classes were in sync with the database schema and that any changes made were accurately reflected.

```csharp
public class RealEstateDbContext : DbContext
{
public DbSet<Owner> Owners { get; set; }
public DbSet<Property> Properties { get; set; }
public DbSet<Tenant> Tenants { get; set; }
public DbSet<RentalContract> RentalContracts { get; set; }
public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(@"Server=.;Database=RealEstateDB;Trusted_Connection=True;");
}
}
```

- **CRUD Operations:** EF was then used to perform CRUD (Create, Read, Update, Delete) operations in the .NET application. This ensured a seamless interaction with the database.

```csharp
// Example: Adding a new property
using (var context = new RealEstateDbContext())
{
var property = new Property { Address = "Nidarosgatan 40", Status = "Available", OwnerID = 1 };
context.Properties.Add(property);
context.SaveChanges();
}
// Example: Fetching all properties
using (var context = new RealEstateDbContext())
{
var properties = context.Properties.ToList();
}
```

The integration of Entity Framework provided a straightforward and efficient way to interact with the SQL Server database, thereby streamlining the development of the .NET application.

## **Security Implementation:**

- **Database Security and Access Control.**

In the Real Estate Property Management System project, ensuring the security and privacy of the users' data was a key consideration. Various measures have been put into place to control access to data at different levels.

SQL Server provides a comprehensive security model that includes various mechanisms for controlling access to the database and its contents. As part of this project, SQL Server's built-in roles and permissions were utilized to manage database access.

Users were grouped into roles, each associated with specific permissions. For instance, an 'Administrator' role was created that had full access to all the tables and could perform any operations on the data. On the other hand, a 'Manager' role had restricted permissions and could only access certain tables, such as the 'Properties' and 'RentalContracts' tables.

Below is a simplified example of how roles and permissions were implemented:

```sql
-- Create roles
CREATE ROLE Administrator;
CREATE ROLE Manager;
-- Grant permissions to the Administrator
GRANT ALL PRIVILEGES ON Properties TO Administrator;
GRANT ALL PRIVILEGES ON Owners TO Administrator;
GRANT ALL PRIVILEGES ON Tenants TO Administrator;
GRANT ALL PRIVILEGES ON RentalContracts TO Administrator;
GRANT ALL PRIVILEGES ON MaintenanceRequests TO Administrator;
-- Grant permissions to Manager
GRANT SELECT, INSERT, UPDATE, DELETE ON Properties TO Manager;
GRANT SELECT, INSERT, UPDATE, DELETE ON RentalContracts TO Manager;
```

- **Data Encryption**

To ensure the privacy and security of sensitive data, some of the data in the database was encrypted. SQL Server provides built-in functions to encrypt data at the column level.

For instance, suppose the 'Owners' table contained sensitive data such as the 'Email' column. The data in this column could be encrypted using the **`ENCRYPTBYPASSPHRASE`** function when inserting or updating data and decrypted using the **`DECRYPTBYPASSPHRASE`** function when reading the data.

```sql
-- Insert data with encryption
INSERT INTO Owners(Name, Email)
VALUES ('Kenneth Okalang', ENCRYPTBYPASSPHRASE('MySecretPassphrase', 'ken.okalang@example.com'));
-- Query data with decryption
SELECT Name, CONVERT(varchar, DECRYPTBYPASSPHRASE('MySecretPassphrase', Email)) AS Email
FROM Owners;
```

## **Transaction Management:**

In a real estate property management system, there are many situations where data consistency is crucial across multiple operations. One mistake or system error in the middle of an operation can cause serious data inconsistencies. Hence, a robust transaction management strategy is needed.

A transaction is a single unit of work that consists of multiple related tasks. All tasks within a transaction must succeed for the transaction to be considered successful. If any task within the transaction fails, then the entire transaction fails, and the database is rolled back to its state before the transaction started.

In SQL Server, transactions are handled using the **`BEGIN TRANSACTION`**, **`COMMIT`**, and **`ROLLBACK`** statements. Here's an example of how transactions were used in this project:

Let's say we have an operation to move a tenant from one property to another. This operation involves several tasks:

1. Update the status of the current property to 'Available'.
2. Update the status of the new property to 'Not Available'.
3. Insert a new rental contract for the new property and tenant.
4. Update the end date of the current rental contract.

These tasks need to be performed in a single transaction to ensure data consistency. Here's how it could be done:

```sql
Create FilesqlCopy code
BEGIN TRANSACTION
BEGIN TRY
    -- Update current property status
    UPDATE Properties
    SET Status = 'Available'
    WHERE PropertyID = @currentPropertyId;

    -- Update new property status
    UPDATE Properties
    SET Status = 'Not Available'
    WHERE PropertyID = @newPropertyId;

    -- Insert new rental contract
    INSERT INTO RentalContracts (PropertyID, TenantID, StartDate, EndDate, RentAmount, DepositAmount)
    VALUES (@newPropertyId, @tenantId, @startDate, @endDate, @rentAmount, @depositAmount);

    -- Update end date of current rental contract
    UPDATE RentalContracts
    SET EndDate = @today
    WHERE ContractID = @currentContractId;

    COMMIT;
END TRY
BEGIN CATCH
    -- In case of error, rollback the transaction
    ROLLBACK;

    -- Raise an error with the details of the exception
    THROW;
END CATCH

```

With this approach, if there's a problem with any of the steps (for example, the new property doesn't exist, or the rental contract details are invalid), none of the changes will be applied, and the database will remain in a consistent state.

In Entity Framework Core, a similar approach can be followed using **`DbContext.Database.BeginTransaction()`**, **`DbContext.Database.CommitTransaction()`**, and **`DbContext.Database.RollbackTransaction()`** methods. The**`.SaveChanges()`** method is called only when committing the transaction.

The use of transactions in these ways ensures that our real estate property management system maintains data consistency at all times, even when carrying out complex multi-step operations.

## **Reporting:**

In any property management system, being able to generate clear and insightful reports is key to making informed decisions. From understanding which properties are most frequently rented to finding out which tenants are the most reliable, good reporting can be the difference between success and failure.

- **Creating Views**

Views are a powerful feature of SQL that allows us to save a particular query and then use it as if it were its own table. They can simplify complex queries by encapsulating them into a single view, which can then be queried as if it were a table. They can also provide a level of abstraction, allowing us to hide the complexities of the underlying database schema from the users who are writing the queries.

For example, in our property management system, we may want to create a view that provides information on all active rental contracts. This could include the property address, tenant name, start and end dates of the contract, and rent amount. Here is how we could define such a view:

```sql
CREATE VIEW ActiveRentalContracts AS
SELECT
P.Address,
T.FirstName + ' ' + T.LastName as TenantName,
RC.StartDate,
RC.EndDate,
RC.RentAmount
FROM RentalContracts RC
JOIN Properties P ON RC.PropertyID = P.PropertyID
JOIN Tenants T ON RC.TenantID = T.TenantID
WHERE RC.EndDate > GETDATE();
```

With this view, generating a report on active contracts is as simple as **`SELECT * FROM ActiveRentalContracts;`**

- **Writing Efficient Queries**

To generate efficient reports, we need to write efficient queries. This means taking full advantage of SQL's set-based processing capabilities, minimizing the use of cursors or iterative processing, and making good use of indexes.

Indexes can greatly speed up data retrieval. They work like a table of contents for a book - instead of reading through the entire book to find a particular topic (which is what a full table scan does), you can use the index to find the pages where the topic is discussed.

In the context of our property management system, we might frequently need to look up properties based on their status. An index on the **`Status`** column of the **`Properties`** table can significantly speed up these queries:

```sql
CREATE INDEX idx_Properties_Status ON Properties (Status);
```

When querying, we also want to reduce the amount of data returned by the server. If we only need certain columns from a table, we should only ask for those columns. This can significantly reduce network traffic and memory usage. Also, we should filter data at the source by using **`WHERE`** clause, which can significantly reduce the amount of data that needs to be processed.

With these methods, our property management system is capable of producing fast, efficient, and insightful reports.

## **Backup and Recovery Strategy:**

A backup and recovery strategy is an essential part of any database management system. This ensures that you can restore your data in the event of a system failure, data corruption, or other unforeseen events that may lead to data loss. Here's how I implemented it in the Real Estate Property Management System:

- **Full Backups**

The backbone of any SQL Server backup strategy is the full database backup. A full backup includes all data in the database, as well as the transaction log, which allows for point-in-time recovery. Here is a simple example of how you can take a full backup using T-SQL:

```sql
Create FilesqlCopy code
BACKUP DATABASE RealEstateDB
TO DISK = 'D:\Backups\RealEstateDB.bak';

```

This script creates a full backup of **`RealEstateDB`** and saves it to a file named **`RealEstateDB.bak`** on the **`D:\Backups\`** drive.

- **Differential Backups**

While full backups are important, they can take a significant amount of time and storage space for large databases. To mitigate this, we also use differential backups. A differential backup only includes the data that has changed since the last full backup. This means they are typically faster and smaller than full backups.

```sql
Create FilesqlCopy code
BACKUP DATABASE RealEstateDB
TO DISK = 'D:\Backups\RealEstateDB_Diff.bak'
WITH DIFFERENTIAL;
```

This script creates a differential backup of **`RealEstateDB`** and saves it to a file named **`RealEstateDB_Diff.bak`**.

- **Transaction Log Backups**

Transaction log backups allow for point-in-time recovery, which can be extremely useful if you need to recover the database to a specific moment in time, such as just before a critical error occurred.

```sql
Create FilesqlCopy code
BACKUP LOG RealEstateDB
TO DISK = 'D:\Backups\RealEstateDB_Log.trn';
```

This script creates a backup of the transaction log for **`RealEstateDB`**.

- **Recovery**

In case of a data loss event, the database can be recovered using the full backup, the latest differential backup, and all transaction log backups taken since the last differential backup. Here is an example of how you might restore **`RealEstateDB`**:

```sql
Create FilesqlCopy code
RESTORE DATABASE RealEstateDB
FROM DISK = 'D:\Backups\RealEstateDB.bak'
WITH NORECOVERY;

RESTORE DATABASE RealEstateDB
FROM DISK = 'D:\Backups\RealEstateDB_Diff.bak'
WITH NORECOVERY;

RESTORE LOG RealEstateDB
FROM DISK = 'D:\Backups\RealEstateDB_Log.trn'
WITH RECOVERY;

```

This is the backup and recovery strategy I implemented in our project, providing robust protection against data loss.

## **Challenges and Learnings:**

The process of creating the 'Real Estate Property Management System database posed several challenges, each of which offered valuable learning experiences.

### **Challenges.**

1. **Database Design:** One of the biggest challenges was designing the database schema. This involved determining the tables needed, the relationships among them, and the appropriate constraints to ensure data integrity. The normalization process was challenging but crucial to avoid redundancy and ensure data consistency.
2. **Implementation of Constraints:** Implementing constraints to enforce business rules was another challenge. This included ensuring that only permissible values are entered into the database, such as limiting the status of properties to 'Available' or 'Not Available'. It took some time to figure out the appropriate use of CHECK constraints to enforce these rules.
3. **Integration with .NET:** Integrating the database with a .NET application using Entity Framework also posed some challenges, particularly in handling the relations between tables and managing transactions. Understanding how Entity Framework handles relationships and its conventions was crucial to overcoming these challenges.
4. **Performance Tuning:** Writing efficient queries to minimize the load on the server and ensure speedy responses was also a considerable challenge. This required a good understanding of SQL query optimization techniques.

### **Learnings**

1. **Importance of Good Database Design:** The project highlighted the importance of careful planning and design in creating a database. A well-designed database not only makes it easier to work with but also ensures data integrity and consistency.
2. **Use of Advanced SQL Features:** This project provided hands-on experience with advanced SQL features, such as stored procedures, views, triggers, and transactions. These features are powerful tools for enforcing business rules, simplifying complex queries, and ensuring data consistency.
3. **Integration with Application Layer:** The process of integrating the database with a .NET application using Entity Framework provided valuable insights into how databases interact with application layers. It also highlighted the importance of understanding the Object-Relational Mapping (ORM) tool being used, as each ORM has its conventions and ways of handling relationships and transactions.
4. **Backup and Recovery:** The project underscored the importance of having a robust backup and recovery strategy in place to prevent data loss. It also emphasized the need to regularly test the recovery process to ensure backups can be successfully restored when needed.
5. **Security:** Implementing security measures such as data encryption and user roles was a crucial part of the project, highlighting the importance of data protection in database systems.

## **Conclusion:**

The 'Real Estate Property Management System' database project is currently in a completed state. It includes the fundamental features of a property management system, such as managing property details, owner and tenant information, rental contracts, and maintenance requests. The project also includes advanced features like data encryption for sensitive data, user roles and permissions, transaction management, efficient reporting, and a robust backup and recovery strategy.

The system was designed and implemented using SQL Server, while Entity Framework was used to integrate the database into a .NET application, demonstrating how databases interact with application layers.

In terms of future plans, there is always scope for extending the project. Some potential enhancements could include:

1. **Expanded Feature Set:** Additional features could be added to further enhance the capabilities of the system, such as online payment processing for rent payments, integration with an email system for communication with tenants and owners, and functionality for handling property inspections and repairs.
2. **Enhanced Reporting Capabilities:** While the project currently includes basic reporting functionality, this could be expanded to include more complex and detailed reports, such as financial reports for property owners, performance metrics for properties, and predictive analytics to help with property management decisions.
3. **Web or Mobile Interface:** While the current project is a backend-focused database system, it could be extended by adding a user-friendly web or mobile interface. This would allow tenants, property owners, and property managers to interact with the system directly, enhancing the overall user experience.
4. **Integration with Other Systems:** The system could potentially be integrated with other systems, such as a financial accounting system, a property listing portal, or a customer relationship management system.

In conclusion, although the 'Real Estate Property Management System' project is currently complete, it offers a strong foundation that can be built upon for future enhancements. This project not only provided a practical application of SQL programming, database design, and Entity Framework but also demonstrated how a well-designed database system can effectively manage and organize complex real-world data.

## **Gallery:**

Include screenshots or video clips of the working application, database diagrams, and any other visuals relevant to the project.
