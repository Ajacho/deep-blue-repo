// Function to delete a report
function deleteReport(reportId) {

    fetch(`/Home/DeleteReport?reportId=${reportId}`, {
        method: 'DELETE'
    })
    .then(response => {
        if (response.ok) {
            const reportElement = document.getElementById(`report-${reportId}`);
            if (reportElement) {
                reportElement.remove(); // Remove the report from the DOM

                for (const domain in groupedReports) {
                    const reportIndex = groupedReports[domain].findIndex(r => r.id === reportId);
                    if (reportIndex !== -1) {
                        // Remove the report from the groupedReports object
                        groupedReports[domain].splice(reportIndex, 1);

                        // Update the badge count
                        const badge = document.querySelector(`#websiteAccordion-${domain.replace(/\W/g, '')} .badge`);
                        if (badge) {
                            badge.textContent = groupedReports[domain].length;
                        }

                        // If no reports remain for the domain, remove the domain card
                        if (groupedReports[domain].length === 0) {
                            const domainCard = document.getElementById(`websiteAccordion-${domain.replace(/\W/g, '')}`);
                            if (domainCard) {
                                domainCard.parentElement.remove();
                            }
                        }
                        break;
                    }
                }
            }
        } else {
            console.error('Failed to delete report:', response.statusText);
        }
    })
    .catch(error => console.error('Error deleting report:', error));
}


// Function to format date
function formattedDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
         year: 'numeric', month: 'short', day: 'numeric' });
}

// Function to download a report as a PDF
function downloadReport(reportId) {
    fetch(`/Home/DownloadReport?id=${reportId}`)  
        .then(response => response.blob())
        .then(blob => {
            const link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.download = `Report_${reportId}.pdf`;  
            link.click();  
        })
        .catch(error => console.error('Error downloading the report:', error));
}

// Function to view a report's details in a modal
function viewReportDetails(reportId) {
    const report = findReportById(reportId);
    if (report) {
        const modalBody = document.getElementById('reportModalBody');
        const safeReportId = `report-${report.id}`; 

        modalBody.innerHTML = `

            <header class="myUserDashHeader">
                <h1 id="reportHeader">${report.url}</h1>
                <p>${formattedDate(report.date)}</p>
                <a href="#" class="btn btn-secondary" onclick="downloadReport(${report.id})">Download Report</a>
            </header>

            <h3 class="mt-3 title-issues">Design Issues</h3>
            <p class="text-subtle">${report.designIssues.length} Issues found</p>
            ${report.designIssues.length > 0 ? `
                <div class="accordion" id="designIssuesAccordion-${safeReportId}">
                    ${report.designIssues.map((issue, index) => `
                        <div class="accordion-item">
                            <h2 class="accordion-header" id="designIssueHeader-${safeReportId}-${index}">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#designIssueContent-${safeReportId}-${index}" aria-expanded="false" aria-controls="designIssueContent-${safeReportId}-${index}">
                                    Severity: ${issue.severity}
                                </button>
                            </h2>
                            <div id="designIssueContent-${safeReportId}-${index}" class="accordion-collapse collapse" aria-labelledby="designIssueHeader-${safeReportId}-${index}" data-bs-parent="#designIssuesAccordion-${safeReportId}">
                                <div class="accordion-body">
                                    <p><strong>Message:</strong> ${issue.message}</p>
                                    <p><small class="text-subtle">Category: ${issue.category}</small></p>
                                </div>
                            </div>
                        </div>
                    `).join('')}
                </div>
            ` : `
                <p class="text-success">Congrats! No Design Issues</p>
            `}

            <h3 class="mt-3 title-issues">Accessibility Issues</h3>
            <p class="text-subtle">${report.accessibilityIssues.length} Issues found</p>
            ${report.accessibilityIssues.length > 0 ? `
                <div class="accordion" id="accessibilityIssuesAccordion-${safeReportId}">
                    ${report.accessibilityIssues.map((issue, index) => `
                        <div class="accordion-item">
                            <h2 class="accordion-header" id="accessibilityIssueHeader-${safeReportId}-${index}">
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#accessibilityIssueContent-${safeReportId}-${index}" aria-expanded="false" aria-controls="accessibilityIssueContent-${safeReportId}-${index}">
                                    Severity: ${issue.severity}
                                </button>
                            </h2>
                            <div id="accessibilityIssueContent-${safeReportId}-${index}" class="accordion-collapse collapse" aria-labelledby="accessibilityIssueHeader-${safeReportId}-${index}" data-bs-parent="#accessibilityIssuesAccordion-${safeReportId}">
                                <div class="accordion-body">
                                    <p><strong>Message:</strong> ${issue.message}</p>
                                    <p><small class="text-subtle">Category: ${issue.category} | WCAG: ${issue.wcag}</small></p>
                                </div>
                            </div>
                        </div>
                    `).join('')}
                </div>
            ` : `
                <p class="text-success">No accessibility issues</p>
            `}
        `;

        const modal = new bootstrap.Modal(document.getElementById('reportModal'));
        modal.show();
    } else {
        console.error('Report not found:', reportId);
    }
}

