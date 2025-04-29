﻿// JavaScript file to handle user input URL
// Function to validate the URL format and screenshot preview
document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM fully loaded, binding handleUserUrl...");

    // Get the form element and the URL input field
    const form = document.getElementById("urlForm");
    const urlInputField = document.getElementById("urlInput");

    // Bind custom submit handler to the form
    form.addEventListener("submit", handleUserUrl);

    // Asynchronous function that runs when the user submits the form
    async function handleUserUrl(event) {
        event.preventDefault(); // Prevents the form from submitting normally (page reload)
        console.log("handleUserUrl triggered");

        // Get the raw input and strip leading/trailing spaces
        let urlInput = urlInputField.value.trim();
        const responseMessage = document.getElementById('responseMessage');
        console.log("Raw input:", urlInput);

        // Prepend https:// for safety 
        if (!urlInput.startsWith("http://") && !urlInput.startsWith("https://")) {
            urlInput = "https://" + urlInput;
        }

        // Remove any spaces and trailing slashes for URL consistency
        urlInput = urlInput.replace(/\s+/g, "").replace(/\/$/, "");

        // Update the input box value with the normalized URL
        urlInputField.value = urlInput;
        console.log("Normalized URL:", urlInput);

        // // Ensure domain extension
        // if (!urlInput || !/\.[a-z]{2,}/.test(urlInput)) {
        //     console.warn("Validation failed");
        //     return false; 
        // }

        // try {
        //     // Make an AJAX call to the screenshot API endpoint
        //     console.log("Sending POST to /api/screenshot");
        //     const res = await fetch('/api/screenshot', {
        //         method: 'POST',
        //         headers: { 'Content-Type': 'application/json' },

        //         body: JSON.stringify({ Url: urlInput })
        //     });

        //     console.log("Screenshot API response status:", res.status);

        //     // If the API failed read and log the error response
        //     if (!res.ok) {
        //         const errorText = await res.text();
        //         console.error("Screenshot error:", errorText);
        //         return false;
        //     }

        //     // Get the base64 screenshot string from the response
        //     const base64Screenshot = await res.text();
        //     console.log("🖼 Screenshot received (base64 length):", base64Screenshot.length);

        //     // Find the image element inside the loader and inject the screenshot
        //     const previewImage = document.getElementById('screenshotPreview');
        //     if (previewImage) {
        //         previewImage.src = base64Screenshot; 
        //         previewImage.alt = `Screenshot of ${urlInput}`; 
        //         // Set CSS transition class
        //         previewImage.classList.add('loaded');
        //     }

        //     // Trigger transition to the loading overlay and auto-submit the form after a short delay
        //     showScanningTransition(); 

        // } catch (err) {
        //     // If the fetch call fails 
        //     console.error("AJAX failed:", err);
        // }

        // Prevent default form behavior regardless of success/failure
        return false;
    }
});

// Function to validate the URL format
function validateURL(urlInput) {
    var urlRegex = /^(https?:\/\/)([\w-]+\.)+[\w-]+(\/[\w-]*)*$/;
    return urlRegex.test(urlInput);
}

// Function to handle user input URL
function handleUserUrl(event) {

    event.preventDefault();
    var urlInput = document.getElementById('urlInput').value.trim();
    var responseMessage = document.getElementById('responseMessage');
    var loaderWrapper = document.getElementById('loaderWrapper');
    

    // Check if the URL is empty
    if (urlInput === "") {
        responseMessage.innerHTML = `
        <div class='alert alert-danger'>
            <h5><strong>Empty URL!</strong></h5>
            <p>Please enter a URL before</p>
        </div>`;
        return false;
    }

    // Check if the URL has spaces
    if (urlInput.includes(' ')) {
        responseMessage.innerHTML = `
        <div class='alert alert-danger'>
            <h5><strong>Spaces are not allowed in URLs.</strong></h5>
            <p>Please check your URL for any spaces</strong></p>
        </div>`;
        return false;
    }

    //Check if domain extension is missing
    if (!/\.[a-z]{2,}($|\/)/.test(urlInput)) {
        responseMessage.innerHTML = `
        <div class='alert alert-danger'>
            <h5><strong>Missing or incorrect domain extension.</strong></h5>
            <p>Please ensure your URL has a valid domain extension (e.g., .com, .org, .edu, etc.)</p>
        </div>`;
        return false;
    }

    // Check if the URL has domain extension http/https
    if (!urlInput.includes('http://') && !urlInput.includes('https://') ) {
        responseMessage.innerHTML = `
        <div class='alert alert-danger'>
            <h5><strong>Invalid URL! A non-valid URL can occur if:</strong></h5>
            <p><strong>Missing Protocol:</strong> example.com (should have http:// or https://)</p>
            <p><strong>Incorrect format:</strong> http:/example.com (missing one slash)</p>
        </div>`;
        return false; 
    }
    
    // Hide the confirmation message and show loader
    responseMessage.innerHTML = "";
    // loaderWrapper.style.display = 'flex'; // Show loader state

    if (loaderWrapper) {
        loaderWrapper.style.display = 'flex'; // Show loader state
    } else {
        console.error("Element with ID 'loaderWrapper' not found.");
    }

    showScanningTransition(); 

    closePopup();

}

// Function to close the custom popup message
function closePopup() {
    var customPopup = document.getElementById('customPopupMessage');
    customPopup.style.display = 'none';  // Hide the popup message
}

// Check if the window object is available
if (typeof window !== 'undefined') {
    window.onload = function () {
        document.getElementById('urlForm').addEventListener('submit', handleUserUrl);
    };
}

// Function show each message scanning issues one at a time 
function showEachMessage() {
    var messages = document.getElementsByClassName('messageScanningIssues');
    var i = 0;
    var interval = setInterval(function () {
        if (i < messages.length) {
            messages[i].style.display = 'block'; 
            if (i > 0) {
                messages[i - 1].style.display = 'none'; 
            }
            i++;
        } else {
            clearInterval(interval); 
        }
    }, 2000); 
}

showEachMessage(); 

// Export the functions for testing purposes
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { validateURL, handleUserUrl };
}