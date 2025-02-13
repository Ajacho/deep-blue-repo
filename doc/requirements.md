# UX Checkmate Requirements
### **Elicitation**

1. **Is the goal or outcome well-defined? Does it make sense?**
   - **Goal**: Yes, the goal is well-defined. The application aims to improve website accessibility and UX by providing actionable recommendations and AI-generated mockups.
   - **Outcome**: Users receive a categorized list of UX and accessibility issues and visual suggestions to improve their websites. It aligns well with the vision statement.

2. **What is not clear from the given description?**
   - Specific implementation details for mockup generation:
     - Will mockups adapt to user-specific themes or design preferences?
   - Are mockups editable by users after they are generated?
   - Is it scanning just a web page or the entire site?

3. **How about scope? Is it clear what is included and what isn't?**
   - **Included**:
     - URL submission for accessibility/UX analysis.
     - Account management, including guest access and recovery options.
     - Mockup generation using AI (OpenAI, DALL-E).
     - Feedback and admin dashboards.
   - **Not Included**:
     - Analysis of private or password-protected websites.
     - Real-time collaboration editing within reports.
     - Non-English language support in recommendations.

4. **What do you not understand?**
   - **Technical Domain Knowledge**:
     - Will Selenium require additional configurations for websites with heavy authentication or dynamic content?
   - **Business Domain Knowledge**:
     - How often do ux principle and accessibilties change over time?
     - More research on competitors
     - Do these standards differ based on location or industry?

5. **Is there something missing?**
   - There is no mention of accessibility features for the tool itself
   - Success metrics are not defined
   - Ability to display a screenshot of the Website
   - Web pages such as FAQ or Privacy Policy are missing.
   - Time consuming features that we needed to discard for now.

6. **Get answers to these questions.**
   - Schedule discussions with stakeholders to clarify:
     - Extent of customization for generated mockups.
     - Are there needs for translation services?


### **Analysis**

1. **Attributes, Terms, Entities, Relationships, and Activities**:
   - Entities:
      - User, Report, Feedback.
   - Attributes:
      - User: UserID, Name, Email, Password, RoleID.
      - Report: ReportID, UserID, URL, AccessibilityIssues, UXRecommendations, MockupPath, Date.
      - Feedback: FeedbackID, UserID, DateSubmitted, Content
   - Relationships:
      - A User can have multiple Reports. A report belongs to one user.
      - A User can submit multiple feedback entries. A feedback belongs to one User
      - User.RoleId determines the role of the user. Can view all Feedback and Reports.
   - Activities:
      - Input URL → Analyze DOM → Generate Recommendations → Store Report → Display to User
      - User account creation, login, and management.
      - URL submission and report generation.
      - AI analysis of accessibility and UX issues.
      - AI mockup creation.
      - Dashboard customization (viewing reports, managing settings).
      - Feedback submission by users.
      - Admin management of reports and feedback (based on RoleID = 1).

2. **Conflicts or Missing Requirements**:
    - Guests cannot store reports but may need temporary access to results.


3. **Have you discovered if something is missing?**:
   - Clarify the granularity of recommendations


### **Design and Modeling**

1. **Entities and Attributes**:
- *User*:  

    | Attribute | Type |
    | :------ | --: |
    | UserId (Pk) | INT |
    | Name | VARCHAR (50) |
    | Email | VARCHAR (255) |
    | Password | VARCHAR (255) |
    | Role (fk) | INT |

- *Report*:

    | Attribute | Type |
    | :------ | --: |
    | ReportId (Pk) | INT |
    | Categories | VARCHAR (50) |
    | Recommendations | VARCHAR (255) |
    | MockupPath | VARCHAR (255) |
    | Date | DATETIME |
    | UserId (fk) | INT |

- *Feedback*:

    | Attribute | Type |
    | :------ | --: |
    | FeedbackId (Pk) | INT |
    | Content | VARCHAR (255) |
    | Date | DATETIME |
    | UserId (fk) | INT |

2. **Relationships**:
- A Report is created by a User.
- Feedback is tied to a User and viewable by Admins.

3. **Entity-Relationship Diagram**:
- User<->Report: 1-to-many
- Admin<->Feedback: many-to-1


### **Analysis of the Design**

1. **Does the design support all requirements/features/behaviors?**
   - **Yes**, the design supports:
     - URL submission, analysis, and report generation.
     - Account creation and role-based access (User/Admin).
     - Feedback submission and management.

2. **Does it meet all non-functional requirements?**
   - **Performance**:
     - Using cloud-based APIs ensures scalability.
   - **Security**:
     - Password encryption ensures secure storage.
   - **Usability**:
     - Modular dashboard and custom themes enhance user experience.