// Function to find a report by its ID
function findReportById(id) {
    for (const domain in groupedReports) {
        const report = groupedReports[domain].find(r => r.id === id);
        if (report) return report;
    }
    return null;
}

// Group reports by domain and dynamically load them into the dashboard
let groupedReports = {}; 

document.addEventListener('DOMContentLoaded', () => {
    groupedReports = {};

    reportsByDomain.forEach(report => {
        const url = report.url;
        const domain = url.replace("https://", "").replace("http://", "").split('/')[0];

        if (!groupedReports[domain]) {
            groupedReports[domain] = [];
        }
        groupedReports[domain].push(report);
    });

    const container = document.getElementById('reportListSameDomain');
    
    for (const domain in groupedReports) {
        const folderDiv = document.createElement('div');
        folderDiv.classList.add('folder-card', 'mb-3', 'p-3', 'glass-card');
    
        const safeDomain = domain.replace(/\W/g, '');
        const accordionId = `websiteAccordion-${safeDomain}`;
        const headingId = `heading-${safeDomain}`;
        const collapseId = `collapse-${safeDomain}`;
    
        folderDiv.innerHTML = `
            <div class="accordion" id="${accordionId}">
                <div class="accordion-item">
                    <h2 class="accordion-header" id="${headingId}">
                        <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#${collapseId}" aria-expanded="false" aria-controls="${collapseId}">
                            <div class="col-md-8 d-flex justify-content-between align-items-left">
                                ${domain}
                            </div>
                            <div class="col-md-4 d-flex justify-content-center align-items-center">
                                <span class="badge bg-primary">${groupedReports[domain].length} reports</span>
                            </div>
                        </button>
                    </h2>
    
                    <div id="${collapseId}" class="accordion-collapse collapse" aria-labelledby="${headingId}" data-bs-parent="#${accordionId}">
                        <div class="accordion-body">
                            <div class="row fw-bold mb-2">
                                <div class="col-sm">Webpage</div>
                                <div class="col-sm">Report ID</div>
                                <div class="col-sm">Date</div>
                                <div class="col-sm">Actions</div>
                                <div class="col-sm">Delete</div>
                            </div>
    
                            ${groupedReports[domain].map(r => `
                                <div class="row mb-2" id="report-${r.id}">
                                    <div class="col-sm text-break">${r.url}</div> 
                                    <div class="col-sm">${r.id}</div>
                                    <div class="col-sm">${formattedDate(r.date)}</div>
                                    <div class="col-sm">
                                        <button class="btn btn-secondary btn-sm" onclick="viewReportDetails(${r.id})">View Report</button>
                                    </div>
                                    <div class="col-sm">
                                        <button class="btn btn-danger btn-sm" onclick="deleteReport(${r.id})">Delete</button>
                                    </div>
                                </div>
                            `).join('')}
                        </div>
                    </div>
    
                </div>
            </div>
        `;
    
        container.appendChild(folderDiv);
    }
});

// Toggle function to show/hide reports inside a folder
function toggleReports(domain) {
    const list = document.getElementById(`reports-${domain}`);
    list.style.display = (list.style.display === 'none') ? 'block' : 'none';
}
