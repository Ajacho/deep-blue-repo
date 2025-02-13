# Ux-9

## URL Submission and Analysis
### Story: 
> As a user, I want to be able to submit a URL for my website so that I don’t have to provide source code to an outside entity. 

### Assumptions/Preconditions
- MVC project has been set up
- Index page has been created 
- Test folders have been created


### Description
The process of entering a URL should be straightforward and user-friendly. The system will provide a form interface where users can submit a valid website URL without needing to provide additional details. This form will serve as the primary method for users to input their website for further processing in future stages.  

The form will be designed for ease of use, ensuring that users receive immediate feedback if they enter an invalid URL. The interface will include clear labels, error handling, and validation rules to guide users toward proper input.

Here are the implementation details we've settled on after discussions with the development team:
- **Single input field**: Users enter a URL into a dedicated form field.
- **Validation**: The system will check if the input is a properly formatted URL
- **Error handling**: If an invalid URL is entered, the form will display a clear error message.
- **User feedback**: The form will provide real-time validation to assist users in correcting mistakes before submission.
- **Submit button**: Users must click a button to confirm their entry, ensuring intentional submission.
- **Basic accessibility features**: The form will include proper labels, keyboard navigation support, and screen reader compatibility.

### Acceptance Criteria:

**Given** the user is on the form page,   
**Then** I will see a clearly labeled input field for entering a URL,   
**And** I will see a submit button to confirm the entry,   
**And** the form layout will be accessible and user-friendly.   

**Given** the user enters a valid URL,  
**Then** the system will accept the input without errors,  
**And** the submit button will become active,   
**And** no validation error messages will be displayed. 

**Given** the user enters an invalid URL,   
**Then** the system will display an error message explaining the issue,   
**And** the submit button will remain disabled,   
**And** the input field will be highlighted to indicate the error.


### Tasks
- Write Unit Tests.
- Write a controller method for getting form information. 
- Create model for report.URL
- Create form for url input
- Validation for empty input
- Build out UI: Large text box on index, with submit button
- Write the JavaScript code to handle user input and AJAX calls.
- Test privacy and security measures to protect user data.


### Effort Points: 1
### Dependencies: None
### Owner: 
### Branch: feature/url-submission