Feature: Custom 404 Page
    As a user I want to access multiple pages.

  Scenario: Navigating to a non-existent page should display a custom 404 page
    Given David requests a non-existent page "/takis"
    Then he should be redirected to "/Home/ErrorPage"
