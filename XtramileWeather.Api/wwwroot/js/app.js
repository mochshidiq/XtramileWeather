const countrySelect = document.getElementById("countrySelect");
const citySelect = document.getElementById("citySelect");

const loadWeatherButton =
    document.getElementById("loadWeatherButton");

const addFavoriteButton =
    document.getElementById("addFavoriteButton");

const weatherResult =
    document.getElementById("weatherResult");

const favoriteList =
    document.getElementById("favoriteList");

const statusMessage =
    document.getElementById("statusMessage");

const unitToggleButton =
    document.getElementById("unitToggleButton");

let currentWeather = null;
let selectedUnit = "C";

document.addEventListener("DOMContentLoaded", async () => {
    bindEvents();

    await Promise.all([
        loadCountries(),
        loadFavorites()
    ]);
});

function bindEvents() {
    countrySelect.addEventListener("change", loadCities);
    citySelect.addEventListener("change", updateButtons);

    loadWeatherButton.addEventListener(
        "click",
        loadWeather);

    addFavoriteButton.addEventListener(
        "click",
        addFavorite);

    unitToggleButton.addEventListener(
        "click",
        toggleTemperatureUnit);
}

async function apiRequest(url, options = {}) {
    const response = await fetch(url, {
        ...options,
        headers: {
            Accept: "application/json",
            ...options.headers
        }
    });

    const text = await response.text();
    let data = null;

    if (text) {
        try {
            data = JSON.parse(text);
        } catch {
            data = text;
        }
    }

    if (!response.ok) {
        const message =
            typeof data === "string"
                ? data
                : data?.message || data?.detail;

        throw new Error(
            message || `Request failed with status ${response.status}.`);
    }

    return data;
}

async function loadCountries() {
    try {
        const countries = await apiRequest("/api/countries");

        for (const country of countries) {
            const option = document.createElement("option");

            option.value = country.id;
            option.textContent =
                `${country.name} (${country.code})`;

            countrySelect.appendChild(option);
        }
    } catch (error) {
        showStatus(error.message, true);
    }
}

async function loadCities() {
    const countryId = countrySelect.value;

    citySelect.innerHTML =
        '<option value="">Select city</option>';

    citySelect.disabled = true;
    resetWeather();
    updateButtons();

    if (!countryId) {
        return;
    }

    try {
        const cities = await apiRequest(
            `/api/cities?countryId=${countryId}`);

        for (const city of cities) {
            const option = document.createElement("option");

            option.value = city.id;
            option.textContent = city.name;

            citySelect.appendChild(option);
        }

        citySelect.disabled = false;
    } catch (error) {
        showStatus(error.message, true);
    }
}

function updateButtons() {
    const citySelected = Boolean(citySelect.value);

    loadWeatherButton.disabled = !citySelected;
    addFavoriteButton.disabled = !citySelected;
    resetWeather();
}

async function loadWeather() {
    const cityId = citySelect.value;

    if (!cityId) {
        return;
    }

    showStatus("Loading weather...");

    try {
        currentWeather = await apiRequest(
            `/api/weather/${cityId}`);

        selectedUnit = "C";
        renderWeather();

        unitToggleButton.classList.remove("hidden");
        unitToggleButton.textContent = "Switch to °F";

        showStatus("");
    } catch (error) {
        resetWeather();
        showStatus(error.message, true);
    }
}

async function addFavorite() {
    const cityId = citySelect.value;

    if (!cityId) {
        return;
    }

    try {
        await apiRequest(
            `/api/favorite-cities/${cityId}`,
            { method: "POST" });

        showStatus("City added to favorites.");
        await loadFavorites();
    } catch (error) {
        showStatus(error.message, true);
    }
}

