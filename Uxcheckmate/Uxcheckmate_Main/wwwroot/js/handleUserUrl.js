// JavaScript file to handle user input URL

// Function to validate the URL format
function validateURL(urlInput) {
    var urlRegex = /^(https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w-]*)*$/;
    return urlRegex.test(urlInput);
}

// Function to handle user input URL
function handleUserUrl(event) {

    event.preventDefault();
    var urlInput = document.getElementById('urlInput').value.trim();
    var confirmationMessage = document.getElementById('confirmationMessage');

    if (urlInput === "") {
        confirmationMessage.innerHTML = `
        <div class='alert alert-danger'>
            <h5><strong>Empty URL!</strong></h5>
            <p>Please enter a URL before submitting.</p>
        </div>`;
        return false;
    }



    if(!validateURL(urlInput)) {    
        confirmationMessage.innerHTML = `
        <div class='alert alert-danger'>
            <h5><strong>Invalid URL! A non-valid URL can occur if:</strong></h5>
            <p><strong>Invalid characters:</strong> http://example[dot]com (no brackets or spaces)</p>
            <p><strong>Incorrect format:</strong> http:/example.com (missing one slash)</p>
            <p><strong>Missing Domain Extension:</strong> http://example (should have .com, .org, .edu, etc.)</p>
            <p><strong>Spaces:</strong> http://example .com (spaces not allowed)</p>
        </div>`;
        return false
    }

    confirmationMessage.innerHTML = "<div class='alert alert-success'>Your URL has been successfully submitted! Please wait to get it analyzed...</div>";

    // Added a delay to show the confirmation message before submitting the form
    setTimeout(() => {
        document.getElementById('urlForm').submit();
    }, 2000);

}

if (typeof window !== 'undefined') {
    window.onload = function () {
        document.getElementById('urlForm').addEventListener('submit', handleUserUrl);
    };
}

// Export the functions to be used in other files (Jest_Tests/handleUserUrl.test.js)
module.exports = { validateURL, handleUserUrl };