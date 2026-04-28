const form = document.getElementById("report-form");
const itemNameInput = document.getElementById("itemName");
const categoryInput = document.getElementById("category");
const lostDateInput = document.getElementById("lostDate");
const descriptionInput = document.getElementById("description");
const locationQueryInput = document.getElementById("locationQuery");
const contactNameInput = document.getElementById("contactName");
const contactMethodInput = document.getElementById("contactMethod");
const reportStatusInput = document.getElementById("statusValue");
const searchLocationButton = document.getElementById("search-location");
const locationResultsElement = document.getElementById("location-results");
const resultsElement = document.getElementById("results");
const summaryElement = document.getElementById("summary");
const statusElement = document.getElementById("status");

function setStatus(message) {
    statusElement.textContent = message;
}

function escapeHtml(value) {
    return value.replace(/[&<>"']/g, (character) => (
        {
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            "\"": "&quot;",
            "'": "&#39;"
        }[character]
    ));
}

function formatDate(value) {
    return new Date(value).toLocaleDateString(undefined, {
        year: "numeric",
        month: "short",
        day: "numeric"
    });
}

function renderReports(reports) {
    if (!reports.length) {
        summaryElement.textContent = "No lost-item reports yet. Create the first one above.";
        resultsElement.innerHTML = '<div class="empty-state">Your dashboard is ready for the first report.</div>';
        return;
    }

    const openCount = reports.filter((report) => report.status !== "Found").length;
    summaryElement.textContent = `${reports.length} report${reports.length === 1 ? "" : "s"} in the system, ${openCount} still active.`;
    resultsElement.innerHTML = reports
        .map((report) => {
            const safeDescription = escapeHtml(report.description);
            const safeItemName = escapeHtml(report.itemName);
            const safeLocation = escapeHtml(report.lastSeenLocation);
            const safeCategory = escapeHtml(report.category);
            const safeContactName = escapeHtml(report.contactName);
            const safeContactMethod = escapeHtml(report.contactMethod);
            const safeStatus = escapeHtml(report.status);

            return `
                <article class="result-card">
                    <div class="card-top">
                        <h3>${safeItemName}</h3>
                        <span class="status-tag status-${safeStatus.toLowerCase().replaceAll(" ", "-")}">${safeStatus}</span>
                    </div>
                    <p>${safeDescription}</p>
                    <div class="meta-row">
                        <span class="chip">${safeCategory}</span>
                        <span class="chip">${formatDate(report.lostDate)}</span>
                        <span class="chip">${safeLocation}</span>
                    </div>
                    <div class="contact-line">Contact ${safeContactName} at ${safeContactMethod}</div>
                </article>
            `;
        })
        .join("");
}

function renderLocationOptions(results) {
    if (!results.length) {
        locationResultsElement.innerHTML = '<div class="empty-state compact">No places matched that search.</div>';
        return;
    }

    locationResultsElement.innerHTML = results.slice(0, 5).map((result) => {
        const label = escapeHtml(result.displayName);
        return `
            <button type="button" class="location-option" data-location="${label}">
                <strong>${label}</strong>
                <span>${escapeHtml(result.type || "place")}</span>
            </button>
        `;
    }).join("");
}

async function loadReports() {
    summaryElement.textContent = "Loading reports from the API...";

    try {
        const response = await fetch("/api/lostitemreports");
        if (!response.ok) {
            throw new Error("Could not load reports.");
        }

        const reports = await response.json();
        renderReports(reports);
        setStatus("Ready");
    } catch (error) {
        summaryElement.textContent = "The report list could not be loaded.";
        resultsElement.innerHTML = '<div class="empty-state">Check the API and reload the page.</div>';
        setStatus("Error");
    }
}

async function searchLocations() {
    const query = locationQueryInput.value.trim();
    if (!query) {
        locationResultsElement.innerHTML = '<div class="empty-state compact">Enter a location before searching.</div>';
        return;
    }

    setStatus("Searching places");
    locationResultsElement.innerHTML = '<div class="empty-state compact">Looking up places...</div>';

    try {
        const response = await fetch(`/api/locationsearch?q=${encodeURIComponent(query)}`);
        if (!response.ok) {
            throw new Error("Location search failed.");
        }

        const results = await response.json();
        renderLocationOptions(results);
        setStatus("Ready");
    } catch (error) {
        locationResultsElement.innerHTML = '<div class="empty-state compact">Unable to reach the location search service right now.</div>';
        setStatus("Error");
    }
}

async function createReport(event) {
    event.preventDefault();

    const payload = {
        itemName: itemNameInput.value.trim(),
        category: categoryInput.value.trim(),
        lostDate: lostDateInput.value,
        description: descriptionInput.value.trim(),
        lastSeenLocation: locationQueryInput.value.trim(),
        contactName: contactNameInput.value.trim(),
        contactMethod: contactMethodInput.value.trim(),
        status: reportStatusInput.value
    };

    if (Object.values(payload).some((value) => !value)) {
        setStatus("Missing details");
        locationResultsElement.innerHTML = '<div class="empty-state compact">Complete every field before creating a report.</div>';
        return;
    }

    setStatus("Saving");

    try {
        const response = await fetch("/api/lostitemreports", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error("Create report failed.");
        }

        form.reset();
        lostDateInput.value = new Date().toISOString().split("T")[0];
        locationResultsElement.innerHTML = "";
        await loadReports();
        setStatus("Saved");
    } catch (error) {
        setStatus("Error");
        locationResultsElement.innerHTML = '<div class="empty-state compact">The report could not be saved.</div>';
    }
}

searchLocationButton.addEventListener("click", searchLocations);
form.addEventListener("submit", createReport);
locationResultsElement.addEventListener("click", (event) => {
    const button = event.target.closest(".location-option");
    if (!button) {
        return;
    }

    locationQueryInput.value = button.dataset.location || "";
    locationResultsElement.innerHTML = "";
    setStatus("Place selected");
});

lostDateInput.value = new Date().toISOString().split("T")[0];
loadReports();
