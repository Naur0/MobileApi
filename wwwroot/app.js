const form = document.getElementById("search-form");
const queryInput = document.getElementById("query");
const resultsElement = document.getElementById("results");
const summaryElement = document.getElementById("summary");
const statusElement = document.getElementById("status");

function setStatus(message) {
    statusElement.textContent = message;
}

function renderResults(query, results) {
    if (!results.length) {
        summaryElement.textContent = `No places found for "${query}". Try a broader or more specific phrase.`;
        resultsElement.innerHTML = '<div class="empty-state">No matching places came back from the search API.</div>';
        return;
    }

    summaryElement.textContent = `Found ${results.length} place${results.length === 1 ? "" : "s"} for "${query}".`;
    resultsElement.innerHTML = results
        .map((result) => {
            const title = result.displayName.split(",").slice(0, 2).join(", ");
            const coordinates = `${Number(result.latitude).toFixed(4)}, ${Number(result.longitude).toFixed(4)}`;

            return `
                <article class="result-card">
                    <h3>${title}</h3>
                    <p>${result.displayName}</p>
                    <div class="meta-row">
                        <span class="chip">${result.category || "place"}</span>
                        <span class="chip">${result.type || "unknown type"}</span>
                        <span class="chip">${coordinates}</span>
                    </div>
                </article>
            `;
        })
        .join("");
}

async function runSearch(query) {
    setStatus("Searching");
    summaryElement.textContent = "Fetching locations from your API...";
    resultsElement.innerHTML = "";

    try {
        const response = await fetch(`/api/locationsearch?q=${encodeURIComponent(query)}`);
        if (!response.ok) {
            throw new Error("Search request failed.");
        }

        const results = await response.json();
        renderResults(query, results);
        setStatus("Complete");
    } catch (error) {
        summaryElement.textContent = "The search request did not complete. Check the API connection and try again.";
        resultsElement.innerHTML = '<div class="empty-state">Something went wrong while talking to the location service.</div>';
        setStatus("Error");
    }
}

form.addEventListener("submit", (event) => {
    event.preventDefault();
    const query = queryInput.value.trim();

    if (!query) {
        summaryElement.textContent = "Enter a search phrase before submitting.";
        resultsElement.innerHTML = '<div class="empty-state">A search term is required.</div>';
        setStatus("Waiting");
        return;
    }

    runSearch(query);
});

runSearch(queryInput.value.trim());
