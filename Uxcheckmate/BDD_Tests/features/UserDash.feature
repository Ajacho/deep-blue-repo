Feature: User Dashboard Feature
# --filter "FullyQualifiedName~UserDashboardFeature"

# Mock Test Below
Scenario: Reach user Dashboard
    Given user is logged in
    When they click user dash
    Then they should be in user dash page

Scenario: Logout from user dashboard
    Given user is logged in
    When they click logout button
    Then they should be logged out

Scenario: Deleting a report from the dashboard
    Given user is logged in
    When they click user dash
    Then they should be in the user dash page
    When the user clicks on the delete button for a report
    Then the report should be removed from the dashboard

# Selenium Test Below
@login 
Scenario: Test login with valid credentials
    Given the user navigates to the site
    And the user logs in
    Then they should see user dashboard

# Should currently fail Selenium test below until ux-96 implemented
Scenario: View previous report after login
    Given the user navigates to the site
    And the user logs in
    # And they have submitted previous report which default account has
    When they go to user dashboard
    Then they should see that report

@groupedReports
Scenario: Viewing grouped page reports under the same website
    Given user is logged in
    And has scanned multiple pages from different websites
    When they click on the user dashboard
    Then the user should see grouped page reports by domain
    And clicking a group should show all reports for that website