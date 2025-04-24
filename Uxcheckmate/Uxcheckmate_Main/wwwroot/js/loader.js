// This file is to handle loader and scanning transitions

// Function to transition from loader to scanning
function showScanningTransition() {
    var loaderWrapper = document.getElementById('loaderWrapper');
    var scanningWrapper = document.getElementById('scanningWrapper');

    document.body.style.overflow = 'hidden'; 

    // Delay the scanning state
    setTimeout(function () {
        if (loaderWrapper && scanningWrapper) {
            loaderWrapper.style.display = 'none';
            scanningWrapper.style.display = 'block';

            // After scanning, submit the form
            setTimeout(function () {
                document.getElementById('urlForm').submit();
            }, 1000); // Submit after 1 second
        }
    }, 1000); // Show scanning after 1 second
}

// Array of scanning messages
const scanningMessages = [
    "Evaluating Design Issues requirements...",
    "Evaluating Accessibility requirements...",
    "Evaluating Severity...",
    "Evaluating Code Structure...",
    "Evaluating Performance...",
    "Evaluating Mobile Compatibility...",
    "Evaluating Color Contrast...",
    "Evaluating Keyboard Navigation...",
    "Evaluating Screen Reader Support...",
    "Evaluating Interactive Elements...",
    "Evaluating Forms and Inputs Accessibility...",
    "Evaluating Content Readability...",
    "Evaluating Image Alt Text...",
    "Evaluating Link Accessibility...",
    "Evaluating Media and Video Accessibility...",
    "Evaluating Site Responsiveness..."
];

// Function to dynamically populate scanning messages
function populateScanningMessages() {
    const scanningWrapper = document.getElementById('scanningWrapper');
    const scanStatus = scanningWrapper.querySelector('.scan-status');

    // Clear existing messages
    scanStatus.innerHTML = '<h1>Scanning your website...</h1>';

    // Add messages dynamically
    scanningMessages.forEach(message => {
        const p = document.createElement('p');
        p.className = 'messageScanningIssues';
        p.style.display = 'none'; // Initially hidden
        p.textContent = message;
        p.style.fontSize = '20px'; // Set font size
        scanStatus.appendChild(p);
    });
}

// Call the function to populate messages
populateScanningMessages();

// Function to show each scanning message one by one
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
            messages[messages.length - 1].style.display = 'none'; // Hide the last message
            i = 0; // Reset index to loop through messages again
        }
    }, 2000);
    return interval; // Return the interval ID for potential clearing
}

if (typeof window !== 'undefined') {
    window.onload = function () {
        showEachMessage();
    };
}

// Export for testing
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        showScanningTransition,
        showEachMessage
    };
}