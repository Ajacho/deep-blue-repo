Feature: Symmetry Pattern
# --filter "TestCategory=Symmetry"

# Selenium Test Below
@Patterns @Symmetry
Scenario: Symmetry issue exists
    Given the user navigates to the site
    And their local site url is "http://localhost:5002/badSymmetry.html"
    When the user enters "http://localhost:5002/badSymmetry.html" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the user should see the symmetry issue


@Patterns @Symmetry
Scenario: Symmetry issue does not exist
    Given the user navigates to the site
    And their local site url is "http://localhost:5002/goodSymmetry.html"
    When the user enters "http://localhost:5002/goodSymmetry.html" to analyze
    When the user starts the analysis
    When the report view has loaded
    Then the user should not see the symmetry issue

