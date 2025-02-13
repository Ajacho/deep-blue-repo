# Ux-8 

## Epic: Initial Application Setup

### Story: 
> As a system, I want to be able to access a database, so that I can gather saved information of users

### Assumptions/Preconditions
- The MVC project has been set up.
- The database has basic up and down scripts.

### Description
We want the application to be connected to a database in order to keep track of information. It should smoothly connect to the application with information from the database able to be transmitted to the project.

### Acceptance Criteria:

#### Scenario: Application Connects To The Database  

**Given** I am the application  
**And** I have the database string configured  
**When** I start up  
**Then** I should establish a connection to the database

#### Scenario: Database Test Record  

**Given** I am the application  
**When** I send a test query to fetch database information  
**Then** I shall receive the test record

#### Scenario: Repository Pattern

**Given** I am the application  
**And** I have a repository pattern  
**When** I request data from the database  
**Then** I should use the repository layer instead of querying the database

 #### Scenario: Unit Tests 

**Given** I am the application  
**And** I have a repository pattern  
**When** unit tests for the database are run  
**Then** all tests shall pass

### Tasks
- Make sure database has at least one test record
- Set up repository pattern
- Write unit tests
- Implement database API access until it passes all the tests

### Effort Points: 2
### Dependencies: None
### Owner: 
### Branch: Feature/database-setup