async function loadFavorites() {
    try {
        const favorites = await apiRequest(
            "/api/favorite-cities");

        if (favorites.length === 0) {
            favoriteList.innerHTML =
                "<p>No favorite cities yet.</p>";

            return;
        }

        favoriteList.innerHTML = favorites
            .map(favorite => `
                <div class="favorite-item">
                    <div>
                        <strong>
                            ${escapeHtml(favorite.cityName)}
                        </strong>

                        <span>
                            (${escapeHtml(favorite.countryCode)})
                        </span>
                    </div>

                    <button
                        class="danger"
                        data-favorite-id="${favorite.id}">
                        Remove
                    </button>
                </div>
            `)
            .join("");

        favoriteList
            .querySelectorAll("[data-favorite-id]")
            .forEach(button => {
                button.addEventListener("click", async () => {
                    await deleteFavorite(
                        button.dataset.favoriteId);
                });
            });
    } catch (error) {
        favoriteList.innerHTML =
            `<p>${escapeHtml(error.message)}</p>`;
    }
}

async function deleteFavorite(favoriteId) {
    try {
        await apiRequest(
            `/api/favorite-cities/${favoriteId}`,
            { method: "DELETE" });

        showStatus("Favorite city removed.");
        await loadFavorites();
    } catch (error) {
        showStatus(error.message, true);
    }
}

function resetWeather() {
    currentWeather = null;

    weatherResult.classList.add("hidden");
    weatherResult.innerHTML = "";

    unitToggleButton.classList.add("hidden");
}

function showStatus(message, isError = false) {
    statusMessage.textContent = message;
    statusMessage.classList.toggle("error", isError);
}

function formatNumber(value) {
    return Number(value).toFixed(1);
}

function escapeHtml(value) {
    const element = document.createElement("div");
    element.textContent = value ?? "";
    return element.innerHTML;
}

function toggleTemperatureUnit() {
    selectedUnit =
        selectedUnit === "C" ? "F" : "C";

    unitToggleButton.textContent =
        selectedUnit === "C"
            ? "Switch to °F"
            : "Switch to °C";

    renderWeather();
}

function renderWeather() {
    if (!currentWeather) {
        return;
    }

    const isCelsius = selectedUnit === "C";
    const symbol = isCelsius ? "°C" : "°F";

    const temperature = isCelsius
        ? currentWeather.temperatureCelsius
        : currentWeather.temperatureFahrenheit;

    const feelsLike = isCelsius
        ? currentWeather.feelsLikeCelsius
        : currentWeather.feelsLikeFahrenheit;

    const minimum = isCelsius
        ? currentWeather.minimumTemperatureCelsius
        : currentWeather.minimumTemperatureFahrenheit;

    const maximum = isCelsius
        ? currentWeather.maximumTemperatureCelsius
        : currentWeather.maximumTemperatureFahrenheit;

    const dewPoint = isCelsius
        ? currentWeather.dewPointCelsius
        : currentWeather.dewPointFahrenheit;

    const visibilityKilometers =
        currentWeather.visibilityMeters / 1000;

    weatherResult.innerHTML = `
        <h3>
            ${escapeHtml(currentWeather.cityName)},
            ${escapeHtml(currentWeather.countryCode)}
        </h3>

        <div class="temperature">
            ${formatNumber(temperature)} ${symbol}
        </div>

        <p>
            ${escapeHtml(currentWeather.sky)} —
            ${escapeHtml(currentWeather.description)}
        </p>

        <div class="weather-details">
            <span>
                Feels like:
                ${formatNumber(feelsLike)} ${symbol}
            </span>

            <span>
                Minimum:
                ${formatNumber(minimum)} ${symbol}
            </span>

            <span>
                Maximum:
                ${formatNumber(maximum)} ${symbol}
            </span>

            <span>
                Dew point:
                ${formatNumber(dewPoint)} ${symbol}
            </span>

            <span>
                Humidity:
                ${currentWeather.humidity}%
            </span>

            <span>
                Pressure:
                ${currentWeather.pressureHpa} hPa
            </span>

            <span>
                Visibility:
                ${formatNumber(visibilityKilometers)} km
            </span>

            <span>
                Wind:
                ${formatNumber(
        currentWeather.windSpeedMetersPerSecond)} m/s
            </span>

            <span>
                Observed:
                ${formatUtc(currentWeather.observedAtUtc)}
            </span>
        </div>
    `;

    weatherResult.classList.remove("hidden");
}

function formatUtc(value) {
    const date = new Date(value);

    return `${date.toLocaleString("en-GB", {
        timeZone: "UTC",
        year: "numeric",
        month: "short",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit",
        hour12: false
    })} UTC`;
}