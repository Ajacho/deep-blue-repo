Feature: Role-based dashboard redirection

  @RolesBasedAccess
  Scenario: Standard user visits dashboard
    Given the user navigates to the site
    And the user logs in
    When they go to user dashboard
    Then they should see user dashboard

  @RolesBasedAccess
  Scenario: Admin visits dashboard
    Given the user navigates to the site
    And the user logs in as admin
    When they go to user dashboard
    Then they should see admin dashboard
